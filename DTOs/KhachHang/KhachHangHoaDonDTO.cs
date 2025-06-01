using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelManagementAPI.DTOs
{
    public class KhachHangHoaDonDTO
    {
      
        public string MaHoaDon { get; set; } = null!; // Tự động tạo

       
        public string MaNguoiDung { get; set; } = null!; // Bắt buộc

      
        public string MaDatPhong { get; set; } = null!; // Bắt buộc

        public decimal TongTien { get; set; } // Bắt buộc

        [JsonIgnore]
        public DateTime? NgayTaoHoaDon { get; set; } // Tự động tạo

        public DateTime? NgayThanhToan { get; set; } // Tùy chọn

        [Required]
        public byte TinhTrangHoaDon { get; set; } // 1: Chưa thanh toán, 2: Đã thanh toán
    }
}