using System;
using System.Text.Json.Serialization;

namespace HotelManagementAPI.DTOs
{
    public class SuaThongTinNguoiDungDTO
    {
       
        public string Email { get; set; } = null!;
        public string TenTaiKhoan { get; set; } = null!;

        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
     
        public string? CanCuocCongDan { get; set; }
   
    }
}