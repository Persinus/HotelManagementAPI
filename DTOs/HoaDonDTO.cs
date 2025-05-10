namespace HotelManagementAPI.DTOs
{
    public class HoaDonDTO
    {
        public string MaHoaDon { get; set; } // Tự động tạo
        public string MaNguoiDung { get; set; } // Bắt buộc
        public string MaDatPhong { get; set; } // Bắt buộc
        public decimal TongTien { get; set; } // Bắt buộc
        public DateTime? NgayTaoHoaDon { get; set; } // Tự động tạo
        public DateTime? NgayThanhToan { get; set; } // Tùy chọn
        public string TinhTrangHoaDon { get; set; } // Bắt buộc
    }
}