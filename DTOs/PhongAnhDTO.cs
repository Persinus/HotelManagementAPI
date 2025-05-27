using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs
{
    public class PhongAnhDTO
    {
       
        public string MaAnh { get; set; } = null!;

     
        public string MaPhong { get; set; } = null!;

        public string UrlAnh { get; set; } = null!;
    }
}