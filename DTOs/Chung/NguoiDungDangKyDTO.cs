using System;

namespace HotelManagementAPI.DTOs
{
    public class NguoiDungDangKyDTO
    {
        public string TenTaiKhoan { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public string SoDienThoai { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public string Email { get; set; } = null!;
        public string? CanCuocCongDan { get; set; }
        public string? TrangThai { get; set; }

        // Không cần HinhAnhUrl vì sẽ upload qua Cloudinary và gán ở controller
    }
}