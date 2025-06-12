using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using HotelManagementAPI.Services;
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
    options.UseInlineDefinitionsForEnums(); // ✅ Hiển thị enum trong Swagger
    options.EnableAnnotations(); // ✅ Bật hỗ trợ ghi chú trong Swagger
    options.DescribeAllParametersInCamelCase(); // ✅ Hiển thị tham số theo camelCase
    options.CustomSchemaIds(type => type.FullName); // Sử dụng tên đầy đủ của lớp làm ID schema


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
- Nuxt 3

🎯 **Chức năng chính:**
- Đăng ký người dùng bao gồm Quản Trị Viên, nhân viên, khách hàng
+ Quản Trị Viên (admin) có quyền thêm sửa xóa phòng, xem trạng thái phòng, thêm tiện nghi
+ Nhân Viên có quyền sửa phòng
+ Khách Hàng có quyền đặt phòng
- Quản lý phòng khách sạn (thêm, sửa, xoá, xem chi tiết)
- Quản lý đặt phòng (thêm, sửa, xoá, xem chi tiết)
- Quản lý khách hàng (thêm, sửa, xoá, xem chi tiết)
- Quản lý nhân viên (thêm, sửa, xoá, xem chi tiết)
- Quản lý dịch vụ (thêm, sửa, xoá, xem chi tiết)
- Quản lý tiện nghi phòng (thêm, sửa, xoá, xem chi tiết)
- Quản lý hoá đơn (thêm, sửa, xoá, xem chi tiết)
- Đặt phòng, thanh toán, và xuất hoá đơn, và xem lịch sử giao dịch
- Quản lý dịch vụ đi kèm
- Xem phản hồi từ khách hàng (feedback)
- Phân quyền người dùng (admin, nhân viên, khách hàng)

🔗 **GitHub Repository:** [https://github.com/Persinus/HotelManagementAPI](https://github.com/Persinus/HotelManagementAPI)
"
    });
});
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Đăng ký IMemoryCache
builder.Services.AddMemoryCache();
// Cấu hình Cloudinary
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));
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

     options.Events = new JwtBearerEvents
{
    OnChallenge = context =>
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\":\"Unauthorized. Authorization header is missing.\"}");
        }

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync("{\"error\":\"Unauthorized. Token is missing or invalid.\"}");
    },
    OnAuthenticationFailed = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync("{\"error\":\"Authentication failed. Invalid token.\"}");
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


builder.Services.AddSingleton<IVnpayService, VnpayService>();

var app = builder.Build();

// Cấu hình Swagger
var enableSwagger = builder.Configuration.GetValue<bool>("Swagger:Enable");
if (enableSwagger || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Management API V1");
        c.RoutePrefix = string.Empty;
    
    

        c.DocumentTitle = "Hotel Management API Documentation";
        c.DisplayRequestDuration();
        c.ConfigObject.PersistAuthorization = true;
        c.ConfigObject.ShowExtensions = true;
        c.ConfigObject.ShowCommonExtensions = true;
        c.ConfigObject.DisplayOperationId = true;
        c.ConfigObject.DisplayRequestDuration = true;

        c.ConfigObject.DeepLinking = true;



       

    });
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
