using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.Models;

// DTO cho PhongWithTienNghi
public class PhongWithTienNghiDTO
{
    public string MaPhong { get; set; }
    public string LoaiPhong { get; set; }
    public decimal GiaPhong { get; set; }
    public string TinhTrang { get; set; }
    public int SoLuongPhong { get; set; }
    public int Tang { get; set; }
    public string KieuGiuong { get; set; }
    public string MoTa { get; set; }
    public string UrlAnhChinh { get; set; }
    public string UrlAnhPhu1 { get; set; }
    public string UrlAnhPhu2 { get; set; }
    public string TienNghi { get; set; }
}

namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhongWithTienNghiController : ControllerBase
    {
        private readonly IDbConnection _db;

        public PhongWithTienNghiController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Lấy danh sách tất cả phòng với tiện nghi.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về danh sách tất cả phòng và tiện nghi có trong hệ thống.
        ///
        /// **Mã trạng thái**:
        /// - 200: Thành công, trả về danh sách phòng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <returns>Danh sách phòng.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhongWithTienNghiDTO>>> GetAll()
        {
            const string query = "SELECT * FROM PhongWithTienNghi";
            var result = await _db.QueryAsync<PhongWithTienNghiDTO>(query);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin phòng theo mã phòng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về thông tin chi tiết của một phòng dựa trên mã phòng (`MaPhong`).
        ///
        /// **Mã trạng thái**:
        /// - 200: Thành công, trả về thông tin phòng.
        /// - 404: Không tìm thấy phòng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã phòng cần tìm. Ví dụ: `P001`.</param>
        /// <returns>Thông tin phòng.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PhongWithTienNghiDTO>> GetById(string id)
        {
            const string query = "SELECT * FROM PhongWithTienNghi WHERE MaPhong = @Id";
            var result = await _db.QueryFirstOrDefaultAsync<PhongWithTienNghiDTO>(query, new { Id = id });

            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Lấy danh sách phòng theo tầng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về danh sách phòng thuộc tầng được chỉ định.
        /// 
        /// **Mã trạng thái**:
        /// - 200: Thành công, trả về danh sách phòng.
        /// - 404: Không tìm thấy phòng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="tang">Tầng cần tìm. Ví dụ: `2`.</param>
        /// <returns>Danh sách phòng thuộc tầng.</returns>
        [HttpGet("by-floor/{tang}")]
        public async Task<ActionResult<IEnumerable<PhongWithTienNghiDTO>>> GetByFloor(int tang)
        {
            const string query = "SELECT * FROM PhongWithTienNghi WHERE Tang = @Tang";
            var result = await _db.QueryAsync<PhongWithTienNghiDTO>(query, new { Tang = tang });

            return result.Any() ? Ok(result) : NotFound();
        }

        /// <summary>
        /// Tạo mới một phòng với tiện nghi.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này tạo mới một phòng trong hệ thống với thông tin được cung cấp.
        ///
        /// **Mã trạng thái**:
        /// - 201: Tạo thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="phong">Thông tin phòng cần tạo.</param>
        /// <returns>Thông tin phòng vừa được tạo.</returns>
        [HttpPost]
        public async Task<ActionResult<PhongWithTienNghiDTO>> Create([FromBody] PhongWithTienNghiDTO phong)
        {
            const string query = @"
                INSERT INTO PhongWithTienNghi (
                    MaPhong, LoaiPhong, GiaPhong, TinhTrang,
                    SoLuongPhong, Tang, KieuGiuong, MoTa,
                    UrlAnhChinh, UrlAnhPhu1, UrlAnhPhu2, TienNghi
                )
                VALUES (
                    @MaPhong, @LoaiPhong, @GiaPhong, @TinhTrang,
                    @SoLuongPhong, @Tang, @KieuGiuong, @MoTa,
                    @UrlAnhChinh, @UrlAnhPhu1, @UrlAnhPhu2, @TienNghi)";

            await _db.ExecuteAsync(query, phong);
            return CreatedAtAction(nameof(GetById), new { id = phong.MaPhong }, phong);
        }

        /// <summary>
        /// Cập nhật thông tin phòng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này cập nhật thông tin của một phòng dựa trên mã phòng (`MaPhong`).
        ///
        /// **Mã trạng thái**:
        /// - 204: Cập nhật thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 404: Không tìm thấy phòng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã phòng cần cập nhật. Ví dụ: `P002`.</param>
        /// <param name="phong">Thông tin phòng cần cập nhật.</param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PhongWithTienNghiDTO phong)
        {
            if (id != phong.MaPhong) return BadRequest();

            const string query = @"
                UPDATE PhongWithTienNghi
                SET LoaiPhong = @LoaiPhong,
                    GiaPhong = @GiaPhong,
                    TinhTrang = @TinhTrang,
                    SoLuongPhong = @SoLuongPhong,
                    Tang = @Tang,
                    KieuGiuong = @KieuGiuong,
                    MoTa = @MoTa,
                    UrlAnhChinh = @UrlAnhChinh,
                    UrlAnhPhu1 = @UrlAnhPhu1,
                    UrlAnhPhu2 = @UrlAnhPhu2,
                    TienNghi = @TienNghi
                WHERE MaPhong = @MaPhong";

            var affected = await _db.ExecuteAsync(query, phong);
            return affected > 0 ? NoContent() : NotFound();
        }

        /// <summary>
        /// Xóa một phòng theo mã phòng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này xóa một phòng khỏi hệ thống dựa trên mã phòng (`MaPhong`).
        ///
        /// **Mã trạng thái**:
        /// - 204: Xóa thành công.
        /// - 404: Không tìm thấy phòng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã phòng cần xóa. Ví dụ: `P002`.</param>
        /// <returns>Kết quả xóa.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            const string query = "DELETE FROM PhongWithTienNghi WHERE MaPhong = @Id";
            var affected = await _db.ExecuteAsync(query, new { Id = id });
            return affected > 0 ? NoContent() : NotFound();
        }

        /// <summary>
        /// Cập nhật tình trạng phòng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này cập nhật tình trạng của một phòng dựa trên mã phòng (`MaPhong`).
        /// 
        /// **Mã trạng thái**:
        /// - 204: Cập nhật thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 404: Không tìm thấy phòng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã phòng cần cập nhật. Ví dụ: `P001`.</param>
        /// <param name="tinhTrang">Tình trạng mới của phòng. Ví dụ: `Unavailable`.</param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] string tinhTrang)
        {
            const string query = "UPDATE PhongWithTienNghi SET TinhTrang = @TinhTrang WHERE MaPhong = @MaPhong";
            var affected = await _db.ExecuteAsync(query, new { MaPhong = id, TinhTrang = tinhTrang });

            return affected > 0 ? NoContent() : NotFound();
        }
    }
}