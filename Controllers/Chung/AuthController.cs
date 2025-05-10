using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HotelManagementAPI.Helper;
using HotelManagementAPI.DTOs;
using System.Text.RegularExpressions;

namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly IConfiguration _config;

        public AuthController(IDbConnection db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        /// <summary>
        /// Đăng ký tài khoản khách hàng.
        /// </summary>
        [HttpPost("dangky-khachhang")]
        [AllowAnonymous]
        public async Task<ActionResult<NguoiDungDTO>> DangKyKhachHang([FromBody] NguoiDungDTO nguoiDung)
        {
            // Gán vai trò mặc định là "KhachHang"
            nguoiDung.Vaitro = "KhachHang";

            // Tự động tạo mã người dùng với tiền tố NDXXX
            nguoiDung.MaNguoiDung = await GenerateUniqueMaNguoiDung();

            // Gán ngày tạo
            nguoiDung.NgayTao = DateTime.Now;

            // Kiểm tra trùng lặp Email
            if (string.IsNullOrEmpty(nguoiDung.Email) || !Regex.IsMatch(nguoiDung.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return BadRequest(new { Message = "Email không hợp lệ. Vui lòng nhập đúng định dạng email." });
            }

            const string checkEmailQuery = "SELECT COUNT(1) FROM NguoiDung WHERE Email = @Email";
            var isEmailDuplicate = await _db.ExecuteScalarAsync<int>(checkEmailQuery, new { nguoiDung.Email });

            if (isEmailDuplicate > 0)
            {
                return Conflict(new { Message = "Email đã tồn tại. Vui lòng sử dụng email khác." });
            }

            // Chèn dữ liệu nếu không trùng lặp
            const string insertQuery = @"
                INSERT INTO NguoiDung (MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen, SoDienThoai, DiaChi, NgaySinh, GioiTinh, HinhAnhUrl, CanCuocCongDan, NgayTao)
                VALUES (@MaNguoiDung, @Vaitro, @Email, @TenTaiKhoan, @MatKhau, @HoTen, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @HinhAnhUrl, @CanCuocCongDan, @NgayTao)";
            await _db.ExecuteAsync(insertQuery, nguoiDung);

            return CreatedAtAction(nameof(DangKyKhachHang), new { id = nguoiDung.MaNguoiDung }, nguoiDung);
        }

        /// <summary>
        /// Đăng ký tài khoản quản trị viên.
        /// </summary>
        [HttpPost("dangky-quantrivien")]
        [Authorize(Policy = "QuanTriVienPolicy")]
        public async Task<ActionResult<NguoiDungDTO>> DangKyQuanTriVien([FromBody] NguoiDungDTO nguoiDung)
        {
            // Gán vai trò mặc định là "QuanTriVien"
            nguoiDung.Vaitro = "QuanTriVien";
            nguoiDung.NgayTao = DateTime.Now;

            // Tự động tạo mã người dùng
            nguoiDung.MaNguoiDung = await GenerateUniqueMaNguoiDung();

            // Kiểm tra trùng lặp Email
            if (string.IsNullOrEmpty(nguoiDung.Email) || !Regex.IsMatch(nguoiDung.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return BadRequest(new { Message = "Email không hợp lệ. Vui lòng nhập đúng định dạng email." });
            }

            const string checkEmailQuery = "SELECT COUNT(1) FROM NguoiDung WHERE Email = @Email";
            var isEmailDuplicate = await _db.ExecuteScalarAsync<int>(checkEmailQuery, new { nguoiDung.Email });

            if (isEmailDuplicate > 0)
            {
                return Conflict(new { Message = "Email đã tồn tại. Vui lòng sử dụng email khác." });
            }

            // Kiểm tra trùng lặp CanCuocCongDan nếu không null
            if (!string.IsNullOrEmpty(nguoiDung.CanCuocCongDan))
            {
                const string checkCCCDQuery = "SELECT COUNT(1) FROM NguoiDung WHERE CanCuocCongDan = @CanCuocCongDan";
                var isCCCDDuplicate = await _db.ExecuteScalarAsync<int>(checkCCCDQuery, new { CanCuocCongDan = nguoiDung.CanCuocCongDan });

                if (isCCCDDuplicate > 0)
                {
                    return Conflict(new { Message = "Căn cước công dân đã tồn tại. Vui lòng sử dụng số khác." });
                }
            }

            // Chèn dữ liệu nếu không trùng lặp
            const string insertQuery = @"
                INSERT INTO NguoiDung (MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen, SoDienThoai, DiaChi, NgaySinh, GioiTinh, HinhAnhUrl, CanCuocCongDan, NgayTao)
                VALUES (@MaNguoiDung, @Vaitro, @Email, @TenTaiKhoan, @MatKhau, @HoTen, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @HinhAnhUrl, @CanCuocCongDan, @NgayTao)";
            await _db.ExecuteAsync(insertQuery, nguoiDung);

            return CreatedAtAction(nameof(DangKyQuanTriVien), new { id = nguoiDung.MaNguoiDung }, nguoiDung);
        }

        /// <summary>
        /// Đăng ký tài khoản nhân viên.
        /// </summary>
        [HttpPost("dangky-nhanvien")]
        [Authorize(Policy = "QuanTriVienPolicy")]
        public async Task<ActionResult<NguoiDungDTO>> DangKyNhanVien([FromBody] NguoiDungDTO nguoiDung)
        {
            // Gán vai trò mặc định là "NhanVien"
            nguoiDung.Vaitro = "NhanVien";
            nguoiDung.NgayTao = DateTime.Now;

            // Tự động tạo MaNguoiDung nếu chưa có
            nguoiDung.MaNguoiDung ??= Guid.NewGuid().ToString();

            // Kiểm tra trùng lặp Email
            if (string.IsNullOrEmpty(nguoiDung.Email) || !Regex.IsMatch(nguoiDung.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return BadRequest(new { Message = "Email không hợp lệ. Vui lòng nhập đúng định dạng email." });
            }

            const string checkEmailQuery = "SELECT COUNT(1) FROM NguoiDung WHERE Email = @Email";
            var isEmailDuplicate = await _db.ExecuteScalarAsync<int>(checkEmailQuery, new { nguoiDung.Email });

            if (isEmailDuplicate > 0)
            {
                return Conflict(new { Message = "Email đã tồn tại. Vui lòng sử dụng email khác." });
            }

            // Chèn dữ liệu nếu không trùng lặp
            const string insertQuery = @"
                INSERT INTO NguoiDung (MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen, SoDienThoai, DiaChi, NgaySinh, GioiTinh, HinhAnhUrl, CanCuocCongDan, NgayTao)
                VALUES (@MaNguoiDung, @Vaitro, @Email, @TenTaiKhoan, @MatKhau, @HoTen, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @HinhAnhUrl, @CanCuocCongDan, @NgayTao)";
            await _db.ExecuteAsync(insertQuery, nguoiDung);

            return CreatedAtAction(nameof(DangKyNhanVien), new { id = nguoiDung.MaNguoiDung }, nguoiDung);
        }

        /// <summary>
        /// Đăng nhập.
        /// </summary>
        [HttpPost("dangnhap")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> DangNhap([FromBody] LoginDTO login)
        {
            const string query = "SELECT * FROM NguoiDung WHERE TenTaiKhoan = @TenTaiKhoan AND MatKhau = @MatKhau";
            var nguoiDung = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { login.TenTaiKhoan, login.MatKhau });

            if (nguoiDung == null)
            {
                return Unauthorized(new { Message = "Tên tài khoản hoặc mật khẩu không đúng." });
            }

            // Tạo JWT token
            var secretKey = _config["Jwt:SecretKey"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var nguoiDungModel = new HotelManagementAPI.Models.NguoiDung
            {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                Vaitro = nguoiDung.Vaitro,
                Email = nguoiDung.Email,
                TenTaiKhoan = nguoiDung.TenTaiKhoan,
                MatKhau = nguoiDung.MatKhau,
                HoTen = nguoiDung.HoTen,
                SoDienThoai = nguoiDung.SoDienThoai,
                DiaChi = nguoiDung.DiaChi,
                NgaySinh = nguoiDung.NgaySinh,
                GioiTinh = nguoiDung.GioiTinh,
                HinhAnhUrl = nguoiDung.HinhAnhUrl,
                NgayTao = nguoiDung.NgayTao
            };

            var token = JwtHelper.GenerateJwtToken(nguoiDungModel, secretKey, issuer, audience);

            return Ok(new { Token = token });
        }

        /// <summary>
        /// Tạo mã người dùng với tiền tố NDXXX.
        /// </summary>
        /// <returns>Mã người dùng dạng NDXXX</returns>
        private async Task<string> GenerateUniqueMaNguoiDung()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaNguoiDung, 3, LEN(MaNguoiDung) - 2) AS INT)), 0) + 1
                FROM NguoiDung";

            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"ND{nextId:D3}"; // Định dạng NDXXX (ví dụ: ND001, ND002)
        }
    }
}