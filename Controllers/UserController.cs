using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HotelManagementAPI.Helper;
using HotelManagementAPI.DTOs;
using System.Text.RegularExpressions;
namespace HotelManagementAPI.Controllers
{
    /// <summary>
    /// Controller để quản lý người dùng.
    /// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
     private readonly IDbConnection _db;
        private readonly IConfiguration _config;

        public UserController(IDbConnection db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

    /// <summary>
    /// Lấy thông tin chi tiết của người dùng đã xác thực.
    /// </summary>
    [HttpGet("profile-QuanTri-Vien")]
    [Authorize(Policy = "QuanTriVienPolicy")]
    public async Task<ActionResult<NguoiDungDTO>> GetProfile()
    {
        try
        {
            // Ghi log danh sách claims
            Console.WriteLine("=== Claims từ token ===");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            // Lấy MaNguoiDung từ claim "nameidentifier"
            var maNguoiDung = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(maNguoiDung))
            {
                return Unauthorized(new { Message = "Không tìm thấy thông tin người dùng trong token." });
            }

            // Truy vấn thông tin người dùng từ cơ sở dữ liệu
            const string query = "SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var nguoiDung = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { MaNguoiDung = maNguoiDung });

            if (nguoiDung == null)
            {
                return NotFound(new { Message = "Không tìm thấy thông tin người dùng." });
            }

            // Trả về thông tin người dùng
            return Ok(nguoiDung);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi: {ex.Message}");
            return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy thông tin người dùng." });
        }
    }
    [HttpGet("profile-KhachHang")]
    [Authorize(Policy = "KhachHangPolicy")]
    public async Task<ActionResult<NguoiDungDTO>> GetProfile1()
    {
        try
        {
            // Ghi log danh sách claims
            Console.WriteLine("=== Claims từ token ===");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            // Lấy MaNguoiDung từ claim "nameidentifier"
            var maNguoiDung = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(maNguoiDung))
            {
                return Unauthorized(new { Message = "Không tìm thấy thông tin người dùng trong token." });
            }

            // Truy vấn thông tin người dùng từ cơ sở dữ liệu
            const string query = "SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var nguoiDung = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { MaNguoiDung = maNguoiDung });

            if (nguoiDung == null)
            {
                return NotFound(new { Message = "Không tìm thấy thông tin người dùng." });
            }

            // Trả về thông tin người dùng
            return Ok(nguoiDung);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi: {ex.Message}");
            return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy thông tin người dùng." });
        }
    }
}
}