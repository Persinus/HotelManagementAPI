using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.Models;

namespace HotelManagementAPI.Controllers
{
    public class DichVuDTO
{
    public string MaChiTietDichVu { get; set; }
    public string TenDichVu { get; set; }
    public decimal DonGia { get; set; }
    public string? MoTaDichVu { get; set; }
    public string? UrlAnh { get; set; }
}
    [ApiController]
    [Route("api/[controller]")]
    public class DichVuController : ControllerBase
    {
        private readonly IDbConnection _db;

        public DichVuController(IDbConnection db)
        {
            _db = db;
        }

        // Lấy tất cả dịch vụ
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DichVuDTO>>> GetAll()
        {
            const string query = "SELECT * FROM DichVu";
            var result = await _db.QueryAsync<DichVuDTO>(query);
            return Ok(result);
        }

        // Lấy dịch vụ theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<DichVuDTO>> GetById(string id)
        {
            const string query = "SELECT * FROM DichVu WHERE MaChiTietDichVu = @Id";
            var result = await _db.QueryFirstOrDefaultAsync<DichVuDTO>(query, new { Id = id });

            return result == null ? NotFound() : Ok(result);
        }

        // Tạo dịch vụ mới
        [HttpPost]
        public async Task<ActionResult<DichVuDTO>> Create([FromBody] DichVuDTO dichVuDto)
        {
            const string query = @"
                INSERT INTO DichVu (MaChiTietDichVu, TenDichVu, DonGia, MoTaDichVu, UrlAnh)
                VALUES (@MaChiTietDichVu, @TenDichVu, @DonGia, @MoTaDichVu, @UrlAnh)";

            await _db.ExecuteAsync(query, dichVuDto);
            return CreatedAtAction(nameof(GetById), new { id = dichVuDto.MaChiTietDichVu }, dichVuDto);
        }

        // Cập nhật dịch vụ
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] DichVuDTO dichVuDto)
        {
            if (id != dichVuDto.MaChiTietDichVu) return BadRequest();

            const string query = @"
                UPDATE DichVu
                SET TenDichVu = @TenDichVu,
                    DonGia = @DonGia,
                    MoTaDichVu = @MoTaDichVu,
                    UrlAnh = @UrlAnh
                WHERE MaChiTietDichVu = @MaChiTietDichVu";

            var rows = await _db.ExecuteAsync(query, dichVuDto);
            return rows == 0 ? NotFound() : NoContent();
        }

        // Xoá dịch vụ
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            const string query = "DELETE FROM DichVu WHERE MaChiTietDichVu = @Id";
            var rows = await _db.ExecuteAsync(query, new { Id = id });
            return rows == 0 ? NotFound() : NoContent();
        }
    }
}