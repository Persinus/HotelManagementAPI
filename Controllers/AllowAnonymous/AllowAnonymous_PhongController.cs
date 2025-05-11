using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagementAPI.Controllers.AllowAnonymous
{
    [ApiController]
    [AllowAnonymous] // Cho phép tất cả người dùng truy cập

     [Route("api/allowanonymous")]
    public class PhongController : ControllerBase
    {
        private readonly IDbConnection _db;

        public PhongController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Lấy danh sách tất cả phòng và tiện nghi.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về danh sách tất cả phòng và tiện nghi có trong hệ thống.
        /// **Trạng thái**:
        /// - 200: Thành công, trả về danh sách phòng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <returns>Danh sách phòng.</returns>
        [HttpGet("XemTatCaPhong")]
        [AllowAnonymous] // Cho phép tất cả người dùng truy cập
        public async Task<ActionResult<IEnumerable<PhongDetailsDTO>>> GetAll()
        {
            try
            {
                // Truy vấn danh sách phòng
                const string roomQuery = @"
                    SELECT p.MaPhong, p.LoaiPhong, p.GiaPhong, p.TinhTrang, p.SoLuongPhong, p.Tang, 
                           p.KieuGiuong, p.MoTa, p.UrlAnhChinh, p.SucChua, p.SoGiuong, 
                           p.DonViTinh, p.SoSaoTrungBinh
                    FROM Phong p";
                var rooms = (await _db.QueryAsync<PhongDetailsDTO>(roomQuery)).ToList();

                foreach (var room in rooms)
                {
                    // Lấy danh sách ảnh phụ
                    const string imagesQuery = "SELECT UrlAnh FROM PhongAnh WHERE MaPhong = @MaPhong";
                    room.UrlAnhPhu = (await _db.QueryAsync<string>(imagesQuery, new { MaPhong = room.MaPhong })).ToList();

                    // Lấy danh sách tiện nghi
                    const string amenitiesQuery = @"
                        SELECT tn.MaTienNghi, tn.TenTienNghi, tn.MoTa
                        FROM TienNghi tn
                        JOIN Phong_TienNghi ptn ON tn.MaTienNghi = ptn.MaTienNghi
                        WHERE ptn.MaPhong = @MaPhong";
                    room.TienNghi = (await _db.QueryAsync<TienNghiDTO>(amenitiesQuery, new { MaPhong = room.MaPhong })).ToList();

                    // Lấy danh sách giảm giá
                    const string discountsQuery = @"
                        SELECT gg.MaGiamGia, gg.TenGiamGia, gg.LoaiGiamGia, gg.GiaTriGiam, gg.NgayBatDau, gg.NgayKetThuc, gg.MoTa
                        FROM GiamGia gg
                        JOIN Phong_GiamGia pg ON gg.MaGiamGia = pg.MaGiamGia
                        WHERE pg.MaPhong = @MaPhong";
                    room.GiamGia = (await _db.QueryAsync<GiamGiaDTO>(discountsQuery, new { MaPhong = room.MaPhong })).ToList();
                }

                return Ok(rooms);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy danh sách phòng." });
            }
        }
    }

    // Removed duplicate TienNghiDTO class to avoid conflicts.
}