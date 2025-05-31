//Mục đích: Quản lý dịch vụ

//GET /api/dichvu – Danh sách dịch vụ

//POST /api/dichvu – Thêm dịch vụ-- cái này cho quản trị viên

//PUT /api/dichvu/{id} – Cập nhật dịch vụ

//DELETE /api/dichvu/{id} – Xóa dịch vụ-- cái này cho quản trị viên

using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagementAPI.Controllers.NhanVien
{
    [ApiController]
    [Route("api/dichvu")]
    public class NhanVien_DichVuQuanLyController : ControllerBase
    {
        private readonly IDbConnection _db;

        public NhanVien_DichVuQuanLyController(IDbConnection db)
        {
            _db = db;
        }

        // GET: /api/dichvu
        [HttpGet]
        [Authorize(Roles = "NhanVien,QuanTriVien")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _db.QueryAsync<DichVuDTO>("SELECT * FROM DichVu");
            return Ok(result);
        }

        // POST: /api/dichvu (Chỉ quản trị viên)
        [HttpPost]
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> Create([FromBody] DichVuDTO dto)
        {
            dto.MaDichVu = await GenerateUniqueMaDichVu();
            const string sql = @"
                INSERT INTO DichVu (MaDichVu, TenDichVu, DonGia, MoTaDichVu, HinhAnhDichVu, SoLuong, TrangThai, LoaiDichVu, DonViTinh)
                VALUES (@MaDichVu, @TenDichVu, @DonGia, @MoTaDichVu, @HinhAnhDichVu, @SoLuong, @TrangThai, @LoaiDichVu, @DonViTinh)";
            await _db.ExecuteAsync(sql, dto);
            return CreatedAtAction(nameof(GetAll), new { id = dto.MaDichVu }, dto);
        }

        // PUT: /api/dichvu/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "NhanVien,QuanTriVien")]
        public async Task<IActionResult> Update(string id, [FromBody] DichVuDTO dto)
        {
            const string sql = @"
                UPDATE DichVu SET
                    TenDichVu = @TenDichVu,
                    DonGia = @DonGia,
                    MoTaDichVu = @MoTaDichVu,
                    HinhAnhDichVu = @HinhAnhDichVu,
                    SoLuong = @SoLuong,
                    TrangThai = @TrangThai,
                    LoaiDichVu = @LoaiDichVu,
                    DonViTinh = @DonViTinh
                WHERE MaDichVu = @MaDichVu";
            dto.MaDichVu = id;
            var affected = await _db.ExecuteAsync(sql, dto);
            if (affected == 0) return NotFound();
            return NoContent();
        }

        // DELETE: /api/dichvu/{id} (Chỉ quản trị viên)
        [HttpDelete("{id}")]
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> Delete(string id)
        {
            var affected = await _db.ExecuteAsync("DELETE FROM DichVu WHERE MaDichVu = @id", new { id });
            if (affected == 0) return NotFound();
            return NoContent();
        }

        private async Task<string> GenerateUniqueMaDichVu()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaDichVu, 3, LEN(MaDichVu) - 2) AS INT)), 0) + 1
                FROM DichVu";
            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"DV{nextId:D3}";
        }
    }
}

