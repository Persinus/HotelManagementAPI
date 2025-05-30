using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using System.Security.Claims;
using System.Collections.Generic;

namespace HotelManagementAPI.Controllers.KhachHang
{
    [ApiController]
    [Route("api/KhachHang/datphong")]
    public class KhachHang_DatPhongController : ControllerBase
    {
        private readonly IDbConnection _db;

        public KhachHang_DatPhongController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Tạo đơn đặt phòng (kèm dịch vụ đi kèm nếu có).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> TaoDonDatPhong([FromBody] DatPhongDTO datPhongDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Lấy mã người dùng từ JWT
            var maNguoiDung = User.FindFirstValue("sub")
                              ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            datPhongDTO.MaNguoiDung = maNguoiDung;
            datPhongDTO.MaDatPhong = await GenerateUniqueMaDatPhong();
            datPhongDTO.NgayDat = DateTime.Now;

            // Kiểm tra trạng thái phòng
            const string checkPhongQuery = "SELECT TinhTrang FROM Phong WHERE MaPhong = @MaPhong";
            var tinhTrang = await _db.ExecuteScalarAsync<byte?>(checkPhongQuery, new { datPhongDTO.MaPhong });
            if (tinhTrang != 1)
                return BadRequest(new { Message = "Phòng này không còn trống để đặt." });

            // Kiểm tra trạng thái đặt phòng FE gửi lên
            if (datPhongDTO.TinhTrangDatPhong != 1)
                return BadRequest(new { Message = "Phòng này bạn đã đặt sang hóa đơn để xem ." });

            // Kiểm tra dịch vụ đi kèm (nếu có)
            if (datPhongDTO.DichVuDiKem != null && datPhongDTO.DichVuDiKem.Any())
            {
                foreach (var dv in datPhongDTO.DichVuDiKem)
                {
                    if (dv.SoLuong <= 0)
                        return BadRequest(new { Message = $"Số lượng dịch vụ {dv.MaDichVu} phải lớn hơn 0." });

                    // Kiểm tra dịch vụ còn lại
                    const string checkDichVuQuery = "SELECT SoLuong FROM DichVu WHERE MaDichVu = @MaDichVu";
                    var dichVuSoLuong = await _db.ExecuteScalarAsync<int?>(checkDichVuQuery, new { dv.MaDichVu });
                    if (dichVuSoLuong == null)
                        return NotFound(new { Message = $"Mã dịch vụ {dv.MaDichVu} không tồn tại." });

                    if (dichVuSoLuong == 0)
                        return BadRequest(new { Message = $"Dịch vụ {dv.MaDichVu} đã hết hàng." });

                    if (dv.SoLuong > dichVuSoLuong)
                        return BadRequest(new { Message = $"Số lượng dịch vụ {dv.MaDichVu} yêu cầu vượt quá số lượng còn lại." });
                }
            }

            // Lấy giá phòng
            const string getGiaPhongQuery = "SELECT GiaPhong FROM Phong WHERE MaPhong = @MaPhong";
            var giaPhong = await _db.ExecuteScalarAsync<decimal>(getGiaPhongQuery, new { datPhongDTO.MaPhong });

            // Tính số ngày ở (tối thiểu 1 ngày)
            var soNgay = (datPhongDTO.NgayCheckOut.Date - datPhongDTO.NgayCheckIn.Date).Days;
            if (soNgay < 1) soNgay = 1;

            // Tính tổng tiền dịch vụ
            decimal tongTienDichVu = 0;
            if (datPhongDTO.DichVuDiKem != null && datPhongDTO.DichVuDiKem.Any())
            {
                foreach (var dv in datPhongDTO.DichVuDiKem)
                {
                    // Lấy đơn giá dịch vụ
                    const string getDonGiaQuery = "SELECT DonGia FROM DichVu WHERE MaDichVu = @MaDichVu";
                    var donGia = await _db.ExecuteScalarAsync<decimal>(getDonGiaQuery, new { dv.MaDichVu });
                    tongTienDichVu += donGia * dv.SoLuong;
                }
            }

            // Tổng tiền = tiền phòng * số ngày + tổng tiền dịch vụ
            var tongTien = giaPhong * soNgay + tongTienDichVu;

            

            // Thêm đơn đặt phòng (KHÔNG có cột TongTien, TrangThai, NgayNhan, NgayTra)
            const string insertQuery = @"
                INSERT INTO DatPhong (MaDatPhong, MaNguoiDung, MaPhong, NgayDat, NgayCheckIn, NgayCheckOut, TinhTrangDatPhong)
                VALUES (@MaDatPhong, @MaNguoiDung, @MaPhong, @NgayDat, @NgayCheckIn, @NgayCheckOut, @TinhTrangDatPhong)";
            await _db.ExecuteAsync(insertQuery, datPhongDTO);

            // Ngay sau khi insert, cập nhật trạng thái sang 2
            const string updateTinhTrangDatPhong = "UPDATE DatPhong SET TinhTrangDatPhong = 2 WHERE MaDatPhong = @MaDatPhong";
            await _db.ExecuteAsync(updateTinhTrangDatPhong, new { datPhongDTO.MaDatPhong });

            // Thêm dịch vụ đi kèm (nếu có)
            if (datPhongDTO.DichVuDiKem != null && datPhongDTO.DichVuDiKem.Any())
            {
                foreach (var dv in datPhongDTO.DichVuDiKem)
                {
                    dv.MaDatDichVu = await GenerateUniqueMaDatDichVu();
                    dv.MaDatPhong = datPhongDTO.MaDatPhong;
                    const string insertDichVu = @"
                        INSERT INTO DatDichVu (MaDatDichVu, MaDatPhong, MaDichVu, SoLuong)
                        VALUES (@MaDatDichVu, @MaDatPhong, @MaDichVu, @SoLuong)";
                    await _db.ExecuteAsync(insertDichVu, dv);

                    // Trừ số lượng dịch vụ
                    const string updateDichVuQuery = "UPDATE DichVu SET SoLuong = SoLuong - @SoLuong WHERE MaDichVu = @MaDichVu";
                    await _db.ExecuteAsync(updateDichVuQuery, new { dv.SoLuong, dv.MaDichVu });
                }
            }

            // Cập nhật trạng thái phòng về 2 (đã có người đặt chính là mình đã đặt 1 rồi ko đặt nữa , chờ thanh toán)
            // const string updatePhongQuery = @"UPDATE Phong SET TinhTrang = 2 WHERE MaPhong = @MaPhong";
            // await _db.ExecuteAsync(updatePhongQuery, new { datPhongDTO.MaPhong });

            return Ok(new
            {
                Message = $"Đặt phòng thành công. Tổng tiền tạm tính là: {tongTien:N0} VNĐ. Hãy sang phần tạo hóa đơn, nhập mã đặt phòng để nhận các thông tin thanh toán chi tiết.",
                MaDatPhong = datPhongDTO.MaDatPhong,
                TongTienTamTinh = tongTien
            });
        }

