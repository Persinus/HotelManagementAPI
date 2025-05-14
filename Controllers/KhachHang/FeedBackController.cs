//FeedbackController.cs
//Mục đích: Hiển thị đánh giá/phản hồi
//GET /api/feedback/phong/{maPhong} – Xem feedback theo phòng

using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;

namespace HotelManagementAPI.Controllers.KhachHang
{
    [ApiController]
    [Route("api/feedback")]
    public class FeedBackController : ControllerBase
    {
        private readonly IDbConnection _db;

        public FeedBackController(IDbConnection db)
        {
            _db = db;
        }

        // GET: /api/feedback/phong/{maPhong}
        [HttpGet("phong/{maPhong}")]
        public async Task<IActionResult> GetFeedbackByPhong(string maPhong)
        {
            const string query = @"
                SELECT SoSao, BinhLuan, PhanLoai
                FROM Feedback
                WHERE MaPhong = @MaPhong";
            var feedbacks = await _db.QueryAsync<FeedBackDTO>(query, new { MaPhong = maPhong });
            return Ok(feedbacks);
        }

        // GET: /api/feedback/phong/loai
        [HttpGet("phong/loai")]
        public async Task<IActionResult> GetMaPhongVaLoaiPhong()
        {
            const string query = @"
                SELECT MaPhong, LoaiPhong
                FROM Phong";
            var result = await _db.QueryAsync(query);
            return Ok(result);
        }

        // POST: /api/feedback
        [HttpPost]
        public async Task<IActionResult> PostFeedback([FromBody] FeedBackDTO feedback)
        {
            const string insertQuery = @"
                INSERT INTO Feedback (MaPhong, MaNguoiDung, SoSao, BinhLuan, NgayFeedback, PhanLoai)
                VALUES (@MaPhong, @MaNguoiDung, @SoSao, @BinhLuan, GETDATE(), @PhanLoai)";
            await _db.ExecuteAsync(insertQuery, feedback);
            return Ok(new { Message = "Gửi feedback thành công!" });
        }
    }
}