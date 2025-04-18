using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementAPI.Controllers
{
    // DTO không có JWK vì server tự sinh
    public class NguoiDungDTO
    {
        public string MaNguoiDung { get; set; }
        public string Email { get; set; }
        public string TenTaiKhoan { get; set; }
        public string MatKhau { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class NguoiDungController : ControllerBase
    {
        private readonly IDbConnection _db;

        public NguoiDungController(IDbConnection db) => _db = db;

        // Hàm sinh JWK (giả lập)
        private string GenerateJwt(string userId)
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + "-" + userId;
        }

        // Lấy tất cả người dùng
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string sql = "SELECT * FROM NguoiDung";
            var result = await _db.QueryAsync<NguoiDungDTO>(sql);
            return Ok(result);
        }

        // Lấy theo MaNguoiDung
        [HttpGet("{maNguoiDung}")]
        public async Task<IActionResult> GetByMaNguoiDung(string maNguoiDung)
        {
            const string sql = "SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var result = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(sql, new { MaNguoiDung = maNguoiDung });

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // Tạo người dùng mới và tự sinh JWK
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NguoiDungDTO nguoiDung)
        {
            if (string.IsNullOrEmpty(nguoiDung.MaNguoiDung) || string.IsNullOrEmpty(nguoiDung.Email))
            {
                return BadRequest("Thông tin không hợp lệ.");
            }

            var jwk = GenerateJwt(nguoiDung.MaNguoiDung);

            const string sql = @"INSERT INTO NguoiDung (MaNguoiDung, Email, TenTaiKhoan, MatKhau, JWK) 
                                 VALUES (@MaNguoiDung, @Email, @TenTaiKhoan, @MatKhau, @JWK)";

            await _db.ExecuteAsync(sql, new
            {
                nguoiDung.MaNguoiDung,
                nguoiDung.Email,
                nguoiDung.TenTaiKhoan,
                nguoiDung.MatKhau,
                JWK = jwk
            });

            return Ok(new
            {
                Message = "Tạo người dùng thành công",
                nguoiDung.MaNguoiDung,
                JWK = jwk
            });
        }

        // Cập nhật người dùng (không cập nhật JWK)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] NguoiDungDTO nguoiDung)
        {
            if (string.IsNullOrEmpty(nguoiDung.Email) || string.IsNullOrEmpty(nguoiDung.TenTaiKhoan))
            {
                return BadRequest("Thông tin không hợp lệ.");
            }

            const string sql = @"UPDATE NguoiDung 
                                 SET Email = @Email, TenTaiKhoan = @TenTaiKhoan, MatKhau = @MatKhau 
                                 WHERE MaNguoiDung = @Id";

            var affectedRows = await _db.ExecuteAsync(sql, new
            {
                nguoiDung.Email,
                nguoiDung.TenTaiKhoan,
                nguoiDung.MatKhau,
                Id = id
            });

            if (affectedRows == 0)
                return NotFound();

            return Ok(new { Message = "Cập nhật thành công" });
        }

        // Xoá người dùng
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            const string sql = "DELETE FROM NguoiDung WHERE MaNguoiDung = @Id";
            var affectedRows = await _db.ExecuteAsync(sql, new { Id = id });

            if (affectedRows == 0)
                return NotFound();

            return Ok(new { Message = "Xoá thành công" });
        }
    }
}