using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienThemNhieuDichVuDTO
    {
        [Required]
        public List<QuanTriVienThem1DichVuDTO> DanhSachDichVu { get; set; } = new();
    }
}