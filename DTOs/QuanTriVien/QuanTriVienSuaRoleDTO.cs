using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienSuaRoleDTO
    {
        [Required]
        public string VaiTroMoi { get; set; } = null!;
    }
}