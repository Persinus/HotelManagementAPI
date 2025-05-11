using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;


// QuanTriNguoiDungController.cs
// Mục đích: Quản lý toàn bộ người dùng

//GET /api/admin/nguoidung – Danh sách người dùng

//PUT /api/admin/nguoidung/{id} – Cập nhật vai trò, trạng thái

//DELETE /api/admin/nguoidung/{id} – Xóa người dùng
namespace HotelManagementAPI.Controllers.QuanTriVien
{
    [ApiController]
    [Route("api/quantrivien")]
    [Authorize(Policy = "QuanTriVienPolicy")] // Chỉ cho phép người dùng có vai trò QuanTriVien
    public class QuanTriVienController : ControllerBase
    {
        private readonly IDbConnection _db;

        public QuanTriVienController(IDbConnection db)
        {
            _db = db;
        }

        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            return Ok(new { Message = "Đây là dashboard của Quản trị viên." });
        }

        /// <summary>
        /// Đăng ký tài khoản quản trị viên.
        /// </summary>
        [HttpPost("dangky")]
        [Authorize(Policy = "QuanTriVienPolicy")]
        public async Task<ActionResult<NguoiDungDTO>> DangKyQuanTriVien([FromBody] NguoiDungDTO nguoiDung)
        {
            nguoiDung.Vaitro = "QuanTriVien";
            nguoiDung.MaNguoiDung = await GenerateUniqueMaNguoiDung();
            nguoiDung.NgayTao = DateTime.Now;

            const string checkEmailQuery = "SELECT COUNT(1) FROM NguoiDung WHERE Email = @Email";
            var isEmailDuplicate = await _db.ExecuteScalarAsync<int>(checkEmailQuery, new { nguoiDung.Email });

            if (isEmailDuplicate > 0)
            {
                return Conflict(new { Message = "Email đã tồn tại. Vui lòng sử dụng email khác." });
            }

            const string insertQuery = @"
                INSERT INTO NguoiDung (MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen, SoDienThoai, DiaChi, NgaySinh, GioiTinh, HinhAnhUrl, CanCuocCongDan, NgayTao)
                VALUES (@MaNguoiDung, @Vaitro, @Email, @TenTaiKhoan, @MatKhau, @HoTen, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @HinhAnhUrl, @CanCuocCongDan, @NgayTao)";
            await _db.ExecuteAsync(insertQuery, nguoiDung);

            return CreatedAtAction(nameof(DangKyQuanTriVien), new { id = nguoiDung.MaNguoiDung }, nguoiDung);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của quản trị viên đã xác thực.
        /// </summary>
        [HttpGet("profile")]
        public async Task<ActionResult<NguoiDungDTO>> GetProfile()
        {
            try
            {
                // Ghi log danh sách claims
                Console.WriteLine("=== Claims từ token ===");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }

                // Lấy MaNguoiDung từ claim "nameidentifier"
                var maNguoiDung = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(maNguoiDung))
                {
                    return Unauthorized(new { Message = "Không tìm thấy thông tin người dùng trong token." });
                }

                // Truy vấn thông tin người dùng từ cơ sở dữ liệu
                const string query = "SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
                var nguoiDung = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { MaNguoiDung = maNguoiDung });

                if (nguoiDung == null)
                {
                    return NotFound(new { Message = "Không tìm thấy thông tin người dùng." });
                }

                // Trả về thông tin người dùng
                return Ok(nguoiDung);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy thông tin người dùng." });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả khách hàng.
        /// </summary>
        [HttpGet("khachhang")]
        public async Task<ActionResult<IEnumerable<NguoiDungDTO>>> GetAllKhachHang()
        {
            const string query = "SELECT * FROM NguoiDung WHERE Vaitro = 'KhachHang'";
            var khachHangList = await _db.QueryAsync<NguoiDungDTO>(query);
            return Ok(khachHangList);
        }

        /// <summary>
        /// Lấy danh sách tất cả nhân viên.
        /// </summary>
        [HttpGet("nhanvien")]
        public async Task<ActionResult<IEnumerable<NguoiDungDTO>>> GetAllNhanVien()
        {
            const string query = "SELECT * FROM NguoiDung WHERE Vaitro = 'NhanVien'";
            var nhanVienList = await _db.QueryAsync<NguoiDungDTO>(query);
            return Ok(nhanVienList);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một khách hàng dựa trên MaNguoiDung.
        /// </summary>
        [HttpGet("khachhang/{maNguoiDung}")]
        public async Task<ActionResult<NguoiDungDTO>> GetKhachHangByMaNguoiDung(string maNguoiDung)
        {
            const string query = "SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung AND Vaitro = 'KhachHang'";
            var khachHang = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { MaNguoiDung = maNguoiDung });

            if (khachHang == null)
            {
                return NotFound(new { Message = "Không tìm thấy thông tin khách hàng." });
            }

            return Ok(khachHang);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một nhân viên dựa trên MaNguoiDung.
        /// </summary>
        [HttpGet("nhanvien/{maNguoiDung}")]
        public async Task<ActionResult<NguoiDungDTO>> GetNhanVienByMaNguoiDung(string maNguoiDung)
        {
            const string query = "SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung AND Vaitro = 'NhanVien'";
            var nhanVien = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { MaNguoiDung = maNguoiDung });

            if (nhanVien == null)
            {
                return NotFound(new { Message = "Không tìm thấy thông tin nhân viên." });
            }

            return Ok(nhanVien);
        }

        private async Task<string> GenerateUniqueMaNguoiDung()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaNguoiDung, 3, LEN(MaNguoiDung) - 2) AS INT)), 0) + 1
                FROM NguoiDung";

            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"ND{nextId:D3}";
        }
    }
}