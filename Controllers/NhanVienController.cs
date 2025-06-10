using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;

using Microsoft.AspNetCore.Authorization;
using HotelManagementAPI.DTOs.NhanVien;
using System.Security.Claims;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Options;

namespace HotelManagementAPI.Controllers.NhanVien
{
    [ApiController]
    [Route("api/nhanvien")]
    [Authorize(Roles = "NhanVien")]
    public class NhanVienController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly Cloudinary _cloudinary;

        public NhanVienController(
            IDbConnection db,
            IOptions<CloudinarySettings> cloudinaryOptions,
            IConfiguration config)
        {
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
        /// Thêm bài viết mới (nhân viên).
        /// </summary>
        [HttpPost("baiviet")]
        [SwaggerOperation(
            Summary = "Thêm bài viết mới",
            Description = "Nhân viên thêm bài viết mới, có thể upload ảnh kèm theo."
        )]
        [SwaggerResponse(200, "Thêm bài viết thành công.")]
        [SwaggerResponse(401, "Không xác định được nhân viên.")]
        public async Task<IActionResult> ThemBaiViet([FromForm] NhanVienThemBaiVietDTO dto, IFormFile? file)
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "❌ Không xác định được nhân viên. Vui lòng đăng nhập lại." });

            string? imageUrl = null;
            if (file != null && file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Width(800).Height(800).Crop("limit"),
                    Folder = "baiviet"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    imageUrl = uploadResult.SecureUrl.ToString();
                else
                    return StatusCode(500, new { Message = $"❌ Xin lỗi, upload ảnh thất bại: {uploadResult.Error?.Message}" });
            }

            // Sinh mã bài viết tự động dạng MB001, MB002, ...
            const string getMaxSql = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaBaiViet, 3, LEN(MaBaiViet)-2) AS INT)), 0) + 1 FROM BaiViet";
            var nextId = await _db.ExecuteScalarAsync<int>(getMaxSql);
            var maBaiViet = $"MB{nextId:D3}";
            const string sql = @"
    INSERT INTO BaiViet (MaBaiViet, MaNguoiDung, TieuDe, NoiDung, NgayDang, HinhAnhUrl, TrangThai)
    VALUES (@MaBaiViet, @MaNguoiDung, @TieuDe, @NoiDung, @NgayDang, @HinhAnhUrl, @TrangThai)";
            await _db.ExecuteAsync(sql, new
            {
                MaBaiViet = maBaiViet,
                MaNguoiDung = maNguoiDung,
                dto.TieuDe,
                dto.NoiDung,
                NgayDang = DateTime.UtcNow.AddHours(7),
                HinhAnhUrl = imageUrl,
                TrangThai = "Chờ Duyệt"
            });
            // Thêm bài viết thành công
            return Ok(new
            {
                Message = "🎉 Thêm bài viết thành công! Bài viết của bạn đang chờ duyệt.",
                MaBaiViet = maBaiViet,
                HinhAnhUrl = imageUrl
            });
        }

      
        /// <summary>
        /// Xóa bài viết (nhân viên).
        /// </summary>
        [HttpDelete("baiviet/{maBaiViet}")]
        [SwaggerOperation(
            Summary = "Xóa bài viết",
            Description = "Nhân viên chỉ được xóa bài viết của chính mình."
        )]
        [SwaggerResponse(200, "Xóa bài viết thành công.")]
        [SwaggerResponse(401, "Không xác định được nhân viên.")]
        [SwaggerResponse(404, "Không tìm thấy bài viết hoặc không có quyền xóa.")]
        public async Task<IActionResult> XoaBaiViet(string maBaiViet)
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "❌ Không xác định được nhân viên. Vui lòng đăng nhập lại." });

            // Chỉ cho phép xóa bài viết của chính mình
            const string checkQuery = "SELECT COUNT(1) FROM BaiViet WHERE MaBaiViet = @MaBaiViet AND MaNguoiDung = @MaNguoiDung";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaBaiViet = maBaiViet, MaNguoiDung = maNguoiDung });
            if (isExists == 0)
                return NotFound(new { Message = "❌ Xin lỗi, không tìm thấy bài viết hoặc bạn không có quyền xóa." });

            const string sql = "DELETE FROM BaiViet WHERE MaBaiViet = @MaBaiViet";
            await _db.ExecuteAsync(sql, new { MaBaiViet = maBaiViet });
            // Xóa bài viết thành công
            return Ok(new { Message = "✅ Xóa bài viết thành công! Bài viết đã được xóa khỏi hệ thống." });
        }
    }

}

