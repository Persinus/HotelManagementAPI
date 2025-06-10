using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienThem1PhongDTO
    {
        public string LoaiPhong { get; set; }
        public decimal GiaPhong { get; set; }

        public int Tang { get; set; }
        public string KieuGiuong { get; set; }
        public string? MoTa { get; set; }
        public int SucChua { get; set; }
        public int SoGiuong { get; set; }
       
        // KHÔNG có List, mảng, object phức tạp ở đây!
    }
}