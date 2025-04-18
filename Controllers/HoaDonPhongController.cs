// Các controller dưới đây viết theo cấu trúc giống KhachHangController mẫu, sử dụng Dapper
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementAPI.Controllers
{
    public class HoaDonPhongDTO
    {
        public string MaHoaDon { get; set; }
        public string MaDatPhong { get; set; }
        public string MaNhanVien { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThaiThanhToan { get; set; }
        public string  HinhThucThanhToan { get; set; }
    }

  
    [ApiController]
    [Route("api/[controller]")]
    public class HoaDonPhongController : ControllerBase
    {
        private readonly IDbConnection _db;

        public HoaDonPhongController(IDbConnection db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.QueryAsync("SELECT * FROM HoaDonThanhToanPhong"));

        [HttpPost]
        public async Task<IActionResult> Create(HoaDonPhongDTO hd)
        {
            const string sql = "INSERT INTO HoaDonThanhToanPhong (MaHoaDon, MaDatPhong, MaNhanVien, TongTien, TrangThaiThanhToan, HinhThucThanhToan) VALUES (@MaHoaDon, @MaDatPhong, @MaNhanVien, @TongTien, @TrangThaiThanhToan, @HinhThucThanhToan)";
            await _db.ExecuteAsync(sql, hd);
            return CreatedAtAction(nameof(GetAll), new { id = hd.MaHoaDon }, hd);
        }
    }
    }
    