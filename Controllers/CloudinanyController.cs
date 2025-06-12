using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;

#region Giới thiệu Cloudinary
// Cloudinary là dịch vụ đám mây chuyên dùng để lưu trữ, xử lý và phân phối hình ảnh, video.
// Ưu điểm: tự động tối ưu hóa, hỗ trợ CDN, chỉnh sửa ảnh/video linh hoạt qua URL.
// Website: https://cloudinary.com
// Thư viện .NET: CloudinaryDotNet
#endregion

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly Cloudinary _cloudinary; // Đối tượng dùng để giao tiếp với Cloudinary
    private readonly CloudinarySettings _settings; // Cấu hình (CloudName, ApiKey, ApiSecret)

    // Constructor khởi tạo Cloudinary từ cấu hình trong appsettings.json
    public UploadController(IOptions<CloudinarySettings> config)
    {
        _settings = config.Value;

        // Debug kiểm tra cấu hình đã được load chưa
        Console.WriteLine("==== Cloudinary Configuration ====");
        Console.WriteLine($"CloudName: {_settings.CloudName}");
        Console.WriteLine($"ApiKey: {_settings.ApiKey}");
        Console.WriteLine($"ApiSecret: {_settings.ApiSecret}");
        Console.WriteLine("==================================");

        // Khởi tạo tài khoản Cloudinary từ cấu hình
        var account = new Account(
            _settings.CloudName,
            _settings.ApiKey,
            _settings.ApiSecret
        );

        // Khởi tạo đối tượng Cloudinary
        _cloudinary = new Cloudinary(account);
    }

    // API kiểm tra cấu hình Cloudinary đã được load thành công chưa
    [HttpGet("config")]
    public IActionResult GetCloudinaryConfig()
    {
        if (string.IsNullOrWhiteSpace(_settings.CloudName) ||
            string.IsNullOrWhiteSpace(_settings.ApiKey) ||
            string.IsNullOrWhiteSpace(_settings.ApiSecret))
        {
            // Nếu thiếu cấu hình => trả về lỗi
            return BadRequest("❌ Cloudinary config is missing or invalid.");
        }

        // Trả về cấu hình (ẩn ApiSecret vì lý do bảo mật)
        return Ok(new
        {
            message = "✅ Cloudinary config loaded successfully.",
            cloudName = _settings.CloudName,
            apiKey = _settings.ApiKey,
            apiSecret = "********", // Không in thật ApiSecret ra ngoài
        });
    }

    // API upload 1 file ảnh lên Cloudinary
    [HttpPost("image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        // Kiểm tra cấu hình Cloudinary
        if (string.IsNullOrWhiteSpace(_settings.CloudName) ||
            string.IsNullOrWhiteSpace(_settings.ApiKey) ||
            string.IsNullOrWhiteSpace(_settings.ApiSecret))
        {
            return BadRequest("❌ Cloudinary settings are not configured properly.");
        }

        // Kiểm tra file hợp lệ
        if (file == null || file.Length == 0)
        {
            return BadRequest("❌ No file uploaded.");
        }

        try
        {
            // Mở stream từ file upload
            await using var stream = file.OpenReadStream();

            // Tạo tham số upload
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Width(800).Height(800).Crop("limit"), // Resize ảnh
                Folder = "test_upload" // Thư mục trên Cloudinary
            };

            // Thực hiện upload
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            // Nếu thành công
            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("✅ Upload successful!");
                Console.WriteLine($"🔗 Image URL: {uploadResult.SecureUrl}");

                return Ok(new
                {
                    message = "✅ Upload successful",
                    url = uploadResult.SecureUrl.ToString() // Trả về link ảnh
                });
            }

            // Nếu thất bại
            Console.WriteLine($"❌ Upload failed: {uploadResult.Error?.Message}");
            return StatusCode(500, $"❌ Upload failed: {uploadResult.Error?.Message}");
        }
        catch (Exception ex)
        {
            // Bắt lỗi và log
            Console.WriteLine($"❌ Exception during upload: {ex.Message}");
            return StatusCode(500, $"❌ Exception during upload: {ex.Message}");
        }
    }
}
