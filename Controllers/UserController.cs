using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    /// <summary>
    /// Lấy danh sách claims từ JWT token.
    /// </summary>
    [HttpGet("claims")]
    [AllowAnonymous]
    public IActionResult GetClaims()
    {
        // Ghi log danh sách claims
        Console.WriteLine("=== Claims từ Controller ===");
        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
        }

        // Trả về danh sách claims dưới dạng JSON
        return Ok(new
        {
            Claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    /// <summary>
    /// Nhập JWT token và hiển thị danh sách claims cùng vai trò.
    /// </summary>
    [HttpPost("decode-token")]
    public IActionResult DecodeToken([FromBody] string jwtToken)
    {
        try
        {
            // Giải mã JWT token
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            // Lấy danh sách claims
            var claims = token.Claims.Select(c => new { c.Type, c.Value }).ToList();

            // Tìm vai trò (Vaitro) từ claims
            var userRole = claims.FirstOrDefault(c => c.Type == "Vaitro")?.Value;

            // Ghi log danh sách claims
            Console.WriteLine("=== Claims từ JWT token ===");
            foreach (var claim in claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            // Ghi log vai trò
            Console.WriteLine($"Vai trò người dùng: {userRole ?? "Không tìm thấy vai trò"}");

            // Trả về danh sách claims và vai trò dưới dạng JSON
            return Ok(new
            {
                Claims = claims,
                Role = userRole ?? "Không tìm thấy vai trò"
            });
        }
        catch
        {
            return BadRequest(new { Message = "JWT token không hợp lệ." });
        }
    }
}
