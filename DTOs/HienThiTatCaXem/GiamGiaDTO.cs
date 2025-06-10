using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class GiamGiaDTO
    {
        
        public string MaGiamGia { get; set; } = null!;
        public decimal GiaTriGiam { get; set; }
        public DateTime NgayKetThuc { get; set; }
    }
}
