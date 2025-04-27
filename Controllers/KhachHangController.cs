using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.Models;

/// <summary>
/// DTO đại diện cho thông tin khách hàng.
/// </summary>
public class KhachHangDTO
{
    /// <summary>
    /// Mã khách hàng (duy nhất). Ví dụ: `KH001`.
    /// </summary>
    public string MaKhachHang { get; set; } = null!;

    /// <summary>
    /// Họ và tên khách hàng. Ví dụ: `Nguyen Van A`.
    /// </summary>
    public string HoTen { get; set; } = null!;

    /// <summary>
    /// Số căn cước công dân của khách hàng. Ví dụ: `123456789`.
    /// </summary>
    public string CanCuocCongDan { get; set; } = null!;

    /// <summary>
    /// Số điện thoại của khách hàng. Ví dụ: `0912345678`.
    /// </summary>
    public string SoDienThoai { get; set; } = null!;

    /// <summary>
    /// Địa chỉ của khách hàng. Ví dụ: `123 Nguyen Trai, Ha Noi`.
    /// </summary>
    public string? DiaChi { get; set; }

    /// <summary>
    /// Ngày sinh của khách hàng. Ví dụ: `1990-01-01`.
    /// </summary>
    public DateTime? NgaySinh { get; set; }

    /// <summary>
    /// Giới tính của khách hàng. Ví dụ: `Nam` hoặc `Nu`.
    /// </summary>
    public string? GioiTinh { get; set; }

    /// <summary>
    /// Nghề nghiệp của khách hàng. Ví dụ: `Giao Vien`.
    /// </summary>
    public string? NgheNghiep { get; set; }

    /// <summary>
    /// Trạng thái của khách hàng. Ví dụ: `Hoat Dong` hoặc `Tam Ngung`.
    /// </summary>
    public string? TrangThai { get; set; }

    /// <summary>
    /// Ngày tạo thông tin khách hàng. Ví dụ: `2025-04-27`.
    /// </summary>
    public DateTime? NgayTao { get; set; }

    /// <summary>
    /// Ngày cập nhật thông tin khách hàng. Ví dụ: `2025-04-28`.
    /// </summary>
    public DateTime? NgayCapNhat { get; set; }

    /// <summary>
    /// URL hình ảnh của khách hàng. Ví dụ: `https://example.com/image.jpg`.
    /// </summary>
    public string? HinhAnh { get; set; }

    /// <summary>
    /// Email của khách hàng. Ví dụ: `example@gmail.com`.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Tên tài khoản của khách hàng. Ví dụ: `nguyenvana`.
    /// </summary>
    public string TenTaiKhoan { get; set; } = null!;

    /// <summary>
    /// Mật khẩu của khách hàng. Ví dụ: `123456`.
    /// </summary>
    public string MatKhau { get; set; } = null!;

    /// <summary>
    /// Khóa JWK (JSON Web Key) của khách hàng. Ví dụ: `null` hoặc một chuỗi JSON.
    /// </summary>
    public string? Jwk { get; set; }
}

namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KhachHangController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public KhachHangController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Lấy danh sách tất cả khách hàng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về danh sách tất cả khách hàng có trong hệ thống.
        /// </remarks>
        /// <returns>Danh sách khách hàng.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhachHangDTO>>> GetAll()
        {
            const string query = "SELECT * FROM KhachHang";
            var result = await _dbConnection.QueryAsync<KhachHangDTO>(query);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo mã khách hàng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về thông tin chi tiết của một khách hàng dựa trên mã khách hàng (`MaKhachHang`).
        /// </remarks>
        /// <param name="id">Mã khách hàng cần tìm. Ví dụ: `KH001`.</param>
        /// <returns>Thông tin khách hàng.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<KhachHangDTO>> GetById(string id)
        {
            const string query = "SELECT * FROM KhachHang WHERE MaKhachHang = @Id";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<KhachHangDTO>(query, new { Id = id });

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Tạo mới một khách hàng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này tạo mới một khách hàng trong hệ thống với thông tin được cung cấp.
        ///
        /// **Ví dụ Request Body**:
        /// <code>
        /// {
        ///   "MaKhachHang": "KH002",
        /// 
        ///   "HoTen": "Tran Thi B",
        ///   "CanCuocCongDan": "987654321",
        ///   "SoDienThoai": "0912345678",
        ///   "DiaChi": "Ho Chi Minh",
        ///   "NgaySinh": "1995-05-15T00:00:00",
        ///   "GioiTinh": "Nu",
        ///   "NgheNghiep": "Giao Vien",
        ///   "TrangThai": "Hoat Dong",
        ///   "NgayTao": "2025-04-27T00:00:00",
        ///   "NgayCapNhat": null,
        ///   "HinhAnh": null,
        ///   "Email": "tranthib@gmail.com",
        ///   "TenTaiKhoan": "tranthib",
        ///   "MatKhau": "abcdef",
        ///   "Jwk": null
        /// }
        /// </code>
        ///
        /// **Mã trạng thái**:
        /// - 201: Tạo thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="khachHangDto">Thông tin khách hàng cần tạo.</param>
        /// <returns>Thông tin khách hàng vừa được tạo.</returns>
        [HttpPost]
        public async Task<ActionResult<KhachHangDTO>> Create([FromBody] KhachHangDTO khachHangDto)
        {
            const string query = @"
                INSERT INTO KhachHang 
                (MaKhachHang, HoTen, CanCuocCongDan, SoDienThoai, DiaChi, NgaySinh, GioiTinh, NgheNghiep, TrangThai, 
                NgayTao, NgayCapNhat, HinhAnh, Email, TenTaiKhoan, MatKhau, Jwk)
                VALUES 
                (@MaKhachHang, @HoTen, @CanCuocCongDan, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @NgheNghiep, @TrangThai, 
                @NgayTao, @NgayCapNhat, @HinhAnh, @Email, @TenTaiKhoan, @MatKhau, @Jwk)";

            await _dbConnection.ExecuteAsync(query, khachHangDto);
            return CreatedAtAction(nameof(GetById), new { id = khachHangDto.MaKhachHang }, khachHangDto);
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này cập nhật thông tin của một khách hàng dựa trên mã khách hàng (`MaKhachHang`).
        /// </remarks>
        /// <param name="id">Mã khách hàng cần cập nhật. Ví dụ: `KH002`.</param>
        /// <param name="khachHangDto">Thông tin khách hàng cần cập nhật. Ví dụ:
        /// ```json
        /// {
        ///   "MaKhachHang": "KH002",
        ///   "HoTen": "Tran Thi B Updated",
        ///   "CanCuocCongDan": "987654321",
        ///   "SoDienThoai": "0912345678",
        ///   "DiaChi": "Da Nang",
        ///   "NgaySinh": "1995-05-15T00:00:00",
        ///   "GioiTinh": "Nu",
        ///   "NgheNghiep": "Giao Vien",
        ///   "TrangThai": "Tam Ngung",
        ///   "NgayTao": "2025-04-27T00:00:00",
        ///   "NgayCapNhat": "2025-04-28T00:00:00",
        ///   "HinhAnh": null,
        ///   "Email": "tranthib_updated@gmail.com",
        ///   "TenTaiKhoan": "tranthib",
        ///   "MatKhau": "abcdef",
        ///   "Jwk": null
        /// }
        /// ```
        /// </param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] KhachHangDTO khachHangDto)
        {
            if (id != khachHangDto.MaKhachHang)
                return BadRequest();

            const string query = @"
                UPDATE KhachHang
                SET HoTen = @HoTen,
                    CanCuocCongDan = @CanCuocCongDan,
                    SoDienThoai = @SoDienThoai,
                    DiaChi = @DiaChi,
                    NgaySinh = @NgaySinh,
                    GioiTinh = @GioiTinh,
                    NgheNghiep = @NgheNghiep,
                    TrangThai = @TrangThai,
                    NgayTao = @NgayTao,
                    NgayCapNhat = @NgayCapNhat,
                    HinhAnh = @HinhAnh,
                    Email = @Email,
                    TenTaiKhoan = @TenTaiKhoan,
                    MatKhau = @MatKhau,
                    Jwk = @Jwk
                WHERE MaKhachHang = @MaKhachHang";

            var affectedRows = await _dbConnection.ExecuteAsync(query, khachHangDto);

            if (affectedRows == 0)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Xóa một khách hàng theo mã khách hàng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này xóa một khách hàng khỏi hệ thống dựa trên mã khách hàng (`MaKhachHang`).
        /// </remarks>
        /// <param name="id">Mã khách hàng cần xóa. Ví dụ: `KH002`.</param>
        /// <returns>Kết quả xóa.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            const string query = "DELETE FROM KhachHang WHERE MaKhachHang = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(query, new { Id = id });

            if (affectedRows == 0)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Tìm kiếm khách hàng theo tên hoặc số điện thoại.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về danh sách khách hàng phù hợp với tên hoặc số điện thoại được cung cấp.
        /// </remarks>
        /// <param name="name">Tên khách hàng cần tìm (tùy chọn). Ví dụ: `Nguyen`.</param>
        /// <param name="phone">Số điện thoại khách hàng cần tìm (tùy chọn). Ví dụ: `0912345678`.</param>
        /// <returns>Danh sách khách hàng phù hợp.</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<KhachHangDTO>>> Search([FromQuery] string? name, [FromQuery] string? phone)
        {
            const string query = @"
                SELECT * 
                FROM KhachHang
                WHERE (@Name IS NULL OR HoTen LIKE '%' + @Name + '%')
                  AND (@Phone IS NULL OR SoDienThoai LIKE '%' + @Phone + '%')";

            var result = await _dbConnection.QueryAsync<KhachHangDTO>(query, new { Name = name, Phone = phone });

            if (!result.Any())
                return NotFound();

            return Ok(result);
        }
    }
}