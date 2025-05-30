//FeedbackController.cs
//Mục đích: Hiển thị đánh giá/phản hồi
//GET /api/feedback/phong/{maPhong} – Xem feedback theo phòng

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using System.Threading.Tasks;
using HotelManagementAPI.DTOs;
using System.Data;
using Dapper;
using SentimentAnalysisExample.Data ;

namespace HotelManagementAPI.Controllers.KhachHang
{
    // SentimentData class for ML.NET sentiment analysis
   

    [ApiController]
    [Route("api/feedback")]
    public class FeedBackController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly PredictionEngine<SentimentData, SentimentPrediction> _predictionEngine;

        public FeedBackController(IDbConnection db, IConfiguration configuration)
        {
            _db = db;
            var mlContext = new MLContext();
            string modelPath = configuration["SentimentModelPath"];
            var model = mlContext.Model.Load(modelPath, out var schema);
            _predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
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
            // Phân tích cảm xúc từ bình luận
            string phanLoai = "Không xác định";
            if (!string.IsNullOrWhiteSpace(feedback.BinhLuan))
            {
                var result = _predictionEngine.Predict(new SentimentData { SentimentText = feedback.BinhLuan });
                phanLoai = result.Prediction ? "Tích cực" : "Tiêu cực";
            }
            feedback.PhanLoai = phanLoai;

            const string insertQuery = @"
                INSERT INTO Feedback (MaPhong, MaNguoiDung, SoSao, BinhLuan, NgayFeedback, PhanLoai)
                VALUES (@MaPhong, @MaNguoiDung, @SoSao, @BinhLuan, GETDATE(), @PhanLoai)";
            await _db.ExecuteAsync(insertQuery, feedback);
            return Ok(new { Message = "Gửi feedback thành công!", PhanLoai = phanLoai });
        }

        // GET: /api/feedback/phong/{maPhong}/full
        [HttpGet("phong/{maPhong}/full")]
        public async Task<IActionResult> GetFeedbackByPhongFull(string maPhong)
        {
            const string query = @"
                SELECT MaNguoiDung, PhanLoai, BinhLuan
                FROM Feedback
                WHERE MaPhong = @MaPhong";
            var feedbacks = await _db.QueryAsync(query, new { MaPhong = maPhong });
            return Ok(feedbacks);
        }

        // GET: /api/feedback/loai/{phanLoai}
        [HttpGet("loai/{phanLoai}")]
        public async Task<IActionResult> GetFeedbackByLoai(string phanLoai)
        {
            const string query = @"
                SELECT PhanLoai, MaNguoiDung, MaPhong, BinhLuan
                FROM Feedback
                WHERE PhanLoai = @PhanLoai";
            var feedbacks = await _db.QueryAsync(query, new { PhanLoai = phanLoai });
            return Ok(feedbacks);
        }
    }
}
