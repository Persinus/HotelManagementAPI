using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using HotelManagementAPI.DTOs.QuanTriVien;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;

namespace HotelManagementAPI.Controllers.QuanTriVien
{
    [ApiController]
    [Route("api/quantrivien")]
    [Authorize(Roles = "QuanTriVien")]
    public class QuanTriVienController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly IConfiguration _config;
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _settings;

        public QuanTriVienController(
            IDbConnection db,
            IOptions<CloudinarySettings> cloudinaryOptions,
            IConfiguration config)
        {
            _db = db;
            _config = config;
            _settings = cloudinaryOptions.Value;

            var account = new Account(
                _settings.CloudName,
                _settings.ApiKey,
                _settings.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }
   
        
        
        // Lấy danh sách tất cả khách hàng
        [HttpGet("khachhang")]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả khách hàng", Description = "Trả về danh sách tất cả khách hàng trong hệ thống.")]
        [SwaggerResponse(200, "Danh sách khách hàng.")]
        public async Task<ActionResult<IEnumerable<NguoiDungDTO>>> GetAllKhachHang()
        {
            const string query = "SELECT * FROM NguoiDung WHERE Vaitro = 'KhachHang'";
            var khachHangList = await _db.QueryAsync<NguoiDungDTO>(query);
            return Ok(new { Message = "✅ Lấy danh sách khách hàng thành công.", Data = khachHangList });
        }

        // Lấy danh sách tất cả nhân viên
        [HttpGet("nhanvien")]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả nhân viên", Description = "Trả về danh sách tất cả nhân viên trong hệ thống.")]
        [SwaggerResponse(200, "Danh sách nhân viên.")]
        public async Task<ActionResult<IEnumerable<NguoiDungDTO>>> GetAllNhanVien()
        {
            const string query = "SELECT * FROM NguoiDung WHERE Vaitro = 'NhanVien'";
            var nhanVienList = await _db.QueryAsync<NguoiDungDTO>(query);
            return Ok(new { Message = "✅ Lấy danh sách nhân viên thành công.", Data = nhanVienList });
        }

        // Lấy thông tin chi tiết của một khách hàng
        [HttpGet("khachhang/{maNguoiDung}")]
        [SwaggerOperation(Summary = "Lấy thông tin chi tiết khách hàng", Description = "Trả về thông tin chi tiết của một khách hàng theo mã.")]
        [SwaggerResponse(200, "Thông tin khách hàng.")]
        [SwaggerResponse(404, "Không tìm thấy thông tin khách hàng.")]
        public async Task<ActionResult<NguoiDungDTO>> GetKhachHangByMaNguoiDung([FromRoute] string maNguoiDung)
        {
            const string query = "SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung AND Vaitro = 'KhachHang'";
            var khachHang = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { MaNguoiDung = maNguoiDung });

            // Lấy thông tin chi tiết khách hàng
            if (khachHang == null)
                return NotFound(new { Message = "❌ Không tìm thấy thông tin khách hàng." });
            return Ok(new { Message = "✅ Lấy thông tin khách hàng thành công.", Data = khachHang });
        }

        // Lấy thông tin chi tiết của một nhân viên
        [HttpGet("nhanvien/{maNguoiDung}")]
        [SwaggerOperation(Summary = "Lấy thông tin chi tiết nhân viên", Description = "Trả về thông tin chi tiết của một nhân viên theo mã.")]
        [SwaggerResponse(200, "Thông tin nhân viên.")]
        [SwaggerResponse(404, "Không tìm thấy thông tin nhân viên.")]
        public async Task<ActionResult<NguoiDungDTO>> GetNhanVienByMaNguoiDung([FromRoute] string maNguoiDung)
        {
            const string query = "SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung AND Vaitro = 'NhanVien'";
            var nhanVien = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { MaNguoiDung = maNguoiDung });

