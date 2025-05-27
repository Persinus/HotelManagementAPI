using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}