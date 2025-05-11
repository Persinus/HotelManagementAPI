using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự.")]
        public string NewPassword { get; set; } = null!;
    }
}