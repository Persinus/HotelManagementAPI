using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HotelManagementAPI.DTOs
{
    public class PhongDetailsDTO
    {
        public string MaPhong { get; set; } = null!;
        public string LoaiPhong { get; set; } = null!;
        public decimal GiaPhong { get; set; }
        public string TinhTrang { get; set; } = null!;
        [JsonIgnore]
        public int SoLuongPhong { get; set; }
        public int Tang { get; set; }
        public string KieuGiuong { get; set; } = null!;
        public string? MoTa { get; set; }
        public string UrlAnhChinh { get; set; } = null!;
        public string? MotaPhong { get; set; }
        public int SucChua { get; set; }
        public int SoGiuong { get; set; }
        public string DonViTinh { get; set; } = null!;
        public decimal SoSaoTrungBinh { get; set; }
        public List<PhongAnhDTO> UrlAnhPhu { get; set; } = new List<PhongAnhDTO>();
        public List<TienNghiDTO> TienNghi { get; set; } = new List<TienNghiDTO>();
        public List<GiamGiaDTO> GiamGia { get; set; } = new List<GiamGiaDTO>();
        public List<FeedBackDTO> Feedbacks { get; set; } = new List<FeedBackDTO>();
        public decimal GiaPhongSauGiam { get; set; }
    }
}
