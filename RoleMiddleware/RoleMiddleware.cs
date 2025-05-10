using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

public class RoleMiddleware
{
    private readonly RequestDelegate _next;

    public RoleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Lấy vai trò từ JWT token
        var userRole = context.User.Claims.FirstOrDefault(c => c.Type == "VaiTro")?.Value;

        // Kiểm tra quyền truy cập cho các API quản trị viên
        if (context.Request.Path.StartsWithSegments("/api/admin") && userRole != "QuanTriVien")
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Bạn không có quyền truy cập vào tài nguyên này.");
            return;
        }

        await _next(context);
    }
}