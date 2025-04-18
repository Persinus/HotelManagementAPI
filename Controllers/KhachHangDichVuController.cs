using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementAPI.Controllers
{
     public class KhachHangDichVuDTO
    {
        public string Id { get; set; }
        public string MaKhachHang { get; set; }
        public string MaChiTietDichVu { get; set; }
        public int SoLuong { get; set; }
    }
    [ApiController]
    [Route("api/[controller]")]
    public class KhachHangDichVuController : ControllerBase
    {
        private readonly IDbConnection _db;

        public KhachHangDichVuController(IDbConnection db) => _db = db;

        // Lấy tất cả dữ liệu từ bảng KhachHang_DichVu
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string sql = "SELECT * FROM KhachHang_DichVu";
            var result = await _db.QueryAsync<KhachHangDichVuDTO>(sql); // Sử dụng DTO
            return Ok(result);
        }

        // Lấy dữ liệu từ bảng KhachHang_DichVu theo MaKhachHang
        [HttpGet("{maKhachHang}")]
        public async Task<IActionResult> GetByMaKhachHang(string maKhachHang)
        {
            const string sql = "SELECT * FROM KhachHang_DichVu WHERE MaKhachHang = @MaKhachHang";
            var result = await _db.QueryAsync<KhachHangDichVuDTO>(sql, new { MaKhachHang = maKhachHang }); // Sử dụng DTO
            return Ok(result);
        }

        // Thêm mới bản ghi vào bảng KhachHang_DichVu
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KhachHangDichVuDTO khdv)
        {
            const string sql = "INSERT INTO KhachHang_DichVu (MaKhachHang, MaChiTietDichVu, SoLuong) VALUES (@MaKhachHang, @MaChiTietDichVu, @SoLuong)";
            await _db.ExecuteAsync(sql, khdv); // Sử dụng DTO
            return Ok(khdv);
        }

        // Cập nhật bản ghi trong bảng KhachHang_DichVu
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] KhachHangDichVuDTO khdv)
        {
            const string sql = "UPDATE KhachHang_DichVu SET MaKhachHang = @MaKhachHang, MaChiTietDichVu = @MaChiTietDichVu, SoLuong = @SoLuong WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { khdv.MaKhachHang, khdv.MaChiTietDichVu, khdv.SoLuong, Id = id }); // Sử dụng DTO
            return Ok(khdv);
        }

        // Xoá bản ghi trong bảng KhachHang_DichVu
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            const string sql = "DELETE FROM KhachHang_DichVu WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
            return Ok(new { Message = "Deleted successfully" });
        }
    }
}
