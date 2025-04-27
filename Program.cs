using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;

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

// Thêm Swagger và các dịch vụ khác
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddControllers();

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
        c.InjectStylesheet("/swagger-custom.css");
    });
   
   


}

// Sử dụng CORS
app.UseCors("AllowAllOrigins"); // Áp dụng chính sách CORS đã cấu hình

// Sử dụng Authentication và Authorization nếu cần
// app.UseAuthentication();
// app.UseAuthorization(); // Nếu bạn có sử dụng Authorization
// Nếu không sử dụng Authentication và Authorization, bạn có thể bỏ qua hai dòng trên
// Cấu hình các middleware khác nếu cần


// Định tuyến các controller
app.MapControllers();

// Cấu hình chuyển hướng HTTPS
app.UseHttpsRedirection();

app.Run();