using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using HotelManagementAPI.Models; 

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly DataQlks115Nhom2Context _dbContext;

    public TestController(DataQlks115Nhom2Context dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("protected")]
    [Authorize]
    public IActionResult Protected()
    {
        var jwk = User.Claims.FirstOrDefault(c => c.Type == "jwk")?.Value;

        if (string.IsNullOrEmpty(jwk))
        {
            return Unauthorized(new { Message = "Jwk không hợp lệ hoặc không tồn tại." });
        }

        // Tìm người dùng dựa trên Jwk
        var khachHang = _dbContext.KhachHangs.FirstOrDefault(kh => kh.Jwk == jwk);
        if (khachHang != null)
        {
            return Ok(new
            {
                Message = "Xác thực thành công (Khách hàng)",
                UserType = "Khách hàng",
                UserInfo = new
                {
                    khachHang.MaKhachHang,
                    khachHang.HoTen,
                    khachHang.Email,
                    khachHang.SoDienThoai,
                    khachHang.DiaChi,
                    khachHang.NgaySinh,
                    khachHang.GioiTinh,
                    khachHang.NgheNghiep
                }
            });
        }

        var nhanVien = _dbContext.NhanViens.FirstOrDefault(nv => nv.Jwk == jwk);
        if (nhanVien != null)
        {
            return Ok(new
            {
                Message = "Xác thực thành công (Nhân viên)",
                UserType = "Nhân viên",
                UserInfo = new
                {
                    nhanVien.MaNhanVien,
                    nhanVien.HoTen,
                    nhanVien.Email,
                    nhanVien.SoDienThoai,
                    nhanVien.NgayVaoLam,
                    nhanVien.TrangThai
                }
            });
        }

        var quanTriVien = _dbContext.QuanTriViens.FirstOrDefault(qtv => qtv.Jwk == jwk);
        if (quanTriVien != null)
        {
            return Ok(new
            {
                Message = "Xác thực thành công (Quản trị viên)",
                UserType = "Quản trị viên",
                UserInfo = new
                {
                    quanTriVien.MaQuanTri,
                    quanTriVien.TenAdmin,
                    quanTriVien.Email,
                    quanTriVien.SoDienThoai,
                    quanTriVien.NgayTao,
                    quanTriVien.NgayCapNhat
                }
            });
        }

        return Unauthorized(new { Message = "Không tìm thấy thông tin người dùng." });
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
    {
        return Ok("Đây là API công khai.");
    }
}

public abstract class BaseUser
{
    public string? Jwk { get; set; }
    public string Email { get; set; } = null!;
    public string TenTaiKhoan { get; set; } = null!;
    public string SoDienThoai { get; set; } = null!;
}

public partial class QuanTriVien : BaseUser
{
    public string MaQuanTri { get; set; } = null!;
    public string TenAdmin { get; set; } = null!;
    public DateTime? NgayTao { get; set; }
    public DateTime? NgayCapNhat { get; set; }
}

public partial class NhanVien : BaseUser
{
    public string MaNhanVien { get; set; } = null!;
    public string HoTen { get; set; } = null!;
    public string CanCuocCongDan { get; set; } = null!;
    public string? HinhAnh { get; set; }
    public DateOnly NgayVaoLam { get; set; }
    public string? TrangThai { get; set; }
}
