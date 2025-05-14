using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace HotelManagementAPI.DTOs
{
    public class FeedBackDTO
    {
        [JsonIgnore]
        public string? MaFeedback { get; set; } = null!;
        [JsonIgnore]
        public string? MaPhong { get; set; } = null!;
        [JsonIgnore]
        public string? MaNguoiDung { get; set; } = null!;
        public int SoSao { get; set; }
        public string? BinhLuan { get; set; }
        [JsonIgnore]
        public DateTime? NgayFeedback { get; set; }
        public string PhanLoai { get; set; } = null!;
    }
}