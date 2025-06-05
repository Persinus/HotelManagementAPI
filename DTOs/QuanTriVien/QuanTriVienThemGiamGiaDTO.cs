namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienThemGiamGiaDTO
    {
        public string TenGiamGia { get; set; } = null!;
        public string? LoaiGiamGia { get; set; }
        public decimal GiaTriGiam { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string? MoTa { get; set; }
    }
}