        /// <summary>
        /// Lấy lịch sử đặt phòng của người dùng.
        /// </summary>
        [HttpGet("lichsu")]
        public async Task<IActionResult> LichSuDatPhong()
        {
            var maNguoiDung = User.FindFirstValue("sub")
                              ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            const string query = @"
                SELECT dp.MaDatPhong, dp.MaNguoiDung, dp.MaPhong, dp.NgayDat, dp.NgayCheckIn, dp.NgayCheckOut, dp.TinhTrangDatPhong,
                       p.LoaiPhong, p.GiaPhong
                FROM DatPhong dp
                INNER JOIN Phong p ON dp.MaPhong = p.MaPhong
                WHERE dp.MaNguoiDung = @MaNguoiDung";

            var result = await _db.QueryAsync(query, new { MaNguoiDung = maNguoiDung });

            return Ok(result);
        }

        /// <summary>
        /// Hủy đặt phòng.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> HuyDatPhong(string id)
        {
            // Kiểm tra xem mã đặt phòng có tồn tại không
            const string checkQuery = "SELECT COUNT(1) FROM DatPhong WHERE MaDatPhong = @MaDatPhong";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaDatPhong = id });
            if (isExists == 0)
            {
                return NotFound(new { Message = "Mã đặt phòng không tồn tại." });
            }

            // Xóa đơn đặt phòng
            const string deleteQuery = "DELETE FROM DatPhong WHERE MaDatPhong = @MaDatPhong";
            await _db.ExecuteAsync(deleteQuery, new { MaDatPhong = id });

            return Ok(new { Message = "Hủy đặt phòng thành công." });
        }

        // Helper methods
        private async Task<string> GenerateUniqueMaDatPhong()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaDatPhong, 3, LEN(MaDatPhong) - 2) AS INT)), 0) + 1
                FROM DatPhong";
            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"DP{nextId:D3}";
        }

        private async Task<string> GenerateUniqueMaDatDichVu()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaDatDichVu, 4, LEN(MaDatDichVu) - 3) AS INT)), 0) + 1
                FROM DatDichVu";
            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"DDV{nextId:D3}";
        }
    }
}