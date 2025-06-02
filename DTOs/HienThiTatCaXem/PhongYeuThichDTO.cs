using System;

namespace HotelManagementAPI.DTOs
{
    public class PhongYeuThichDTO
    {
        public int Id { get; set; }
        public string MaPhong { get; set; } = null!;
        public string MaNguoiDung { get; set; } = null!;
        public DateTime? NgayYeuThich { get; set; }
    }
}