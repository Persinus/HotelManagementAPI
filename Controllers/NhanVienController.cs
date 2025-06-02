using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using HotelManagementAPI.DTOs.NhanVien;
using System.Security.Claims;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace HotelManagementAPI.Controllers.NhanVien
{
    [ApiController]
    [Route("api/nhanvien")]
    [Authorize(Roles = "NhanVien")]
    public class NhanVienController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly Cloudinary _cloudinary;

        public NhanVienController(IDbConnection db, Cloudinary cloudinary)
        {
            _db = db;
            _cloudinary = cloudinary;
        }

        /// <summary>
        /// Cập nhật thông tin phòng.
        /// </summary>
        /// <param name="maPhong">Mã phòng cần cập nhật</param>
        /// <param name="phongDetailsDTO">Thông tin phòng mới</param>
        // PUT: /api/nhanvien/phong/{maPhong}
        [HttpPut("{maPhong}")]
        public async Task<IActionResult> CapNhatPhong(string maPhong, [FromBody] PhongDetailsDTO phongDetailsDTO)
        {
            const string checkQuery = "SELECT COUNT(1) FROM Phong WHERE MaPhong = @MaPhong";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaPhong = maPhong });

            if (isExists == 0)
                return NotFound(new { Message = "Phòng không tồn tại." });

            const string updateQuery = @"
                UPDATE Phong
                SET LoaiPhong = @LoaiPhong, GiaPhong = @GiaPhong, TinhTrang = @TinhTrang, SoLuongPhong = @SoLuongPhong,
                    Tang = @Tang, KieuGiuong = @KieuGiuong, MoTa = @MoTa, UrlAnhChinh = @UrlAnhChinh, SucChua = @SucChua,
                    SoGiuong = @SoGiuong, DonViTinh = @DonViTinh, SoSaoTrungBinh = @SoSaoTrungBinh
                WHERE MaPhong = @MaPhong";
            await _db.ExecuteAsync(updateQuery, new
            {
                MaPhong = maPhong,
                phongDetailsDTO.LoaiPhong,
                phongDetailsDTO.GiaPhong,
                phongDetailsDTO.TinhTrang,
                phongDetailsDTO.SoLuongPhong,
                phongDetailsDTO.Tang,
                phongDetailsDTO.KieuGiuong,
                phongDetailsDTO.MoTa,
                phongDetailsDTO.UrlAnhChinh,
                phongDetailsDTO.SucChua,
                phongDetailsDTO.SoGiuong,
                phongDetailsDTO.DonViTinh,
                phongDetailsDTO.SoSaoTrungBinh
            });

            return Ok(new { Message = "Cập nhật phòng thành công." });
        }

        /// <summary>
        /// Cập nhật thông tin giảm giá của phòng.
        /// </summary>
        /// <param name="maPhong">Mã phòng</param>
        /// <param name="maGiamGia">Mã giảm giá</param>
        /// <param name="dto">Thông tin giảm giá mới</param>
        // PUT: /api/nhanvien/phong/{maPhong}/giamgia/{maGiamGia}
        [HttpPut("{maPhong}/giamgia/{maGiamGia}")]
        public async Task<IActionResult> CapNhatGiamGia(string maPhong, string maGiamGia, [FromBody] NhanVienSuaPhongGiamGiaDTO dto)
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
        public async Task<IActionResult> CapNhatPhongAnh(string maPhong, string maAnh, [FromBody] NhanVienSuaPhongAnhDTO dto)
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

        /// <summary>
        /// Cập nhật tiện nghi phòng.
        /// </summary>
        /// <param name="maPhong">Mã phòng</param>
        /// <param name="maTienNghi">Mã tiện nghi</param>
        /// <param name="dto">Thông tin tiện nghi mới</param>
        // PUT: /api/nhanvien/phong/{maPhong}/tiennghi/{maTienNghi}
        [HttpPut("{maPhong}/tiennghi/{maTienNghi}")]
        public async Task<IActionResult> CapNhatTienNghi(string maPhong, string maTienNghi, [FromBody] NhanVienSuaPhongTienNghiDTO dto)
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
        /// <param name="maDichVu">Mã dịch vụ</param>
        /// <param name="dto">Thông tin dịch vụ mới</param>
        // PUT: /api/nhanvien/phong/{maDichVu}
        [HttpPut("{maDichVu}")]
        public async Task<IActionResult> UpdateDichVu(string maDichVu, [FromBody] NhanVienSuaDichVuDTO dto)
        {
            // Kiểm tra xem dịch vụ có tồn tại không
            const string checkQuery = "SELECT COUNT(1) FROM DichVu WHERE MaDichVu = @MaDichVu";
            var exists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaDichVu = maDichVu });
            if (exists == 0)
                return NotFound(new { Message = "Dịch vụ không tồn tại." });

            // Cập nhật thông tin dịch vụ
            return await UpdateDichVuInternal(maDichVu, dto);
        }

        private async Task<IActionResult> UpdateDichVuInternal(string maDichVu, NhanVienSuaDichVuDTO dto)
        {
            const string sql = @"
                UPDATE DichVu SET
                    TenDichVu = @TenDichVu,
                    DonGia = @DonGia,
                    MoTaDichVu = @MoTaDichVu,
                    HinhAnhDichVu = @HinhAnhDichVu,
                    SoLuong = @SoLuong,
                    TrangThai = @TrangThai,
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
                dto.TrangThai,
                dto.LoaiDichVu,
                dto.DonViTinh
            });
            if (affected == 0) return NotFound();
            return Ok(new { Message = "Cập nhật dịch vụ thành công." });
        }

        /// <summary>
        /// Thêm bài viết mới (nhân viên).
        /// </summary>
        
        [HttpPost("baiviet")]
        public async Task<IActionResult> ThemBaiViet([FromForm] NhanVienThemBaiVietDTO dto, IFormFile? file)
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được nhân viên." });

            string? imageUrl = null;
            if (file != null && file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Width(800).Height(800).Crop("limit"),
                    Folder = "baiviet"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    imageUrl = uploadResult.SecureUrl.ToString();
                else
                    return StatusCode(500, $"Upload ảnh thất bại: {uploadResult.Error?.Message}");
            }

            var maBaiViet = Guid.NewGuid().ToString("N").Substring(0, 6);
            const string sql = @"
        INSERT INTO BaiViet (MaBaiViet, MaNguoiDung, TieuDe, NoiDung, NgayDang, HinhAnhUrl, TrangThai)
        VALUES (@MaBaiViet, @MaNguoiDung, @TieuDe, @NoiDung, @NgayDang, @HinhAnhUrl, @TrangThai)";
            await _db.ExecuteAsync(sql, new
            {
                MaBaiViet = maBaiViet,
                MaNguoiDung = maNguoiDung,
                dto.TieuDe,
                dto.NoiDung,
                NgayDang = DateTime.UtcNow.AddHours(7),
                HinhAnhUrl = imageUrl,
                dto.TrangThai
            });
            return Ok(new { Message = "Thêm bài viết thành công.", MaBaiViet = maBaiViet, HinhAnhUrl = imageUrl });
        }

        /// <summary>
        /// Sửa bài viết (nhân viên).
        /// </summary>
        [HttpPut("baiviet")]
        public async Task<IActionResult> SuaBaiViet([FromBody] NhanVienSuaBaiVietDTO dto)
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

        /// <summary>
        /// Xóa bài viết (nhân viên).
        /// </summary>
        [HttpDelete("baiviet/{maBaiViet}")]
        public async Task<IActionResult> XoaBaiViet(string maBaiViet)
        {
            var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không xác định được nhân viên." });

            // Chỉ cho phép xóa bài viết của chính mình
            const string checkQuery = "SELECT COUNT(1) FROM BaiViet WHERE MaBaiViet = @MaBaiViet AND MaNguoiDung = @MaNguoiDung";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaBaiViet = maBaiViet, MaNguoiDung = maNguoiDung });
            if (isExists == 0)
                return NotFound(new { Message = "Không tìm thấy bài viết hoặc bạn không có quyền xóa." });

            const string sql = "DELETE FROM BaiViet WHERE MaBaiViet = @MaBaiViet";
            await _db.ExecuteAsync(sql, new { MaBaiViet = maBaiViet });
            return Ok(new { Message = "Xóa bài viết thành công." });
        }
    }

}

