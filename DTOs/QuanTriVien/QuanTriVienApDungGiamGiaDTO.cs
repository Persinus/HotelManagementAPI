using System.Collections.Generic;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienApDungGiamGiaDTO
    {
        public string MaGiamGia { get; set; } = null!;
        public List<string> DanhSachMaPhong { get; set; } = new();
    }
}