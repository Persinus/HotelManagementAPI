using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.Models;

namespace HotelManagementAPI.Controllers
{
    public class NhanVienDTO
    {
        public string MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string CanCuocCongDan { get; set; }
        public string HinhAnh { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class NhanVienController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public NhanVienController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Lấy tất cả nhân viên
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVienDTO>>> GetAll()
        {
            const string query = "SELECT * FROM NhanVien";
            var result = await _dbConnection.QueryAsync<NhanVienDTO>(query);
            return Ok(result);
        }

        // Lấy thông tin theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVienDTO>> GetById(string id)
        {
            const string query = "SELECT * FROM NhanVien WHERE MaNhanVien = @Id";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<NhanVienDTO>(query, new { Id = id });

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // Tạo nhân viên mới
        [HttpPost]
        public async Task<ActionResult<NhanVienDTO>> Create([FromBody] NhanVienDTO nhanVienDto)
        {
            const string query = @"
                INSERT INTO NhanVien (MaNhanVien, HoTen, SoDienThoai, CanCuocCongDan, HinhAnh)
                VALUES (@MaNhanVien, @HoTen, @SoDienThoai, @CanCuocCongDan, @HinhAnh)";

            await _dbConnection.ExecuteAsync(query, nhanVienDto);
            return CreatedAtAction(nameof(GetById), new { id = nhanVienDto.MaNhanVien }, nhanVienDto);
        }

        // Cập nhật thông tin nhân viên
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] NhanVienDTO nhanVienDto)
        {
            if (id != nhanVienDto.MaNhanVien)
                return BadRequest();

            const string query = @"
                UPDATE NhanVien
                SET HoTen = @HoTen,
                    SoDienThoai = @SoDienThoai,
                    CanCuocCongDan = @CanCuocCongDan,
                    HinhAnh = @HinhAnh
                WHERE MaNhanVien = @MaNhanVien";

            var affectedRows = await _dbConnection.ExecuteAsync(query, nhanVienDto);

            if (affectedRows == 0)
                return NotFound();

            return NoContent();
        }

        // Xoá nhân viên
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            const string query = "DELETE FROM NhanVien WHERE MaNhanVien = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(query, new { Id = id });

            if (affectedRows == 0)
                return NotFound();

            return NoContent();
        }
    }
}