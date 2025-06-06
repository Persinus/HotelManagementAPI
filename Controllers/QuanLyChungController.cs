using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs.QuanLyChung;
using HotelManagementAPI.DTOs;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Configuration;


namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/quanlychung")]
    [Authorize(Roles = "NhanVien,QuanTriVien")]
    public class QuanLyChungController : ControllerBase
    {
        private readonly IDbConnection _db;

        public QuanLyChungController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Cập nhật tiện nghi phòng.
        /// </summary>
        [HttpPut("phong/{maPhong}/tiennghi/{maTienNghi}")]
        [SwaggerOperation(
            Summary = "Cập nhật tiện nghi phòng",
            Description = "Cập nhật tiện nghi phòng, cả nhân viên và quản trị viên đều có quyền."
        )]
        [SwaggerResponse(200, "Cập nhật tiện nghi phòng thành công.")]
        [SwaggerResponse(404, "Tiện nghi không tồn tại.")]
        public async Task<IActionResult> CapNhatTienNghi(string maPhong, string maTienNghi, [FromBody] QuanLyChungSuaPhongTienNghiDTO dto)
        {
            const string checkQuery = "SELECT COUNT(1) FROM Phong_TienNghi WHERE MaPhong = @MaPhong AND MaTienNghi = @MaTienNghi";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaPhong = maPhong, MaTienNghi = maTienNghi });

            if (isExists == 0)
                return NotFound(new { Message = "Tiện nghi không tồn tại." });

            const string updateQuery = @"
                UPDATE TienNghi
                SET TenTienNghi = @TenTienNghi, MoTa = @MoTa
                WHERE MaTienNghi = @MaTienNghi";
            await _db.ExecuteAsync(updateQuery, new
            {
                MaTienNghi = maTienNghi,
                dto.TenTienNghi,
                dto.MoTa
            });

            return Ok(new { Message = "Cập nhật tiện nghi phòng thành công." });
        }

        /// <summary>
        /// Cập nhật dịch vụ.
        /// </summary>
        [HttpPut("dichvu/{maDichVu}")]
        [SwaggerOperation(
            Summary = "Cập nhật dịch vụ",
            Description = "Cập nhật dịch vụ, cả nhân viên và quản trị viên đều có quyền."
        )]
        [SwaggerResponse(200, "Cập nhật dịch vụ thành công.")]
        [SwaggerResponse(404, "Dịch vụ không tồn tại.")]
        public async Task<IActionResult> UpdateDichVu(string maDichVu, [FromBody] QuanLyChungSuaDichVuDTO dto)
        {
            // Kiểm tra xem dịch vụ có tồn tại không
            const string checkQuery = "SELECT COUNT(1) FROM DichVu WHERE MaDichVu = @MaDichVu";
            var exists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaDichVu = maDichVu });
            if (exists == 0)
                return NotFound(new { Message = "Dịch vụ không tồn tại." });

            const string sql = @"
                UPDATE DichVu SET
                    TenDichVu = @TenDichVu,
                    DonGia = @DonGia,
                    MoTaDichVu = @MoTaDichVu,
                    HinhAnhDichVu = @HinhAnhDichVu,
                    SoLuong = @SoLuong,
                    LoaiDichVu = @LoaiDichVu,
                    DonViTinh = @DonViTinh
                WHERE MaDichVu = @MaDichVu";
            var affected = await _db.ExecuteAsync(sql, new
            {
                MaDichVu = maDichVu,
                dto.TenDichVu,
                dto.DonGia,
                dto.MoTaDichVu,
                dto.HinhAnhDichVu,
                dto.SoLuong,
                dto.LoaiDichVu,
                dto.DonViTinh
            });
            if (affected == 0) return NotFound();
            return Ok(new { Message = "Cập nhật dịch vụ thành công." });
        }
        /// <summary>
        /// Cập nhật thông tin giảm giá của phòng.
        /// </summary>
        /// <param name="maPhong">Mã phòng</param>
        /// <param name="maGiamGia">Mã giảm giá</param>
        /// <param name="dto">Thông tin giảm giá mới</param>
        // PUT: /api/nhanvien/phong/{maPhong}/giamgia/{maGiamGia}
        [HttpPut("{maPhong}/giamgia/{maGiamGia}")]
        public async Task<IActionResult> CapNhatGiamGia(string maPhong, string maGiamGia, [FromBody] QuanLyChungSuaPhongGiamGiaDTO dto)
        {
            const string checkQuery = "SELECT COUNT(1) FROM Phong_GiamGia WHERE MaPhong = @MaPhong AND MaGiamGia = @MaGiamGia";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaPhong = maPhong, MaGiamGia = maGiamGia });

            if (isExists == 0)
                return NotFound(new { Message = "Giảm giá không tồn tại." });

            const string updateQuery = @"
                UPDATE GiamGia
                SET TenGiamGia = @TenGiamGia, LoaiGiamGia = @LoaiGiamGia, GiaTriGiam = @GiaTriGiam, 
                    NgayBatDau = @NgayBatDau, NgayKetThuc = @NgayKetThuc, MoTa = @MoTa
                WHERE MaGiamGia = @MaGiamGia";
            await _db.ExecuteAsync(updateQuery, new
            {
                MaGiamGia = maGiamGia,
                dto.TenGiamGia,
                dto.LoaiGiamGia,
                dto.GiaTriGiam,
                dto.NgayBatDau,
                dto.NgayKetThuc,
                dto.MoTa
            });

            return Ok(new { Message = "Cập nhật giảm giá cho phòng thành công." });
        }

        /// <summary>
        /// Cập nhật ảnh phòng.
        /// </summary>
        /// <param name="maPhong">Mã phòng</param>
        /// <param name="maAnh">Mã ảnh</param>
        /// <param name="dto">Thông tin ảnh mới</param>
        // PUT: /api/nhanvien/phong/{maPhong}/phonganh/{maAnh}
        [HttpPut("{maPhong}/phonganh/{maAnh}")]
        public async Task<IActionResult> CapNhatPhongAnh(string maPhong, string maAnh, [FromBody] QuanLyChungSuaPhongAnhDTO dto)
        {
            const string checkQuery = "SELECT COUNT(1) FROM PhongAnh WHERE MaPhong = @MaPhong AND MaAnh = @MaAnh";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaPhong = maPhong, MaAnh = maAnh });

            if (isExists == 0)
                return NotFound(new { Message = "Ảnh không tồn tại." });

            const string updateQuery = @"
                UPDATE PhongAnh
                SET UrlAnh = @UrlAnh
                WHERE MaPhong = @MaPhong AND MaAnh = @MaAnh";
            await _db.ExecuteAsync(updateQuery, new
            {
                MaPhong = maPhong,
                MaAnh = maAnh,
                dto.UrlAnh
            });

            return Ok(new { Message = "Cập nhật ảnh phòng thành công." });
        }

        // Xem tất cả mã giảm giá
        [HttpGet("giamgia/all")]
        [SwaggerOperation(Summary = "Lấy tất cả mã giảm giá", Description = "Lấy danh sách tất cả mã giảm giá.")]
        [SwaggerResponse(200, "Danh sách mã giảm giá.")]
        public async Task<IActionResult> GetAllGiamGia()
        {
            const string query = "SELECT * FROM GiamGia";
            var list = await _db.QueryAsync<GiamGiaDetailDTO>(query);
            return Ok(list);
        }

        // Xem chi tiết mã giảm giá theo mã
        [HttpGet("giamgia/{maGiamGia}")]
        [SwaggerOperation(Summary = "Lấy chi tiết mã giảm giá", Description = "Lấy chi tiết một mã giảm giá theo mã.")]
        [SwaggerResponse(200, "Chi tiết mã giảm giá.")]
        [SwaggerResponse(404, "Không tìm thấy mã giảm giá.")]
        public async Task<IActionResult> GetGiamGiaByMa(string maGiamGia)
        {
            const string query = "SELECT * FROM GiamGia WHERE MaGiamGia = @MaGiamGia";
            var result = await _db.QueryFirstOrDefaultAsync<GiamGiaDetailDTO>(query, new { MaGiamGia = maGiamGia });
            if (result == null)
                return NotFound(new { Message = "Không tìm thấy mã giảm giá." });
            return Ok(result);
        }
        /// <summary>
        /// Sửa bài viết (nhân viên).
        /// </summary>
        [HttpPut("baiviet")]
        [SwaggerOperation(
            Summary = "Sửa bài viết",
            Description = "Nhân viên chỉ được sửa bài viết của chính mình."
        )]
        [SwaggerResponse(200, "Sửa bài viết thành công.")]
        [SwaggerResponse(401, "Không xác định được nhân viên.")]
        [SwaggerResponse(404, "Không tìm thấy bài viết hoặc không có quyền sửa.")]
        public async Task<IActionResult> SuaBaiViet([FromBody] QuanLyChungSuaBaiVietDTO dto)
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được nhân viên." });

            // Chỉ cho phép sửa bài viết của chính mình
            const string checkQuery = "SELECT COUNT(1) FROM BaiViet WHERE MaBaiViet = @MaBaiViet AND MaNguoiDung = @MaNguoiDung";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { dto.MaBaiViet, MaNguoiDung = maNguoiDung });
            if (isExists == 0)
                return NotFound(new { Message = "Không tìm thấy bài viết hoặc bạn không có quyền sửa." });

            const string sql = @"
        UPDATE BaiViet
        SET TieuDe = @TieuDe, NoiDung = @NoiDung, HinhAnhUrl = @HinhAnhUrl
        WHERE MaBaiViet = @MaBaiViet";
            await _db.ExecuteAsync(sql, new
            {
                dto.MaBaiViet,
                dto.TieuDe,
                dto.NoiDung,
                dto.HinhAnhUrl
            });
            return Ok(new { Message = "Sửa bài viết thành công." });
        }
    }
}