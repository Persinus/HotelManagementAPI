using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class KhachHangLichSuGiaoDichDTO
    {
        [Required]
        [StringLength(6)]
        public string MaGiaoDich { get; set; } = null!; // Tự động tạo

        [Required]
        [StringLength(6)]
        public string MaNguoiDung { get; set; } = null!; // Bắt buộc

        [Required]
        public string LoaiGiaoDich { get; set; } = null!; // Bắt buộc

        public DateTime? ThoiGianGiaoDich { get; set; } // Tự động tạo

        public string? MoTa { get; set; } // Tùy chọn
    }
}