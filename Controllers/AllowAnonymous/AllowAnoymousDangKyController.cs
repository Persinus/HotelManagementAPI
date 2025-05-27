using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using HotelManagementAPI.Helper;
using BCrypt.Net;

namespace HotelManagementAPI.Controllers.AllowAnonymous
{
    [ApiController]
    [Route("api/allowanonymous")]
    [AllowAnonymous] // Cho phép tất cả người dùng truy cập
    public class AllowAnonymousDangKyController : ControllerBase
    {
        private readonly IDbConnection _db;

        public AllowAnonymousDangKyController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Đăng ký tài khoản cho người dùng chưa phải là khách hàng.
        /// </summary>
        [HttpPost("dangky")]
        public async Task<ActionResult<NguoiDungDTO>> DangKyNguoiDung([FromBody] NguoiDungDTO nguoiDung)
        {
            nguoiDung.Vaitro = "KhachHang";
            nguoiDung.MaNguoiDung = await GenerateUniqueMaNguoiDung();
            nguoiDung.NgayTao = DateTime.Now;

            // Kiểm tra email trùng lặp
            const string checkEmailQuery = "SELECT COUNT(1) FROM NguoiDung WHERE Email = @Email";
            var isEmailDuplicate = await _db.ExecuteScalarAsync<int>(checkEmailQuery, new { nguoiDung.Email });

            if (isEmailDuplicate > 0)
            {
                return Conflict(new { Message = "Email đã tồn tại. Vui lòng sử dụng email khác." });
            }

            // Kiểm tra tên tài khoản trùng lặp
            const string checkTenTaiKhoanQuery = "SELECT COUNT(1) FROM NguoiDung WHERE TenTaiKhoan = @TenTaiKhoan";
            var isTenTaiKhoanDuplicate = await _db.ExecuteScalarAsync<int>(checkTenTaiKhoanQuery, new { nguoiDung.TenTaiKhoan });

            if (isTenTaiKhoanDuplicate > 0)
            {
                return Conflict(new { Message = "Tên đăng nhập đã có người sử dụng. Vui lòng chọn tên đăng nhập khác." });
            }

            // Mã hóa CCCD trước khi lưu
            if (!string.IsNullOrEmpty(nguoiDung.CanCuocCongDan))
                nguoiDung.CanCuocCongDan = SensitiveDataHelper.Encrypt(nguoiDung.CanCuocCongDan);

            // Mã hóa mật khẩu (nên dùng BCrypt hoặc ít nhất là SHA256)
            nguoiDung.MatKhau = BCrypt.Net.BCrypt.HashPassword(nguoiDung.MatKhau);

            // Thêm người dùng mới
            const string insertQuery = @"
                INSERT INTO NguoiDung (MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen, SoDienThoai, DiaChi, NgaySinh, GioiTinh, HinhAnhUrl, CanCuocCongDan, NgayTao)
                VALUES (@MaNguoiDung, @Vaitro, @Email, @TenTaiKhoan, @MatKhau, @HoTen, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @HinhAnhUrl, @CanCuocCongDan, @NgayTao)";
            await _db.ExecuteAsync(insertQuery, nguoiDung);

            return CreatedAtAction(nameof(DangKyNguoiDung), new { id = nguoiDung.MaNguoiDung }, nguoiDung);
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