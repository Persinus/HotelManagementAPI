using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class GiamGiaDetailDTO
    {
        [StringLength(6)]
        public string MaGiamGia { get; set; } = null!;

        [StringLength(50)]
        public string TenGiamGia { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal GiaTriGiam { get; set; }

        public DateTime NgayBatDau { get; set; }

        public DateTime NgayKetThuc { get; set; }

        [StringLength(255)]
        public string? MoTa { get; set; }
    }
}