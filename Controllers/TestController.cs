using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("protected")]
    [Authorize]
    public IActionResult Protected()
    {
        var username = User.Identity?.Name ?? "Không xác định";

        // Nếu bạn gán các claim tùy chỉnh trong token như email, role, v.v.
        var email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        var role = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

        return Ok(new
        {
            Message = "Xác thực thành công",
            Username = username,
            Email = email,
            Role = role
        });
    }

    // API này không yêu cầu xác thực
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
    {
        return Ok("Đây là API công khai.");
    }

}
