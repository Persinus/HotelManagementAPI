using System.Collections.Generic;

namespace HotelManagementAPI.DTOs
{
    public class PhongDTO
    {
        public string MaPhong { get; set; } = null!;
        public List<string> MaAnhList { get; set; } = new();
        public List<string> MaTienNghiList { get; set; } = new();
        public List<string> MaGiamGiaList { get; set; } = new();
        public List<string> MaFeedbackList { get; set; } = new();
    }
}