using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
namespace HotelManagementAPI.DTOs
{
   

    public class TienNghiDTO

    {
        [StringLength(6)]
        public string MaTienNghi { get; set; } = null!;

        [StringLength(100)]
        public string TenTienNghi { get; set; } = null!;

        public string? MoTa { get; set; }
    }
}