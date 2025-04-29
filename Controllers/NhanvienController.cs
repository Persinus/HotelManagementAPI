using System.Collections.Generic;//chưa sửa
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.Models;

namespace HotelManagementAPI.Controllers
{
    public class NhanVienDTO
{
    /// <summary>
    /// Mã nhân viên (duy nhất). Ví dụ: `NV001`.
    /// Trường này sẽ được tự động sinh, không cần nhập.
    /// </summary>
    public string? MaNhanVien { get; set; }

    /// <summary>
    /// Họ và tên nhân viên. Ví dụ: `Nguyen Van A`.
    /// </summary>
    public string HoTen { get; set; } = null!;

    /// <summary>
    /// Số điện thoại của nhân viên. Ví dụ: `0912345678`.
    /// </summary>
    public string SoDienThoai { get; set; } = null!;

    /// <summary>
    /// Số căn cước công dân của nhân viên. Ví dụ: `123456789`.
    /// </summary>
    public string CanCuocCongDan { get; set; } = null!;

    /// <summary>
    /// URL hình ảnh của nhân viên. Ví dụ: `https://example.com/image.jpg`.
    /// </summary>
    public string? HinhAnh { get; set; }

    /// <summary>
    /// Ngày vào làm của nhân viên. Ví dụ: `2025-04-27`.
    /// </summary>
    public DateTime NgayVaoLam { get; set; }

    /// <summary>
    /// Trạng thái của nhân viên. Ví dụ: `Hoat Dong` hoặc `Tam Ngung`.
    /// </summary>
    public string? TrangThai { get; set; }

    /// <summary>
    /// Email của nhân viên. Ví dụ: `nguyenvana@gmail.com`.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Tên tài khoản của nhân viên. Ví dụ: `nguyenvana`.
    /// </summary>
    public string TenTaiKhoan { get; set; } = null!;

    /// <summary>
    /// Mật khẩu của nhân viên. Ví dụ: `123456`.
    /// </summary>
    public string MatKhau { get; set; } = null!;

    /// <summary>
    /// Khóa JWK (JSON Web Key) của nhân viên. Ví dụ: `null` hoặc một chuỗi JSON.
    /// Trường này sẽ được tự động tạo, không cần nhập.
    /// </summary>
    public string? Jwk { get; set; }
}

