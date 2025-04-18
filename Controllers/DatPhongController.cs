// Các controller dưới đây viết theo cấu trúc giống KhachHangController mẫu, sử dụng Dapper
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementAPI.Controllers
{
    public class DatPhongDTO
{
    public string MaDatPhong { get; set; }
    public string MaKhachHang { get; set; }
    public string MaNhanVien { get; set; }
    public string MaPhong { get; set; }
    public DateTime NgayNhanPhong { get; set; }
    public DateTime NgayTraPhong { get; set; }
    public string TrangThai { get; set; } // '1': Đặt, '2': Hủy
}

    [ApiController]
[Route("api/[controller]")]
public class DatPhongController : ControllerBase
{
    private readonly IDbConnection _db;

    public DatPhongController(IDbConnection db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _db.QueryAsync("SELECT * FROM DatPhong");
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var query = "SELECT * FROM DatPhong WHERE MaDatPhong = @Id";
        var result = await _db.QueryFirstOrDefaultAsync(query, new { Id = id });
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(DatPhongDTO datPhong)
    {
        const string sql = @"
            INSERT INTO DatPhong 
            (MaDatPhong, MaKhachHang, MaNhanVien, MaPhong, NgayNhanPhong, NgayTraPhong, TrangThai) 
            VALUES (@MaDatPhong, @MaKhachHang, @MaNhanVien, @MaPhong, @NgayNhanPhong, @NgayTraPhong, @TrangThai)";
        
        await _db.ExecuteAsync(sql, datPhong);
        return CreatedAtAction(nameof(GetById), new { id = datPhong.MaDatPhong }, datPhong);
    }
}

    }
