using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagementAPI.Controllers.AllowAnonymous
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/allowanonymous/view")]
    public class AllowAnonymousViewController : ControllerBase
    {
        private readonly IDbConnection _db;

        public AllowAnonymousViewController(IDbConnection db)
        {
            _db = db;
        }

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
                    var imageUrls = (await _db.QueryAsync<string>(imagesQuery, new { MaPhong = room.MaPhong })).ToList();
                    room.UrlAnhPhu = imageUrls.Select(url => new PhongAnhDTO { UrlAnh = url }).ToList();

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

                    // Lấy danh sách feedback
                    const string feedbackQuery = @"
                        SELECT SoSao, BinhLuan, PhanLoai
                        FROM Feedback
                        WHERE MaPhong = @MaPhong";
                    room.Feedbacks = (await _db.QueryAsync<FeedBackDTO>(feedbackQuery, new { MaPhong = room.MaPhong })).ToList();

                    // Ảnh phụ
                    if (room.UrlAnhPhu == null || !room.UrlAnhPhu.Any())
                    {
                        room.UrlAnhPhu = new List<PhongAnhDTO>
                        {
                            new PhongAnhDTO { UrlAnh = "Phòng này chưa có ảnh phụ nào." }
                        };
                    }

                    // Tiện nghi
                    if (room.TienNghi == null || !room.TienNghi.Any())
                    {
                        room.TienNghi = new List<TienNghiDTO>
                        {
                            new TienNghiDTO { MaTienNghi = "", TenTienNghi = "Phòng này chưa có tiện nghi nào." }
                        };
                    }

                    // Giảm giá
                    if (room.GiamGia == null || !room.GiamGia.Any())
                    {
                        room.GiamGia = new List<GiamGiaDTO>
                        {
                            new GiamGiaDTO { MaGiamGia = "", TenGiamGia = "Phòng này chưa có giảm giá nào." }
                        };
                    }

                    // Feedback
                    if (room.Feedbacks == null || !room.Feedbacks.Any())
                    {
                        room.Feedbacks = new List<FeedBackDTO>
                        {
                            new FeedBackDTO { SoSao = 0, BinhLuan = "Phòng này chưa có feedback nào.", PhanLoai = "" }
                        };
                    }

                    // Tính giá phòng sau giảm giá (nếu có giảm giá)
                    if (room.GiamGia != null && room.GiamGia.Any())
                    {
                        var giamGia = room.GiamGia.First(); // Nếu có nhiều, lấy cái đầu tiên hoặc bạn có thể chọn theo điều kiện khác
                        if (giamGia.LoaiGiamGia?.ToLower() == "phantram")
                        {
                            room.GiaPhongSauGiam = room.GiaPhong - (room.GiaPhong * giamGia.GiaTriGiam / 100);
                        }
                        else if (giamGia.LoaiGiamGia?.ToLower() == "trutien")
                        {
                            room.GiaPhongSauGiam = room.GiaPhong - giamGia.GiaTriGiam;
                        }
                        else
                        {
                            room.GiaPhongSauGiam = room.GiaPhong;
                        }
                    }
                    else
                    {
                        room.GiaPhongSauGiam = room.GiaPhong;
                    }
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
        /// <summary>
        /// Lấy danh sách tất cả tiện nghi.
        /// </summary>
        [HttpGet("tiennghi")]
        public async Task<IActionResult> GetAllTienNghi()
        {
            const string query = @"
                SELECT MaTienNghi, TenTienNghi, MoTa
                FROM TienNghi";
            var tienNghiList = await _db.QueryAsync<TienNghiDTO>(query);
            return Ok(tienNghiList);
        }

        /// <summary>
        /// Lấy danh sách toàn bộ dịch vụ.
        /// </summary>
        [HttpGet("dichvu")]
        public async Task<IActionResult> GetAllDichVu()
        {
            const string query = @"
                SELECT 
                    MaDichVu, 
                    TenDichVu, 
                    DonGia, 
                    MoTaDichVu, 
                    HinhAnhDichVu, 
                    SoLuong, 
                    TrangThai, 
                    LoaiDichVu, 
                    DonViTinh
                FROM DichVu";
            var dichVuList = await _db.QueryAsync<DichVuDTO>(query);
            return Ok(dichVuList);
        }

        /// <summary>
        /// Lấy danh sách feedback của một phòng.
        /// </summary>
        [HttpGet("feedback/phong/{maPhong}")]
        public async Task<IActionResult> GetFeedbackByPhong(string maPhong)
        {
            const string query = @"
                SELECT SoSao, BinhLuan, PhanLoai
                FROM Feedback
                WHERE MaPhong = @MaPhong";
            var feedbacks = await _db.QueryAsync<FeedBackDTO>(query, new { MaPhong = maPhong });
            return Ok(feedbacks);
        }
    }
}