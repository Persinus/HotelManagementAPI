using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienThem1DichVuDTO
    {
        [Required]
        [StringLength(100)]
        public string TenDichVu { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0.")]
        public decimal DonGia { get; set; }

        [Required]
        public string? MoTaDichVu { get; set; }

  

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0.")]
        public int SoLuong { get; set; }

      

        [Required]
        [StringLength(50)]
        public string LoaiDichVu { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string DonViTinh { get; set; } = null!;
    }
}