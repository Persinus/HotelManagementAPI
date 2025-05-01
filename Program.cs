using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HotelManagementAPI.Hus;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình chuỗi kết nối
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký IDbConnection với DI
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Thay bằng URL frontend của bạn
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Cho phép credentials
    });
});

// Thêm Swagger và các dịch vụ khác
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
   
});

builder.Services.AddControllers();

// Cấu hình Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("your_secret_key_here")),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Thêm SignalR vào DI container
builder.Services.AddSignalR();

var app = builder.Build();

// Sử dụng Static Files
app.UseStaticFiles();

// Sử dụng Routing
app.UseRouting();

// Sử dụng CORS
app.UseCors("AllowAllOrigins");

// Sử dụng Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();

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
        c.InjectStylesheet("/swagger-custom.css");
    });
}

// Cấu hình chuyển hướng HTTPS
app.UseHttpsRedirection();

// Định nghĩa SignalR Hub route
// Thêm Middleware
app.UseAuthentication();
app.UseAuthorization();

// Sử dụng CORS
app.UseCors("AllowSpecificOrigins");

// Định nghĩa endpoint SignalR
app.MapHub<ChatHub>("/chathub").RequireCors("AllowSpecificOrigins");

// Định tuyến các controller
app.MapControllers();

app.Run();