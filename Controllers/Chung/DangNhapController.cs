using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using HotelManagementAPI.Helper;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagementAPI.Controllers.Chung
{
    [ApiController]
    [Route("api/auth")]
    public class DangNhapController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly IConfiguration _config;

        public DangNhapController(IDbConnection db, IConfiguration config)
        {
            _db = db;
            _config = config;
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
    }
}