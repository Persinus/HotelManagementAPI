using System.ComponentModel.DataAnnotations;
namespace HotelManagementAPI.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string TenTaiKhoan { get; set; } // Bắt buộc
        [Required]
        public string MatKhau { get; set; } // Bắt buộc
    }
}