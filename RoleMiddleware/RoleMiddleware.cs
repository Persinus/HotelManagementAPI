using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

public class RoleMiddleware
{
    private readonly RequestDelegate _next;

    public RoleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Bỏ qua kiểm tra nếu endpoint cho phép truy cập ẩn danh
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null)
        {
            await _next(context);
            return;
        }

        // Lấy thông tin vai trò từ token
        var userRole = context.User.Claims.FirstOrDefault(c => c.Type == "Vaitro")?.Value;

        // Kiểm tra nếu người dùng chưa đăng nhập
        if (string.IsNullOrEmpty(userRole))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Bạn cần đăng nhập để truy cập tài nguyên này.");
            return;
        }

        // Phân quyền dựa trên đường dẫn
        if (context.Request.Path.StartsWithSegments("/api/quantrivien") && userRole != "QuanTriVien")
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Bạn không có quyền truy cập vào tài nguyên này.");
            return;
        }
        else if (context.Request.Path.StartsWithSegments("/api/nhanvien") && userRole != "NhanVien" && userRole != "QuanTriVien")
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Bạn không có quyền truy cập vào tài nguyên này.");
            return;
        }
        else if (context.Request.Path.StartsWithSegments("/api/khachhang") && userRole != "KhachHang" && userRole != "NhanVien" && userRole != "QuanTriVien")
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Bạn không có quyền truy cập vào tài nguyên này.");
            return;
        }

        // Nếu không có vấn đề gì, tiếp tục xử lý request
        await _next(context);
    }
}

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    public TokenBlacklistMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token) && _cache.TryGetValue(token, out _))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token đã bị thu hồi. Vui lòng đăng nhập lại.");
            return;
        }

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
