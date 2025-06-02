using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class GiamGiaDTO
    {
        [Range(0, double.MaxValue)]
        public decimal GiaTriGiam { get; set; }
        public string? LoaiGiamGia { get; set; }
        public DateTime NgayKetThuc { get; set; }
    }
}
