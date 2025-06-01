using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/khachhang")]
    [Authorize (Roles = "KhachHang")]
    public class KhachHangController : ControllerBase
    {
        private readonly IDbConnection _db;

        public KhachHangController(IDbConnection db)
        {
            _db = db;
        }

        // ----------- ĐẶT PHÒNG -----------
        /// <summary>
        /// Đặt phòng mới cho khách hàng.
        /// </summary>
        /// <remarks>
        /// Tạo đơn đặt phòng mới, kiểm tra trạng thái phòng và dịch vụ đi kèm.
        /// </remarks>
        [HttpPost("datphong")]
        public async Task<IActionResult> TaoDonDatPhong([FromBody] KhachHangDatPhongDTO datPhongDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
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
                return BadRequest(new { Message = "Phòng này bạn đã đặt sang hóa đơn để xem." });

            // Kiểm tra dịch vụ đi kèm (nếu có)
            if (datPhongDTO.DichVuDiKem != null && datPhongDTO.DichVuDiKem.Any())
            {
                foreach (var dv in datPhongDTO.DichVuDiKem)
                {
                    if (dv.SoLuong <= 0)
                        return BadRequest(new { Message = $"Số lượng dịch vụ {dv.MaDichVu} phải lớn hơn 0." });

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
                    const string getDonGiaQuery = "SELECT DonGia FROM DichVu WHERE MaDichVu = @MaDichVu";
                    var donGia = await _db.ExecuteScalarAsync<decimal>(getDonGiaQuery, new { dv.MaDichVu });
                    tongTienDichVu += donGia * dv.SoLuong;
                }
            }

            var tongTien = giaPhong * soNgay + tongTienDichVu;

            // Thêm đơn đặt phòng
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

                    const string updateDichVuQuery = "UPDATE DichVu SET SoLuong = SoLuong - @SoLuong WHERE MaDichVu = @MaDichVu";
                    await _db.ExecuteAsync(updateDichVuQuery, new { dv.SoLuong, dv.MaDichVu });
                }
            }

            return Ok(new
            {
                Message = $"Đặt phòng thành công. Tổng tiền tạm tính là: {tongTien:N0} VNĐ. Hãy sang phần tạo hóa đơn, nhập mã đặt phòng để nhận các thông tin thanh toán chi tiết.",
                MaDatPhong = datPhongDTO.MaDatPhong,
                TongTienTamTinh = tongTien
            });
        }

        /// <summary>
        /// Lấy lịch sử đặt phòng của khách hàng.
        /// </summary>
        [HttpGet("datphong/lichsu")]
        public async Task<IActionResult> LichSuDatPhong()
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
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
        /// Hủy đơn đặt phòng theo mã.
        /// </summary>
        /// <param name="id">Mã đặt phòng</param>
        [HttpDelete("datphong/{id}")]
        public async Task<IActionResult> HuyDatPhong(string id)
        {
            const string checkQuery = "SELECT COUNT(1) FROM DatPhong WHERE MaDatPhong = @MaDatPhong";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaDatPhong = id });
            if (isExists == 0)
                return NotFound(new { Message = "Mã đặt phòng không tồn tại." });

            const string deleteQuery = "DELETE FROM DatPhong WHERE MaDatPhong = @MaDatPhong";
            await _db.ExecuteAsync(deleteQuery, new { MaDatPhong = id });

            return Ok(new { Message = "Hủy đặt phòng thành công." });
        }

        // ----------- HÓA ĐƠN -----------
        /// <summary>
        /// Lấy hóa đơn theo mã đặt phòng.
        /// </summary>
        /// <param name="maDatPhong">Mã đặt phòng</param>
        [HttpGet("hoadon/by-madatphong/{maDatPhong}")]
        public async Task<IActionResult> GetHoaDonByMaDatPhong(string maDatPhong)
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            const string checkQuery = "SELECT MaNguoiDung FROM DatPhong WHERE MaDatPhong = @MaDatPhong";
            var owner = await _db.ExecuteScalarAsync<string>(checkQuery, new { MaDatPhong = maDatPhong });
            if (owner != maNguoiDung)
                return Forbid("Bạn không có quyền xem hóa đơn này.");

            const string query = @"SELECT * FROM HoaDon WHERE MaDatPhong = @MaDatPhong";
            var hoaDon = await _db.QueryFirstOrDefaultAsync<KhachHangHoaDonDTO>(query, new { MaDatPhong = maDatPhong });

            if (hoaDon == null)
                return NotFound(new { Message = "Không tìm thấy hóa đơn cho mã đặt phòng này." });

            return Ok(hoaDon);
        }

        /// <summary>
        /// Tạo hóa đơn mới cho đơn đặt phòng.
        /// </summary>
        [HttpPost("hoadon/tao")]
        public async Task<IActionResult> TaoHoaDon([FromBody] TaoHoaDonRequestDTO request)
        {
            const string datPhongQuery = @"
                SELECT dp.MaDatPhong, dp.MaNguoiDung, dp.MaPhong, dp.NgayCheckIn, dp.NgayCheckOut
                FROM DatPhong dp
                WHERE dp.MaDatPhong = @MaDatPhong";
            var datPhong = await _db.QueryFirstOrDefaultAsync<KhachHangDatPhongDTO>(datPhongQuery, new { request.MaDatPhong });

            if (datPhong == null)
                return NotFound(new { Message = "Không tìm thấy mã đặt phòng." });

            // Kiểm tra quyền sở hữu
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (datPhong.MaNguoiDung != maNguoiDung)
                return Forbid("Bạn không có quyền tạo hóa đơn cho mã đặt phòng này.");

            const string getGiaPhongQuery = "SELECT GiaPhong FROM Phong WHERE MaPhong = @MaPhong";
            var giaPhong = await _db.ExecuteScalarAsync<decimal>(getGiaPhongQuery, new { datPhong.MaPhong });

            var soNgay = (datPhong.NgayCheckOut.Date - datPhong.NgayCheckIn.Date).Days;
            if (soNgay < 1) soNgay = 1;

            const string getDichVuQuery = @"
                SELECT ddv.SoLuong, dv.DonGia
                FROM DatDichVu ddv
                INNER JOIN DichVu dv ON ddv.MaDichVu = dv.MaDichVu
                WHERE ddv.MaDatPhong = @MaDatPhong";
            var dichVus = await _db.QueryAsync<(int SoLuong, decimal DonGia)>(getDichVuQuery, new { datPhong.MaDatPhong });
            decimal tongTienDichVu = dichVus.Sum(x => x.SoLuong * x.DonGia);

            var tongTien = giaPhong * soNgay + tongTienDichVu;

            string maHoaDon = await GenerateUniqueMaHoaDon();

            const string insertHoaDonQuery = @"
                INSERT INTO HoaDon (MaHoaDon, MaNguoiDung, MaDatPhong, TongTien, NgayTaoHoaDon, NgayThanhToan, TinhTrangHoaDon)
                VALUES (@MaHoaDon, @MaNguoiDung, @MaDatPhong, @TongTien, @NgayTaoHoaDon, @NgayThanhToan, @TinhTrangHoaDon)";
            var hoaDonDTO = new KhachHangHoaDonDTO
            {
                MaHoaDon = maHoaDon,
                MaNguoiDung = datPhong.MaNguoiDung!,
                MaDatPhong = datPhong.MaDatPhong!,
                TongTien = tongTien,
                NgayTaoHoaDon = DateTime.Now,
                NgayThanhToan = null,
                TinhTrangHoaDon = 1
            };
            await _db.ExecuteAsync(insertHoaDonQuery, hoaDonDTO);

            return Ok(hoaDonDTO);
        }

        // ----------- THANH TOÁN -----------
        /// <summary>
        /// Thanh toán hóa đơn.
        /// </summary>
        [HttpPost("thanhtoan")]
        public async Task<IActionResult> ThanhToan([FromBody] KhachHangThanhToanDTO request)
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            const string checkQuery = "SELECT MaNguoiDung FROM HoaDon WHERE MaHoaDon = @MaHoaDon";
            var owner = await _db.ExecuteScalarAsync<string>(checkQuery, new { request.MaHoaDon });
            if (owner != maNguoiDung)
                return Forbid("Bạn không có quyền thanh toán hóa đơn này.");

            const string updateHoaDonQuery = @"
                UPDATE HoaDon
                SET NgayThanhToan = @NgayThanhToan, TinhTrangHoaDon = 2
                WHERE MaHoaDon = @MaHoaDon";
            var now = DateTime.Now;
            await _db.ExecuteAsync(updateHoaDonQuery, new { NgayThanhToan = now, request.MaHoaDon });

            const string insertThanhToanQuery = @"
                INSERT INTO ThanhToan (MaThanhToan, MaHoaDon, SoTienThanhToan, NgayThanhToan, PhuongThucThanhToan, TinhTrangThanhToan)
                VALUES (@MaThanhToan, @MaHoaDon, @SoTienThanhToan, @NgayThanhToan, @PhuongThucThanhToan, @TinhTrangThanhToan)";
            await _db.ExecuteAsync(insertThanhToanQuery, new
            {
                request.MaThanhToan,
                request.MaHoaDon,
                request.SoTienThanhToan,
                NgayThanhToan = now,
                request.PhuongThucThanhToan,
                request.TinhTrangThanhToan
            });

            return Ok(new { Message = "Thanh toán thành công!", NgayThanhToan = now });
        }

        /// <summary>
        /// Lấy lịch sử thanh toán của khách hàng.
        /// </summary>
        [HttpGet("thanhtoan/lichsu")]
        public async Task<IActionResult> LichSuThanhToan()
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            const string query = @"
                SELECT t.*
                FROM ThanhToan t
                INNER JOIN HoaDon h ON t.MaHoaDon = h.MaHoaDon
                WHERE h.MaNguoiDung = @MaNguoiDung
                ORDER BY t.NgayThanhToan DESC";

            var lichSu = await _db.QueryAsync<KhachHangThanhToanDTO>(query, new { MaNguoiDung = maNguoiDung });
            return Ok(lichSu);
        }

        /// <summary>
        /// Lấy tổng hợp lịch sử giao dịch của khách hàng.
        /// </summary>
        [HttpGet("lichsu")]
        public async Task<IActionResult> GetLichSuGiaoDich()
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            const string datPhongQuery = @"
                SELECT MaDatPhong, MaPhong, NgayDat, NgayCheckIn, NgayCheckOut, TinhTrangDatPhong
                FROM DatPhong
                WHERE MaNguoiDung = @MaNguoiDung";
            var datPhongs = await _db.QueryAsync(datPhongQuery, new { MaNguoiDung = maNguoiDung });

            const string hoaDonQuery = @"
                SELECT MaHoaDon, MaDatPhong, TongTien, NgayTaoHoaDon, NgayThanhToan, TinhTrangHoaDon
                FROM HoaDon
                WHERE MaNguoiDung = @MaNguoiDung";
            var hoaDons = await _db.QueryAsync(hoaDonQuery, new { MaNguoiDung = maNguoiDung });

            const string thanhToanQuery = @"
                SELECT t.MaThanhToan, t.MaHoaDon, t.SoTienThanhToan, t.NgayThanhToan, t.PhuongThucThanhToan, t.TinhTrangThanhToan
                FROM ThanhToan t
                INNER JOIN HoaDon h ON t.MaHoaDon = h.MaHoaDon
                WHERE h.MaNguoiDung = @MaNguoiDung";
            var thanhToans = await _db.QueryAsync(thanhToanQuery, new { MaNguoiDung = maNguoiDung });

            return Ok(new
            {
                DatPhongs = datPhongs,
                HoaDons = hoaDons,
                ThanhToans = thanhToans
            });
        }

        // ----------- Helper methods -----------
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

        private async Task<string> GenerateUniqueMaHoaDon()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaHoaDon, 3, LEN(MaHoaDon) - 2) AS INT)), 0) + 1
                FROM HoaDon";
            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"HD{nextId:D3}";
        }
    }
}