using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class KhachHangThanhToanDTO
    {
        [Required]
        [StringLength(6)]
        public string MaThanhToan { get; set; } = null!;

        [Required]
        [StringLength(6)]
        public string MaHoaDon { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền thanh toán phải lớn hơn hoặc bằng 0.")]
        public decimal SoTienThanhToan { get; set; }

        [Required]
        public DateTime? NgayThanhToan { get; set; }

        [Required]
        public string PhuongThucThanhToan { get; set; } = null!;

        [Required]
   
        public string TinhTrangThanhToan { get; set; } = null!;
    }}