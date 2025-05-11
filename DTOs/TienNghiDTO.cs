using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
   

    public class TienNghiDTO
    {
        [StringLength(6)]
        public string MaTienNghi { get; set; } = null!;

        [StringLength(100)]
        public string TenTienNghi { get; set; } = null!;

        [StringLength(500)]
        public string? MoTa { get; set; }
    }
}