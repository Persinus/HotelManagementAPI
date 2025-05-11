using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class DatPhongDTO
    {
        [Required]
        public string MaPhong { get; set; } = null!;

        [Required]
        public string MaNguoiDung { get; set; } = null!;

        [Required]
        public DateTime NgayDat { get; set; }

        [Required]
        public DateTime NgayCheckIn { get; set; }

        [Required]
        public DateTime NgayCheckOut { get; set; }

        [Required]
        public string TinhTrangDatPhong { get; set; } = null!;

        public string? MaDatPhong { get; set; } // This will be generated later
    }
}