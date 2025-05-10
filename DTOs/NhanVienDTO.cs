namespace HotelManagementAPI.DTOs
{
    public class NhanVienDTO
    {
        public string MaNhanVien { get; set; } // Tự động tạo
        public string MaNguoiDung { get; set; } // Tự động tạo
        public string ChucVu { get; set; } // Bắt buộc
        public decimal Luong { get; set; } // Bắt buộc
        public DateTime? NgayVaoLam { get; set; } // Bắt buộc
        public string? CaLamViec { get; set; } // Tùy chọn
        public string Email { get; set; } // Bắt buộc
        public string TenTaiKhoan { get; set; } // Bắt buộc
        public string MatKhau { get; set; } // Bắt buộc
        public string? HoTen { get; set; } // Tùy chọn
        public string? SoDienThoai { get; set; } // Tùy chọn
        public string? DiaChi { get; set; } // Tùy chọn
        public DateTime? NgaySinh { get; set; } // Tùy chọn
        public string? GioiTinh { get; set; } // Tùy chọn
        public string? HinhAnhUrl { get; set; } // Tùy chọn
    }
}