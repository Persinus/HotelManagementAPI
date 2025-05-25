using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System;

namespace HotelManagementAPI.DTOs
{
    public class DatPhongDTO
    {
        [JsonIgnore]
        public string? MaDatPhong { get; set; } // Không required, BE tự sinh
        [JsonIgnore]
        public string? MaNguoiDung { get; set; } // BE tự lấy từ JWT

        public string? MaPhong { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime NgayNhan { get; set; }
        public DateTime NgayTra { get; set; }
        public decimal TongTien { get; set; }
        public int TrangThai { get; set; }
        public List<DatDichVuDTO>? DichVuDiKem { get; set; }
    }
}