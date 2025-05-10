namespace HotelManagementAPI.DTOs
{
    public class LichSuGiaoDichDTO
    {
        public string MaGiaoDich { get; set; } // Tự động tạo
        public string MaNguoiDung { get; set; } // Bắt buộc
        public string LoaiGiaoDich { get; set; } // Bắt buộc
        public DateTime? ThoiGianGiaoDich { get; set; } // Tự động tạo
        public string? MoTa { get; set; } // Tùy chọn
    }
}