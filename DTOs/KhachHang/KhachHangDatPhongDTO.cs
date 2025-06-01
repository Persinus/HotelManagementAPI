using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;

namespace HotelManagementAPI.DTOs
{
    public class KhachHangDatPhongDTO
    {
        [JsonIgnore]
        public string? MaDatPhong { get; set; }
        [JsonIgnore]
        public string? MaNguoiDung { get; set; }

        public string? MaPhong { get; set; }
        [JsonIgnore]
        public DateTime NgayDat { get; set; }

        // Đổi tên cho đúng với DB
        public DateTime NgayCheckIn { get; set; }
        public DateTime NgayCheckOut { get; set; }
        // Nếu DB không có cột TongTien thì bỏ dòng này
        // public decimal TongTien { get; set; }
        [JsonIgnore]
        public byte TinhTrangDatPhong { get; set; }
        public List<KhachHangDatDichVuDTO>? DichVuDiKem { get; set; }
    }
}