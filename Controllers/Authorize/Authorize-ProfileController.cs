using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Data;
using Dapper;

namespace HotelManagementAPI.Controllers.Authorize
{
    [ApiController]
    [Authorize]
    [Route("api/authorize/profile")]
    public class AuthorizeProfileController : ControllerBase
    {
        private readonly IDbConnection _db;

        public AuthorizeProfileController(IDbConnection db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            // Lấy mã người dùng từ JWT
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            // Lấy vai trò từ JWT
            var role = User.FindFirstValue(ClaimTypes.Role);

            // Lấy thông tin người dùng từ DB
            const string query = @"SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var profile = await _db.QueryFirstOrDefaultAsync(query, new { MaNguoiDung = maNguoiDung });

            if (profile == null)
                return NotFound(new { Message = "Không tìm thấy thông tin người dùng." });

            return Ok(new
            {
                Profile = profile

            });
        }
        /// <summary>
        /// Lấy thông tin người dùng đã mã hóa (chỉ trả về CanCuocCongDan và MatKhau).
        /// </summary>
        [HttpGet("profile/mahoa")]
        public async Task<IActionResult> GetProfileMaHoa()
        {
            // Lấy mã người dùng từ JWT
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            // Lấy 2 trường từ DB
            const string query = "SELECT CanCuocCongDan, MatKhau FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var result = await _db.QueryFirstOrDefaultAsync(query, new { MaNguoiDung = maNguoiDung });

            if (result == null)
                return NotFound(new { Message = "Không tìm thấy thông tin người dùng." });

            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin người dùng đã mã hóa và giải mã (trả về CanCuocCongDan gốc và MatKhau).
        /// </summary>
        [HttpGet("profile/mahoa-giaima")]
        public async Task<IActionResult> GetProfileMaHoaGiaiMa()
        {
            // Lấy mã người dùng từ JWT
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            // Lấy 2 trường từ DB
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