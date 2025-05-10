using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HotelManagementAPI.Helper;

public class DangNhapDTO
{
    public string TenTaiKhoan { get; set; }
    public string MatKhau { get; set; }
}

public class LichSuGiaoDichDTO
{
    public string MaGiaoDich { get; set; }
    public string MaNguoiDung { get; set; }
    public string LoaiGiaoDich { get; set; }
    public DateTime? ThoiGianGiaoDich { get; set; }
    public string? MoTa { get; set; }
}

public class HoaDonDTO
{
    public string MaHoaDon { get; set; }
    public string MaNguoiDung { get; set; }
    public string MaDatPhong { get; set; }
    public decimal TongTien { get; set; }
    public DateTime? NgayTaoHoaDon { get; set; }
    public DateTime? NgayThanhToan { get; set; }
    public string TinhTrangHoaDon { get; set; }
}

public class DatPhongDTO
{
    public string MaDatPhong { get; set; }
    public string MaNguoiDung { get; set; }
    public string MaPhong { get; set; }
    public DateTime? NgayDat { get; set; }
    public DateTime? NgayCheckIn { get; set; }
    public DateTime? NgayCheckOut { get; set; }
    public string TinhTrangDatPhong { get; set; }
}

public class NhanVienDTO
{
    public string MaNhanVien { get; set; } // Tự động tạo
    public string MaNguoiDung { get; set; } // Tự động tạo
    public string ChucVu { get; set; } // Bắt buộc
    public decimal Luong { get; set; } // Bắt buộc
    public DateTime? NgayVaoLam { get; set; } // Bắt buộc
    public string? CaLamViec { get; set; } // Tùy chọn
    public string Email { get; set; } // Bắt buộc
    public string TenTaiKhoan { get; set; } // Bắt buộc
    public string MatKhau { get; set; } // Bắt buộc
    public string? HoTen { get; set; } // Tùy chọn
    public string? SoDienThoai { get; set; } // Tùy chọn
    public string? DiaChi { get; set; } // Tùy chọn
    public DateTime? NgaySinh { get; set; } // Tùy chọn
    public string? GioiTinh { get; set; } // Tùy chọn
    public string? HinhAnhUrl { get; set; } // Tùy chọn
}

public class NguoiDungDTO
{
    public string MaNguoiDung { get; set; } // Tự động tạo
    public string Vaitro { get; set; } // Tự động gán
    public string Email { get; set; } // Bắt buộc
    public string TenTaiKhoan { get; set; } // Bắt buộc
    public string MatKhau { get; set; } // Bắt buộc
    public string? HoTen { get; set; } // Tùy chọn
    public string? SoDienThoai { get; set; } // Tùy chọn
    public string? DiaChi { get; set; } // Tùy chọn
    public DateTime? NgaySinh { get; set; } // Tùy chọn
    public string? GioiTinh { get; set; } // Tùy chọn
    public string? HinhAnhUrl { get; set; } // Tùy chọn
    public DateTime? NgayTao { get; set; } // Tự động gán
    public string? CanCuocCongDan { get; set; } // Tùy chọn
}

namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/admin/nguoidung")]
    public class NguoiDungController : ControllerBase
    {
        private readonly IDbConnection _db;

        public NguoiDungController(IDbConnection db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(Policy = "Quản trị viên")]
        public async Task<ActionResult<IEnumerable<NguoiDungDTO>>> GetAll()
        {
            const string query = "SELECT * FROM NguoiDung";
            var users = await _db.QueryAsync<NguoiDungDTO>(query);
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Quản trị viên")]
        public async Task<ActionResult<NguoiDungDTO>> GetById(string id)
        {
            const string query = "SELECT * FROM NguoiDung WHERE MaNguoiDung = @Id";
            var user = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { Id = id });

            if (user == null)
                return NotFound(new { Message = "Không tìm thấy người dùng." });

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Policy = "Quản trị viên")]
        public async Task<ActionResult<NguoiDungDTO>> Create([FromBody] NguoiDungDTO nguoiDung)
        {
            const string query = @"
                INSERT INTO NguoiDung (MaNguoiDung, Vaitro, Email, TenTaiKhoan, HoTen, SoDienThoai, DiaChi, NgaySinh, GioiTinh, HinhAnhUrl, NgayTao)
                VALUES (@MaNguoiDung, @Vaitro, @Email, @TenTaiKhoan, @HoTen, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @HinhAnhUrl, @NgayTao)";
            await _db.ExecuteAsync(query, nguoiDung);

            return CreatedAtAction(nameof(GetById), new { id = nguoiDung.MaNguoiDung }, nguoiDung);
        }

        /// <summary>
        /// Cập nhật thông tin người dùng.
        /// </summary>
        /// <remarks>
        /// **Quyền**: Chỉ dành cho Quản trị viên.
        /// </remarks>
        /// <param name="id">Mã người dùng cần cập nhật.</param>
        /// <param name="nguoiDung">Thông tin người dùng cần cập nhật.</param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "Quản trị viên")]
        public async Task<IActionResult> Update(string id, [FromBody] NguoiDungDTO nguoiDung)
        {
            if (id != nguoiDung.MaNguoiDung)
                return BadRequest(new { Message = "Mã người dùng không khớp." });

            const string query = @"
                UPDATE NguoiDung
                SET Vaitro = @Vaitro, Email = @Email, TenTaiKhoan = @TenTaiKhoan, HoTen = @HoTen, 
                    SoDienThoai = @SoDienThoai, DiaChi = @DiaChi, NgaySinh = @NgaySinh, GioiTinh = @GioiTinh, 
                    HinhAnhUrl = @HinhAnhUrl, NgayTao = @NgayTao
                WHERE MaNguoiDung = @MaNguoiDung";
            var affected = await _db.ExecuteAsync(query, nguoiDung);

            return affected > 0 ? NoContent() : NotFound(new { Message = "Không tìm thấy người dùng." });
        }

        /// <summary>
        /// Xóa một người dùng.
        /// </summary>
        /// <remarks>
        /// **Quyền**: Chỉ dành cho Quản trị viên.
        /// </remarks>
        /// <param name="id">Mã người dùng cần xóa.</param>
        /// <returns>Kết quả xóa.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "Quản trị viên")]
        public async Task<IActionResult> Delete(string id)
        {
            const string query = "DELETE FROM NguoiDung WHERE MaNguoiDung = @Id";
            var affected = await _db.ExecuteAsync(query, new { Id = id });

            return affected > 0 ? NoContent() : NotFound(new { Message = "Không tìm thấy người dùng." });
        }

        /// <summary>
        /// Đăng ký tài khoản khách hàng.
        /// </summary>
        /// <remarks>
        /// Vai trò mặc định là "KhachHang".
        /// </remarks>
        /// <param name="nguoiDung">Thông tin người dùng cần đăng ký.</param>
        /// <returns>Người dùng vừa được đăng ký.</returns>
        [HttpPost("dangky-khachhang")]
        [AllowAnonymous]
        public async Task<ActionResult<NguoiDungDTO>> DangKyKhachHang([FromBody] NguoiDungDTO nguoiDung)
        {
            // Gán vai trò mặc định là "KhachHang"
            nguoiDung.Vaitro = "KhachHang";
            nguoiDung.NgayTao = DateTime.Now;

            // Tự động tạo MaNguoiDung nếu chưa có
            nguoiDung.MaNguoiDung ??= Guid.NewGuid().ToString();

            // Kiểm tra trùng lặp Email
            const string checkEmailQuery = "SELECT COUNT(1) FROM NguoiDung WHERE Email = @Email";
            var isEmailDuplicate = await _db.ExecuteScalarAsync<int>(checkEmailQuery, new { nguoiDung.Email });

            if (isEmailDuplicate > 0)
            {
                return Conflict(new { Message = "Email đã tồn tại. Vui lòng sử dụng email khác." });
            }

            // Kiểm tra trùng lặp Số CCCD
            if (!string.IsNullOrEmpty(nguoiDung.CanCuocCongDan))
            {
                const string checkCCCDQuery = "SELECT COUNT(1) FROM NguoiDung WHERE CanCuocCongDan = @CanCuocCongDan";
                var isCCCDDuplicate = await _db.ExecuteScalarAsync<int>(checkCCCDQuery, new { nguoiDung.CanCuocCongDan });

                if (isCCCDDuplicate > 0)
                {
                    return Conflict(new { Message = "Số CCCD đã tồn tại. Vui lòng kiểm tra lại." });
                }
            }

            // Chèn dữ liệu nếu không trùng lặp
            const string insertQuery = @"
                INSERT INTO NguoiDung (MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen, SoDienThoai, DiaChi, NgaySinh, GioiTinh, HinhAnhUrl, CanCuocCongDan, NgayTao)
                VALUES (@MaNguoiDung, @Vaitro, @Email, @TenTaiKhoan, @MatKhau, @HoTen, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @HinhAnhUrl, @CanCuocCongDan, @NgayTao)";
            await _db.ExecuteAsync(insertQuery, nguoiDung);

            return CreatedAtAction(nameof(GetById), new { id = nguoiDung.MaNguoiDung }, nguoiDung);
        }

        [HttpPut("update-profile/{id}")]
        [Authorize] // Yêu cầu người dùng phải đăng nhập
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] NguoiDungDTO nguoiDung)
        {
            // Kiểm tra xem người dùng có quyền cập nhật thông tin của chính mình không
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID người dùng từ JWT
            if (currentUserId != id)
            {
                return Forbid("Bạn không có quyền cập nhật thông tin của người dùng khác.");
            }

            // Cập nhật thông tin cá nhân
            const string query = @"
                UPDATE NguoiDung
                SET HoTen = @HoTen, SoDienThoai = @SoDienThoai, DiaChi = @DiaChi, NgaySinh = @NgaySinh, GioiTinh = @GioiTinh, HinhAnhUrl = @HinhAnhUrl, CanCuocCongDan = @CanCuocCongDan
                WHERE MaNguoiDung = @MaNguoiDung";
            var affected = await _db.ExecuteAsync(query, new
            {
                nguoiDung.HoTen,
                nguoiDung.SoDienThoai,
                nguoiDung.DiaChi,
                nguoiDung.NgaySinh,
                nguoiDung.GioiTinh,
                nguoiDung.HinhAnhUrl,
                nguoiDung.CanCuocCongDan,
                MaNguoiDung = id
            });

            return affected > 0 ? NoContent() : NotFound(new { Message = "Không tìm thấy người dùng." });
        }
    }

    [ApiController]
    [Route("api/admin/nhanvien")]
    public class NhanVienController : ControllerBase
    {
        private readonly IDbConnection _db;

        public NhanVienController(IDbConnection db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(Policy = "Quản trị viên")]
        public async Task<ActionResult<IEnumerable<NhanVienDTO>>> GetAll()
        {
            const string query = "SELECT * FROM NhanVien";
            var employees = await _db.QueryAsync<NhanVienDTO>(query);
            return Ok(employees);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Quản trị viên")]
        public async Task<ActionResult<NhanVienDTO>> GetById(string id)
        {
            const string query = "SELECT * FROM NhanVien WHERE MaNhanVien = @Id";
            var employee = await _db.QueryFirstOrDefaultAsync<NhanVienDTO>(query, new { Id = id });

            if (employee == null)
                return NotFound(new { Message = "Không tìm thấy nhân viên." });

            return Ok(employee);
        }

        [HttpPost]
        public async Task<ActionResult<NhanVienDTO>> Create([FromBody] NhanVienDTO nhanVien)
        {
            const string query = @"
                INSERT INTO NhanVien (MaNhanVien, MaNguoiDung, ChucVu, Luong, NgayVaoLam, CaLamViec)
                VALUES (@MaNhanVien, @MaNguoiDung, @ChucVu, @Luong, @NgayVaoLam, @CaLamViec)";
            await _db.ExecuteAsync(query, nhanVien);

            return CreatedAtAction(nameof(GetById), new { id = nhanVien.MaNhanVien }, nhanVien);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] NhanVienDTO nhanVien)
        {
            if (id != nhanVien.MaNhanVien)
                return BadRequest(new { Message = "Mã nhân viên không khớp." });

            const string query = @"
                UPDATE NhanVien
                SET MaNguoiDung = @MaNguoiDung, ChucVu = @ChucVu, Luong = @Luong, 
                    NgayVaoLam = @NgayVaoLam, CaLamViec = @CaLamViec
                WHERE MaNhanVien = @MaNhanVien";
            var affected = await _db.ExecuteAsync(query, nhanVien);

            return affected > 0 ? NoContent() : NotFound(new { Message = "Không tìm thấy nhân viên." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            const string query = "DELETE FROM NhanVien WHERE MaNhanVien = @Id";
            var affected = await _db.ExecuteAsync(query, new { Id = id });

            return affected > 0 ? NoContent() : NotFound(new { Message = "Không tìm thấy nhân viên." });
        }

        [HttpPost("dangky")]
        [Authorize(Policy = "Quản trị viên")] // Chỉ dành cho quản trị viên
        public async Task<ActionResult<NhanVienDTO>> DangKyNhanVien([FromBody] NhanVienDTO nhanVien)
        {
            // Tạo tài khoản người dùng cho nhân viên
            const string insertNguoiDungQuery = @"
                INSERT INTO NguoiDung (MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen, SoDienThoai, DiaChi, NgaySinh, GioiTinh, HinhAnhUrl, NgayTao)
                VALUES (@MaNguoiDung, 'NhanVien', @Email, @TenTaiKhoan, @MatKhau, @HoTen, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @HinhAnhUrl, @NgayTao)";
            await _db.ExecuteAsync(insertNguoiDungQuery, new
            {
                nhanVien.MaNguoiDung,
                nhanVien.Email,
                nhanVien.TenTaiKhoan,
                nhanVien.MatKhau,
                nhanVien.HoTen,
                nhanVien.SoDienThoai,
                nhanVien.DiaChi,
                nhanVien.NgaySinh,
                nhanVien.GioiTinh,
                nhanVien.HinhAnhUrl,
                NgayTao = DateTime.Now
            });

            // Tạo thông tin nhân viên
            const string insertNhanVienQuery = @"
                INSERT INTO NhanVien (MaNhanVien, MaNguoiDung, ChucVu, Luong, NgayVaoLam, CaLamViec)
                VALUES (@MaNhanVien, @MaNguoiDung, @ChucVu, @Luong, @NgayVaoLam, @CaLamViec)";
            await _db.ExecuteAsync(insertNhanVienQuery, nhanVien);

            return CreatedAtAction(nameof(GetById), new { id = nhanVien.MaNhanVien }, nhanVien);
        }
    }

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IDbConnection _db;

        public AuthController(IDbConnection db)
        {
            _db = db;
        }

        [HttpPost("dangnhap")]
        [AllowAnonymous]
        public async Task<IActionResult> DangNhap([FromBody] DangNhapDTO dangNhap)
        {
            const string query = "SELECT * FROM NguoiDung WHERE TenTaiKhoan = @TenTaiKhoan AND MatKhau = @MatKhau";
            var nguoiDung = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { dangNhap.TenTaiKhoan, dangNhap.MatKhau });

            if (nguoiDung == null)
                return Unauthorized(new { Message = "Tên tài khoản hoặc mật khẩu không đúng." });

            var token = JwtHelper.GenerateJwtToken(
                nguoiDung,
                "your_super_secret_key_1234567890",
                "your-issuer",
                "your-audience"
            );

            return Ok(new
            {
                Message = "Đăng nhập thành công.",
                Token = token,
                User = new
                {
                    nguoiDung.MaNguoiDung,
                    nguoiDung.Vaitro,
                    nguoiDung.Email,
                    nguoiDung.TenTaiKhoan,
                    nguoiDung.HoTen
                }
            });
        }

        [HttpPost("dangky")]
        [AllowAnonymous]
        public async Task<ActionResult<NguoiDungDTO>> DangKy([FromBody] NguoiDungDTO nguoiDung)
        {
            nguoiDung.Vaitro = "KhachHang";
            nguoiDung.NgayTao = DateTime.Now;

            const string query = @"
                INSERT INTO NguoiDung (MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen, SoDienThoai, DiaChi, NgaySinh, GioiTinh, HinhAnhUrl, NgayTao)
                VALUES (@MaNguoiDung, @Vaitro, @Email, @TenTaiKhoan, @MatKhau, @HoTen, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @HinhAnhUrl, @NgayTao)";
            await _db.ExecuteAsync(query, nguoiDung);

            return CreatedAtAction(nameof(DangNhap), new { id = nguoiDung.MaNguoiDung }, nguoiDung);
        }
    }
}