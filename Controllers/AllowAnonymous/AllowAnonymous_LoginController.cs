using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using HotelManagementAPI.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace HotelManagementAPI.Controllers.AllowAnonymous
{
    [ApiController]
    [AllowAnonymous] // Cho phép tất cả người dùng truy cập
    [Route("api/allowanonymous")]
    
    public class AllowAnonymous_LoginController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly IConfiguration _config;
        

        public AllowAnonymous_LoginController(IDbConnection db, IConfiguration config, IMemoryCache cache)
        {
            _db = db;
            _config = config;
           
        }

        /// <summary>
        /// Đăng nhập.
        /// </summary>
        [HttpPost("dangnhap")]
        public async Task<ActionResult<string>> DangNhap([FromBody] LoginDTO login)
        {
            // Lấy user theo tên tài khoản
            const string query = "SELECT * FROM NguoiDung WHERE TenTaiKhoan = @TenTaiKhoan";
            var nguoiDung = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { login.TenTaiKhoan });

            if (nguoiDung == null)
                return Unauthorized(new { Message = "Tên tài khoản hoặc mật khẩu không đúng 1." });

            // So sánh mật khẩu nhập vào với mật khẩu đã hash trong DB
            bool isValid = BCrypt.Net.BCrypt.Verify(login.MatKhau, nguoiDung.MatKhau);
            if (!isValid)
                return Unauthorized(new { Message = "Tên tài khoản hoặc mật khẩu không đúng 2." });

            // Tạo JWT token như cũ...
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
        /// Cập nhật mật khẩu mới.
        /// </summary>
        [HttpPut("datlaimatkhau")]
     
        public async Task<IActionResult> DatLaiMatKhau([FromBody] ResetPasswordDTO resetPassword)
        {
            try
            {
                // Kiểm tra email có tồn tại trong hệ thống không
                const string query = "SELECT COUNT(1) FROM NguoiDung WHERE Email = @Email";
                var emailExists = await _db.ExecuteScalarAsync<int>(query, new { resetPassword.Email });

                if (emailExists == 0)
                {
                    return NotFound(new { Message = "Email không tồn tại trong hệ thống." });
                }

                // Mã hóa mật khẩu mới trước khi lưu
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(resetPassword.NewPassword);

                // Cập nhật mật khẩu mới (đã mã hóa)
                const string updatePasswordQuery = "UPDATE NguoiDung SET MatKhau = @MatKhau WHERE Email = @Email";
                await _db.ExecuteAsync(updatePasswordQuery, new { MatKhau = hashedPassword, resetPassword.Email });

                return Ok(new { Message = "Mật khẩu đã được cập nhật thành công." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi cập nhật mật khẩu." });
            }
        }

        
    }
}