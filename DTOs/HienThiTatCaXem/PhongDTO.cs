using System.Collections.Generic;

namespace HotelManagementAPI.DTOs
{
    public class PhongDTO
    {
        public string MaPhong { get; set; } = null!;
        public string LoaiPhong { get; set; } = null!;
        public decimal GiaPhong { get; set; }
        public decimal GiaUuDai { get; set; }
        public int Tang { get; set; }
        public string TinhTrang { get; set; } = null!;
        public string DonViTinh { get; set; } = null!;
        public decimal SoSaoTrungBinh { get; set; }
        public string MoTa { get; set; } = null!;
        public string UrlAnhChinh { get; set; } = null!;
        public List<GiamGiaDTO> GiamGia { get; set; } = new List<GiamGiaDTO>();
    }
}