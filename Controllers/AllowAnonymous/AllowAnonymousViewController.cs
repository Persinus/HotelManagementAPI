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

        /// <summary>
        /// Lấy danh sách tất cả phòng.
        /// </summary>
        [HttpGet("phong")]
        public async Task<IActionResult> GetAllPhong()
        {
            const string query = @"
                SELECT MaPhong, LoaiPhong, GiaPhong, TinhTrang, SoLuongPhong, Tang, 
                       KieuGiuong, MoTa, UrlAnhChinh, SucChua, SoGiuong, DonViTinh, SoSaoTrungBinh
                FROM Phong";
            var rooms = await _db.QueryAsync<PhongDetailsDTO>(query);
            return Ok(rooms);
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