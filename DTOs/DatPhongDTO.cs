using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class DatPhongDTO
    {
        [Required]
        [StringLength(10)]
        public string MaDatPhong { get; set; } // Tự động tạo

        [Required]
        [StringLength(10)]
        public string MaNguoiDung { get; set; } // Bắt buộc

        [Required]
        [StringLength(10)]
        public string MaPhong { get; set; } // Bắt buộc

        [Required]
        public DateTime? NgayDat { get; set; } // Bắt buộc

        [Required]
        public DateTime? NgayCheckIn { get; set; } // Bắt buộc

        [Required]
        public DateTime? NgayCheckOut { get; set; } // Bắt buộc

        [Required]
        [StringLength(20)]
        public string TinhTrangDatPhong { get; set; } // Bắt buộc
    }
}