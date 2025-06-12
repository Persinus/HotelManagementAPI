namespace HotelManagementAPI.DTOs.QuanLyChung
{
    public class QuanLyChungSuaPhongDTO
    {

        // Các trường dưới đây cho phép null, chỉ sửa trường nào cần
        public string? LoaiPhong { get; set; }
        public decimal? GiaPhong { get; set; }
        public int? Tang { get; set; }
        public int? TinhTrang { get; set; }
        public string? DonViTinh { get; set; }
        public string? MoTa { get; set; }
        public string? KieuGiuong { get; set; }
        public int? SucChua { get; set; }
        public int? SoGiuong { get; set; }
        public string? UrlAnhChinh { get; set; }
    }
}