using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagementAPI.Controllers.NhanVien
{
    [ApiController]
    [Route("api/quantrivien/phong")]
    [Authorize(Roles = "QuanTriVien")]
    public class QuanTriVienPhongController : ControllerBase
    {
        private readonly IDbConnection _db;

        public QuanTriVienPhongController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Thêm phòng mới.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ThemPhong([FromBody] PhongDetailsDTO phongDetailsDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Sinh mã phòng tự động dạng P001, P002, ...
            var maPhong = await GenerateMaPhong();

            const string insertQuery = @"
                INSERT INTO Phong (MaPhong, LoaiPhong, GiaPhong, TinhTrang, SoLuongPhong, Tang, KieuGiuong, MoTa, UrlAnhChinh, SucChua, SoGiuong, DonViTinh, SoSaoTrungBinh)
                VALUES (@MaPhong, @LoaiPhong, @GiaPhong, @TinhTrang, @SoLuongPhong, @Tang, @KieuGiuong, @MoTa, @UrlAnhChinh, @SucChua, @SoGiuong, @DonViTinh, @SoSaoTrungBinh)";
            await _db.ExecuteAsync(insertQuery, new
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

            return Ok(new { Message = "Thêm phòng thành công.", MaPhong = maPhong });
        }

        /// <summary>
        /// Sửa thông tin phòng.
        /// </summary>
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
        /// Xóa phòng.
        /// </summary>
        [HttpDelete("{maPhong}")]
        public async Task<IActionResult> XoaPhong(string maPhong)
        {
            const string checkQuery = "SELECT COUNT(1) FROM Phong WHERE MaPhong = @MaPhong";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaPhong = maPhong });

            if (isExists == 0)
                return NotFound(new { Message = "Phòng không tồn tại." });

            const string deleteQuery = "DELETE FROM Phong WHERE MaPhong = @MaPhong";
            await _db.ExecuteAsync(deleteQuery, new { MaPhong = maPhong });

            return Ok(new { Message = "Xóa phòng thành công." });
        }

        // Hàm sinh mã phòng tự động
        private async Task<string> GenerateMaPhong()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaPhong, 2, LEN(MaPhong)-1) AS INT)), 0) + 1
                FROM Phong";
            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"P{nextId:D3}";
        }
    }
}