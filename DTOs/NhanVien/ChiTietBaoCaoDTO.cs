using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.NhanVien
{
    public class ChiTietBaoCaoDTO
    {
        [Required]
        [StringLength(6)]
        public string MaChiTiet { get; set; } = null!;

        [Required]
        [StringLength(6)]
        public string MaBaoCao { get; set; } = null!;

        [Required]
        public string NoiDungChiTiet { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị phải lớn hơn hoặc bằng 0.")]
        public decimal GiaTri { get; set; }
    }
}