using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;

namespace HotelManagementAPI.Controllers.KhachHang
{
    [ApiController]
    [Route("api/KhachHang/datdichvu")]
    public class DatDichVuController : ControllerBase
    {
        private readonly IDbConnection _db;

        public DatDichVuController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Đặt dịch vụ đi kèm.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DatDichVu([FromBody] DatDichVuDTO datDichVuDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra giá trị soLuong
            if (datDichVuDTO.SoLuong < 1 || datDichVuDTO.SoLuong > 1000)
            {
                return BadRequest(new { Message = "Số lượng phải nằm trong khoảng từ 1 đến 1000." });
            }

            // Kiểm tra xem MaDatPhong có tồn tại không
            const string checkDatPhongQuery = "SELECT COUNT(1) FROM DatPhong WHERE MaDatPhong = @MaDatPhong";
            var isDatPhongExists = await _db.ExecuteScalarAsync<int>(checkDatPhongQuery, new { datDichVuDTO.MaDatPhong });
            if (isDatPhongExists == 0)
            {
                return NotFound(new { Message = "Mã đặt phòng không tồn tại." });
            }

            // Kiểm tra xem MaDichVu có tồn tại không
            const string checkDichVuQuery = "SELECT SoLuong FROM DichVu WHERE MaDichVu = @MaDichVu";
            var dichVuSoLuong = await _db.ExecuteScalarAsync<int?>(checkDichVuQuery, new { datDichVuDTO.MaDichVu });
            if (dichVuSoLuong == null)
            {
                return NotFound(new { Message = "Mã dịch vụ không tồn tại." });
            }

            // Kiểm tra số lượng dịch vụ còn lại
            if (datDichVuDTO.SoLuong > dichVuSoLuong)
            {
                return BadRequest(new { Message = "Số lượng dịch vụ yêu cầu vượt quá số lượng còn lại." });
            }

            // Tạo mã đặt dịch vụ
            datDichVuDTO.MaDatDichVu = await GenerateUniqueMaDatDichVu();

            // Chèn dữ liệu vào bảng DatDichVu
            const string insertQuery = @"
                INSERT INTO DatDichVu (MaDatDichVu, MaDatPhong, MaDichVu, SoLuong, MaHoaDon)
                VALUES (@MaDatDichVu, @MaDatPhong, @MaDichVu, @SoLuong, @MaHoaDon)";
            await _db.ExecuteAsync(insertQuery, datDichVuDTO);

            // Cập nhật số lượng dịch vụ còn lại
            const string updateDichVuQuery = "UPDATE DichVu SET SoLuong = SoLuong - @SoLuong WHERE MaDichVu = @MaDichVu";
            await _db.ExecuteAsync(updateDichVuQuery, new { datDichVuDTO.SoLuong, datDichVuDTO.MaDichVu });

            return Ok(new { Message = "Đặt dịch vụ thành công.", MaDatDichVu = datDichVuDTO.MaDatDichVu });
        }

        /// <summary>
        /// Lấy lịch sử đặt dịch vụ.
        /// </summary>
        [HttpGet("lichsu")]
        public async Task<IActionResult> LichSuDatDichVu([FromQuery] string maDatPhong)
        {
            const string query = @"
                SELECT ddv.MaDatDichVu, ddv.MaDatPhong, ddv.MaDichVu, ddv.SoLuong, ddv.MaHoaDon,
                       dv.TenDichVu, dv.DonGia, dv.MoTaDichVu, dv.HinhAnhDichVu
                FROM DatDichVu ddv
                INNER JOIN DichVu dv ON ddv.MaDichVu = dv.MaDichVu
                WHERE ddv.MaDatPhong = @MaDatPhong";

            var result = await _db.QueryAsync(query, new { MaDatPhong = maDatPhong });

            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách toàn bộ dịch vụ.
        /// </summary>
        [HttpGet("danhsach")]
        public async Task<IActionResult> GetAllDichVu()
        {
            const string query = @"
                SELECT MaDichVu, TenDichVu, DonGia, MoTaDichVu, HinhAnhDichVu, SoLuong, TrangThai, LoaiDichVu, DonViTinh
                FROM DichVu";

            var dichVuList = await _db.QueryAsync(query);

            return Ok(dichVuList);
        }

        private async Task<string> GenerateUniqueMaDatDichVu()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaDatDichVu, 4, LEN(MaDatDichVu) - 3) AS INT)), 0) + 1
                FROM DatDichVu";

            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"DDV{nextId:D3}";
        }
    }
}