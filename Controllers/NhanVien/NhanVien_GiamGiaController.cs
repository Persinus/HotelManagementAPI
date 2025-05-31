using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagementAPI.Controllers.NhanVien
{
    [ApiController]
    [Route("api/giamgia")]
    [Authorize(Roles = "NhanVien")]
    public class NhanVien_GiamGiaController : ControllerBase
    {
        private readonly IDbConnection _db;

        public NhanVien_GiamGiaController(IDbConnection db)
        {
            _db = db;
        }

        // GET: /api/giamgia – Danh sách chương trình giảm giá
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _db.QueryAsync<GiamGiaDTO>("SELECT * FROM GiamGia");
            return Ok(result);
        }

        // GET: /api/giamgia/{id} – Chi tiết mã giảm giá
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _db.QueryFirstOrDefaultAsync<GiamGiaDTO>(
                "SELECT * FROM GiamGia WHERE MaGiamGia = @id", new { id });
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}