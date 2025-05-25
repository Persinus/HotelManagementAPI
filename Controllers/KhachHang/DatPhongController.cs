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
    public class DatPhongController : ControllerBase
    {
        private readonly IDbConnection _db;

        public DatPhongController(IDbConnection db)
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
            var maNguoiDung = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            datPhongDTO.MaNguoiDung = maNguoiDung;
            datPhongDTO.MaDatPhong = await GenerateUniqueMaDatPhong();

            // Kiểm tra trạng thái phòng
            const string checkPhongQuery = "SELECT TinhTrang FROM Phong WHERE MaPhong = @MaPhong";
            var tinhTrang = await _db.ExecuteScalarAsync<byte?>(checkPhongQuery, new { datPhongDTO.MaPhong });
            if (tinhTrang != 1)
                return BadRequest(new { Message = "Phòng này hiện không trống, không thể đặt phòng." });

            // Thêm đơn đặt phòng
            const string insertQuery = @"
                INSERT INTO DatPhong (MaDatPhong, MaNguoiDung, MaPhong, NgayDat, NgayNhan, NgayTra, TongTien, TrangThai)
                VALUES (@MaDatPhong, @MaNguoiDung, @MaPhong, @NgayDat, @NgayNhan, @NgayTra, @TongTien, @TrangThai)";
            await _db.ExecuteAsync(insertQuery, datPhongDTO);

            // Thêm dịch vụ đi kèm nếu có
            if (datPhongDTO.DichVuDiKem != null && datPhongDTO.DichVuDiKem.Any())
            {
                foreach (var dv in datPhongDTO.DichVuDiKem)
                {
                    dv.MaDatDichVu = await GenerateUniqueMaDatDichVu();
                    dv.MaDatPhong = datPhongDTO.MaDatPhong;
                    const string insertDichVu = @"
                        INSERT INTO DatDichVu (MaDatDichVu, MaDatPhong, MaDichVu, SoLuong, MaHoaDon)
                        VALUES (@MaDatDichVu, @MaDatPhong, @MaDichVu, @SoLuong, @MaHoaDon)";
                    await _db.ExecuteAsync(insertDichVu, dv);

                    // Cập nhật số lượng dịch vụ còn lại
                    const string updateDichVuQuery = "UPDATE DichVu SET SoLuong = SoLuong - @SoLuong WHERE MaDichVu = @MaDichVu";
                    await _db.ExecuteAsync(updateDichVuQuery, new { dv.SoLuong, dv.MaDichVu });
                }
            }

            // Cập nhật trạng thái phòng về 2 (Đã đặt)
            const string updatePhongQuery = @"UPDATE Phong SET TinhTrang = 2 WHERE MaPhong = @MaPhong";
            await _db.ExecuteAsync(updatePhongQuery, new { datPhongDTO.MaPhong });

            return Ok(new { Message = "Đặt phòng thành công.", datPhongDTO.MaDatPhong });
        }

        /// <summary>
        /// Lấy lịch sử đặt phòng của người dùng.
        /// </summary>
        [HttpGet("lichsu")]
        public async Task<IActionResult> LichSuDatPhong()
        {
            var maNguoiDung = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            const string query = @"
                SELECT dp.MaDatPhong, dp.MaNguoiDung, dp.MaPhong, dp.NgayDat, dp.NgayNhan, dp.NgayTra, dp.TongTien, dp.TrangThai,
                       p.TenPhong, p.LoaiPhong, p.GiaPhong
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

        /// <summary>
        /// Đặt thêm dịch vụ đi kèm cho đơn đặt phòng đã có.
        /// </summary>
        [HttpPost("datdichvu")]
        public async Task<IActionResult> DatDichVu([FromBody] DatDichVuDTO datDichVuDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra giá trị soLuong
            if (datDichVuDTO.SoLuong < 1 || datDichVuDTO.SoLuong > 1000)
                return BadRequest(new { Message = "Số lượng phải nằm trong khoảng từ 1 đến 1000." });

            // Kiểm tra xem MaDatPhong có tồn tại không
            const string checkDatPhongQuery = "SELECT COUNT(1) FROM DatPhong WHERE MaDatPhong = @MaDatPhong";
            var isDatPhongExists = await _db.ExecuteScalarAsync<int>(checkDatPhongQuery, new { datDichVuDTO.MaDatPhong });
            if (isDatPhongExists == 0)
                return NotFound(new { Message = "Mã đặt phòng không tồn tại." });

            // Kiểm tra xem MaDichVu có tồn tại không
            const string checkDichVuQuery = "SELECT SoLuong FROM DichVu WHERE MaDichVu = @MaDichVu";
            var dichVuSoLuong = await _db.ExecuteScalarAsync<int?>(checkDichVuQuery, new { datDichVuDTO.MaDichVu });
            if (dichVuSoLuong == null)
                return NotFound(new { Message = "Mã dịch vụ không tồn tại." });

            // Kiểm tra số lượng dịch vụ còn lại
            if (datDichVuDTO.SoLuong > dichVuSoLuong)
                return BadRequest(new { Message = "Số lượng dịch vụ yêu cầu vượt quá số lượng còn lại." });

            // Tạo mã đặt dịch vụ
            datDichVuDTO.MaDatDichVu = await GenerateUniqueMaDatDichVu();

            // Chèn dữ liệu vào bảng DatDichVu
            const string insertQuery = @"
                INSERT INTO DatDichVu (MaDatDichVu, MaDatPhong, MaDichVu, SoLuong, MaHoaDon)
                VALUES (@MaDatDichVu, @MaDatPhong, @MaDichVu, @SoLuong, @MaHoaDon)";
            await _db.ExecuteAsync(insertQuery, datDichVuDTO);

            // Cập nhật số lượng dịch vụ còn lại
            const string updateDichVuQuery = "UPDATE DichVu SET SoLuong = SoLuong - @SoLuong WHERE MaDichVu = @MaDichVu";
            await _db.ExecuteAsync(updateDichVuQuery, new { datDichVuDTO.SoLuong, datDichVuDTO.MaDichVu });

            return Ok(new { Message = "Đặt dịch vụ thành công.", MaDatDichVu = datDichVuDTO.MaDatDichVu });
        }

        /// <summary>
        /// Lấy lịch sử đặt dịch vụ cho một đơn đặt phòng.
        /// </summary>
        [HttpGet("dichvu/lichsu")]
        public async Task<IActionResult> LichSuDatDichVu([FromQuery] string maDatPhong)
        {
            const string query = @"
                SELECT ddv.MaDatDichVu, ddv.MaDatPhong, ddv.MaDichVu, ddv.SoLuong, ddv.MaHoaDon,
                       dv.TenDichVu, dv.DonGia, dv.MoTaDichVu, dv.HinhAnhDichVu
                FROM DatDichVu ddv
                INNER JOIN DichVu dv ON ddv.MaDichVu = dv.MaDichVu
                WHERE ddv.MaDatPhong = @MaDatPhong";

            var result = await _db.QueryAsync(query, new { MaDatPhong = maDatPhong });

            return Ok(result);
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