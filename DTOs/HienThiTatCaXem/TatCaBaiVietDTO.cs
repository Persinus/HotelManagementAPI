using System;

namespace HotelManagementAPI.DTOs
{
    public class TatCaBaiVietDTO
    {
        public string MaBaiViet { get; set; } = null!;
        public string MaNguoiDung { get; set; } = null!;
        public string TieuDe { get; set; } = null!;
        public string NoiDung { get; set; } = null!;
        public DateTime? NgayDang { get; set; }
        public string? HinhAnhUrl { get; set; }
        public string? TrangThai { get; set; }
        
        // Nếu muốn show tên người đăng bài, có thể thêm:
        // public string? TenNguoiDung { get; set; }
    }
}