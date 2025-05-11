using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class PhongAnhDTO
    {
        [Required]
        [StringLength(6)]
        public string MaAnh { get; set; } = null!;

        [Required]
        [StringLength(6)]
        public string MaPhong { get; set; } = null!;

        [Required]
        public string UrlAnh { get; set; } = null!;
    }
}