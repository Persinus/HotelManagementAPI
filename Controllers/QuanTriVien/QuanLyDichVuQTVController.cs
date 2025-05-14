using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;

namespace HotelManagementAPI.Controllers.QuanTriVien
{
    /// <summary>
    /// Controller quản lý dịch vụ dành cho Quản trị viên.
    /// </summary>
    [ApiController]
    [Route("api/quantrivien/dichvu")]
    [Authorize(Roles = "QuanTriVien")] // Chỉ cho phép Quản trị viên truy cập
    public class QuanLyDichVuQTVController : ControllerBase
    {
        private readonly IDbConnection _db;

        /// <summary>
        /// Khởi tạo controller với kết nối cơ sở dữ liệu.
        /// </summary>
        /// <param name="db">Kết nối cơ sở dữ liệu.</param>
        public QuanLyDichVuQTVController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Thêm dịch vụ mới (Mã dịch vụ tự sinh).
        /// </summary>
        /// <param name="dichVuDTO">Thông tin dịch vụ.</param>
        /// <returns>Kết quả thêm dịch vụ.</returns>
        [HttpPost("themdichvu")]
        public async Task<IActionResult> ThemDichVu([FromBody] DichVuDTO dichVuDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Tạo mã dịch vụ tự động
            const string generateMaDichVuQuery = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaDichVu, 3, LEN(MaDichVu) - 2) AS INT)), 0) + 1
                FROM DichVu";
            var nextId = await _db.ExecuteScalarAsync<int>(generateMaDichVuQuery);
            dichVuDTO.MaDichVu = $"DV{nextId:D3}";

            // Thêm dịch vụ vào cơ sở dữ liệu
            const string insertQuery = @"
                INSERT INTO DichVu (MaDichVu, TenDichVu, DonGia, MoTaDichVu, HinhAnhDichVu, SoLuong, TrangThai, LoaiDichVu, DonViTinh)
                VALUES (@MaDichVu, @TenDichVu, @DonGia, @MoTaDichVu, @HinhAnhDichVu, @SoLuong, @TrangThai, @LoaiDichVu, @DonViTinh)";
            await _db.ExecuteAsync(insertQuery, dichVuDTO);

            return Ok(new { Message = "Thêm dịch vụ thành công.", MaDichVu = dichVuDTO.MaDichVu });
        }
    }
}