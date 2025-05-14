using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class HoaDonDTO
    {
        [Required]
        [StringLength(6)]
        public string MaHoaDon { get; set; } = null!; // Tự động tạo

        [Required]
        [StringLength(6)]
        public string MaNguoiDung { get; set; } = null!; // Bắt buộc

        [Required]
        [StringLength(6)]
        public string MaDatPhong { get; set; } = null!; // Bắt buộc

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền phải lớn hơn hoặc bằng 0.")]
        public decimal TongTien { get; set; } // Bắt buộc

        public DateTime? NgayTaoHoaDon { get; set; } // Tự động tạo

        public DateTime? NgayThanhToan { get; set; } // Tùy chọn

        [Required]
        public string TinhTrangHoaDon { get; set; } = null!; // Bắt buộc
    }
}