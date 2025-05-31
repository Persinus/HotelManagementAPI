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

namespace HotelManagementAPI.Controllers.AnDanh
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/andanh")]
    public class AnDanhController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly IConfiguration _config;

        public AnDanhController(IDbConnection db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // Đăng nhập
        [HttpPost("dangnhap")]
        public async Task<ActionResult<string>> DangNhap([FromBody] LoginDTO login)
        {
            const string query = "SELECT * FROM NguoiDung WHERE TenTaiKhoan = @TenTaiKhoan";
            var nguoiDung = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { login.TenTaiKhoan });

            if (nguoiDung == null)
                return Unauthorized(new { Message = "Tên tài khoản hoặc mật khẩu không đúng 1." });

            bool isValid = BCrypt.Net.BCrypt.Verify(login.MatKhau, nguoiDung.MatKhau);
            if (!isValid)
                return Unauthorized(new { Message = "Tên tài khoản hoặc mật khẩu không đúng 2." });

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

            return Ok(new { Token = token });
        }

        // Đăng ký
        [HttpPost("dangky")]
        public async Task<ActionResult<NguoiDungDTO>> DangKyNguoiDung([FromBody] NguoiDungDTO nguoiDung)
        {
            nguoiDung.Vaitro = "KhachHang";
            nguoiDung.MaNguoiDung = await GenerateUniqueMaNguoiDung();
            nguoiDung.NgayTao = DateTime.Now;

            // Kiểm tra email trùng lặp
            const string checkEmailQuery = "SELECT COUNT(1) FROM NguoiDung WHERE Email = @Email";
            var isEmailDuplicate = await _db.ExecuteScalarAsync<int>(checkEmailQuery, new { nguoiDung.Email });

            if (isEmailDuplicate > 0)
                return Conflict(new { Message = "Email đã tồn tại. Vui lòng sử dụng email khác." });

            // Kiểm tra tên tài khoản trùng lặp
            const string checkTenTaiKhoanQuery = "SELECT COUNT(1) FROM NguoiDung WHERE TenTaiKhoan = @TenTaiKhoan";
            var isTenTaiKhoanDuplicate = await _db.ExecuteScalarAsync<int>(checkTenTaiKhoanQuery, new { nguoiDung.TenTaiKhoan });

            if (isTenTaiKhoanDuplicate > 0)
                return Conflict(new { Message = "Tên đăng nhập đã có người sử dụng. Vui lòng chọn tên đăng nhập khác." });

            // Mã hóa CCCD trước khi lưu
            if (!string.IsNullOrEmpty(nguoiDung.CanCuocCongDan))
                nguoiDung.CanCuocCongDan = SensitiveDataHelper.Encrypt(nguoiDung.CanCuocCongDan);

            // Mã hóa mật khẩu
            nguoiDung.MatKhau = BCrypt.Net.BCrypt.HashPassword(nguoiDung.MatKhau);

            // Thêm người dùng mới
            const string insertQuery = @"
                INSERT INTO NguoiDung (MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen, SoDienThoai, DiaChi, NgaySinh, GioiTinh, HinhAnhUrl, CanCuocCongDan, NgayTao)
                VALUES (@MaNguoiDung, @Vaitro, @Email, @TenTaiKhoan, @MatKhau, @HoTen, @SoDienThoai, @DiaChi, @NgaySinh, @GioiTinh, @HinhAnhUrl, @CanCuocCongDan, @NgayTao)";
            await _db.ExecuteAsync(insertQuery, nguoiDung);

            return CreatedAtAction(nameof(DangKyNguoiDung), new { id = nguoiDung.MaNguoiDung }, nguoiDung);
        }

        // Đặt lại mật khẩu
        [HttpPut("datlaimatkhau")]
        public async Task<IActionResult> DatLaiMatKhau([FromBody] ResetPasswordDTO resetPassword)
        {
            const string query = "SELECT COUNT(1) FROM NguoiDung WHERE Email = @Email";
            var emailExists = await _db.ExecuteScalarAsync<int>(query, new { resetPassword.Email });

            if (emailExists == 0)
                return NotFound(new { Message = "Email không tồn tại trong hệ thống." });

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(resetPassword.NewPassword);

            const string updatePasswordQuery = "UPDATE NguoiDung SET MatKhau = @MatKhau WHERE Email = @Email";
            await _db.ExecuteAsync(updatePasswordQuery, new { MatKhau = hashedPassword, resetPassword.Email });

            return Ok(new { Message = "Mật khẩu đã được cập nhật thành công." });
        }

        // Xem tất cả phòng
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
                    SELECT gg.MaGiamGia, gg.TenGiamGia, gg.LoaiGiamGia, gg.GiaTriGiam, gg.NgayBatDau, gg.NgayKetThuc, gg.MoTa
                    FROM GiamGia gg
                    JOIN Phong_GiamGia pg ON gg.MaGiamGia = pg.MaGiamGia
                    WHERE pg.MaPhong = @MaPhong";
                room.GiamGia = (await _db.QueryAsync<GiamGiaDTO>(discountsQuery, new { MaPhong = room.MaPhong })).ToList();

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
                    room.GiamGia = new List<GiamGiaDTO>
                    {
                        new GiamGiaDTO { MaGiamGia = "", TenGiamGia = "Phòng này chưa có giảm giá nào." }
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
                    if (giamGia.LoaiGiamGia?.ToLower() == "phantram")
                    {
                        room.GiaPhongSauGiam = room.GiaPhong - (room.GiaPhong * giamGia.GiaTriGiam / 100);
                    }
                    else if (giamGia.LoaiGiamGia?.ToLower() == "trutien")
                    {
                        room.GiaPhongSauGiam = room.GiaPhong - giamGia.GiaTriGiam;
                    }
                    else
                    {
                        room.GiaPhongSauGiam = room.GiaPhong;
                    }
                }
                else
                {
                    room.GiaPhongSauGiam = room.GiaPhong;
                }
            }

            return Ok(rooms);
        }

        // Lấy tất cả tiện nghi
        [HttpGet("tiennghi")]
        public async Task<IActionResult> GetAllTienNghi()
        {
            const string query = @"
                SELECT MaTienNghi, TenTienNghi, MoTa
                FROM TienNghi";
            var tienNghiList = await _db.QueryAsync<TienNghiDTO>(query);
            return Ok(tienNghiList);
        }

        // Lấy tất cả dịch vụ
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
            var dichVuList = await _db.QueryAsync<DichVuDTO>(query);
            return Ok(dichVuList);
        }

        // Lấy feedback của một phòng
        [HttpGet("feedback/phong/{maPhong}")]
        public async Task<IActionResult> GetFeedbackByPhong(string maPhong)
        {
            const string query = @"
                SELECT SoSao, BinhLuan, PhanLoai
                FROM Feedback
                WHERE MaPhong = @MaPhong";
            var feedbacks = await _db.QueryAsync<FeedBackDTO>(query, new { MaPhong = maPhong });
            return Ok(feedbacks);
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