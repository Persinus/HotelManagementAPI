using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình chuỗi kết nối
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký IDbConnection với DI
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

builder.Logging.AddConsole();

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {

        policy.AllowAnyOrigin() // Cho phép mọi domain
              .AllowAnyHeader()  // Cho phép mọi header
              .AllowAnyMethod(); // Cho phép mọi HTTP method (GET, POST, PUT, DELETE...)

    
    });
});

builder.Services.AddHttpClient();

// Đọc cấu hình SentimentModelPath từ appsettings.json
var sentimentModelPath = builder.Configuration["SentimentModelPath"];
builder.Services.AddSingleton(new SentimentModelConfig { ModelPath = sentimentModelPath });

// Thêm Swagger và các dịch vụ khác
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập 'Bearer' [space] và sau đó là token của bạn.",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });

    // Thêm thông tin mô tả API
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "🏨 Hệ thống Quản lý Khách sạn",
        Version = "v1",
        Description = @"
📘 **Đề tài:** Xây dựng hệ thống quản lý khách sạn.

🔧 **Công nghệ sử dụng:**
- ASP.NET Core Web API
- SQL Server
- Entity Framework/Dapper
- JWT Authentication
- Swagger UI
- (Tuỳ chọn: React/Vue cho frontend, nếu có)

🎯 **Chức năng chính:**
- Đăng ký người dùng bao gồm admin, nhân viên, khách hàng
+ admin*(quản trị viên) có quyền thêm sửa xóa phòng ,xem trạng thái phòng ,thêm tiện nghi 
+ nhân viên có quyền sửa phòng 
+ khách hàng có quyền đặt phòng

- Đặt phòng, thanh toán, và xuất hoá đơn, và xem lịch sử giao dịch
- Quản lý dịch vụ đi kèm
- Xem phản hồi từ khách hàng (feedback)
- Phân quyền người dùng (admin, nhân viên, khách hàng)

🔗 **GitHub Repository:** [https://github.com/Persinus/HotelManagementAPI](https://github.com/Persinus/HotelManagementAPI)
"
    });
});

builder.Services.AddControllers();

// Đăng ký IMemoryCache
builder.Services.AddMemoryCache();

// Cấu hình JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Kiểm tra Issuer
            ValidateAudience = true, // Kiểm tra Audience
            ValidateLifetime = true, // Kiểm tra thời gian sống của token
            ValidateIssuerSigningKey = true, // Kiểm tra chữ ký của token
            ValidIssuer = "your-issuer", // Thay thế với Issuer của bạn
            ValidAudience = "your-audience", // Thay thế với Audience của bạn
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("your_super_secret_key_1234567890") // Khóa bí mật của bạn
            ),
            NameClaimType = "sub", // Chỉ định claim `sub` làm `Name`
            RoleClaimType = "Vaitro" // Chỉ định claim `Vaitro` làm `Role`
        };

        // Đảm bảo rằng Bearer token được truyền vào
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                // Kiểm tra xem token có trong header Authorization không
                if (!context.Request.Headers.ContainsKey("Authorization"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    return Task.CompletedTask;
                }

                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("QuanTriVienPolicy", policy =>
        policy.RequireClaim("Vaitro", "QuanTriVien"));

    options.AddPolicy("NhanVienPolicy", policy =>
        policy.RequireClaim("Vaitro", "NhanVien", "QuanTriVien"));

    options.AddPolicy("KhachHangPolicy", policy =>
        policy.RequireClaim("Vaitro", "KhachHang","NhanVien", "QuanTriVien"));
});

var app = builder.Build();
app.UseStaticFiles();

// Cấu hình Swagger
var enableSwagger = builder.Configuration.GetValue<bool>("Swagger:Enable");
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Management API V1");
        c.RoutePrefix = string.Empty; // Đặt Swagger UI ở root URL
        c.DocumentTitle = "Hotel Management API Documentation";
        c.InjectJavascript("/swagger-login.js"); // Correctly inject JavaScript in SwaggerUIOptions
        c.InjectStylesheet("/swagger-custom.css");
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins"); // Áp dụng chính sách CORS đã cấu hình

// Thêm middleware xác thực và phân quyền
app.UseAuthentication();
app.UseMiddleware<RoleMiddleware>();
app.UseAuthorization();

// Định tuyến các controller
app.MapControllers();

// Cấu hình chuyển hướng HTTPS
app.UseHttpsRedirection();

app.Run();

// Lớp cấu hình cho SentimentModelPath
public class SentimentModelConfig
{
    public string ModelPath { get; set; }
}
