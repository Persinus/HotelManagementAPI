using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly Cloudinary _cloudinary;
    private readonly CloudinarySettings _settings;

    public UploadController(IOptions<CloudinarySettings> config)
    {
        _settings = config.Value;

        Console.WriteLine("==== Cloudinary Configuration ====");
        Console.WriteLine($"CloudName: {_settings.CloudName}");
        Console.WriteLine($"ApiKey: {_settings.ApiKey}");
        Console.WriteLine($"ApiSecret: {_settings.ApiSecret}");
        Console.WriteLine("==================================");

        var account = new Account(
            _settings.CloudName,
            _settings.ApiKey,
            _settings.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
    }

[HttpGet("config")]
public IActionResult GetCloudinaryConfig()
{
    if (string.IsNullOrWhiteSpace(_settings.CloudName) ||
        string.IsNullOrWhiteSpace(_settings.ApiKey) ||
        string.IsNullOrWhiteSpace(_settings.ApiSecret))
    {
        return BadRequest("❌ Cloudinary config is missing or invalid.");
    }

    return Ok(new
    {
        message = "✅ Cloudinary config loaded successfully.",
        cloudName = _settings.CloudName,
        apiKey = _settings.ApiKey,
        apiSecret = "********", // không in thật ra secret để tránh lộ key
    });
}

    [HttpPost("image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (string.IsNullOrWhiteSpace(_settings.CloudName) ||
            string.IsNullOrWhiteSpace(_settings.ApiKey) ||
            string.IsNullOrWhiteSpace(_settings.ApiSecret))
        {
            return BadRequest("❌ Cloudinary settings are not configured properly.");
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest("❌ No file uploaded.");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Width(800).Height(800).Crop("limit"),
                Folder = "test_upload"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("✅ Upload successful!");
                Console.WriteLine($"🔗 Image URL: {uploadResult.SecureUrl}");

                return Ok(new
                {
                    message = "✅ Upload successful",
                    url = uploadResult.SecureUrl.ToString()
                });
            }

            Console.WriteLine($"❌ Upload failed: {uploadResult.Error?.Message}");
            return StatusCode(500, $"❌ Upload failed: {uploadResult.Error?.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception during upload: {ex.Message}");
            return StatusCode(500, $"❌ Exception during upload: {ex.Message}");
        }
    }
}
