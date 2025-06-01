using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class KhachHangChiTietHoaDonDTO
    {
        [Required]
        [StringLength(6)]
        public string MaChiTiet { get; set; } = null!;

        [Required]
        [StringLength(6)]
        public string MaHoaDon { get; set; } = null!;

        [Required]
        [StringLength(12)]
        public string LoaiKhoanMuc { get; set; } = null!;

        [Required]
        [StringLength(12)]
        public string MaKhoanMuc { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 1.")]
        public int SoLuong { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Thành tiền phải lớn hơn hoặc bằng 0.")]
        public decimal ThanhTien { get; set; }
    }
}