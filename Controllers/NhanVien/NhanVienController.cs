using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;


//NhanVienController.cs
//Mục đích: Quản lý thông tin cá nhân nhân viên

//GET /api/nhanvien/profile – Xem thông tin

//PUT /api/nhanvien/update – Cập nhật thông tin

namespace HotelManagementAPI.Controllers.NhanVien
{
    [ApiController]
    [Route("api/nhanvien")]
    public class NhanVienController : ControllerBase
    {
        private readonly IDbConnection _db;

        public NhanVienController(IDbConnection db)
        {
            _db = db;
        }

        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            return Ok(new { Message = "Đây là dashboard của Nhân viên." });
        }

        /// <summary>
        /// Đăng ký tài khoản nhân viên.
        /// </summary>
        [HttpPost("dangky")]
        [Authorize(Policy = "QuanTriVienPolicy")]
        public async Task<ActionResult<NguoiDungDTO>> DangKyNhanVien([FromBody] NguoiDungDTO nguoiDung)
        {
            nguoiDung.Vaitro = "NhanVien";
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

            return CreatedAtAction(nameof(DangKyNhanVien), new { id = nguoiDung.MaNguoiDung }, nguoiDung);
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