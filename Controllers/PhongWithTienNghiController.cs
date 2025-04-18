using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.Models;

// DTO cho PhongWithTienNghi
public class PhongWithTienNghiDTO
{
    public string MaPhong { get; set; }
    public string LoaiPhong { get; set; }
    public decimal GiaPhong { get; set; }
    public string TinhTrang { get; set; }
    public int SoLuongPhong { get; set; }
    public int Tang { get; set; }
    public string KieuGiuong { get; set; }
    public string MoTa { get; set; }
    public string UrlAnhChinh { get; set; }
    public string UrlAnhPhu1 { get; set; }
    public string UrlAnhPhu2 { get; set; }
    public string TienNghi { get; set; }
}

namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhongWithTienNghiController : ControllerBase
    {
        private readonly IDbConnection _db;

        public PhongWithTienNghiController(IDbConnection db)
        {
            _db = db;
        }

        // Lấy tất cả phòng với tiện nghi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhongWithTienNghiDTO>>> GetAll()
        {
            const string query = "SELECT * FROM PhongWithTienNghi";
            var result = await _db.QueryAsync<PhongWithTienNghiDTO>(query);
            return Ok(result);
        }

        // Lấy theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PhongWithTienNghiDTO>> GetById(string id)
        {
            const string query = "SELECT * FROM PhongWithTienNghi WHERE MaPhong = @Id";
            var result = await _db.QueryFirstOrDefaultAsync<PhongWithTienNghiDTO>(query, new { Id = id });

            return result == null ? NotFound() : Ok(result);
        }

        // Tạo mới phòng với tiện nghi
        [HttpPost]
        public async Task<ActionResult<PhongWithTienNghiDTO>> Create([FromBody] PhongWithTienNghiDTO phong)
        {
            const string query = @"
                INSERT INTO PhongWithTienNghi (
                    MaPhong, LoaiPhong, GiaPhong, TinhTrang,
                    SoLuongPhong, Tang, KieuGiuong, MoTa,
                    UrlAnhChinh, UrlAnhPhu1, UrlAnhPhu2, TienNghi
                )
                VALUES (
                    @MaPhong, @LoaiPhong, @GiaPhong, @TinhTrang,
                    @SoLuongPhong, @Tang, @KieuGiuong, @MoTa,
                    @UrlAnhChinh, @UrlAnhPhu1, @UrlAnhPhu2, @TienNghi)";

            await _db.ExecuteAsync(query, phong);
            return CreatedAtAction(nameof(GetById), new { id = phong.MaPhong }, phong);
        }

        // Cập nhật thông tin phòng
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PhongWithTienNghiDTO phong)
        {
            if (id != phong.MaPhong) return BadRequest();

            const string query = @"
                UPDATE PhongWithTienNghi
                SET LoaiPhong = @LoaiPhong,
                    GiaPhong = @GiaPhong,
                    TinhTrang = @TinhTrang,
                    SoLuongPhong = @SoLuongPhong,
                    Tang = @Tang,
                    KieuGiuong = @KieuGiuong,
                    MoTa = @MoTa,
                    UrlAnhChinh = @UrlAnhChinh,
                    UrlAnhPhu1 = @UrlAnhPhu1,
                    UrlAnhPhu2 = @UrlAnhPhu2,
                    TienNghi = @TienNghi
                WHERE MaPhong = @MaPhong";

            var affected = await _db.ExecuteAsync(query, phong);
            return affected > 0 ? NoContent() : NotFound();
        }

        // Xoá phòng
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            const string query = "DELETE FROM PhongWithTienNghi WHERE MaPhong = @Id";
            var affected = await _db.ExecuteAsync(query, new { Id = id });
            return affected > 0 ? NoContent() : NotFound();
        }
    }
}