using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace HotelManagementAPI.DTOs
{
    public class DatPhongDTO
    {

        [Required]
        public string MaPhong { get; set; } = null!;

        [JsonIgnore]
        public string? MaNguoiDung { get; set; } = null!;

        [Required]
        public DateTime NgayDat { get; set; }

        [Required]
        public DateTime NgayCheckIn { get; set; }

        [Required]
        public DateTime NgayCheckOut { get; set; }
        
         [Range(1, 3, ErrorMessage = "TinhTrangDatPhong chỉ nhận giá trị từ 1 đến 3.")]
        public byte? TinhTrangDatPhong { get; set; } = null!;

        [JsonIgnore]
        public string? MaDatPhong { get; set; } // This will be generated later
    }
}