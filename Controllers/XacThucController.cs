using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Data;
using Dapper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/xacthuc")]
    public class XacThucController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IDbConnection _db;

        public XacThucController(IMemoryCache cache, IDbConnection db)
        {
            _cache = cache;
            _db = db;
        }

        /// <summary>
        /// Đăng xuất người dùng dựa trên vai trò từ token.
        /// </summary>
        [HttpPost("logout")]
            public IActionResult Logout()
        {
            try
            {
                // Lấy token từ header Authorization
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                    return BadRequest(new { Message = "Token không hợp lệ." });

                // Lấy vai trò từ claim "Vaitro"
                var userRole = User.Claims.FirstOrDefault(c => c.Type == "Vaitro")?.Value;

                if (string.IsNullOrEmpty(userRole))
                    return Unauthorized(new { Message = "Không tìm thấy vai trò trong token." });

                // Lưu token vào danh sách bị thu hồi (cache)
                var expirationTime = DateTime.UtcNow.AddHours(1); // Thời gian token hết hạn
                _cache.Set(token, true, expirationTime);

                return Ok(new { Message = $"Đăng xuất thành công cho vai trò {userRole}. Token đã bị thu hồi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi đăng xuất." });
            }
        }

        /// <summary>
        /// Lấy thông tin profile người dùng.
        /// </summary>
        [HttpGet("profile")]
       
        public async Task<IActionResult> GetProfile()
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            var role = User.FindFirstValue(ClaimTypes.Role);

            const string query = @"SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var profile = await _db.QueryFirstOrDefaultAsync(query, new { MaNguoiDung = maNguoiDung });

            if (profile == null)
                return NotFound(new { Message = "Không tìm thấy thông tin người dùng." });

            return Ok(new { Profile = profile });
        }

       
        /// <summary>
        /// Lấy thông tin người dùng đã mã hóa và giải mã (trả về CanCuocCongDan gốc và MatKhau).
        /// </summary>
        [HttpGet("profile/mahoa-giaima")]
        [Authorize]
        public async Task<IActionResult> GetProfileMaHoaGiaiMa()
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            const string query = "SELECT CanCuocCongDan FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var result = await _db.QueryFirstOrDefaultAsync(query, new { MaNguoiDung = maNguoiDung });

            if (result == null)
                return NotFound(new { Message = "Không tìm thấy thông tin người dùng." });

            // Giải mã CCCD
            string canCuocGoc = null;
            try
            {
                canCuocGoc = HotelManagementAPI.Helper.SensitiveDataHelper.Decrypt((string)result.CanCuocCongDan);
            }
            catch
            {
                canCuocGoc = "Không giải mã được";
            }

            return Ok(new
            {
                CanCuocCongDan = canCuocGoc,
            });
        }
    }
}