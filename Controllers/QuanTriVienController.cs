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

namespace HotelManagementAPI.Controllers.QuanTriVien
{
    [ApiController]
    [Route("api/quantrivien")]
    [Authorize(Roles = "QuanTriVien")]
    public class QuanTriVienController : ControllerBase
    {
        private readonly IDbConnection _db;

        public QuanTriVienController(IDbConnection db)
        {
            _db = db;
        }
   
        // Dashboard
        [HttpGet("dashboard")]
        
        public IActionResult GetDashboard()
        {
            return Ok(new { Message = "Đây là dashboard của Quản trị viên." });
        }

        // Đăng ký tài khoản quản trị viên
        [HttpPost("dangky")]
        public async Task<ActionResult<NguoiDungDTO>> DangKyQuanTriVien([FromBody] NguoiDungDTO nguoiDung)
        {
            nguoiDung.Vaitro = "QuanTriVien";
            nguoiDung.MaNguoiDung = await GenerateUniqueMaNguoiDung();
            nguoiDung.NgayTao = DateTime.Now;

            const string checkEmailQuery = "SELECT COUNT(1) FROM NguoiDung WHERE Email = @Email";
            var isEmailDuplicate = await _db.ExecuteScalarAsync<int>(checkEmailQuery, new { nguoiDung.Email });

            if (isEmailDuplicate > 0)
            {
                return Conflict(new { Message = "Email đã tồn tại. Vui lòng sử dụng email khác." });
            }

            const string insertQuery = @"
                INSERT INTO NguoiDung (MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen, SoDienThoai, DiaChi, NgaySinh, GioiTinh, HinhAnhUrl, CanCuocCongDan, NgayTao)
                VALUES (@MaNguoiDung, @Vaitro, @Email, @TenTaiKhoan, @MatKhau, @HoTen, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @HinhAnhUrl, @CanCuocCongDan, @NgayTao)";
            await _db.ExecuteAsync(insertQuery, nguoiDung);

            return CreatedAtAction(nameof(DangKyQuanTriVien), new { id = nguoiDung.MaNguoiDung }, nguoiDung);
        }

        // Lấy profile quản trị viên
        [HttpGet("profile")]
        public async Task<ActionResult<NguoiDungDTO>> GetProfile()
        {
            var maNguoiDung = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(maNguoiDung))
                return Unauthorized(new { Message = "Không tìm thấy thông tin người dùng trong token." });

            const string query = "SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var nguoiDung = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { MaNguoiDung = maNguoiDung });

            if (nguoiDung == null)
                return NotFound(new { Message = "Không tìm thấy thông tin người dùng." });

            return Ok(nguoiDung);
        }

        // Lấy danh sách tất cả khách hàng
        [HttpGet("khachhang")]
        public async Task<ActionResult<IEnumerable<NguoiDungDTO>>> GetAllKhachHang()
        {
            const string query = "SELECT * FROM NguoiDung WHERE Vaitro = 'KhachHang'";
            var khachHangList = await _db.QueryAsync<NguoiDungDTO>(query);
            return Ok(khachHangList);
        }

        // Lấy danh sách tất cả nhân viên
        [HttpGet("nhanvien")]
        public async Task<ActionResult<IEnumerable<NguoiDungDTO>>> GetAllNhanVien()
        {
            const string query = "SELECT * FROM NguoiDung WHERE Vaitro = 'NhanVien'";
            var nhanVienList = await _db.QueryAsync<NguoiDungDTO>(query);
            return Ok(nhanVienList);
        }

        // Lấy thông tin chi tiết của một khách hàng
        [HttpGet("khachhang/{maNguoiDung}")]
        public async Task<ActionResult<NguoiDungDTO>> GetKhachHangByMaNguoiDung(string maNguoiDung)
        {
            const string query = "SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung AND Vaitro = 'KhachHang'";
            var khachHang = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { MaNguoiDung = maNguoiDung });

            if (khachHang == null)
                return NotFound(new { Message = "Không tìm thấy thông tin khách hàng." });

