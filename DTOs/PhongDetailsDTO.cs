

using HotelManagementAPI.DTOs;

public class PhongDetailsDTO
    {
        public string MaPhong { get; set; }
        public string LoaiPhong { get; set; }
        public decimal GiaPhong { get; set; }
        public string TinhTrang { get; set; }
        public int SoLuongPhong { get; set; }
        public int Tang { get; set; }
        public string KieuGiuong { get; set; }
        public string? MoTa { get; set; }
        public string UrlAnhChinh { get; set; }
        public string? MotaPhong { get; set; }
        public int SucChua { get; set; }
        public int SoGiuong { get; set; }
        public string DonViTinh { get; set; }
        public decimal SoSaoTrungBinh { get; set; }

        // Danh sách ảnh phụ
        public List<string> UrlAnhPhu { get; set; } = new List<string>();

       

    
}
