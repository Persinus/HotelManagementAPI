using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienThemAnhDTO
    {
        [Required]
        public string MaPhong { get; set; } = null!;

        [Required]
        [Url(ErrorMessage = "URL ảnh không hợp lệ.")]
        public string UrlAnh { get; set; } = null!;
    }
}