            return Ok(khachHang);
        }

        // Lấy thông tin chi tiết của một nhân viên
        [HttpGet("nhanvien/{maNguoiDung}")]
        public async Task<ActionResult<NguoiDungDTO>> GetNhanVienByMaNguoiDung(string maNguoiDung)
        {
            const string query = "SELECT * FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung AND Vaitro = 'NhanVien'";
            var nhanVien = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { MaNguoiDung = maNguoiDung });

            if (nhanVien == null)
                return NotFound(new { Message = "Không tìm thấy thông tin nhân viên." });

            return Ok(nhanVien);
        }

        // Thêm dịch vụ mới (Mã dịch vụ tự sinh)
        [HttpPost("dichvu/themdichvu")]
        public async Task<IActionResult> ThemDichVu([FromBody] DichVuDTO dichVuDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Tạo mã dịch vụ tự động
            const string generateMaDichVuQuery = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaDichVu, 3, LEN(MaDichVu) - 2) AS INT)), 0) + 1
                FROM DichVu";
            var nextId = await _db.ExecuteScalarAsync<int>(generateMaDichVuQuery);
            dichVuDTO.MaDichVu = $"DV{nextId:D3}";

            // Thêm dịch vụ vào cơ sở dữ liệu
            const string insertQuery = @"
                INSERT INTO DichVu (MaDichVu, TenDichVu, DonGia, MoTaDichVu, HinhAnhDichVu, SoLuong, LoaiDichVu, DonViTinh)
                VALUES (@MaDichVu, @TenDichVu, @DonGia, @MoTaDichVu, @HinhAnhDichVu, @SoLuong, @LoaiDichVu, @DonViTinh)";
            await _db.ExecuteAsync(insertQuery, dichVuDTO);

            return Ok(new { Message = "Thêm dịch vụ thành công.", MaDichVu = dichVuDTO.MaDichVu });
        }

        // Xem vai trò hiện tại của người dùng
        [HttpGet("hethong/nguoidung/{maNguoiDung}/vaitro")]
        [SwaggerOperation(Summary = "Xem vai trò người dùng")]
        public async Task<IActionResult> XemVaiTroNguoiDung([FromRoute] string maNguoiDung)
        {
            const string query = "SELECT Vaitro FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var vaiTro = await _db.ExecuteScalarAsync<string>(query, new { MaNguoiDung = maNguoiDung });
            if (vaiTro == null)
                return NotFound(new { Message = "Không tìm thấy người dùng." });
            return Ok(new { MaNguoiDung = maNguoiDung, VaiTro = vaiTro });
        }

        // Đổi vai trò người dùng (chỉ cho phép giữa Nhân viên và Quản trị viên)
        [HttpPut("hethong/nguoidung/{maNguoiDung}/doivaitro")]
        [SwaggerOperation(Summary = "Đổi vai trò người dùng", Description = "Chỉ đổi giữa Nhân viên và Quản trị viên")]
        public async Task<IActionResult> DoiVaiTroNguoiDung(
            [FromRoute] string maNguoiDung,
            [FromBody] QuanTriVienSuaRoleDTO dto)
        {
            // Kiểm tra tồn tại
            const string checkQuery = "SELECT COUNT(1) FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
            var exists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaNguoiDung = maNguoiDung });
            if (exists == 0)
                return NotFound(new { Message = "Không tìm thấy người dùng." });

            // Chỉ cho phép đổi sang "NhanVien" hoặc "QuanTriVien"
            if (dto.VaiTroMoi != "NhanVien" && dto.VaiTroMoi != "QuanTriVien")
                return BadRequest(new { Message = "Chỉ được đổi sang 'NhanVien' hoặc 'QuanTriVien'." });

            const string updateQuery = "UPDATE NguoiDung SET Vaitro = @VaiTroMoi WHERE MaNguoiDung = @MaNguoiDung";
            await _db.ExecuteAsync(updateQuery, new { VaiTroMoi = dto.VaiTroMoi, MaNguoiDung = maNguoiDung });

            return Ok(new { Message = $"Đã đổi vai trò thành công cho người dùng {maNguoiDung} thành {dto.VaiTroMoi}" });
        }

        // Thêm phòng mới
        [HttpPost("phong")]
        public async Task<IActionResult> ThemPhong([FromBody] PhongDetailsDTO phongDetailsDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Sinh mã phòng tự động dạng P001, P002, ...
            var maPhong = await GenerateMaPhong();

            const string insertQuery = @"
                INSERT INTO Phong (MaPhong, LoaiPhong, GiaPhong, TinhTrang, SoLuongPhong, Tang, KieuGiuong, MoTa, UrlAnhChinh, SucChua, SoGiuong, DonViTinh, SoSaoTrungBinh)
                VALUES (@MaPhong, @LoaiPhong, @GiaPhong, @TinhTrang, @SoLuongPhong, @Tang, @KieuGiuong, @MoTa, @UrlAnhChinh, @SucChua, @SoGiuong, @DonViTinh, @SoSaoTrungBinh)";
            await _db.ExecuteAsync(insertQuery, new
            {
                MaPhong = maPhong,
                phongDetailsDTO.LoaiPhong,
                phongDetailsDTO.GiaPhong,
                phongDetailsDTO.TinhTrang,
                phongDetailsDTO.SoLuongPhong,
                phongDetailsDTO.Tang,
                phongDetailsDTO.KieuGiuong,
                phongDetailsDTO.MoTa,
                phongDetailsDTO.UrlAnhChinh,
                phongDetailsDTO.SucChua,
                phongDetailsDTO.SoGiuong,
                phongDetailsDTO.DonViTinh,
                phongDetailsDTO.SoSaoTrungBinh
            });

            return Ok(new { Message = "Thêm phòng thành công.", MaPhong = maPhong });
        }

        // Sửa thông tin phòng
        [HttpPut("phong/{maPhong}")]
        public async Task<IActionResult> CapNhatPhong(string maPhong, [FromBody] PhongDetailsDTO phongDetailsDTO)
        {
            const string checkQuery = "SELECT COUNT(1) FROM Phong WHERE MaPhong = @MaPhong";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaPhong = maPhong });

            if (isExists == 0)
                return NotFound(new { Message = "Phòng không tồn tại." });

            const string updateQuery = @"
                UPDATE Phong
                SET LoaiPhong = @LoaiPhong, GiaPhong = @GiaPhong, TinhTrang = @TinhTrang, SoLuongPhong = @SoLuongPhong,
                    Tang = @Tang, KieuGiuong = @KieuGiuong, MoTa = @MoTa, UrlAnhChinh = @UrlAnhChinh, SucChua = @SucChua,
                    SoGiuong = @SoGiuong, DonViTinh = @DonViTinh, SoSaoTrungBinh = @SoSaoTrungBinh
                WHERE MaPhong = @MaPhong";
            await _db.ExecuteAsync(updateQuery, new
            {
                MaPhong = maPhong,
                phongDetailsDTO.LoaiPhong,
                phongDetailsDTO.GiaPhong,
                phongDetailsDTO.TinhTrang,
                phongDetailsDTO.SoLuongPhong,
                phongDetailsDTO.Tang,
                phongDetailsDTO.KieuGiuong,
                phongDetailsDTO.MoTa,
                phongDetailsDTO.UrlAnhChinh,
                phongDetailsDTO.SucChua,
                phongDetailsDTO.SoGiuong,
                phongDetailsDTO.DonViTinh,
                phongDetailsDTO.SoSaoTrungBinh
            });

            return Ok(new { Message = "Cập nhật phòng thành công." });
        }

        // Xóa phòng
        [HttpDelete("phong/{maPhong}")]
        public async Task<IActionResult> XoaPhong(string maPhong)
        {
            const string checkQuery = "SELECT COUNT(1) FROM Phong WHERE MaPhong = @MaPhong";
            var isExists = await _db.ExecuteScalarAsync<int>(checkQuery, new { MaPhong = maPhong });

            if (isExists == 0)
                return NotFound(new { Message = "Phòng không tồn tại." });

            const string deleteQuery = "DELETE FROM Phong WHERE MaPhong = @MaPhong";
            await _db.ExecuteAsync(deleteQuery, new { MaPhong = maPhong });

            return Ok(new { Message = "Xóa phòng thành công." });
        }

        // Xóa 1 phòng
        [HttpDelete("phong/xoa1")]
        [SwaggerOperation(Summary = "Xóa 1 phòng", Description = "Xóa 1 phòng dựa theo mã phòng.")]
        [SwaggerResponse(200, "Xóa phòng thành công.")]
        [SwaggerResponse(404, "Không tìm thấy phòng.")]
        public async Task<IActionResult> Xoa1Phong([FromBody] QuanTriVienXoa1PhongDTO dto)
        {
            const string checkQuery = "SELECT COUNT(1) FROM Phong WHERE MaPhong = @MaPhong";
            var exists = await _db.ExecuteScalarAsync<int>(checkQuery, new { dto.MaPhong });
            if (exists == 0)
                return NotFound(new { Message = "Phòng không tồn tại." });

            const string deleteQuery = "DELETE FROM Phong WHERE MaPhong = @MaPhong";
            await _db.ExecuteAsync(deleteQuery, new { dto.MaPhong });
            return Ok(new { Message = "Xóa phòng thành công." });
        }

        // Hàm sinh mã người dùng tự động
        private async Task<string> GenerateUniqueMaNguoiDung()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaNguoiDung, 3, LEN(MaNguoiDung) - 2) AS INT)), 0) + 1
                FROM NguoiDung";

            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"ND{nextId:D3}";
        }

        // Hàm sinh mã phòng tự động
        private async Task<string> GenerateMaPhong()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaPhong, 2, LEN(MaPhong)-1) AS INT)), 0) + 1
                FROM Phong";
            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"P{nextId:D3}";
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
   
        public async Task<IActionResult> Them1DichVu([FromForm] QuanTriVienThem1DichVuDTO dto, IFormFile? file, [FromServices] Cloudinary cloudinary)
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
                var uploadResult = await cloudinary.UploadAsync(uploadParams);
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

            return Ok(new { Message = "Thêm dịch vụ thành công.", MaDichVu = maDichVu, HinhAnhUrl = imageUrl });
        }

        /// <summary>
        /// Thêm nhiều dịch vụ mới (có ảnh).
        /// </summary>
        [HttpPost("dichvu/themnhieuDichvu")]
        [SwaggerOperation(
            Summary = "Thêm nhiều dịch vụ mới",
            Description = "Cho phép thêm nhiều dịch vụ cùng lúc với ảnh đi kèm cho mỗi dịch vụ."
        )]
        [SwaggerResponse(200, "Thêm nhiều dịch vụ thành công.")]
        [SwaggerResponse(400, "Số lượng file ảnh phải bằng số lượng dịch vụ hoặc upload ảnh thất bại.")]
      
        public async Task<IActionResult> ThemNhieuDichVu([FromForm] QuanTriVienThemNhieuDichVuDTO dto, List<IFormFile> files, [FromServices] Cloudinary cloudinary)
        {
            if (dto.DanhSachDichVu.Count != files.Count)
                return BadRequest("Số lượng file ảnh phải bằng số lượng dịch vụ.");

            var results = new List<object>();

            for (int i = 0; i < dto.DanhSachDichVu.Count; i++)
            {
                var dichVu = dto.DanhSachDichVu[i];
                var file = files[i];
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
                    var uploadResult = await cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        imageUrl = uploadResult.SecureUrl.ToString();
                    else
                        return StatusCode(500, $"Upload ảnh thất bại cho dịch vụ thứ {i + 1}: {uploadResult.Error?.Message}");
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
                    dichVu.TenDichVu,
                    dichVu.DonGia,
                    dichVu.MoTaDichVu,
                    HinhAnhDichVu = imageUrl,
                    dichVu.SoLuong,
                    dichVu.LoaiDichVu,
                    dichVu.DonViTinh
                });

                results.Add(new { MaDichVu = maDichVu, HinhAnhUrl = imageUrl });
            }

            return Ok(new { Message = "Thêm nhiều dịch vụ thành công.", DanhSach = results });
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
  
        public async Task<IActionResult> Them1TienNghi([FromForm] QuanTriVienThem1TienNghiDTO dto, IFormFile? file, [FromServices] Cloudinary cloudinary)
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
                var uploadResult = await cloudinary.UploadAsync(uploadParams);
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
                dto.TenTienNghi,
                dto.MoTa,
                HinhAnhTienNghi = imageUrl
            });

            return Ok(new { Message = "Thêm tiện nghi thành công.", MaTienNghi = maTienNghi, HinhAnhUrl = imageUrl });
        }

        /// <summary>
        /// Thêm nhiều tiện nghi mới (có ảnh).
        /// </summary>
        [HttpPost("tiennghi/themnhiutiennghi")]
        [SwaggerOperation(
            Summary = "Thêm nhiều tiện nghi mới",
            Description = "Cho phép thêm nhiều tiện nghi cùng lúc với ảnh đi kèm cho mỗi tiện nghi."
        )]
        [SwaggerResponse(200, "Thêm nhiều tiện nghi thành công.")]
        [SwaggerResponse(400, "Số lượng file ảnh phải bằng số lượng tiện nghi hoặc upload ảnh thất bại.")]

        public async Task<IActionResult> ThemNhieuTienNghi([FromForm] QuanTriVienThemNhieuTienNghiDTO dto, List<IFormFile> files, [FromServices] Cloudinary cloudinary)
        {
            if (dto.DanhSachTienNghi.Count != files.Count)
                return BadRequest("Số lượng file ảnh phải bằng số lượng tiện nghi.");

            var results = new List<object>();

            for (int i = 0; i < dto.DanhSachTienNghi.Count; i++)
            {
                var tienNghi = dto.DanhSachTienNghi[i];
                var file = files[i];
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
                    var uploadResult = await cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        imageUrl = uploadResult.SecureUrl.ToString();
                    else
                        return StatusCode(500, $"Upload ảnh thất bại cho tiện nghi thứ {i + 1}: {uploadResult.Error?.Message}");
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
                    tienNghi.TenTienNghi,
                    tienNghi.MoTa,
                    HinhAnhTienNghi = imageUrl
                });

                results.Add(new { MaTienNghi = maTienNghi, HinhAnhUrl = imageUrl });
            }

            return Ok(new { Message = "Thêm nhiều tiện nghi thành công.", DanhSach = results });
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
            if (exists == 0)
                return NotFound(new { Message = "Không tìm thấy bài viết hoặc bài viết đã được duyệt." });

            // Cập nhật trạng thái
            const string updateQuery = "UPDATE BaiViet SET TrangThai = N'Đã Duyệt' WHERE MaBaiViet = @MaBaiViet";
            await _db.ExecuteAsync(updateQuery, new { MaBaiViet = maBaiViet });

            return Ok(new { Message = "Duyệt bài viết thành công." });
        }

        /// <summary>
        /// Lấy danh sách bài viết đã duyệt.
        /// </summary>
        [HttpGet("baiviet/daduyet")]
        [SwaggerOperation(
            Summary = "Danh sách bài viết đã duyệt",
            Description = "Lấy tất cả bài viết có trạng thái 'Đã Duyệt'."
        )]
        [SwaggerResponse(200, "Danh sách bài viết đã duyệt.")]
        public async Task<IActionResult> GetBaiVietDaDuyet()
        {
            const string query = "SELECT * FROM BaiViet WHERE TrangThai = N'Đã Duyệt' ORDER BY NgayDang DESC";
            var list = await _db.QueryAsync<TatCaBaiVietDTO>(query);
            return Ok(list);
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

        // Xóa nhiều tiện nghi
        [HttpDelete("tiennghi/xoanhiu")]
        [SwaggerOperation(Summary = "Xóa nhiều tiện nghi", Description = "Xóa nhiều tiện nghi dựa theo danh sách mã tiện nghi.")]
        [SwaggerResponse(200, "Xóa nhiều tiện nghi thành công.")]
        public async Task<IActionResult> XoaNhieuTienNghi([FromBody] QuanTriVienXoaNhieuTienNghiDTO dto)
        {
            const string deleteQuery = "DELETE FROM TienNghi WHERE MaTienNghi = @MaTienNghi";
            foreach (var ma in dto.DanhSachMaTienNghi)
            {
                await _db.ExecuteAsync(deleteQuery, new { MaTienNghi = ma });
            }
            return Ok(new { Message = "Xóa nhiều tiện nghi thành công." });
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
            if (exists == 0)
                return NotFound(new { Message = "Tiện nghi không tồn tại." });

            const string deleteQuery = "DELETE FROM TienNghi WHERE MaTienNghi = @MaTienNghi";
            await _db.ExecuteAsync(deleteQuery, new { dto.MaTienNghi });
            return Ok(new { Message = "Xóa tiện nghi thành công." });
        }
    }
}