    [ApiController]
    [Route("api/[controller]")]
    public class NhanVienController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public NhanVienController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Lấy danh sách tất cả nhân viên.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về danh sách tất cả nhân viên có trong hệ thống.
        ///
        /// **Mã trạng thái**:
        /// - 200: Thành công, trả về danh sách nhân viên.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <returns>Danh sách nhân viên.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVienDTO>>> GetAll()
        {
            const string query = "SELECT * FROM NhanVien";
            var result = await _dbConnection.QueryAsync<NhanVienDTO>(query);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin nhân viên theo mã.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về thông tin chi tiết của một nhân viên dựa trên mã nhân viên (`MaNhanVien`).
        ///
        /// **Mã trạng thái**:
        /// - 200: Thành công, trả về thông tin nhân viên.
        /// - 404: Không tìm thấy nhân viên.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã nhân viên cần tìm. Ví dụ: `NV001`.</param>
        /// <returns>Thông tin nhân viên.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVienDTO>> GetById(string id)
        {
            const string query = "SELECT * FROM NhanVien WHERE MaNhanVien = @Id";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<NhanVienDTO>(query, new { Id = id });

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Tạo mới một nhân viên.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này tạo mới một nhân viên trong hệ thống với thông tin được cung cấp.
        /// Mã nhân viên (`MaNhanVien`) sẽ được tự động sinh theo định dạng `NVXXX`.
        ///
        /// **Ví dụ Request Body**:
        /// <code>
        /// {
        ///   "HoTen": "Nguyen Van A",
        ///   "SoDienThoai": "0912345678",
        ///   "CanCuocCongDan": "123456789",
        ///   "HinhAnh": "https://example.com/image.jpg",
        ///   "NgayVaoLam": "2025-04-27",
        ///   "TrangThai": "Hoat Dong",
        ///   "Email": "nguyenvana@gmail.com",
        ///   "TenTaiKhoan": "nguyenvana",
        ///   "MatKhau": "123456"
        /// }
        /// </code>
        ///
        /// **Mã trạng thái**:
        /// - 201: Tạo thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="nhanVienDto">Thông tin nhân viên cần tạo.</param>
        /// <returns>Thông tin nhân viên vừa được tạo.</returns>
        [HttpPost]
        public async Task<ActionResult<NhanVienDTO>> Create([FromBody] NhanVienDTO nhanVienDto)
        {
            // Tự động sinh mã nhân viên
            const string getMaxIdQuery = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaNhanVien, 3, LEN(MaNhanVien) - 2) AS INT)), 0) FROM NhanVien";
            var maxId = await _dbConnection.ExecuteScalarAsync<int>(getMaxIdQuery);
            nhanVienDto.MaNhanVien = $"NV{(maxId + 1):D3}";

            // Tạo JWT
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes("your_super_secret_key_1234567890");
            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("MaNhanVien", nhanVienDto.MaNhanVien),
                    new System.Security.Claims.Claim("Email", nhanVienDto.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                    Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            nhanVienDto.Jwk = tokenHandler.WriteToken(token);

            // Thực hiện chèn dữ liệu
            const string insertQuery = @"
                INSERT INTO NhanVien 
                (MaNhanVien, HoTen, SoDienThoai, CanCuocCongDan, HinhAnh, NgayVaoLam, TrangThai, Email, TenTaiKhoan, MatKhau, Jwk)
                VALUES 
                (@MaNhanVien, @HoTen, @SoDienThoai, @CanCuocCongDan, @HinhAnh, @NgayVaoLam, @TrangThai, @Email, @TenTaiKhoan, @MatKhau, @Jwk)";

            await _dbConnection.ExecuteAsync(insertQuery, nhanVienDto);
            return CreatedAtAction(nameof(GetById), new { id = nhanVienDto.MaNhanVien }, nhanVienDto);
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này cập nhật thông tin của một nhân viên dựa trên mã nhân viên (`MaNhanVien`).
        ///
        /// **Mã trạng thái**:
        /// - 204: Cập nhật thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 404: Không tìm thấy nhân viên.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã nhân viên cần cập nhật. Ví dụ: `NV001`.</param>
        /// <param name="nhanVienDto">Thông tin nhân viên cần cập nhật.</param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] NhanVienDTO nhanVienDto)
        {
            if (id != nhanVienDto.MaNhanVien)
                return BadRequest();

            const string query = @"
                UPDATE NhanVien
                SET HoTen = @HoTen,
                    SoDienThoai = @SoDienThoai,
                    CanCuocCongDan = @CanCuocCongDan,
                    HinhAnh = @HinhAnh,
                    NgayVaoLam = @NgayVaoLam,
                    TrangThai = @TrangThai,
                    Email = @Email,
                    TenTaiKhoan = @TenTaiKhoan,
                    MatKhau = @MatKhau
                WHERE MaNhanVien = @MaNhanVien";

            var affectedRows = await _dbConnection.ExecuteAsync(query, nhanVienDto);

            if (affectedRows == 0)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Xóa một nhân viên theo mã.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này xóa một nhân viên khỏi hệ thống dựa trên mã nhân viên (`MaNhanVien`).
        ///
        /// **Mã trạng thái**:
        /// - 204: Xóa thành công.
        /// - 404: Không tìm thấy nhân viên.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã nhân viên cần xóa. Ví dụ: `NV001`.</param>
        /// <returns>Kết quả xóa.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            const string query = "DELETE FROM NhanVien WHERE MaNhanVien = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(query, new { Id = id });

            if (affectedRows == 0)
                return NotFound();

            return NoContent();
        }
    }
}