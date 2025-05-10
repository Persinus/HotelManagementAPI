using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.Models;
using HotelManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;


// Ensure the namespace containing PhongDTOs is imported

// DTO cho PhongWithTienNghi
public class PhongDTOs
{
    public string MaPhong { get; set; }
    public string LoaiPhong { get; set; }
    public decimal GiaPhong { get; set; }
    public string TinhTrang { get; set; }
    public int SoLuongPhong { get; set; }
    public int Tang { get; set; }
    public string KieuGiuong { get; set; }
    public string MoTa { get; set; }
    public string UrlAnhChinh { get; set; }
    public string MotaPhong { get; set; }
    public int SucChua { get; set; }
    public int SoGiuong { get; set; }
    public string DonViTinh { get; set; }
    public decimal SoSaoTrungBinh { get; set; }
    public List<string> UrlAnhPhu { get; set; } // Danh sách ảnh phụ
    public List<TienNghiDTO> TienNghi { get; set; } // Danh sách tiện nghi
    public List<GiamGiaDTO> GiamGia { get; set; } // Danh sách giảm giá
}

namespace HotelManagementAPI.DTOs
{
    public class PhongDetailsDTO
    {
        public string MaPhong { get; set; }
        public string LoaiPhong { get; set; }
        public decimal GiaPhong { get; set; }
        public string TinhTrang { get; set; }
        public int SoLuongPhong { get; set; }
        public int Tang { get; set; }
        public string KieuGiuong { get; set; }
        public string? MoTa { get; set; }
        public string UrlAnhChinh { get; set; }
        public string? MotaPhong { get; set; }
        public int SucChua { get; set; }
        public int SoGiuong { get; set; }
        public string DonViTinh { get; set; }
        public decimal SoSaoTrungBinh { get; set; }

        // Danh sách ảnh phụ
        public List<string> UrlAnhPhu { get; set; } = new List<string>();

        // Danh sách tiện nghi
        public List<TienNghiDTO> TienNghi { get; set; } = new List<TienNghiDTO>();

        // Danh sách giảm giá
        public List<GiamGiaDTO> GiamGia { get; set; } = new List<GiamGiaDTO>();
    }

    public class TienNghiDTO
    {
        public string MaTienNghi { get; set; }
        public string TenTienNghi { get; set; }
        public string? MoTa { get; set; }
    }

