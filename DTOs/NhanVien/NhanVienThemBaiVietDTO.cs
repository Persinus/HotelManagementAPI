using System;
using System.Text.Json.Serialization;

namespace HotelManagementAPI.DTOs.NhanVien
{
    public class NhanVienThemBaiVietDTO
    {
        public string TieuDe { get; set; } = null!;
        public string NoiDung { get; set; } = null!;
        // KHÔNG có TrangThai
    }
}