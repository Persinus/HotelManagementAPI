using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using HotelManagementAPI.DTOs;
using System.Security.Claims;



namespace HotelManagementAPI.Controllers.KhachHang
{
    [ApiController]
    [Route("api/KhachHang/hoadon")]
    public class KhachHang_HoaDonController : ControllerBase
    {
        private readonly IDbConnection _db;

        public KhachHang_HoaDonController(IDbConnection db)
        {
            _db = db;
        }

        
        /// <summary>
        /// Tra cứu hóa đơn theo mã đặt phòng.
        /// </summary>
        [HttpGet("by-madatphong/{maDatPhong}")]
        public async Task<IActionResult> GetHoaDonByMaDatPhong(string maDatPhong)
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            // Kiểm tra quyền sở hữu mã đặt phòng
            const string checkQuery = "SELECT MaNguoiDung FROM DatPhong WHERE MaDatPhong = @MaDatPhong";
            var owner = await _db.ExecuteScalarAsync<string>(checkQuery, new { MaDatPhong = maDatPhong });
            if (owner != maNguoiDung)
                return Forbid("Bạn không có quyền xem hóa đơn này.");

            const string query = @"SELECT * FROM HoaDon WHERE MaDatPhong = @MaDatPhong";
            var hoaDon = await _db.QueryFirstOrDefaultAsync<HoaDonDTO>(query, new { MaDatPhong = maDatPhong });

            if (hoaDon == null)
                return NotFound(new { Message = "Không tìm thấy hóa đơn cho mã đặt phòng này." });

            return Ok(hoaDon);
        }

        /// <summary>
        /// Tạo hóa đơn mới.
        /// </summary>
        [HttpPost("tao")]
        public async Task<IActionResult> TaoHoaDon([FromBody] TaoHoaDonRequestDTO request)
        {
            // Lấy thông tin đặt phòng
            const string datPhongQuery = @"
                SELECT dp.MaDatPhong, dp.MaNguoiDung, dp.MaPhong, dp.NgayCheckIn, dp.NgayCheckOut
                FROM DatPhong dp
                WHERE dp.MaDatPhong = @MaDatPhong";
            var datPhong = await _db.QueryFirstOrDefaultAsync<DatPhongDTO>(datPhongQuery, new { request.MaDatPhong });

            if (datPhong == null)
                return NotFound(new { Message = "Không tìm thấy mã đặt phòng." });

            // Lấy giá phòng
            const string getGiaPhongQuery = "SELECT GiaPhong FROM Phong WHERE MaPhong = @MaPhong";
            var giaPhong = await _db.ExecuteScalarAsync<decimal>(getGiaPhongQuery, new { datPhong.MaPhong });

            // Tính số ngày ở (tối thiểu 1 ngày)
            var soNgay = (datPhong.NgayCheckOut.Date - datPhong.NgayCheckIn.Date).Days;
            if (soNgay < 1) soNgay = 1;

            // Tính tổng tiền dịch vụ đi kèm
            const string getDichVuQuery = @"
                SELECT ddv.SoLuong, dv.DonGia
                FROM DatDichVu ddv
                INNER JOIN DichVu dv ON ddv.MaDichVu = dv.MaDichVu
                WHERE ddv.MaDatPhong = @MaDatPhong";
            var dichVus = await _db.QueryAsync<(int SoLuong, decimal DonGia)>(getDichVuQuery, new { datPhong.MaDatPhong });
            decimal tongTienDichVu = dichVus.Sum(x => x.SoLuong * x.DonGia);

            // Tổng tiền = tiền phòng * số ngày + tổng tiền dịch vụ
            var tongTien = giaPhong * soNgay + tongTienDichVu;

            // Tạo mã hóa đơn mới
            string maHoaDon = await GenerateUniqueMaHoaDon();

            // Tạo hóa đơn mới
            const string insertHoaDonQuery = @"
                INSERT INTO HoaDon (MaHoaDon, MaNguoiDung, MaDatPhong, TongTien, NgayTaoHoaDon, NgayThanhToan, TinhTrangHoaDon)
                VALUES (@MaHoaDon, @MaNguoiDung, @MaDatPhong, @TongTien, @NgayTaoHoaDon, @NgayThanhToan, @TinhTrangHoaDon)";
            var hoaDonDTO = new HoaDonDTO
            {
                MaHoaDon = maHoaDon,
                MaNguoiDung = datPhong.MaNguoiDung!,
                MaDatPhong = datPhong.MaDatPhong!,
                TongTien = tongTien,
                NgayTaoHoaDon = DateTime.Now,
                NgayThanhToan = null, // Chưa thanh toán
                TinhTrangHoaDon = 1 // 1: Chưa thanh toán, 2: Đã thanh toán
            };
            await _db.ExecuteAsync(insertHoaDonQuery, hoaDonDTO);

            return Ok(hoaDonDTO);
        }

        // Helper tạo mã hóa đơn mới
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