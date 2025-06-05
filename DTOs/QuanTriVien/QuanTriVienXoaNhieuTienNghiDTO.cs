using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienXoaNhieuTienNghiDTO
    {
        [Required]
        public List<string> DanhSachMaTienNghi { get; set; } = new();
    }
}