            // Lấy thông tin chi tiết nhân viên
            if (nhanVien == null)
                return NotFound(new { Message = "❌ Không tìm thấy thông tin nhân viên." });
            return Ok(new { Message = "✅ Lấy thông tin nhân viên thành công.", Data = nhanVien });
        }

       
        // Xem vai trò hiện tại của người dùng
        [HttpGet("hethong/nguoidung/{maNguoiDung}/vaitro")]
        [SwaggerOperation(Summary = "Xem vai trò người dùng")]
        public async Task<IActionResult> XemVaiTroNguoiDung([FromRoute] string maNguoiDung)
        {
            const string query = "SELECT Vaitro FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var vaiTro = await _db.ExecuteScalarAsync<string>(query, new { MaNguoiDung = maNguoiDung });

            // Xem vai trò người dùng
            if (vaiTro == null)
                return NotFound(new { Message = "❌ Không tìm thấy người dùng." });
            return Ok(new { Message = "✅ Lấy vai trò người dùng thành công.", MaNguoiDung = maNguoiDung, VaiTro = vaiTro });
        }

       

       

        // Xóa phòng
        [HttpDelete("phong/{maPhong}")]
        public async Task<IActionResult> XoaPhong(string maPhong)
        {
            const string checkQuery = "SELECT COUNT(1) FROM Phong WHERE MaPhong = @MaPhong";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaPhong = maPhong });

            // Xóa phòng
            if (isExists == 0)
                return NotFound(new { Message = "❌ Phòng không tồn tại." });
            const string deleteQuery = "DELETE FROM Phong WHERE MaPhong = @MaPhong";
            await _db.ExecuteAsync(deleteQuery, new { MaPhong = maPhong });

            return Ok(new { Message = "✅ Xóa phòng thành công." });
        }

       
      
        /// <summary>
        /// Thêm 1 dịch vụ mới (có ảnh).
        /// </summary>
        [HttpPost("dichvu/them1dichvu")]
        [SwaggerOperation(
            Summary = "Thêm 1 dịch vụ mới",
            Description = "Thêm một dịch vụ mới với ảnh đại diện upload lên Cloudinary."
        )]
        [SwaggerResponse(200, "Thêm dịch vụ thành công.")]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ hoặc upload ảnh thất bại.")]
   
        public async Task<IActionResult> Them1DichVu([FromForm] QuanTriVienThem1DichVuDTO dto, IFormFile? file)
        {
            string? imageUrl = null;
            if (file != null && file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Width(800).Height(800).Crop("limit"),
                    Folder = "dichvu"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    imageUrl = uploadResult.SecureUrl.ToString();
                else
                    return StatusCode(500, $"Upload ảnh thất bại: {uploadResult.Error?.Message}");
            }

            // Sinh mã dịch vụ tự động
            const string generateMaDichVuQuery = @"SELECT ISNULL(MAX(CAST(SUBSTRING(MaDichVu, 3, LEN(MaDichVu) - 2) AS INT)), 0) + 1 FROM DichVu";
            var nextId = await _db.ExecuteScalarAsync<int>(generateMaDichVuQuery);
            var maDichVu = $"DV{nextId:D3}";

            const string insertQuery = @"
                INSERT INTO DichVu (MaDichVu, TenDichVu, DonGia, MoTaDichVu, HinhAnhDichVu, SoLuong, LoaiDichVu, DonViTinh)
                VALUES (@MaDichVu, @TenDichVu, @DonGia, @MoTaDichVu, @HinhAnhDichVu, @SoLuong, @LoaiDichVu, @DonViTinh)";
            await _db.ExecuteAsync(insertQuery, new
            {
                MaDichVu = maDichVu,
                dto.TenDichVu,
                dto.DonGia,
                dto.MoTaDichVu,
                HinhAnhDichVu = imageUrl,
                dto.SoLuong,
                dto.LoaiDichVu,
                dto.DonViTinh
            });

            // Thêm dịch vụ thành công
            return Ok(new { Message = "🎉 Thêm dịch vụ thành công.", MaDichVu = maDichVu, HinhAnhUrl = imageUrl });
        }

       

        /// <summary>
        /// Thêm 1 tiện nghi mới (có ảnh).
        /// </summary>
        [HttpPost("tiennghi/them1tiennghi")]
        [SwaggerOperation(
            Summary = "Thêm 1 tiện nghi mới",
            Description = "Thêm một tiện nghi mới với ảnh đại diện upload lên Cloudinary."
        )]
        [SwaggerResponse(200, "Thêm tiện nghi thành công.")]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ hoặc upload ảnh thất bại.")]
  
        public async Task<IActionResult> Them1TienNghi([FromForm] QuanTriVienThem1TienNghiDTO dto, IFormFile? file)
        {
            string? imageUrl = null;
            if (file != null && file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Width(800).Height(800).Crop("limit"),
                    Folder = "tiennghi"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    imageUrl = uploadResult.SecureUrl.ToString();
                else
                    return StatusCode(500, $"Upload ảnh thất bại: {uploadResult.Error?.Message}");
            }

            // Sinh mã tiện nghi tự động
            const string generateMaTienNghiQuery = @"SELECT ISNULL(MAX(CAST(SUBSTRING(MaTienNghi, 3, LEN(MaTienNghi) - 2) AS INT)), 0) + 1 FROM TienNghi";
            var nextId = await _db.ExecuteScalarAsync<int>(generateMaTienNghiQuery);
            var maTienNghi = $"TN{nextId:D3}";

            const string insertQuery = @"
                INSERT INTO TienNghi (MaTienNghi, TenTienNghi, MoTa, HinhAnhTienNghi)
                VALUES (@MaTienNghi, @TenTienNghi, @MoTa, @HinhAnhTienNghi)";
            await _db.ExecuteAsync(insertQuery, new
            {
                MaTienNghi = maTienNghi,
                TenTienNghi = dto.TenTienNghi,
                MoTa = dto.MoTa,
                HinhAnhTienNghi = imageUrl
            });

            return Ok(new { Message = "🎉 Thêm tiện nghi thành công.", MaTienNghi = maTienNghi, HinhAnhUrl = imageUrl });
        }

        /// <summary>
        /// Duyệt bài viết (chuyển trạng thái từ "Chờ Duyệt" sang "Đã Duyệt").
        /// </summary>
        [HttpPatch("baiviet/duyet/{maBaiViet}")]
        [SwaggerOperation(
            Summary = "Duyệt bài viết",
            Description = "Quản trị viên duyệt bài viết, chuyển trạng thái từ 'Chờ Duyệt' sang 'Đã Duyệt'."
        )]
        [SwaggerResponse(200, "Duyệt bài viết thành công.")]
        [SwaggerResponse(404, "Không tìm thấy bài viết hoặc bài viết đã duyệt.")]
        public async Task<IActionResult> DuyetBaiViet(string maBaiViet)
        {
            // Kiểm tra bài viết tồn tại và trạng thái "Chờ Duyệt"
            const string checkQuery = "SELECT COUNT(1) FROM BaiViet WHERE MaBaiViet = @MaBaiViet AND TrangThai = N'Chờ Duyệt'";
            var exists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaBaiViet = maBaiViet });
            
            // Duyệt bài viết
            if (exists == 0)
                return NotFound(new { Message = "❌ Không tìm thấy bài viết hoặc bài viết đã được duyệt." });
            const string updateQuery = "UPDATE BaiViet SET TrangThai = N'Đã Duyệt' WHERE MaBaiViet = @MaBaiViet";
            await _db.ExecuteAsync(updateQuery, new { MaBaiViet = maBaiViet });

            return Ok(new { Message = "✅ Duyệt bài viết thành công." });
        }

        
        /// <summary>
        /// Lấy danh sách bài viết chưa duyệt.
        /// </summary>
        [HttpGet("baiviet/choduyet")]
        [SwaggerOperation(
            Summary = "Danh sách bài viết chờ duyệt",
            Description = "Lấy tất cả bài viết có trạng thái 'Chờ Duyệt'."
        )]
        [SwaggerResponse(200, "Danh sách bài viết chờ duyệt.")]
        public async Task<IActionResult> GetBaiVietChoDuyet()
        {
            const string query = "SELECT * FROM BaiViet WHERE TrangThai = N'Chờ Duyệt' ORDER BY NgayDang DESC";
            var list = await _db.QueryAsync<TatCaBaiVietDTO>(query);
            return Ok(list);
        }

     
        // Xóa 1 tiện nghi
        [HttpDelete("tiennghi/xoa1")]
        [SwaggerOperation(Summary = "Xóa 1 tiện nghi", Description = "Xóa 1 tiện nghi dựa theo mã tiện nghi.")]
        [SwaggerResponse(200, "Xóa tiện nghi thành công.")]
        [SwaggerResponse(404, "Không tìm thấy tiện nghi.")]
        public async Task<IActionResult> Xoa1TienNghi([FromBody] QuanTriVienXoa1TienNghiDTO dto)
        {
            const string checkQuery = "SELECT COUNT(1) FROM TienNghi WHERE MaTienNghi = @MaTienNghi";
            var exists = await _db.ExecuteScalarAsync<int>(checkQuery, new { dto.MaTienNghi });
            
            // Xóa 1 tiện nghi
            if (exists == 0)
                return NotFound(new { Message = "❌ Tiện nghi không tồn tại." });
            const string deleteQuery = "DELETE FROM TienNghi WHERE MaTienNghi = @MaTienNghi";
            await _db.ExecuteAsync(deleteQuery, new { dto.MaTienNghi });
            return Ok(new { Message = "✅ Xóa tiện nghi thành công." });
        }

        // Thêm phòng mới
        [HttpPost("phong/them1phong")]
        [SwaggerOperation(Summary = "Thêm 1 phòng mới", Description = "Thêm một phòng mới vào hệ thống, upload ảnh chính lên Cloudinary.")]
        [SwaggerResponse(200, "Thêm phòng thành công.")]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ hoặc upload ảnh thất bại.")]
        public async Task<IActionResult> Them1Phong([FromForm] QuanTriVienThem1PhongDTO dto, IFormFile? file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Upload ảnh chính lên Cloudinary
            string? imageUrl = null;
            if (file != null && file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Width(1200).Height(800).Crop("limit"),
                    Folder = "phong"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    imageUrl = uploadResult.SecureUrl.ToString();
                else
                    return StatusCode(500, $"Upload ảnh thất bại: {uploadResult.Error?.Message}");
            }
            else
            {
                return BadRequest(new { Message = "Phải upload ảnh chính cho phòng." });
            }

            // Sinh mã phòng tự động theo quy tắc Pxxx
            var maPhong = await GenerateMaPhong();

            const string insertQuery = @"
                INSERT INTO Phong (MaPhong, LoaiPhong, GiaPhong, TinhTrang, Tang, KieuGiuong, MoTa, UrlAnhChinh, SucChua, SoGiuong, DonViTinh, SoSaoTrungBinh)
                VALUES (@MaPhong, @LoaiPhong, @GiaPhong, @TinhTrang, @Tang, @KieuGiuong, @MoTa, @UrlAnhChinh, @SucChua, @SoGiuong, @DonViTinh, @SoSaoTrungBinh)";
            await _db.ExecuteAsync(insertQuery, new
            {
                MaPhong = maPhong,
                LoaiPhong = dto.LoaiPhong,
                GiaPhong = dto.GiaPhong,
                TinhTrang = "1", // Mặc định là 1 khi thêm mới
                Tang = dto.Tang,
                KieuGiuong = dto.KieuGiuong,
                MoTa = dto.MoTa,
                UrlAnhChinh = imageUrl,
                SucChua = dto.SucChua,
                SoGiuong = dto.SoGiuong,
                DonViTinh = "1 ngày",
                SoSaoTrungBinh = 0
            });

            return Ok(new { Message = "🎉 Thêm phòng thành công.", MaPhong = maPhong, UrlAnhChinh = imageUrl });
        }

        // Thêm ảnh cho phòng
        [HttpPost("phong/themanh")]
        [SwaggerOperation(Summary = "Thêm nhiều ảnh cho phòng", Description = "Upload nhiều ảnh lên Cloudinary và lưu vào bảng PhongAnh.")]
        [SwaggerResponse(200, "Thêm ảnh thành công.")]
        [SwaggerResponse(404, "Không tìm thấy phòng.")]
        public async Task<IActionResult> ThemNhieuAnhPhong([FromForm] string maPhong, [FromForm] List<IFormFile> files)
        {
            // Kiểm tra phòng tồn tại
            const string checkPhong = "SELECT COUNT(1) FROM Phong WHERE MaPhong = @MaPhong";
            var exists = await _db.ExecuteScalarAsync<int>(checkPhong, new { MaPhong = maPhong });
            if (exists == 0)
                return NotFound(new { Message = "❌ Không tìm thấy phòng." });

            if (files == null || files.Count == 0)
                return BadRequest(new { Message = "❌ Phải upload ít nhất 1 ảnh." });

            var results = new List<object>();

            foreach (var file in files)
            {
                string? imageUrl = null;
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Width(1200).Height(800).Crop("limit"),
                    Folder = "phong"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    imageUrl = uploadResult.SecureUrl.ToString();
                else
                    return StatusCode(500, $"Upload ảnh thất bại: {uploadResult.Error?.Message}");

                // Sinh mã ảnh tự động
                const string getMaxSql = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaAnh, 3, LEN(MaAnh)-2) AS INT)), 0) + 1 FROM PhongAnh";
                var nextId = await _db.ExecuteScalarAsync<int>(getMaxSql);
                var maAnh = $"PA{nextId:D3}";

                // Lưu vào DB
                const string insertQuery = @"
                    INSERT INTO PhongAnh (MaAnh, MaPhong, UrlAnh)
                    VALUES (@MaAnh, @MaPhong, @UrlAnh)";
                await _db.ExecuteAsync(insertQuery, new
                {
                    MaAnh = maAnh,
                    MaPhong = maPhong,
                    UrlAnh = imageUrl
                });

                results.Add(new { MaAnh = maAnh, UrlAnh = imageUrl });
            }

            // Thêm ảnh cho phòng thành công
            return Ok(new { Message = "✅ Thêm ảnh thành công.", DanhSach = results });
        }

        // Lấy danh sách giảm giá của một phòng
        [HttpGet("phong/{maPhong}/giamgia")]
        [SwaggerOperation(Summary = "Lấy danh sách giảm giá của phòng")]
        public async Task<IActionResult> GetDanhSachGiamGia(string maPhong)
        {
            const string query = @"
                SELECT gg.MaGiamGia, gg.TenGiamGia, gg.GiaTriGiam, gg.NgayBatDau, gg.NgayKetThuc, gg.MoTa
                FROM GiamGia gg
                JOIN Phong_GiamGia pg ON gg.MaGiamGia = pg.MaGiamGia
                WHERE pg.MaPhong = @MaPhong";

            var danhSachGiamGia = await _db.QueryAsync(query, new { MaPhong = maPhong });
            return Ok(new { Message = "✅ Lấy danh sách giảm giá thành công.", Data = danhSachGiamGia });
        }

        // Áp dụng mã giảm giá cho nhiều phòng
        [HttpPost("giamgia/apdung")]
        [SwaggerOperation(Summary = "Áp dụng mã giảm giá cho nhiều phòng", Description = "Quản trị viên áp dụng một mã giảm giá cho nhiều phòng dựa theo danh sách mã phòng.")]
        [SwaggerResponse(200, "Áp dụng giảm giá thành công.")]
        [SwaggerResponse(404, "Không tìm thấy mã giảm giá.")]
        public async Task<IActionResult> ApDungGiamGiaChoNhieuPhong(
            [FromQuery] string maGiamGia,
            [FromBody] List<string> danhSachMaPhong)
        {
            // Kiểm tra mã giảm giá tồn tại
            const string checkGiamGia = "SELECT COUNT(1) FROM GiamGia WHERE MaGiamGia = @MaGiamGia";
            var giamGiaExists = await _db.ExecuteScalarAsync<int>(checkGiamGia, new { MaGiamGia = maGiamGia });
            if (giamGiaExists == 0)
                return NotFound(new { Message = "❌ Không tìm thấy mã giảm giá." });

            var notFoundRooms = new List<string>();
            var existedRooms = new List<string>();
            var successRooms = new List<string>();

            foreach (var maPhong in danhSachMaPhong)
            {
                // Kiểm tra phòng tồn tại
                const string checkPhong = "SELECT COUNT(1) FROM Phong WHERE MaPhong = @MaPhong";
                var phongExists = await _db.ExecuteScalarAsync<int>(checkPhong, new { MaPhong = maPhong });
                if (phongExists == 0)
                {
                    notFoundRooms.Add(maPhong);
                    continue;
                }

                // Kiểm tra đã có mã giảm giá này chưa
                const string checkExist = "SELECT COUNT(1) FROM Phong_GiamGia WHERE MaPhong = @MaPhong AND MaGiamGia = @MaGiamGia";
                var exist = await _db.ExecuteScalarAsync<int>(checkExist, new { MaPhong = maPhong, MaGiamGia = maGiamGia });
                if (exist > 0)
                {
                    existedRooms.Add(maPhong);
                    continue;
                }

                // Thêm vào bảng Phong_GiamGia
                const string insertQuery = "INSERT INTO Phong_GiamGia (MaPhong, MaGiamGia) VALUES (@MaPhong, @MaGiamGia)";
                await _db.ExecuteAsync(insertQuery, new { MaPhong = maPhong, MaGiamGia = maGiamGia });
                successRooms.Add(maPhong);
            }

            return Ok(new
            {
                Message = "🎉 Áp dụng mã giảm giá hoàn tất!",
                ThanhCong = successRooms.Any()
                    ? $"Chúc mừng! Đã áp dụng thành công cho các phòng: {string.Join(", ", successRooms)}."
                    : "Không có phòng nào được áp dụng thành công.",
                PhongKhongTonTai = notFoundRooms.Any()
                    ? $"❌ Các phòng không tồn tại: {string.Join(", ", notFoundRooms)}."
                    : null,
                PhongDaCoMaGiamGia = existedRooms.Any()
                    ? $"⚠️ Các phòng đã có mã giảm giá này: {string.Join(", ", existedRooms)}."
                    : null
            });
        }

        // Sửa trạng thái phòng
        [HttpPatch("phong/{maPhong}/trangthai")]
        [SwaggerOperation(Summary = "Sửa trạng thái phòng", Description = "Cập nhật trạng thái phòng theo mã phòng.")]
        [SwaggerResponse(200, "Cập nhật trạng thái phòng thành công.")]
        [SwaggerResponse(404, "Không tìm thấy phòng.")]
        public async Task<IActionResult> SuaTrangThaiPhong([FromRoute] string maPhong, [FromBody] int trangThai)
        {
            const string checkQuery = "SELECT COUNT(1) FROM Phong WHERE MaPhong = @MaPhong";
            var exists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaPhong = maPhong });
            if (exists == 0)
                return NotFound(new { Message = "❌ Không tìm thấy phòng." });

            const string updateQuery = "UPDATE Phong SET TinhTrang = @TinhTrang WHERE MaPhong = @MaPhong";
            await _db.ExecuteAsync(updateQuery, new { TinhTrang = trangThai, MaPhong = maPhong });

            return Ok(new { Message = "✅ Cập nhật trạng thái phòng thành công.", MaPhong = maPhong, TrangThai = trangThai });
        }

        private async Task<string> GenerateMaPhong()
        {
            const string query = @"SELECT ISNULL(MAX(CAST(SUBSTRING(MaPhong, 2, LEN(MaPhong) - 1) AS INT)), 0) + 1 FROM Phong";
            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"P{nextId:D3}";
        }
    }
}