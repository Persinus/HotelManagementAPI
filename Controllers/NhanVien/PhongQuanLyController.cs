using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagementAPI.Controllers.NhanVien
{
    [ApiController]
    [Route("api/nhanvien/phong")]
    [Authorize(Roles = "NhanVien")]
    public class PhongQuanLyController : ControllerBase
    {
        private readonly IDbConnection _db;

        public PhongQuanLyController(IDbConnection db)
        {
            _db = db;
        }

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

     
    }
}