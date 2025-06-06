using System.Collections.Generic;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienApDungGiamGiaDTO
    {
        public string MaGiamGia { get; set; } = null!;
        public string TenGiamGia { get; set; } = null!;
        public decimal GiaTriGiam { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string? MoTa { get; set; }
        public List<string> DanhSachMaPhong { get; set; } = new();
    }
}