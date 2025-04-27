using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.Models;

namespace HotelManagementAPI.Controllers
{
    public class DichVuDTO
    {
        public string MaChiTietDichVu { get; set; }
        public string TenDichVu { get; set; }
        public decimal DonGia { get; set; }
        public string? MoTaDichVu { get; set; }
        public string? UrlAnh { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class DichVuController : ControllerBase
    {
        private readonly IDbConnection _db;

        public DichVuController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Lấy danh sách tất cả dịch vụ.
        /// </summary>
        /// <remarks>
        /// Mô tả: API này trả về danh sách tất cả dịch vụ có trong hệ thống.
        ///
        /// Mã trạng thái:
        /// - 200: Thành công, trả về danh sách dịch vụ.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <returns>Danh sách dịch vụ.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DichVuDTO>>> GetAll()
        {
            const string query = "SELECT * FROM DichVu";
            var result = await _db.QueryAsync<DichVuDTO>(query);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin dịch vụ theo mã.
        /// </summary>
        /// <remarks>
        /// Mô tả: API này trả về thông tin chi tiết của một dịch vụ dựa trên mã dịch vụ ([MaChiTietDichVu](http://_vscodecontentref_/0)).
        ///
        /// Mã trạng thái:
        /// - 200: Thành công, trả về thông tin dịch vụ.
        /// - 404: Không tìm thấy dịch vụ.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã dịch vụ cần tìm. Ví dụ: `DV001`.</param>
        /// <returns>Thông tin dịch vụ.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DichVuDTO>> GetById(string id)
        {
            const string query = "SELECT * FROM DichVu WHERE MaChiTietDichVu = @Id";
            var result = await _db.QueryFirstOrDefaultAsync<DichVuDTO>(query, new { Id = id });

            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Tạo mới một dịch vụ.
        /// </summary>
        /// <remarks>
        /// Mô tả: API này tạo mới một dịch vụ trong hệ thống với thông tin được cung cấp.
        ///
        /// Mã trạng thái:
        /// - 201: Tạo thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="dichVuDto">Thông tin dịch vụ cần tạo.</param>
        /// <returns>Thông tin dịch vụ vừa được tạo.</returns>
        [HttpPost]
        public async Task<ActionResult<DichVuDTO>> Create([FromBody] DichVuDTO dichVuDto)
        {
            const string query = @"
                INSERT INTO DichVu (MaChiTietDichVu, TenDichVu, DonGia, MoTaDichVu, UrlAnh)
                VALUES (@MaChiTietDichVu, @TenDichVu, @DonGia, @MoTaDichVu, @UrlAnh)";

            await _db.ExecuteAsync(query, dichVuDto);
            return CreatedAtAction(nameof(GetById), new { id = dichVuDto.MaChiTietDichVu }, dichVuDto);
        }

        /// <summary>
        /// Cập nhật thông tin dịch vụ.
        /// </summary>
        /// <remarks>
        /// Mô tả: API này cập nhật thông tin của một dịch vụ dựa trên mã dịch vụ ([MaChiTietDichVu](http://_vscodecontentref_/1)).
        ///
        /// Mã trạng thái:
        /// - 204: Cập nhật thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 404: Không tìm thấy dịch vụ.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã dịch vụ cần cập nhật. Ví dụ: `DV002`.</param>
        /// <param name="dichVuDto">Thông tin dịch vụ cần cập nhật.</param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] DichVuDTO dichVuDto)
        {
            if (id != dichVuDto.MaChiTietDichVu) return BadRequest();

            const string query = @"
                UPDATE DichVu
                SET TenDichVu = @TenDichVu,
                    DonGia = @DonGia,
                    MoTaDichVu = @MoTaDichVu,
                    UrlAnh = @UrlAnh
                WHERE MaChiTietDichVu = @MaChiTietDichVu";

            var rows = await _db.ExecuteAsync(query, dichVuDto);
            return rows == 0 ? NotFound() : NoContent();
        }

        /// <summary>
        /// Xóa một dịch vụ theo mã.
        /// </summary>
        /// <remarks>
        /// Mô tả: API này xóa một dịch vụ khỏi hệ thống dựa trên mã dịch vụ ([MaChiTietDichVu](http://_vscodecontentref_/2)).
        ///
        /// Mã trạng thái:
        /// - 204: Xóa thành công.
        /// - 404: Không tìm thấy dịch vụ.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã dịch vụ cần xóa. Ví dụ: `DV002`.</param>
        /// <returns>Kết quả xóa.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            const string query = "DELETE FROM DichVu WHERE MaChiTietDichVu = @Id";
            var rows = await _db.ExecuteAsync(query, new { Id = id });
            return rows == 0 ? NotFound() : NoContent();
        }
    }
}