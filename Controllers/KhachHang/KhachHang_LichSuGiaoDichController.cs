using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.Security.Claims;

namespace HotelManagementAPI.Controllers.KhachHang
{
    //Mục đích: Lịch sử hoạt động của người dùng

    //GET /api/lichsu – Lấy toàn bộ lịch sử giao dịch
    [ApiController]
    [Route("api/KhachHang/lichsu")]
    public class KhachHang_LichSuGiaoDichController : ControllerBase
    {
        private readonly IDbConnection _db;

        public KhachHang_LichSuGiaoDichController(IDbConnection db)
        {
            _db = db;
        }

        // GET /api/KhachHang/lichsu
        [HttpGet]
        public async Task<IActionResult> GetLichSuGiaoDich()
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            // Lấy lịch sử đặt phòng
            const string datPhongQuery = @"
                SELECT MaDatPhong, MaPhong, NgayDat, NgayCheckIn, NgayCheckOut, TinhTrangDatPhong
                FROM DatPhong
                WHERE MaNguoiDung = @MaNguoiDung";
            var datPhongs = await _db.QueryAsync(datPhongQuery, new { MaNguoiDung = maNguoiDung });

            // Lấy lịch sử hóa đơn
            const string hoaDonQuery = @"
                SELECT MaHoaDon, MaDatPhong, TongTien, NgayTaoHoaDon, NgayThanhToan, TinhTrangHoaDon
                FROM HoaDon
                WHERE MaNguoiDung = @MaNguoiDung";
            var hoaDons = await _db.QueryAsync(hoaDonQuery, new { MaNguoiDung = maNguoiDung });

            // Lấy lịch sử thanh toán
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
    }
}