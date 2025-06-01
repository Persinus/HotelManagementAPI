using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Dapper;
using HotelManagementAPI.DTOs;
using Swashbuckle.AspNetCore.Annotations;
using HotelManagementAPI.DTOs.QuanTriVien;

namespace HotelManagementAPI.Controllers.QuanTriVien
{
    [ApiController]
    [Route("api/QuanTriVien/hethong")]
    [Authorize(Roles = "QuanTriVien")]
    public class QuanTriVien_HeThongController : ControllerBase
    {
        private readonly IDbConnection _db;

        public QuanTriVien_HeThongController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Xem vai trò hiện tại của người dùng
        /// </summary>
        [HttpGet("nguoidung/{maNguoiDung}/vaitro")]
        [SwaggerOperation(Summary = "Xem vai trò người dùng")]
        public async Task<IActionResult> XemVaiTroNguoiDung([FromRoute] string maNguoiDung)
        {
            const string query = "SELECT Vaitro FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var vaiTro = await _db.ExecuteScalarAsync<string>(query, new { MaNguoiDung = maNguoiDung });
            if (vaiTro == null)
                return NotFound(new { Message = "Không tìm thấy người dùng." });
            return Ok(new { MaNguoiDung = maNguoiDung, VaiTro = vaiTro });
        }

        /// <summary>
        /// Đổi vai trò người dùng (chỉ cho phép giữa Nhân viên và Quản trị viên)
        /// </summary>
        [HttpPut("nguoidung/{maNguoiDung}/doivaitro")]
        [SwaggerOperation(Summary = "Đổi vai trò người dùng", Description = "Chỉ đổi giữa Nhân viên và Quản trị viên")]
        public async Task<IActionResult> DoiVaiTroNguoiDung(
            [FromRoute] string maNguoiDung,
            [FromBody] QuanTriVienSuaRoleDTO dto)
        {
            // Kiểm tra tồn tại
            const string checkQuery = "SELECT COUNT(1) FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var exists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaNguoiDung = maNguoiDung });
            if (exists == 0)
                return NotFound(new { Message = "Không tìm thấy người dùng." });

            // Chỉ cho phép đổi sang "NhanVien" hoặc "QuanTriVien"
            if (dto.VaiTroMoi != "NhanVien" && dto.VaiTroMoi != "QuanTriVien")
                return BadRequest(new { Message = "Chỉ được đổi sang 'NhanVien' hoặc 'QuanTriVien'." });

            const string updateQuery = "UPDATE NguoiDung SET Vaitro = @VaiTroMoi WHERE MaNguoiDung = @MaNguoiDung";
            await _db.ExecuteAsync(updateQuery, new { VaiTroMoi = dto.VaiTroMoi, MaNguoiDung = maNguoiDung });

            return Ok(new { Message = $"Đã đổi vai trò thành công cho người dùng {maNguoiDung} thành {dto.VaiTroMoi}" });
        }
    }
}