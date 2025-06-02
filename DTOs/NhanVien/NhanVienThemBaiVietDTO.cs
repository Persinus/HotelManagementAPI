using System;

namespace HotelManagementAPI.DTOs.NhanVien
{
    public class NhanVienThemBaiVietDTO
    {
        public string MaNguoiDung { get; set; } = null!;
        public string TieuDe { get; set; } = null!;
        public string NoiDung { get; set; } = null!;
        public string? HinhAnhUrl { get; set; }
        public string? TrangThai { get; set; }
    }
}