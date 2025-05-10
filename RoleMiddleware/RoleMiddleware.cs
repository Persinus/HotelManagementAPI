using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
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
        // Bỏ qua middleware nếu endpoint được đánh dấu [AllowAnonymous]
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null)
        {
            await _next(context);
            return;
        }

        // Kiểm tra xem token có được gửi trong header Authorization hay không
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            Console.WriteLine("Không tìm thấy token trong header Authorization.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token không được cung cấp.");
            return;
        }

        // Kiểm tra xem user đã được xác thực chưa (token có hợp lệ không)
        if (context.User?.Identity == null || !context.User.Identity.IsAuthenticated)
        {
            Console.WriteLine("Người dùng chưa được xác thực.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Người dùng chưa được xác thực.");
            return;
        }

        // Lấy vai trò từ claims
        var userRole = context.User.Claims.FirstOrDefault(c => c.Type == "Vaitro")?.Value;

        // Kiểm tra xem có vai trò không
        if (string.IsNullOrEmpty(userRole))
        {
            Console.WriteLine("Không tìm thấy vai trò người dùng (userRole is null or empty).");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Không tìm thấy vai trò người dùng.");
            return;
        }

        // Ghi log thông tin người dùng
        Console.WriteLine($"User authenticated. Role: {userRole}");

        // Phân quyền dựa trên vai trò
        if (context.Request.Path.StartsWithSegments("/api/quantri") && userRole != "QuanTriVien")
        {
            Console.WriteLine("Người dùng không có quyền truy cập vào tài nguyên này.");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Bạn không có quyền truy cập vào tài nguyên này.");
            return;
        }

        // Tiếp tục xử lý request
        await _next(context);
    }
}

/// <summary>
/// Provides extension methods for ClaimsPrincipal.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user role from the claims.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>The user role as a string.</returns>
    public static string GetUserRole(this ClaimsPrincipal user)
    {
        // Lấy giá trị của claim "Vaitro" (vai trò người dùng)
        return user?.Claims.FirstOrDefault(c => c.Type == "Vaitro")?.Value;
    }

    /// <summary>
    /// Gets the user ID (sub) from the claims.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>The user ID as a string.</returns>
    public static string GetUserSub(this ClaimsPrincipal user)
    {
        // Lấy giá trị của claim "sub" (ID người dùng)
        return user?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
    }

    /// <summary>
    /// Gets the user email from the claims.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>The user email as a string.</returns>
    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        // Lấy giá trị của claim "email"
        return user?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
    }
    
}
