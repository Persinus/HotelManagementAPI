using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelManagementAPI.DTOs
{
    public class NguoiDungDTO
    {
        [JsonIgnore] // Bỏ qua khi nhận dữ liệu từ client
        public string? MaNguoiDung { get; set; }

        [JsonIgnore] // Bỏ qua khi nhận dữ liệu từ client
        public string? Vaitro { get; set; }

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(50, ErrorMessage = "Tên tài khoản không được vượt quá 50 ký tự.")]
        public string TenTaiKhoan { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự.")]
        public string MatKhau { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public string? HoTen { get; set; }

        [Required]
        public string? SoDienThoai { get; set; }

        [Required]
        public string? DiaChi { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string? GioiTinh { get; set; }

        public string? HinhAnhUrl { get; set; }
        
        [Required]
        [StringLength(12, ErrorMessage = "Căn cước công dân không được vượt quá 12 ký tự.")]
        public string? CanCuocCongDan { get; set; }

        public DateTime? NgayTao { get; set; }
    }
}