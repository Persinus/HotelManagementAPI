using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienThem1PhongDTO
    {
        [Required]
        public string LoaiPhong { get; set; } = null!;

        [Required]
        public decimal GiaPhong { get; set; }

        [Required]
        public byte TinhTrang { get; set; }

        [Required]
        public int SoLuongPhong { get; set; }

        [Required]
        public int Tang { get; set; }

        [Required]
        public string KieuGiuong { get; set; } = null!;

        public string? MoTa { get; set; }

        [Required]
        public string UrlAnhChinh { get; set; } = null!;

        [Required]
        public int SucChua { get; set; }

        [Required]
        public int SoGiuong { get; set; }

        [Required]
        public string DonViTinh { get; set; } = null!;

        public decimal SoSaoTrungBinh { get; set; } = 0;

        // Các bảng con để trống khi thêm mới
        public List<string> MaAnhList { get; set; } = new();
        public List<string> MaTienNghiList { get; set; } = new();
        public List<string> MaGiamGiaList { get; set; } = new();
        public List<string> MaFeedbackList { get; set; } = new();
    }
}