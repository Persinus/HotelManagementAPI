using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelManagementAPI.DTOs.NhanVien
{
    public class NhanVienSuaBaiVietDTO
    {
        [Required]
        public string MaBaiViet { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string TieuDe { get; set; } = null!;

        [Required]
        public string NoiDung { get; set; } = null!;

        [JsonIgnore]
        public string? HinhAnhUrl { get; set; }

        // Ngày cập nhật nên xử lý trong backend, không để client truyền
        // public DateTime? NgayCapNhat { get; set; } 
    }
}
