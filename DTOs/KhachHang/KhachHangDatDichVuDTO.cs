using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelManagementAPI.DTOs
{
    public class KhachHangDatDichVuDTO
    {
        [JsonIgnore]
        public string? MaDatDichVu { get; set; } // Không required, BE tự sinh
        [JsonIgnore]
        public string? MaDatPhong { get; set; } // BE tự gán khi đặt dịch vụ kèm phòng

        public string MaDichVu { get; set; }
        public int SoLuong { get; set; }

    }
}