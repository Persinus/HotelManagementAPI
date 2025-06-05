using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienThemAnhDTO
    {
        [Required]
        public string MaPhong { get; set; } = null!;

       
    }
}