    public class GiamGiaDTO
    {
        public string MaGiamGia { get; set; }
        public string TenGiamGia { get; set; }
        public string? LoaiGiamGia { get; set; }
        public decimal GiaTriGiam { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string? MoTa { get; set; }
    }
}

namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhongController : ControllerBase
    {
        private readonly IDbConnection _db;

        public PhongController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Lấy danh sách tất cả phòng với tiện nghi.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về danh sách tất cả phòng và tiện nghi có trong hệ thống.
        ///
        /// **Mã trạng thái**:
        /// - 200: Thành công, trả về danh sách phòng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <returns>Danh sách phòng.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhongDTOs>>> GetAll()
        {
            const string roomQuery = @"
                SELECT p.MaPhong, p.LoaiPhong, p.GiaPhong, p.TinhTrang, p.SoLuongPhong, p.Tang, 
                       p.KieuGiuong, p.MoTa, p.UrlAnhChinh, p.MotaPhong, p.SucChua, p.SoGiuong, 
                       p.DonViTinh, p.SoSaoTrungBinh
                FROM Phong p";
            var rooms = (await _db.QueryAsync<PhongDTOs>(roomQuery)).ToList();

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

        /// <summary>
        /// Lấy thông tin phòng theo mã phòng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về thông tin chi tiết của một phòng dựa trên mã phòng (`MaPhong`).
        ///
        /// **Mã trạng thái**:
        /// - 200: Thành công, trả về thông tin phòng.
        /// - 404: Không tìm thấy phòng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã phòng cần tìm. Ví dụ: `P001`.</param>
        /// <returns>Thông tin phòng.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PhongDTOs>> GetById(string id)
        {
            // Lấy thông tin phòng
            const string roomQuery = @"
                SELECT p.MaPhong, p.LoaiPhong, p.GiaPhong, p.TinhTrang, p.SoLuongPhong, p.Tang, 
                       p.KieuGiuong, p.MoTa, p.UrlAnhChinh, p.MotaPhong, p.SucChua, p.SoGiuong, 
                       p.DonViTinh, p.SoSaoTrungBinh
                FROM Phong p
                WHERE p.MaPhong = @Id";
            var room = await _db.QueryFirstOrDefaultAsync<PhongDTOs>(roomQuery, new { Id = id });

            if (room == null) return NotFound(new { Message = "Không tìm thấy phòng." });

            // Lấy danh sách ảnh phụ
            const string imagesQuery = "SELECT UrlAnh FROM PhongAnh WHERE MaPhong = @MaPhong";
            room.UrlAnhPhu = (await _db.QueryAsync<string>(imagesQuery, new { MaPhong = id })).ToList();

            // Lấy danh sách tiện nghi
            const string amenitiesQuery = @"
                SELECT tn.MaTienNghi, tn.TenTienNghi, tn.MoTa
                FROM TienNghi tn
                JOIN Phong_TienNghi ptn ON tn.MaTienNghi = ptn.MaTienNghi
                WHERE ptn.MaPhong = @MaPhong";
            room.TienNghi = (await _db.QueryAsync<TienNghiDTO>(amenitiesQuery, new { MaPhong = id })).ToList();

            // Lấy danh sách giảm giá
            const string discountsQuery = @"
                SELECT gg.MaGiamGia, gg.TenGiamGia, gg.LoaiGiamGia, gg.GiaTriGiam, gg.NgayBatDau, gg.NgayKetThuc, gg.MoTa
                FROM GiamGia gg
                JOIN Phong_GiamGia pg ON gg.MaGiamGia = pg.MaGiamGia
                WHERE pg.MaPhong = @MaPhong";
            room.GiamGia = (await _db.QueryAsync<GiamGiaDTO>(discountsQuery, new { MaPhong = id })).ToList();

            return Ok(room);
        }

        /// <summary>
        /// Lấy danh sách phòng theo tầng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về danh sách phòng thuộc tầng được chỉ định.
        /// 
        /// **Mã trạng thái**:
        /// - 200: Thành công, trả về danh sách phòng.
        /// - 404: Không tìm thấy phòng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="tang">Tầng cần tìm. Ví dụ: `2`.</param>
        /// <returns>Danh sách phòng thuộc tầng.</returns>
        [HttpGet("by-floor/{tang}")]
        public async Task<ActionResult<IEnumerable<PhongDTOs>>> GetByFloor(int tang)
        {
            const string query = "SELECT * FROM PhongWithTienNghi WHERE Tang = @Tang";
            var result = await _db.QueryAsync<PhongDTOs>(query, new { Tang = tang });

            return result.Any() ? Ok(result) : NotFound();
        }

        /// <summary>
        /// Tạo mới một phòng với tiện nghi.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này tạo mới một phòng trong hệ thống với thông tin được cung cấp.
        ///
        /// **Mã trạng thái**:
        /// - 201: Tạo thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="phong">Thông tin phòng cần tạo.</param>
        /// <returns>Thông tin phòng vừa được tạo.</returns>
        [HttpPost]
        public async Task<ActionResult<PhongDTOs>> Create([FromBody] PhongDTOs phong)
        {
            // Thêm phòng vào bảng Phong
            const string insertPhongQuery = @"
                INSERT INTO Phong (
                    MaPhong, LoaiPhong, GiaPhong, TinhTrang, SoLuongPhong, Tang, 
                    KieuGiuong, MoTa, UrlAnhChinh, MotaPhong, SucChua, SoGiuong, DonViTinh
                )
                VALUES (
                    @MaPhong, @LoaiPhong, @GiaPhong, @TinhTrang, @SoLuongPhong, @Tang, 
                    @KieuGiuong, @MoTa, @UrlAnhChinh, @MotaPhong, @SucChua, @SoGiuong, @DonViTinh
                )";
            await _db.ExecuteAsync(insertPhongQuery, phong);

            // Thêm ảnh phụ vào bảng PhongAnh
            if (phong.UrlAnhPhu != null && phong.UrlAnhPhu.Any())
            {
                const string insertPhongAnhQuery = "INSERT INTO PhongAnh (MaPhong, UrlAnh) VALUES (@MaPhong, @UrlAnh)";
                foreach (var urlAnh in phong.UrlAnhPhu)
                {
                    await _db.ExecuteAsync(insertPhongAnhQuery, new { MaPhong = phong.MaPhong, UrlAnh = urlAnh });
                }
            }

            // Thêm tiện nghi vào bảng Phong_TienNghi
            if (phong.TienNghi != null && phong.TienNghi.Any())
            {
                foreach (var tienNghi in phong.TienNghi)
                {
                    const string checkTienNghiQuery = "SELECT COUNT(1) FROM TienNghi WHERE MaTienNghi = @MaTienNghi";
                    var exists = await _db.ExecuteScalarAsync<int>(checkTienNghiQuery, new { MaTienNghi = tienNghi.MaTienNghi });

                    if (exists == 0)
                    {
                        const string insertTienNghiQuery = "INSERT INTO TienNghi (MaTienNghi, TenTienNghi, MoTa) VALUES (@MaTienNghi, @TenTienNghi, @MoTa)";
                        await _db.ExecuteAsync(insertTienNghiQuery, tienNghi);
                    }

                    const string insertPhongTienNghiQuery = "INSERT INTO Phong_TienNghi (MaPhong, MaTienNghi) VALUES (@MaPhong, @MaTienNghi)";
                    await _db.ExecuteAsync(insertPhongTienNghiQuery, new { MaPhong = phong.MaPhong, MaTienNghi = tienNghi.MaTienNghi });
                }
            }

            // Xử lý giảm giá
            if (phong.GiamGia != null && phong.GiamGia.Any())
            {
                foreach (var giamGia in phong.GiamGia)
                {
                    // Kiểm tra loại giảm giá
                    if (giamGia.LoaiGiamGia == "PhanTram")
                    {
                        giamGia.GiaTriGiam = phong.GiaPhong - (phong.GiaPhong * giamGia.GiaTriGiam / 100);
                    }
                    else if (giamGia.LoaiGiamGia == "SoTien")
                    {
                        giamGia.GiaTriGiam = phong.GiaPhong - giamGia.GiaTriGiam;
                    }
                    else
                    {
                        return BadRequest(new { Message = "Loại giảm giá không hợp lệ. Chỉ chấp nhận 'PhanTram' hoặc 'SoTien'." });
                    }

                    // Kiểm tra xem MaGiamGia có tồn tại trong bảng GiamGia không
                    const string checkGiamGiaQuery = "SELECT COUNT(1) FROM GiamGia WHERE MaGiamGia = @MaGiamGia";
                    var exists = await _db.ExecuteScalarAsync<int>(checkGiamGiaQuery, new { MaGiamGia = giamGia.MaGiamGia });

                    if (exists == 0)
                    {
                        const string insertGiamGiaQuery = @"
                            INSERT INTO GiamGia (MaGiamGia, TenGiamGia, LoaiGiamGia, GiaTriGiam, NgayBatDau, NgayKetThuc, MoTa)
                            VALUES (@MaGiamGia, @TenGiamGia, @LoaiGiamGia, @GiaTriGiam, @NgayBatDau, @NgayKetThuc, @MoTa)";
                        await _db.ExecuteAsync(insertGiamGiaQuery, giamGia);
                    }

                    // Thêm giảm giá vào bảng Phong_GiamGia
                    const string insertPhongGiamGiaQuery = "INSERT INTO Phong_GiamGia (MaPhong, MaGiamGia) VALUES (@MaPhong, @MaGiamGia)";
                    await _db.ExecuteAsync(insertPhongGiamGiaQuery, new { MaPhong = phong.MaPhong, MaGiamGia = giamGia.MaGiamGia });
                }
            }
            else
            {
                return BadRequest(new { Message = "Không có thông tin giảm giá." });
            }

            return CreatedAtAction(nameof(GetById), new { id = phong.MaPhong }, phong);
        }

        /// <summary>
        /// Cập nhật thông tin phòng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này cập nhật thông tin của một phòng dựa trên mã phòng (`MaPhong`).
        ///
        /// **Mã trạng thái**:
        /// - 204: Cập nhật thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 404: Không tìm thấy phòng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã phòng cần cập nhật. Ví dụ: `P002`.</param>
        /// <param name="phong">Thông tin phòng cần cập nhật.</param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PhongDTOs phong)
        {
            if (id != phong.MaPhong) return BadRequest();

            const string query = @"
                UPDATE PhongWithTienNghi
                SET LoaiPhong = @LoaiPhong,
                    GiaPhong = @GiaPhong,
                    TinhTrang = @TinhTrang,
                    SoLuongPhong = @SoLuongPhong,
                    Tang = @Tang,
                    KieuGiuong = @KieuGiuong,
                    MoTa = @MoTa,
                    UrlAnhChinh = @UrlAnhChinh,
                    UrlAnhPhu1 = @UrlAnhPhu1,
                    UrlAnhPhu2 = @UrlAnhPhu2,
                    TienNghi = @TienNghi
                WHERE MaPhong = @MaPhong";

            var affected = await _db.ExecuteAsync(query, phong);
            return affected > 0 ? NoContent() : NotFound();
        }

        /// <summary>
        /// Xóa một phòng theo mã phòng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này xóa một phòng khỏi hệ thống dựa trên mã phòng (`MaPhong`).
        ///
        /// **Mã trạng thái**:
        /// - 204: Xóa thành công.
        /// - 404: Không tìm thấy phòng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã phòng cần xóa. Ví dụ: `P002`.</param>
        /// <returns>Kết quả xóa.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            // Xóa ảnh phụ liên quan
            const string deletePhongAnhQuery = "DELETE FROM PhongAnh WHERE MaPhong = @MaPhong";
            await _db.ExecuteAsync(deletePhongAnhQuery, new { MaPhong = id });

            // Xóa tiện nghi liên quan
            const string deletePhongTienNghiQuery = "DELETE FROM Phong_TienNghi WHERE MaPhong = @MaPhong";
            await _db.ExecuteAsync(deletePhongTienNghiQuery, new { MaPhong = id });

            // Xóa giảm giá liên quan
            const string deletePhongGiamGiaQuery = "DELETE FROM Phong_GiamGia WHERE MaPhong = @MaPhong";
            await _db.ExecuteAsync(deletePhongGiamGiaQuery, new { MaPhong = id });

            // Xóa phòng
            const string deletePhongQuery = "DELETE FROM Phong WHERE MaPhong = @MaPhong";
            var affected = await _db.ExecuteAsync(deletePhongQuery, new { MaPhong = id });

            return affected > 0 ? NoContent() : NotFound();
        }

        /// <summary>
        /// Cập nhật tình trạng phòng.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này cập nhật tình trạng của một phòng dựa trên mã phòng (`MaPhong`).
        /// 
        /// **Mã trạng thái**:
        /// - 204: Cập nhật thành công.
        /// - 400: Dữ liệu không hợp lệ.
        /// - 404: Không tìm thấy phòng.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <param name="id">Mã phòng cần cập nhật. Ví dụ: `P001`.</param>
        /// <param name="tinhTrang">Tình trạng mới của phòng. Ví dụ: `Unavailable`.</param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] string tinhTrang)
        {
            const string query = "UPDATE PhongWithTienNghi SET TinhTrang = @TinhTrang WHERE MaPhong = @MaPhong";
            var affected = await _db.ExecuteAsync(query, new { MaPhong = id, TinhTrang = tinhTrang });

            return affected > 0 ? NoContent() : NotFound();
        }

        /// <summary>
        /// Tính số sao trung bình của tất cả các phòng dựa trên đánh giá từ bảng User_Feedback.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về danh sách tất cả các phòng cùng với số sao trung bình.
        /// Nếu một phòng không có đánh giá, số sao trung bình sẽ là `0`.
        ///
        /// **Mã trạng thái**:
        /// - 200: Thành công, trả về danh sách phòng với số sao trung bình.
        /// - 500: Lỗi máy chủ.
        /// </remarks>
        /// <returns>Danh sách phòng với số sao trung bình.</returns>
        [HttpGet("average-ratings")]
        public async Task<ActionResult<IEnumerable<PhongDTOs>>> CalculateAverageRatings()
        {
            const string query = @"
                SELECT p.MaPhong, p.LoaiPhong, p.GiaPhong, p.TinhTrang, p.SoLuongPhong, p.Tang, 
                       p.KieuGiuong, p.MoTa, p.UrlAnhChinh, p.UrlAnhPhu1, p.UrlAnhPhu2, p.TienNghi,
                       ISNULL((
                           SELECT AVG(CAST(SoSao AS FLOAT)) 
                           FROM User_Feedback 
                           WHERE User_Feedback.MaPhong = p.MaPhong
                       ), 0) AS SoSaoTrungBinh
                FROM PhongWithTienNghi p";
            
            var result = await _db.QueryAsync<PhongDTOs>(query);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách tất cả phòng với tiện nghi.
        /// </summary>
        /// <remarks>
        /// **Mô tả**: API này trả về danh sách tất cả phòng và tiện nghi có trong hệ thống.
        /// **Trạng thái**:
        /// - 0: Thành công, trả về danh sách phòng.
        /// - 1: Lỗi máy chủ.
        /// </remarks>
        /// <returns>Danh sách phòng.</returns>
        [HttpGet("danhsach")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetAllRoomsWithAmenities()
        {
            try
            {
                const string query = @"
                    SELECT p.MaPhong, p.TenPhong, p.LoaiPhong, p.GiaPhong, t.TenTienNghi
                    FROM Phong p
                    LEFT JOIN PhongTienNghi pt ON p.MaPhong = pt.MaPhong
                    LEFT JOIN TienNghi t ON pt.MaTienNghi = t.MaTienNghi";

                var rooms = await _db.QueryAsync(query);
                return Ok(new { Status = 0, Data = rooms });
            }
            catch
            {
                return StatusCode(500, new { Status = 1, Message = "Lỗi máy chủ." });
            }
        }
    }
}