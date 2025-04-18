using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.Models;

namespace HotelManagementAPI.Controllers
{
    public class HoaDonDichVuDTO
    {
        public string MaHoaDonDichVu { get; set; }
        public string MaKhachHang { get; set; }
        public string MaChiTietDichVu { get; set; }
        public string TrangThaiThanhToan { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
    }
    [ApiController]
    [Route("api/[controller]")]
    public class HoaDonDichVuController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public HoaDonDichVuController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaDonDichVuDTO>>> GetAll()
        {
            const string query = "SELECT * FROM HoaDonThanhToanDichVu";
            var result = await _dbConnection.QueryAsync<HoaDonDichVuDTO>(query);
            return Ok(result);
        }

        [HttpGet("{maHoaDonDichVu}")]
        public async Task<ActionResult<HoaDonDichVuDTO>> GetById(string maHoaDonDichVu)
        {
            const string query = "SELECT * FROM HoaDonThanhToanDichVu WHERE MaHoaDonDichVu = @Id";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<HoaDonDichVuDTO>(query, new { Id = maHoaDonDichVu });

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<HoaDonDichVuDTO>> Create(HoaDonDichVuDTO hoaDonDichVuDto)
        {
            const string query = @"
                INSERT INTO HoaDonThanhToanDichVu (MaHoaDonDichVu, MaKhachHang, MaChiTietDichVu, TrangThaiThanhToan, SoLuong, ThanhTien, NgayLapHoaDon)
                VALUES (@MaHoaDonDichVu, @MaKhachHang, @MaChiTietDichVu, @TrangThaiThanhToan, @SoLuong, @ThanhTien, @NgayLapHoaDon)";

            await _dbConnection.ExecuteAsync(query, hoaDonDichVuDto);
            return CreatedAtAction(nameof(GetById), new { maHoaDonDichVu = hoaDonDichVuDto.MaHoaDonDichVu }, hoaDonDichVuDto);
        }

        [HttpPut("{maHoaDonDichVu}")]
        public async Task<IActionResult> Update(string maHoaDonDichVu, HoaDonDichVuDTO hoaDonDichVuDto)
        {
            if (maHoaDonDichVu != hoaDonDichVuDto.MaHoaDonDichVu)
                return BadRequest();

            const string query = @"
                UPDATE HoaDonThanhToanDichVu 
                SET MaKhachHang = @MaKhachHang, 
                    MaChiTietDichVu = @MaChiTietDichVu,
                    TrangThaiThanhToan = @TrangThaiThanhToan,
                    SoLuong = @SoLuong,
                    ThanhTien = @ThanhTien,
                    NgayLapHoaDon = @NgayLapHoaDon 
                WHERE MaHoaDonDichVu = @MaHoaDonDichVu";

            var affectedRows = await _dbConnection.ExecuteAsync(query, hoaDonDichVuDto);

            if (affectedRows == 0)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{maHoaDonDichVu}")]
        public async Task<IActionResult> Delete(string maHoaDonDichVu)
        {
            const string query = "DELETE FROM HoaDonThanhToanDichVu WHERE MaHoaDonDichVu = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(query, new { Id = maHoaDonDichVu });

            if (affectedRows == 0)
                return NotFound();

            return NoContent();
        }
    }
}