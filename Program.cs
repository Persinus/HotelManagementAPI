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
        Description = "Enter 'Bearer' [space] and then your token",
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
    // Removed InjectJavascript from SwaggerGenOptions as it is not valid here
});

builder.Services.AddControllers();

// Cấu hình JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(secretKey)
            )
        };
    });

// Cấu hình phân quyền
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Quản trị viên", policy => policy.RequireRole("QuanTriVien"));
    options.AddPolicy("Nhân viên", policy => policy.RequireRole("NhanVien"));
    options.AddPolicy("Khách hàng", policy => policy.RequireRole("KhachHang"));
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


app.UseCors("AllowAllOrigins"); // Áp dụng chính sách CORS đã cấu hình



// Thêm middleware xác thực và phân quyền
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RoleMiddleware>();


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
