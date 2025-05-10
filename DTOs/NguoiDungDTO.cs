using System.Text.Json.Serialization;

namespace HotelManagementAPI.DTOs
{
    public class NguoiDungDTO
    {
        [JsonIgnore] // Bỏ qua khi nhận dữ liệu từ client
        public string? MaNguoiDung { get; set; }

        [JsonIgnore] // Bỏ qua khi nhận dữ liệu từ client
        public string? Vaitro { get; set; } 

        public string Email { get; set; } = null!;
        public string TenTaiKhoan { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public string? HinhAnhUrl { get; set; }
        public string? CanCuocCongDan { get; set; }
        public DateTime? NgayTao { get; set; }
    }
}