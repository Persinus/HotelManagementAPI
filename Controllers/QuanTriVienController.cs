using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.Models;

namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuanTriVienController : ControllerBase
    {
        private readonly IDbConnection _db;

        public QuanTriVienController(IDbConnection db) => _db = db;

        /// <summary>
        /// Lấy danh sách tất cả quản trị viên.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về danh sách tất cả quản trị viên có trong hệ thống.
        ///
        /// **Mã trạng thái**:
        /// - 200: Thành công, trả về danh sách quản trị viên.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <returns>Danh sách quản trị viên.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string sql = "SELECT * FROM QuanTriVien";
            var result = await _db.QueryAsync<QuanTriVien>(sql);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin quản trị viên theo mã.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về thông tin chi tiết của một quản trị viên dựa trên mã quản trị (`MaQuanTri`).
        ///
        /// **Mã trạng thái**:
        /// - 200: Thành công, trả về thông tin quản trị viên.
        /// - 404: Không tìm thấy quản trị viên.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã quản trị viên cần tìm. Ví dụ: `QT001`.</param>
        /// <returns>Thông tin quản trị viên.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            const string sql = "SELECT * FROM QuanTriVien WHERE MaQuanTri = @Id";
            var result = await _db.QueryFirstOrDefaultAsync<QuanTriVien>(sql, new { Id = id });

            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Tạo mới một quản trị viên.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này tạo mới một quản trị viên trong hệ thống với thông tin được cung cấp.
        ///
        /// **Mã trạng thái**:
        /// - 201: Tạo thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="model">Thông tin quản trị viên cần tạo.</param>
        /// <returns>Thông tin quản trị viên vừa được tạo.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] QuanTriVien model)
        {
            const string sql = @"
                INSERT INTO QuanTriVien (MaQuanTri, TenAdmin, SoDienThoai, NgayTao) 
                VALUES (@MaQuanTri, @TenAdmin, @SoDienThoai, GETDATE())";

            await _db.ExecuteAsync(sql, model);
            return CreatedAtAction(nameof(GetById), new { id = model.MaQuanTri }, model);
        }

        /// <summary>
        /// Cập nhật thông tin quản trị viên.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này cập nhật thông tin của một quản trị viên dựa trên mã quản trị (`MaQuanTri`).
        ///
        /// **Mã trạng thái**:
        /// - 204: Cập nhật thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 404: Không tìm thấy quản trị viên.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã quản trị viên cần cập nhật. Ví dụ: `QT001`.</param>
        /// <param name="model">Thông tin quản trị viên cần cập nhật.</param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] QuanTriVien model)
        {
            if (id != model.MaQuanTri) return BadRequest();

            const string sql = @"
                UPDATE QuanTriVien
                SET TenAdmin = @TenAdmin,
                    SoDienThoai = @SoDienThoai,
                    NgayCapNhat = GETDATE()
                WHERE MaQuanTri = @MaQuanTri";

            var rows = await _db.ExecuteAsync(sql, model);
            return rows == 0 ? NotFound() : NoContent();
        }

        /// <summary>
        /// Xóa một quản trị viên theo mã.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này xóa một quản trị viên khỏi hệ thống dựa trên mã quản trị (`MaQuanTri`).
        ///
        /// **Mã trạng thái**:
        /// - 204: Xóa thành công.
        /// - 404: Không tìm thấy quản trị viên.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã quản trị viên cần xóa. Ví dụ: `QT001`.</param>
        /// <returns>Kết quả xóa.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            const string sql = "DELETE FROM QuanTriVien WHERE MaQuanTri = @Id";
            var rows = await _db.ExecuteAsync(sql, new { Id = id });
            return rows == 0 ? NotFound() : NoContent();
        }
    }
}