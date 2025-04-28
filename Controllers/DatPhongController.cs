// Các controller dưới đây viết theo cấu trúc giống KhachHangController mẫu, sử dụng Dapper
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementAPI.Controllers
{
    public class DatPhongDTO
    {
        /// <summary>
        /// Mã đặt phòng (duy nhất). Ví dụ: `DP001`.
        /// Trường này sẽ được tự động sinh, không cần nhập.
        /// </summary>
        public string? MaDatPhong { get; set; }

        /// <summary>
        /// Mã khách hàng. Ví dụ: `KH001`.
        /// </summary>
        public string MaKhachHang { get; set; } = null!;

        /// <summary>
        /// Mã nhân viên. Ví dụ: `NV001`.
        /// Trường này sẽ được tự động chọn ngẫu nhiên.
        /// </summary>
        public string? MaNhanVien { get; set; }

        /// <summary>
        /// Mã phòng. Ví dụ: `P203`.
        /// Trường này sẽ được tự động sinh, không cần nhập.
        /// </summary>
        public string? MaPhong { get; set; }

        /// <summary>
        /// Ngày đặt phòng. Ví dụ: `2025-04-29T10:00:00Z`.
        /// Trường này sẽ được tự động thêm.
        /// </summary>
        public DateTime? NgayDat { get; set; }

        /// <summary>
        /// Ngày nhận phòng. Ví dụ: `2025-05-01T14:00:00`.
        /// </summary>
        public DateTime NgayNhanPhong { get; set; }

        /// <summary>
        /// Ngày trả phòng. Ví dụ: `2025-05-05T12:00:00`.
        /// </summary>
        public DateTime NgayTraPhong { get; set; }

        /// <summary>
        /// Trạng thái đặt phòng. Ví dụ: `1` (Đặt), `2` (Hủy).
        /// </summary>
        public int TrangThai { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class DatPhongController : ControllerBase
    {
        private readonly IDbConnection _db;

        public DatPhongController(IDbConnection db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _db.QueryAsync("SELECT * FROM DatPhong");
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var query = "SELECT * FROM DatPhong WHERE MaDatPhong = @Id";
            var result = await _db.QueryFirstOrDefaultAsync(query, new { Id = id });
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Tạo mới một đặt phòng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này tạo mới một đặt phòng trong hệ thống với thông tin được cung cấp.
        /// Mã đặt phòng (`MaDatPhong`) sẽ được tự động sinh theo định dạng `DPXXX`.
        /// Mã khách hàng (`MaKhachHang`) sẽ được lấy từ JWT Token.
        /// Mã nhân viên (`MaNhanVien`) sẽ được chọn ngẫu nhiên.
        ///
        /// **Ví dụ Request Body**:
        /// <code>
        /// {
        ///   "MaPhong": "P203",
        ///   "NgayNhanPhong": "2025-05-01T14:00:00",
        ///   "NgayTraPhong": "2025-05-05T12:00:00",
        ///   "TrangThai": 1
        /// }
        /// </code>
        ///
        /// **Mã trạng thái**:
        /// - 201: Tạo thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 401: Không xác định được mã khách hàng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="datPhong">Thông tin đặt phòng cần tạo (không bao gồm `MaDatPhong`, `MaKhachHang`, và `MaNhanVien`).</param>
        /// <returns>Thông tin đặt phòng vừa được tạo.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DatPhongDTO datPhong)
        {
            // Lấy mã khách hàng từ JWT Token
            var maKhachHang = User.Claims.FirstOrDefault(c => c.Type == "MaKhachHang")?.Value;
            if (string.IsNullOrEmpty(maKhachHang))
            {
                return Unauthorized(new { Message = "Không thể xác định mã khách hàng. Vui lòng đăng nhập lại." });
            }
            datPhong.MaKhachHang = maKhachHang;

            // Tự động sinh mã đặt phòng
            const string getMaxIdQuery = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaDatPhong, 3, LEN(MaDatPhong) - 2) AS INT)), 0) FROM DatPhong";
            var maxId = await _db.ExecuteScalarAsync<int>(getMaxIdQuery);
            datPhong.MaDatPhong = $"DP{(maxId + 1):D3}";

            // Chọn ngẫu nhiên một nhân viên
            const string getRandomNhanVienQuery = "SELECT TOP 1 MaNhanVien FROM NhanVien ORDER BY NEWID()";
            datPhong.MaNhanVien = await _db.ExecuteScalarAsync<string>(getRandomNhanVienQuery);

            // Tự động sinh mã phòng
            const string getRandomPhongQuery = "SELECT TOP 1 MaPhong FROM Phong WHERE TrangThai = 1 ORDER BY NEWID()"; // 1: Phòng trống
            datPhong.MaPhong = await _db.ExecuteScalarAsync<string>(getRandomPhongQuery);

            // Thêm ngày đặt phòng
            datPhong.NgayDat = DateTime.UtcNow;

            // Thực hiện chèn dữ liệu
            const string sql = @"
                INSERT INTO DatPhong 
                (MaDatPhong, MaKhachHang, MaNhanVien, MaPhong, NgayDat, NgayNhanPhong, NgayTraPhong, TrangThai) 
                VALUES (@MaDatPhong, @MaKhachHang, @MaNhanVien, @MaPhong, @NgayDat, @NgayNhanPhong, @NgayTraPhong, @TrangThai)";
            
            await _db.ExecuteAsync(sql, datPhong);
            return CreatedAtAction(nameof(GetById), new { id = datPhong.MaDatPhong }, datPhong);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] DatPhongDTO datPhong)
        {
            if (id != datPhong.MaDatPhong)
                return BadRequest();

            const string sql = @"
                UPDATE DatPhong
                SET MaKhachHang = @MaKhachHang,
                    MaNhanVien = @MaNhanVien,
                    MaPhong = @MaPhong,
                    NgayNhanPhong = @NgayNhanPhong,
                    NgayTraPhong = @NgayTraPhong,
                    TrangThai = @TrangThai
                WHERE MaDatPhong = @MaDatPhong";

            var rows = await _db.ExecuteAsync(sql, datPhong);
            return rows == 0 ? NotFound() : NoContent();
        }
    }
}
