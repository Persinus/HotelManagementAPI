using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Data;
using Dapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementAPI.DTOs;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/xacthuc")]
    public class XacThucController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IDbConnection _db;
        private readonly Cloudinary _cloudinary;

        public XacThucController(
            IMemoryCache cache,
            IDbConnection db,
            IOptions<CloudinarySettings> cloudinaryOptions)
        {
            _cache = cache;
            _db = db;
            var settings = cloudinaryOptions.Value;
            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
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

            const string query = @"SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var profile = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { MaNguoiDung = maNguoiDung });

            if (profile == null)
                return NotFound(new { Message = "Không tìm thấy thông tin người dùng." });

            // Giải mã CCCD nếu có
            try
            {
                profile.CanCuocCongDan = HotelManagementAPI.Helper.SensitiveDataHelper.Decrypt(profile.CanCuocCongDan);
            }
            catch
            {
                profile.CanCuocCongDan = "Không giải mã được";
            }

            return Ok(profile);
        }

      
        /// <summary>
        /// Sửa thông tin người dùng dựa vào token.
        /// </summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] SuaThongTinNguoiDungDTO dto, IFormFile? file)
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được người dùng." });

            // Lấy link ảnh cũ
            const string getSql = "SELECT HinhAnhUrl, CanCuocCongDan FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var user = await _db.QueryFirstOrDefaultAsync(getSql, new { MaNguoiDung = maNguoiDung });
            string? imageUrl = user?.HinhAnhUrl;
            string? canCuocCongDan = user?.CanCuocCongDan;

            // Upload ảnh mới nếu có
            if (file != null && file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Width(400).Height(400).Crop("limit"),
                    Folder = "avatar"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    imageUrl = uploadResult.SecureUrl.ToString();
                else
                    return StatusCode(500, $"Upload ảnh thất bại: {uploadResult.Error?.Message}");
            }

            // Mã hóa CCCD nếu có cập nhật mới
            if (!string.IsNullOrEmpty(dto.CanCuocCongDan))
                canCuocCongDan = HotelManagementAPI.Helper.SensitiveDataHelper.Encrypt(dto.CanCuocCongDan);

            const string sql = @"
        UPDATE NguoiDung SET
            HoTen = @HoTen,
            SoDienThoai = @SoDienThoai,
            DiaChi = @DiaChi,
            NgaySinh = @NgaySinh,
            GioiTinh = @GioiTinh,
            HinhAnhUrl = @HinhAnhUrl,
            CanCuocCongDan = @CanCuocCongDan
        WHERE MaNguoiDung = @MaNguoiDung";

            var affected = await _db.ExecuteAsync(sql, new
            {
                MaNguoiDung = maNguoiDung,
                dto.HoTen,
                dto.SoDienThoai,
                dto.DiaChi,
                dto.NgaySinh,
                dto.GioiTinh,
                HinhAnhUrl = imageUrl,
                CanCuocCongDan = canCuocCongDan
            });

            if (affected == 0)
                return NotFound(new { Message = "Không tìm thấy người dùng để cập nhật." });

            return Ok(new { Message = "Cập nhật thông tin thành công.", HinhAnhUrl = imageUrl });
        }
    }
}