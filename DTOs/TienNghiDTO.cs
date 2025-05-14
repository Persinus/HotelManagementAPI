using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
namespace HotelManagementAPI.DTOs
{
   

    public class TienNghiDTO
    {   [JsonIgnore]
        public string? MaTienNghi { get; set; } = null!;

        [StringLength(100)]
        public string TenTienNghi { get; set; } = null!;

        public string? MoTa { get; set; }
    }
}