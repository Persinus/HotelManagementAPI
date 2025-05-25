using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelManagementAPI.DTOs
{
    public class DatDichVuDTO
    {
        [JsonIgnore]
        public string? MaDatDichVu { get; set; } // Không required, BE tự sinh
        [JsonIgnore]
        public string? MaDatPhong { get; set; } // BE tự gán khi đặt dịch vụ kèm phòng
        [JsonIgnore]
        public string MaDichVu { get; set; }
        public int SoLuong { get; set; }
        
        public string? MaHoaDon { get; set; }
    }
}