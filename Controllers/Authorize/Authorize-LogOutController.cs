using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagementAPI.Controllers.Authorize
{
    [ApiController]
    [Route("api/logout")]
    public class AuthorizeController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public AuthorizeController(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Đăng xuất người dùng dựa trên vai trò từ token.
        /// </summary>
        [HttpPost]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                // Lấy token từ header Authorization
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { Message = "Token không hợp lệ." });
                }

                // Lấy vai trò từ claim "Vaitro"
                var userRole = User.Claims.FirstOrDefault(c => c.Type == "Vaitro")?.Value;

                if (string.IsNullOrEmpty(userRole))
                {
                    return Unauthorized(new { Message = "Không tìm thấy vai trò trong token." });
                }

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
    }
}