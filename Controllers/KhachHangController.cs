using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.Models;

namespace HotelManagementAPI.Controllers
{
    public class KhachHangDTO
    {
        public string MaKhachHang { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class KhachHangController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public KhachHangController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Lấy tất cả khách hàng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhachHangDTO>>> GetAll()
        {
            const string query = "SELECT * FROM KhachHang";
            var result = await _dbConnection.QueryAsync<KhachHangDTO>(query);
            return Ok(result);
        }

        // Lấy theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<KhachHangDTO>> GetById(string id)
        {
            const string query = "SELECT * FROM KhachHang WHERE MaKhachHang = @Id";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<KhachHangDTO>(query, new { Id = id });

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // Tạo khách hàng mới
        [HttpPost]
        public async Task<ActionResult<KhachHangDTO>> Create([FromBody] KhachHangDTO khachHangDto)
        {
            const string query = @"
                INSERT INTO KhachHang (MaKhachHang, TenKhachHang, DiaChi, SoDienThoai)
                VALUES (@MaKhachHang, @HoTen, @DiaChi, @SoDienThoai)";
    
            await _dbConnection.ExecuteAsync(query, khachHangDto);
            return CreatedAtAction(nameof(GetById), new { id = khachHangDto.MaKhachHang }, khachHangDto);
        }

        // Cập nhật khách hàng
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] KhachHangDTO khachHangDto)
        {
            if (id != khachHangDto.MaKhachHang)
                return BadRequest();

            const string query = @"
                UPDATE KhachHang
                SET TenKhachHang = @HoTen,
                    DiaChi = @DiaChi,
                    SoDienThoai = @SoDienThoai
                WHERE MaKhachHang = @MaKhachHang";

            var affectedRows = await _dbConnection.ExecuteAsync(query, khachHangDto);

            if (affectedRows == 0)
                return NotFound();

            return NoContent();
        }

        // Xoá khách hàng
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            const string query = "DELETE FROM KhachHang WHERE MaKhachHang = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(query, new { Id = id });

            if (affectedRows == 0)
                return NotFound();

            return NoContent();
        }
    }
}