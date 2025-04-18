using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.Models;

namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuanTriVienController : ControllerBase
    {
        private readonly IDbConnection _db;

        public QuanTriVienController(IDbConnection db) => _db = db;

        // Lấy tất cả quản trị viên
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string sql = "SELECT * FROM QuanTriVien";
            var result = await _db.QueryAsync<QuanTriVien>(sql);
            return Ok(result);
        }

        // Lấy theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            const string sql = "SELECT * FROM QuanTriVien WHERE MaQuanTri = @Id";
            var result = await _db.QueryFirstOrDefaultAsync<QuanTriVien>(sql, new { Id = id });

            if (result == null) return NotFound();
            return Ok(result);
        }

        // Tạo quản trị viên mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] QuanTriVien model)
        {
            const string sql = @"
                INSERT INTO QuanTriVien (MaQuanTri, TenAdmin, SoDienThoai, NgayTao) 
                VALUES (@MaQuanTri, @TenAdmin, @SoDienThoai, GETDATE())";

            await _db.ExecuteAsync(sql, model);
            return CreatedAtAction(nameof(GetById), new { id = model.MaQuanTri }, model);
        }

        // Cập nhật thông tin quản trị viên
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] QuanTriVien model)
        {
            if (id != model.MaQuanTri) return BadRequest();

            const string sql = @"
                UPDATE QuanTriVien
                SET TenAdmin = @TenAdmin,
                    SoDienThoai = @SoDienThoai,
                    NgayCapNhat = GETDATE()
                WHERE MaQuanTri = @MaQuanTri";

            var rows = await _db.ExecuteAsync(sql, model);
            return rows == 0 ? NotFound() : NoContent();
        }

        // Xoá quản trị viên
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            const string sql = "DELETE FROM QuanTriVien WHERE MaQuanTri = @Id";
            var rows = await _db.ExecuteAsync(sql, new { Id = id });
            return rows == 0 ? NotFound() : NoContent();
        }
    }
}