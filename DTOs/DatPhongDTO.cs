namespace HotelManagementAPI.DTOs
{
    public class DatPhongDTO
    {
        public string MaDatPhong { get; set; } // Tự động tạo
        public string MaNguoiDung { get; set; } // Bắt buộc
        public string MaPhong { get; set; } // Bắt buộc
        public DateTime? NgayDat { get; set; } // Bắt buộc
        public DateTime? NgayCheckIn { get; set; } // Bắt buộc
        public DateTime? NgayCheckOut { get; set; } // Bắt buộc
        public string TinhTrangDatPhong { get; set; } // Bắt buộc
    }
}