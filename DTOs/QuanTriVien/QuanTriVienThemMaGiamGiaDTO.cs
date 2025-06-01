using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienThemMaGiamGiaDTO
    {
        [Required]
        [StringLength(50)]
        public string TenGiamGia { get; set; } = null!;

        [Required]
        [StringLength(6)]
        public string LoaiGiamGia { get; set; } = null!; // Ví dụ: "phantram" hoặc "trutien"

        [Required]
        [Range(0, double.MaxValue)]
        public decimal GiaTriGiam { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }

        [StringLength(255)]
        public string? MoTa { get; set; }
    }
}