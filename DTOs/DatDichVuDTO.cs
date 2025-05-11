using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class DatDichVuDTO
    {
        [Required]
        [StringLength(6)]
        public string MaDatDichVu { get; set; } = null!;

        [Required]
        [StringLength(6)]
        public string MaDatPhong { get; set; } = null!;

        [Required]
        [StringLength(6)]
        public string MaDichVu { get; set; } = null!;

        [Required]
        [Range(1, 1000, ErrorMessage = "Số lượng phải nằm trong khoảng từ 1 đến 1000.")]
        public int SoLuong { get; set; }

        [StringLength(6)]
        public string? MaHoaDon { get; set; }
    }
}