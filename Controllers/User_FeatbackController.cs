using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class User_FeedbackController : ControllerBase
    {
        private readonly IDbConnection _db;

        public User_FeedbackController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Gửi phản hồi về phòng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này cho phép khách hàng đã đăng nhập gửi phản hồi về phòng.
        /// Mã khách hàng (`MaKhachHang`) sẽ được lấy từ JWT Token.
        ///
        /// **Ví dụ Request Body**:
        /// <code>
        /// {
        ///   "MaPhong": "P203",
        ///   "NoiDung": "Phòng rất sạch sẽ và tiện nghi.",
        ///   "DanhGia": 5
        /// }
        /// </code>
        ///
        /// **Mã trạng thái**:
        /// - 201: Gửi phản hồi thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 401: Không xác định được mã khách hàng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="feedback">Thông tin phản hồi của khách hàng.</param>
        /// <returns>Thông báo kết quả.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackDTO feedback)
        {
            // Lấy mã khách hàng từ JWT Token
            var maKhachHang = User.Claims.FirstOrDefault(c => c.Type == "MaKhachHang")?.Value;
            if (string.IsNullOrEmpty(maKhachHang))
            {
                return Unauthorized(new { Message = "Không thể xác định mã khách hàng. Vui lòng đăng nhập lại." });
            }
            feedback.MaKhachHang = maKhachHang;

            // Kiểm tra phòng có tồn tại không
            const string checkRoomQuery = "SELECT COUNT(1) FROM PhongWithTienNghi WHERE MaPhong = @MaPhong";
            var roomExists = await _db.ExecuteScalarAsync<bool>(checkRoomQuery, new { feedback.MaPhong });
            if (!roomExists)
            {
                return NotFound(new { Message = "Phòng không tồn tại." });
            }

            // Thực hiện chèn phản hồi
            const string insertQuery = @"
                INSERT INTO User_Feedback (MaKhachHang, MaPhong, NoiDung, DanhGia, NgayPhanHoi) 
                VALUES (@MaKhachHang, @MaPhong, @NoiDung, @DanhGia, GETDATE())";
            await _db.ExecuteAsync(insertQuery, feedback);

            return CreatedAtAction(nameof(GetFeedbackByRoom), new { maPhong = feedback.MaPhong }, new { Message = "Phản hồi đã được gửi thành công." });
        }

        /// <summary>
        /// Lấy danh sách phản hồi của một phòng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về danh sách phản hồi của một phòng dựa trên mã phòng (`MaPhong`).
        ///
        /// **Mã trạng thái**:
        /// - 200: Thành công, trả về danh sách phản hồi.
        /// - 404: Không tìm thấy phản hồi cho phòng này.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="maPhong">Mã phòng cần lấy phản hồi. Ví dụ: `P203`.</param>
        /// <returns>Danh sách phản hồi.</returns>
        [HttpGet("{maPhong}")]
        public async Task<IActionResult> GetFeedbackByRoom(string maPhong)
        {
            const string query = "SELECT * FROM User_Feedback WHERE MaPhong = @MaPhong";
            var User_Feedbacks = await _db.QueryAsync(query, new { MaPhong = maPhong });

            if (!User_Feedbacks.Any())
            {
                return NotFound(new { Message = "Không tìm thấy phản hồi cho phòng này." });
            }

            return Ok(User_Feedbacks);
        }

        /// <summary>
        /// Lấy danh sách phản hồi của một phòng (sử dụng User_User_Feedback).
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về danh sách phản hồi của một phòng dựa trên mã phòng (`MaPhong`) từ bảng User_Feedback.
        ///
        /// **Mã trạng thái**:
        /// - 200: Thành công, trả về danh sách phản hồi.
        /// - 404: Không tìm thấy phản hồi cho phòng này.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã phòng cần lấy phản hồi. Ví dụ: `P203`.</param>
        /// <returns>Danh sách phản hồi.</returns>
        [HttpGet("{id}/feedbacks")]
        public async Task<IActionResult> GetFeedbacksByRoom(string id)
        {
            const string query = "SELECT * FROM User_Feedback WHERE MaPhong = @MaPhong";
            var feedbacks = await _db.QueryAsync(query, new { MaPhong = id });

            if (!feedbacks.Any())
            {
                return NotFound(new { Message = "Không tìm thấy phản hồi cho phòng này." });
            }

            return Ok(feedbacks);
        }
    }

    /// <summary>
    /// DTO đại diện cho phản hồi của khách hàng.
    /// </summary>
    public class FeedbackDTO
    {
        /// <summary>
        /// Mã khách hàng. Trường này sẽ được tự động lấy từ JWT Token.
        /// </summary>
        public string? MaKhachHang { get; set; }

        /// <summary>
        /// Mã phòng. Ví dụ: `P203`.
        /// </summary>
        public string MaPhong { get; set; } = null!;

        /// <summary>
        /// Nội dung phản hồi. Ví dụ: `Phòng rất sạch sẽ và tiện nghi.`.
        /// </summary>
        public string NoiDung { get; set; } = null!;

        /// <summary>
        /// Đánh giá của khách hàng (1-5). Ví dụ: `5`.
        /// </summary>
        public int DanhGia { get; set; }
    }
}