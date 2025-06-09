using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienThem1TienNghiDTO
    {
        [Required]
        [StringLength(100)]
        public string TenTienNghi { get; set; } = null!;

        [StringLength(500)]
        public string? MoTa { get; set; }
    }
}