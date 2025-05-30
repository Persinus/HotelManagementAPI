//Mục đích: Thanh toán hóa đơn

//POST /api/thanhtoan – Thanh toán hóa đơn

//GET /api/thanhtoan/lichsu – Lịch sử thanh toán

using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using System.Security.Claims;

namespace HotelManagementAPI.Controllers.KhachHang
{
    [ApiController]
    [Route("api/KhachHang/thanhtoan")]
    public class KhachHang_ThanhToanController : ControllerBase
    {
        private readonly IDbConnection _db;

        public KhachHang_ThanhToanController(IDbConnection db)
        {
            _db = db;
        }

        // POST /api/KhachHang/thanhtoan
        [HttpPost]
        public async Task<IActionResult> ThanhToan([FromBody] ThanhToanDTO request)
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            // Kiểm tra quyền sở hữu hóa đơn
            const string checkQuery = "SELECT MaNguoiDung FROM HoaDon WHERE MaHoaDon = @MaHoaDon";
            var owner = await _db.ExecuteScalarAsync<string>(checkQuery, new { request.MaHoaDon });
            if (owner != maNguoiDung)
                return Forbid("Bạn không có quyền thanh toán hóa đơn này.");

            // Cập nhật hóa đơn: NgayThanhToan = thời gian hiện tại, TinhTrangHoaDon = 2 (đã thanh toán)
            const string updateHoaDonQuery = @"
                UPDATE HoaDon
                SET NgayThanhToan = @NgayThanhToan, TinhTrangHoaDon = 2
                WHERE MaHoaDon = @MaHoaDon";
            var now = DateTime.Now;
            await _db.ExecuteAsync(updateHoaDonQuery, new { NgayThanhToan = now, request.MaHoaDon });

            // Thêm bản ghi vào bảng ThanhToan
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

        //GET /api/KhachHang/thanhtoan/lichsu – Lịch sử thanh toán
        [HttpGet("lichsu")]
        public async Task<IActionResult> LichSuThanhToan()
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            // Lấy các thanh toán của user dựa vào các hóa đơn thuộc user đó
            const string query = @"
                SELECT t.*
                FROM ThanhToan t
                INNER JOIN HoaDon h ON t.MaHoaDon = h.MaHoaDon
                WHERE h.MaNguoiDung = @MaNguoiDung
                ORDER BY t.NgayThanhToan DESC";

            var lichSu = await _db.QueryAsync<ThanhToanDTO>(query, new { MaNguoiDung = maNguoiDung });
            return Ok(lichSu);
        }
    }
}