using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using HotelManagementAPI.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;

namespace HotelManagementAPI.Controllers.TatCaXemTatCaXem
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/TatCaTruyCap")]

    public class TatCaTruyCapController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _config;

        public TatCaTruyCapController(
            IDbConnection db,
            IOptions<CloudinarySettings> cloudinaryOptions,
            IConfiguration config)
        {
            _db = db;
            _config = config;
            var settings = cloudinaryOptions.Value;
            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        /// <summary>
        /// Đăng nhập hệ thống.
        /// </summary>
        /// <param name="login">Thông tin đăng nhập</param>
        /// <returns>JWT Token nếu đăng nhập thành công</returns>
        [HttpPost("dangnhap")]
        public async Task<ActionResult<string>> DangNhap([FromBody] LoginDTO login)
        {
            const string query = "SELECT * FROM NguoiDung WHERE TenTaiKhoan = @TenTaiKhoan";
            var nguoiDung = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { login.TenTaiKhoan });

            if (nguoiDung == null)
                return Unauthorized(new { Message = "❌ Xin lỗi, tên tài khoản hoặc mật khẩu không đúng. Vui lòng thử lại." });

            bool isValid = BCrypt.Net.BCrypt.Verify(login.MatKhau, nguoiDung.MatKhau);
            if (!isValid)
                return Unauthorized(new { Message = "❌ Xin lỗi, tên tài khoản hoặc mật khẩu không đúng. Vui lòng thử lại." });

            var secretKey = _config["Jwt:SecretKey"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var nguoiDungModel = new HotelManagementAPI.Models.NguoiDung
            {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                Vaitro = nguoiDung.Vaitro,
                Email = nguoiDung.Email,
                TenTaiKhoan = nguoiDung.TenTaiKhoan,
                MatKhau = nguoiDung.MatKhau,
                HoTen = nguoiDung.HoTen,
                SoDienThoai = nguoiDung.SoDienThoai,
                DiaChi = nguoiDung.DiaChi,
                NgaySinh = nguoiDung.NgaySinh,
                GioiTinh = nguoiDung.GioiTinh,
                HinhAnhUrl = nguoiDung.HinhAnhUrl,
                NgayTao = nguoiDung.NgayTao
            };

            var token = JwtHelper.GenerateJwtToken(nguoiDungModel, secretKey, issuer, audience);

            // Đăng nhập thành công
            return Ok(new { Message = "🎉 Đăng nhập thành công! Chào mừng bạn quay trở lại.", Token = token });
        }

        /// <summary>
        /// Đăng ký khách hàng.
        /// </summary>
        /// <param name="nguoiDung">Thông tin người dùng</param>
        [HttpPost("dangky")]
        public async Task<IActionResult> DangKyKhachHang([FromForm] NguoiDungDangKyDTO dto, IFormFile? file)
            => await DangKyNguoiDungChung(dto, file, "KhachHang");

        /// <summary>
        /// Đăng ký nhân viên.
        /// </summary>
        /// <param name="nguoiDung">Thông tin nhân viên</param>
        [HttpPost("dangky-nhanvien")]
        [AllowAnonymous]
        public async Task<IActionResult> DangKyNhanVien([FromForm] NguoiDungDangKyDTO dto, IFormFile? file)
            => await DangKyNguoiDungChung(dto, file, "NhanVien");

        /// <summary>
        /// Đăng ký quản trị viên.
        /// </summary>
        /// <param name="nguoiDung">Thông tin quản trị viên</param>
        [HttpPost("dangky-quantrivien")]
        [AllowAnonymous]
        public async Task<IActionResult> DangKyQuanTriVien([FromForm] NguoiDungDangKyDTO dto, IFormFile? file)
            => await DangKyNguoiDungChung(dto, file, "QuanTriVien");

        /// <summary>
        /// Đặt lại mật khẩu.
        /// </summary>
        /// <param name="resetPassword">Thông tin đặt lại mật khẩu</param>
        [HttpPut("datlaimatkhau")]
        public async Task<IActionResult> DatLaiMatKhau([FromBody] ResetPasswordDTO resetPassword)
        {
            const string query = "SELECT COUNT(1) FROM NguoiDung WHERE Email = @Email";
            var emailExists = await _db.ExecuteScalarAsync<int>(query, new { resetPassword.Email });

            if (emailExists == 0)
                return NotFound(new { Message = "❌ Xin lỗi, email không tồn tại trong hệ thống." });

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(resetPassword.NewPassword);

            const string updatePasswordQuery = "UPDATE NguoiDung SET MatKhau = @MatKhau WHERE Email = @Email";
            await _db.ExecuteAsync(updatePasswordQuery, new { MatKhau = hashedPassword, resetPassword.Email });

            // Đặt lại mật khẩu thành công
            return Ok(new { Message = "✅ Mật khẩu đã được cập nhật thành công. Chúc bạn sử dụng dịch vụ vui vẻ!" });
        }

        /// <summary>
        /// Lấy tất cả phòng (chi tiết).
        /// </summary>
        [HttpGet("phong")]
        public async Task<ActionResult<IEnumerable<PhongDetailsDTO>>> GetAllPhong()
        {
            const string roomQuery = @"
                SELECT p.MaPhong, p.LoaiPhong, p.GiaPhong, p.TinhTrang, p.SoLuongPhong, p.Tang, 
                       p.KieuGiuong, p.MoTa, p.UrlAnhChinh, p.SucChua, p.SoGiuong, 
                       p.DonViTinh, p.SoSaoTrungBinh
                FROM Phong p";
            var rooms = (await _db.QueryAsync<PhongDetailsDTO>(roomQuery)).ToList();

            foreach (var room in rooms)
            {
                // Lấy danh sách ảnh phụ
                const string imagesQuery = "SELECT UrlAnh FROM PhongAnh WHERE MaPhong = @MaPhong";
                var imageUrls = (await _db.QueryAsync<string>(imagesQuery, new { MaPhong = room.MaPhong })).ToList();
                room.UrlAnhPhu = imageUrls.Select(url => new PhongAnhDTO { UrlAnh = url }).ToList();

                // Lấy danh sách tiện nghi
                const string amenitiesQuery = @"
                    SELECT tn.MaTienNghi, tn.TenTienNghi, tn.MoTa
                    FROM TienNghi tn
                    JOIN Phong_TienNghi ptn ON tn.MaTienNghi = ptn.MaTienNghi
                    WHERE ptn.MaPhong = @MaPhong";
                room.TienNghi = (await _db.QueryAsync<TienNghiDTO>(amenitiesQuery, new { MaPhong = room.MaPhong })).ToList();

                // Lấy danh sách giảm giá
                const string discountsQuery = @"
                    SELECT gg.MaGiamGia, gg.TenGiamGia, gg.GiaTriGiam, gg.NgayBatDau, gg.NgayKetThuc, gg.MoTa
                    FROM GiamGia gg
                    JOIN Phong_GiamGia pg ON gg.MaGiamGia = pg.MaGiamGia
                    WHERE pg.MaPhong = @MaPhong";
                room.GiamGia = (await _db.QueryAsync<GiamGiaDetailDTO>(discountsQuery, new { MaPhong = room.MaPhong })).ToList();

                // Lấy danh sách feedback
                const string feedbackQuery = @"
                    SELECT SoSao, BinhLuan, PhanLoai
                    FROM Feedback
                    WHERE MaPhong = @MaPhong";
                room.Feedbacks = (await _db.QueryAsync<FeedBackDTO>(feedbackQuery, new { MaPhong = room.MaPhong })).ToList();

                // Ảnh phụ
                if (room.UrlAnhPhu == null || !room.UrlAnhPhu.Any())
                {
                    room.UrlAnhPhu = new List<PhongAnhDTO>
                    {
                        new PhongAnhDTO { UrlAnh = "Phòng này chưa có ảnh phụ nào." }
                    };
                }

                // Tiện nghi
                if (room.TienNghi == null || !room.TienNghi.Any())
                {
                    room.TienNghi = new List<TienNghiDTO>
                    {
                        new TienNghiDTO { MaTienNghi = "", TenTienNghi = "Phòng này chưa có tiện nghi nào." }
                    };
                }

                // Giảm giá
                if (room.GiamGia == null || !room.GiamGia.Any())
                {
                    room.GiamGia = new List<GiamGiaDetailDTO>
                    {
                        new GiamGiaDetailDTO { MaGiamGia = "", TenGiamGia = "Phòng này chưa có giảm giá nào." }
                    };
                }

                // Feedback
                if (room.Feedbacks == null || !room.Feedbacks.Any())
                {
                    room.Feedbacks = new List<FeedBackDTO>
                    {
                        new FeedBackDTO { SoSao = 0, BinhLuan = "Phòng này chưa có feedback nào.", PhanLoai = "" }
                    };
                }

                // Tính giá phòng sau giảm giá (nếu có giảm giá)
                if (room.GiamGia != null && room.GiamGia.Any())
                {
                    var giamGia = room.GiamGia.First();
                    room.GiaPhong = room.GiaPhong - (room.GiaPhong * giamGia.GiaTriGiam / 100);
                }
                // Không cần else vì giữ nguyên giá gốc
            }
            // Lay danh sach phong yeu thich
            const string yeuThichQuery = @"
                SELECT MaPhong, MaNguoiDung
                FROM PhongYeuThich";
            var yeuThichList = await _db.QueryAsync<PhongYeuThichDTO>(yeuThichQuery);
            return Ok(rooms);
        }

        /// <summary>
        /// Lấy tất cả tiện nghi.
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
        /// Lấy tất cả dịch vụ.
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
            var dichVuList = (await _db.QueryAsync<DichVuDTO>(query)).ToList();
            var result = dichVuList.Select(dv => new
            {
                dv.MaDichVu,
                dv.TenDichVu,
                dv.DonGia,
                dv.MoTaDichVu,
                dv.HinhAnhDichVu,
                dv.SoLuong,
                dv.LoaiDichVu,
                dv.DonViTinh,
                TrangThai = dv.SoLuong > 0 ? "Còn hàng" : "Hết hàng"
            });
            // Lấy danh sách dịch vụ thành công
            return Ok(new { Message = "✅ Lấy danh sách dịch vụ thành công.", Data = result });
        }

        /// <summary>
        /// Lấy feedback của một phòng.
        /// </summary>
        /// <param name="maPhong">Mã phòng</param>
        [HttpGet("feedback/phong/{maPhong}")]
        [SwaggerOperation(
            Summary = "Lấy feedback của một phòng",
            Description = "Trả về danh sách feedback (số sao, bình luận, phân loại) của một phòng."
        )]
        [SwaggerResponse(200, "Danh sách feedback của phòng.")]
        public async Task<IActionResult> GetFeedbackByPhong([FromRoute] string maPhong)
        {
            const string query = @"
                SELECT SoSao, BinhLuan, PhanLoai
                FROM Feedback
                WHERE MaPhong = @MaPhong";
            var feedbacks = await _db.QueryAsync<FeedBackDTO>(query, new { MaPhong = maPhong });
            // Lấy feedback thành công
            return Ok(new { Message = "✅ Lấy feedback thành công.", Data = feedbacks });
        }

        /// <summary>
        /// Lấy danh sách phòng rút gọn (chỉ các trường cần thiết).
        /// </summary>
        [HttpGet("phong-rutgon")]
        public async Task<ActionResult<IEnumerable<PhongDTO>>> GetAllPhongRutGon([FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(new { Message = "Page và pageSize phải lớn hơn 0." });

            int skip = (page - 1) * pageSize;
            const string phongQuery = @"
                SELECT MaPhong, LoaiPhong, GiaPhong, Tang, TinhTrang, DonViTinh, SoSaoTrungBinh, MoTa, UrlAnhChinh
                FROM Phong
                ORDER BY MaPhong
                OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY";
            var phongList = (await _db.QueryAsync<PhongDTO>(phongQuery, new { Skip = skip, PageSize = pageSize })).ToList();

            if (!phongList.Any())
                return NotFound(new { Message = "❌ Xin lỗi, không tìm thấy phòng nào phù hợp." });

            foreach (var room in phongList)
            {
                // Lấy giảm giá (nếu có)
                const string giamGiaQuery = @"
                    SELECT gg.MaGiamGia, gg.TenGiamGia, gg.GiaTriGiam, gg.NgayBatDau, gg.NgayKetThuc, gg.MoTa
                    FROM GiamGia gg
                    JOIN Phong_GiamGia pg ON gg.MaGiamGia = pg.MaGiamGia
                    WHERE pg.MaPhong = @MaPhong";
                var giamGiaList = (await _db.QueryAsync<GiamGiaDTO>(giamGiaQuery, new { MaPhong = room.MaPhong })).ToList();
                room.GiamGia = giamGiaList;

                // Tính giá ưu đãi (mặc định giảm giá phần trăm)
                decimal giaUuDai = room.GiaPhong;
                if (giamGiaList.Any())
                {
                    var giamGia = giamGiaList.First();
                    giaUuDai = room.GiaPhong - (room.GiaPhong * giamGia.GiaTriGiam / 100);
                }
                room.GiaUuDai = giaUuDai;
            }

            // Lấy danh sách phòng thành công
            return Ok(new { Message = "✅ Lấy danh sách phòng thành công.", Data = phongList });
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
        /// Lấy tất cả mã giảm giá.
        /// </summary>
        [HttpGet("giamgia")]
        [SwaggerOperation(Summary = "Lấy tất cả mã giảm giá", Description = "Trả về danh sách tất cả mã giảm giá trong hệ thống.")]
        [SwaggerResponse(200, "Danh sách mã giảm giá.")]
        public async Task<IActionResult> GetAllGiamGia()
        {
            const string query = @"
                SELECT MaGiamGia, TenGiamGia, GiaTriGiam, NgayBatDau, NgayKetThuc, MoTa
                FROM GiamGia";
            var giamGiaList = await _db.QueryAsync<GiamGiaDTO>(query);
            // Lấy tất cả mã giảm giá thành công
            return Ok(new { Message = "✅ Lấy danh sách mã giảm giá thành công.", Data = giamGiaList });
        }

        private async Task<IActionResult> DangKyNguoiDungChung(NguoiDungDangKyDTO dto, IFormFile? file, string vaitro)
        {
            // Kiểm tra email trùng lặp
            const string checkEmailQuery = "SELECT COUNT(1) FROM NguoiDung WHERE Email = @Email";
            var isEmailDuplicate = await _db.ExecuteScalarAsync<int>(checkEmailQuery, new { dto.Email });
            if (isEmailDuplicate > 0)
                // Đăng ký thất bại do email trùng
                return Conflict(new { Message = "❌ Xin lỗi, email này đã được sử dụng. Vui lòng chọn email khác." });

            // Kiểm tra tên tài khoản trùng lặp
            const string checkTenTaiKhoanQuery = "SELECT COUNT(1) FROM NguoiDung WHERE TenTaiKhoan = @TenTaiKhoan";
            var isTenTaiKhoanDuplicate = await _db.ExecuteScalarAsync<int>(checkTenTaiKhoanQuery, new { dto.TenTaiKhoan });
            if (isTenTaiKhoanDuplicate > 0)
                // Đăng ký thất bại do tên tài khoản trùng
                return Conflict(new { Message = "❌ Xin lỗi, tên đăng nhập đã tồn tại. Vui lòng chọn tên khác." });

            string? imageUrl = null;
            if (file != null && file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Width(400).Height(400).Crop("limit"),
                    Folder = "avatar"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    imageUrl = uploadResult.SecureUrl.ToString();
                else
                    // Upload ảnh thất bại
                    return StatusCode(500, new { Message = $"❌ Xin lỗi, upload ảnh thất bại: {uploadResult.Error?.Message}" });
            }

            var nguoiDung = new NguoiDungDTO
            {
                TenTaiKhoan = dto.TenTaiKhoan,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau),
                HoTen = dto.HoTen,
                SoDienThoai = string.IsNullOrWhiteSpace(dto.SoDienThoai) ? "0123456789" : dto.SoDienThoai,
                DiaChi = string.IsNullOrWhiteSpace(dto.DiaChi) ? "Chưa cập nhật" : dto.DiaChi,
                NgaySinh = dto.NgaySinh ?? new DateTime(2000, 1, 1),
                GioiTinh = string.IsNullOrWhiteSpace(dto.GioiTinh) ? "Không xác định" : dto.GioiTinh,
                Email = dto.Email,
                Vaitro = vaitro,
                MaNguoiDung = await GenerateUniqueMaNguoiDung(),
                NgayTao = DateTime.Now,
                HinhAnhUrl = string.IsNullOrWhiteSpace(imageUrl) ? "https://i.imgur.com/placeholder.png" : imageUrl,
                CanCuocCongDan = string.IsNullOrEmpty(dto.CanCuocCongDan) ? null : SensitiveDataHelper.Encrypt(dto.CanCuocCongDan)
            };

            const string insertQuery = @"
        INSERT INTO NguoiDung (MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen, SoDienThoai, DiaChi, NgaySinh, GioiTinh, HinhAnhUrl, CanCuocCongDan, NgayTao)
        VALUES (@MaNguoiDung, @Vaitro, @Email, @TenTaiKhoan, @MatKhau, @HoTen, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @HinhAnhUrl, @CanCuocCongDan, @NgayTao)";
            await _db.ExecuteAsync(insertQuery, nguoiDung);
            // Đăng ký thành công
            return Ok(new { Message = "🎉 Đăng ký thành công! Chúc mừng bạn đã trở thành thành viên của hệ thống.", MaNguoiDung = nguoiDung.MaNguoiDung });
        }

        // Helper tạo mã người dùng duy nhất
        private async Task<string> GenerateUniqueMaNguoiDung()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaNguoiDung, 3, LEN(MaNguoiDung) - 2) AS INT)), 0) + 1
                FROM NguoiDung";

            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"ND{nextId:D3}";
        }
    }
}
