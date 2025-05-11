//PhongQuanLyController.cs
//Mục đích: Quản lý trạng thái phòng
//PUT /api/phong/{id}/trangthai – Cập nhật trạng thái phòng (trống, đang dọn, sử dụng)

using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs; // Add this directive

namespace HotelManagementAPI.Controllers.NhanVien
{
    /// <summary>
    /// Controller quản lý trạng thái và thông tin phòng dành cho nhân viên.
    /// </summary>
    [ApiController]
    [Route("api/nhanvien/phong")]
    public class PhongQuanLyController : ControllerBase
    {
        private readonly IDbConnection _db;

        /// <summary>
        /// Khởi tạo controller với kết nối cơ sở dữ liệu.
        /// </summary>
        /// <param name="db">Kết nối cơ sở dữ liệu.</param>
        public PhongQuanLyController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Cập nhật trạng thái phòng (Trống, Đang dọn, Sử dụng).
        /// </summary>
        /// <param name="id">Mã phòng cần cập nhật trạng thái.</param>
        /// <param name="trangThai">Trạng thái mới của phòng (Trống, Đang dọn, Sử dụng).</param>
        /// <returns>Kết quả cập nhật trạng thái phòng.</returns>
        [HttpPut("{id}/trangthai")]
        public async Task<IActionResult> CapNhatTrangThaiPhong(string id, [FromBody] string trangThai)
        {
            // Kiểm tra trạng thái hợp lệ
            var validTrangThai = new[] { "Trống", "Đang dọn", "Sử dụng" };
            if (!validTrangThai.Contains(trangThai))
            {
                return BadRequest(new { Message = "Trạng thái không hợp lệ. Chỉ chấp nhận: Trống, Đang dọn, Sử dụng." });
            }

            // Kiểm tra xem phòng có tồn tại không
            const string checkPhongQuery = "SELECT COUNT(1) FROM Phong WHERE MaPhong = @MaPhong";
            var isPhongExists = await _db.ExecuteScalarAsync<int>(checkPhongQuery, new { MaPhong = id });
            if (isPhongExists == 0)
            {
                return NotFound(new { Message = "Phòng không tồn tại." });
            }

            // Cập nhật trạng thái phòng
            const string updateQuery = "UPDATE Phong SET TinhTrang = @TinhTrang WHERE MaPhong = @MaPhong";
            await _db.ExecuteAsync(updateQuery, new { TinhTrang = trangThai, MaPhong = id });

            return Ok(new { Message = "Cập nhật trạng thái phòng thành công.", MaPhong = id, TrangThai = trangThai });
        }

        /// <summary>
        /// Đặt phòng mới.
        /// </summary>
        /// <param name="datPhongDTO">Thông tin đặt phòng.</param>
        /// <returns>Kết quả đặt phòng.</returns>
        [HttpPost("datphong")]
        public async Task<IActionResult> DatPhong([FromBody] DatPhongDTO datPhongDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem phòng có tồn tại không
            const string checkPhongQuery = "SELECT COUNT(1) FROM Phong WHERE MaPhong = @MaPhong";
            var isPhongExists = await _db.ExecuteScalarAsync<int>(checkPhongQuery, new { MaPhong = datPhongDTO.MaPhong });
            if (isPhongExists == 0)
            {
                return NotFound(new { Message = "Phòng không tồn tại." });
            }

            // Kiểm tra trạng thái phòng (chỉ cho phép đặt nếu phòng đang "Trống")
            const string checkTinhTrangQuery = "SELECT TinhTrang FROM Phong WHERE MaPhong = @MaPhong";
            var tinhTrang = await _db.ExecuteScalarAsync<string>(checkTinhTrangQuery, new { MaPhong = datPhongDTO.MaPhong });
            if (tinhTrang != "Trống")
            {
                return BadRequest(new { Message = "Phòng hiện không khả dụng để đặt." });
            }

            // Tạo mã đặt phòng duy nhất
            const string generateMaDatPhongQuery = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaDatPhong, 3, LEN(MaDatPhong) - 2) AS INT)), 0) + 1
                FROM DatPhong";
            var nextId = await _db.ExecuteScalarAsync<int>(generateMaDatPhongQuery);
            datPhongDTO.MaDatPhong = $"DP{nextId:D3}";

            // Thêm đơn đặt phòng vào cơ sở dữ liệu
            const string insertQuery = @"
                INSERT INTO DatPhong (MaDatPhong, MaNguoiDung, MaPhong, NgayDat, NgayCheckIn, NgayCheckOut, TinhTrangDatPhong)
                VALUES (@MaDatPhong, @MaNguoiDung, @MaPhong, @NgayDat, @NgayCheckIn, @NgayCheckOut, @TinhTrangDatPhong)";
            await _db.ExecuteAsync(insertQuery, datPhongDTO);

            // Cập nhật trạng thái phòng thành "Sử dụng"
            const string updatePhongQuery = "UPDATE Phong SET TinhTrang = 'Sử dụng' WHERE MaPhong = @MaPhong";
            await _db.ExecuteAsync(updatePhongQuery, new { MaPhong = datPhongDTO.MaPhong });

            return Ok(new { Message = "Đặt phòng thành công.", MaDatPhong = datPhongDTO.MaDatPhong });
        }
    }
}