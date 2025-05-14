using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelManagementAPI.DTOs
{
    public class PhongDetailsDTO
    {
       
        public string? MaPhong { get; set; } = null!;
        public string LoaiPhong { get; set; } = null!;
        public decimal GiaPhong { get; set; }
        public byte TinhTrang { get; set; }
        public int SoLuongPhong { get; set; }
        public int Tang { get; set; }
        public string KieuGiuong { get; set; } = null!;
        public string? MoTa { get; set; }
        public string UrlAnhChinh { get; set; } = null!;
        public int SucChua { get; set; }
        public int SoGiuong { get; set; }
        public string DonViTinh { get; set; } = null!;
        public decimal SoSaoTrungBinh { get; set; }
    }
}
