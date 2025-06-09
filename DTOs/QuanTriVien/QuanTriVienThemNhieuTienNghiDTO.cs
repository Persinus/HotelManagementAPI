using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienThemNhieuTienNghiDTO
    {
        [Required]
        public List<QuanTriVienThem1TienNghiDTO> DanhSachTienNghi { get; set; } = new();
    }
}