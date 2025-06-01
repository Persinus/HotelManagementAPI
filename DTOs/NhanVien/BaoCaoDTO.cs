using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class BaoCaoDTO
    {
        [Required]
        [StringLength(6)]
        public string MaBaoCao { get; set; } = null!;

        [Required]
        public string LoaiBaoCao { get; set; } = null!;
        [Required]
        public DateTime? ThoiGian { get; set; }

        [Required]
        public string? NoiDung { get; set; }
    }
}