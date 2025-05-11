using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;

namespace HotelManagementAPI.Controllers.KhachHang
{
    [ApiController]
    [Route("api/KhachHang/datphong")]
    public class DatPhongController : ControllerBase
    {
        private readonly IDbConnection _db;

        public DatPhongController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Tạo đơn đặt phòng.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> TaoDonDatPhong([FromBody] DatPhongDTO datPhongDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Tạo mã đặt phòng duy nhất
            datPhongDTO.MaDatPhong = await GenerateUniqueMaDatPhong();

            const string insertQuery = @"
                INSERT INTO DatPhong (MaDatPhong, MaNguoiDung, MaPhong, NgayDat, NgayNhan, NgayTra, TongTien, TrangThai)
                VALUES (@MaDatPhong, @MaNguoiDung, @MaPhong, @NgayDat, @NgayNhan, @NgayTra, @TongTien, @TrangThai)";
            await _db.ExecuteAsync(insertQuery, datPhongDTO);

            return Ok(new { Message = "Đặt phòng thành công.", MaDatPhong = datPhongDTO.MaDatPhong });
        }

        /// <summary>
        /// Lấy lịch sử đặt phòng của người dùng.
        /// </summary>
        [HttpGet("lichsu")]
        public async Task<IActionResult> LichSuDatPhong([FromQuery] string maNguoiDung)
        {
            const string query = @"
                SELECT dp.MaDatPhong, dp.MaNguoiDung, dp.MaPhong, dp.NgayDat, dp.NgayNhan, dp.NgayTra, dp.TongTien, dp.TrangThai,
                       p.TenPhong, p.LoaiPhong, p.GiaPhong
                FROM DatPhong dp
                INNER JOIN Phong p ON dp.MaPhong = p.MaPhong
                WHERE dp.MaNguoiDung = @MaNguoiDung";

            var result = await _db.QueryAsync(query, new { MaNguoiDung = maNguoiDung });

            return Ok(result);
        }

        /// <summary>
        /// Hủy đặt phòng.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> HuyDatPhong(string id)
        {
            // Kiểm tra xem mã đặt phòng có tồn tại không
            const string checkQuery = "SELECT COUNT(1) FROM DatPhong WHERE MaDatPhong = @MaDatPhong";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaDatPhong = id });
            if (isExists == 0)
            {
                return NotFound(new { Message = "Mã đặt phòng không tồn tại." });
            }

            // Xóa đơn đặt phòng
            const string deleteQuery = "DELETE FROM DatPhong WHERE MaDatPhong = @MaDatPhong";
            await _db.ExecuteAsync(deleteQuery, new { MaDatPhong = id });

            return Ok(new { Message = "Hủy đặt phòng thành công." });
        }

        private async Task<string> GenerateUniqueMaDatPhong()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaDatPhong, 3, LEN(MaDatPhong) - 2) AS INT)), 0) + 1
                FROM DatPhong";

            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"DP{nextId:D3}";
        }
    }
}