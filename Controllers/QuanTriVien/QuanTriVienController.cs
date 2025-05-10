using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;


// QuanTriNguoiDungController.cs
// Mục đích: Quản lý toàn bộ người dùng

//GET /api/admin/nguoidung – Danh sách người dùng

//PUT /api/admin/nguoidung/{id} – Cập nhật vai trò, trạng thái

//DELETE /api/admin/nguoidung/{id} – Xóa người dùng
namespace HotelManagementAPI.Controllers.QuanTriVien
{
    [ApiController]
    [Route("api/quantrivien")]
    public class QuanTriVienController : ControllerBase
    {
        private readonly IDbConnection _db;

        public QuanTriVienController(IDbConnection db)
        {
            _db = db;
        }

        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            return Ok(new { Message = "Đây là dashboard của Quản trị viên." });
        }

        /// <summary>
        /// Đăng ký tài khoản quản trị viên.
        /// </summary>
        [HttpPost("dangky")]
        [Authorize(Policy = "QuanTriVienPolicy")]
        public async Task<ActionResult<NguoiDungDTO>> DangKyQuanTriVien([FromBody] NguoiDungDTO nguoiDung)
        {
            nguoiDung.Vaitro = "QuanTriVien";
            nguoiDung.MaNguoiDung = await GenerateUniqueMaNguoiDung();
            nguoiDung.NgayTao = DateTime.Now;

            const string checkEmailQuery = "SELECT COUNT(1) FROM NguoiDung WHERE Email = @Email";
            var isEmailDuplicate = await _db.ExecuteScalarAsync<int>(checkEmailQuery, new { nguoiDung.Email });

            if (isEmailDuplicate > 0)
            {
                return Conflict(new { Message = "Email đã tồn tại. Vui lòng sử dụng email khác." });
            }

            const string insertQuery = @"
                INSERT INTO NguoiDung (MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen, SoDienThoai, DiaChi, NgaySinh, GioiTinh, HinhAnhUrl, CanCuocCongDan, NgayTao)
                VALUES (@MaNguoiDung, @Vaitro, @Email, @TenTaiKhoan, @MatKhau, @HoTen, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @HinhAnhUrl, @CanCuocCongDan, @NgayTao)";
            await _db.ExecuteAsync(insertQuery, nguoiDung);

            return CreatedAtAction(nameof(DangKyQuanTriVien), new { id = nguoiDung.MaNguoiDung }, nguoiDung);
        }

        private async Task<string> GenerateUniqueMaNguoiDung()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaNguoiDung, 3, LEN(MaNguoiDung) - 2) AS INT)), 0) + 1
                FROM NguoiDung";

            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"ND{nextId:D3}";
        }
    }
}