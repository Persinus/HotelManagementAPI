using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelManagementAPI.DTOs
{
    public class PhongDetailsDTO
    {

        [JsonIgnore]
        public string MaPhong { get; set; } = null!;

        [Required]
        public string LoaiPhong { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phòng phải lớn hơn hoặc bằng 0.")]
        public decimal GiaPhong { get; set; }

        [Required]
        public string TinhTrang { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phòng phải lớn hơn hoặc bằng 1.")]
        public int SoLuongPhong { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Tầng phải lớn hơn hoặc bằng 1.")]
        public int Tang { get; set; }

        [Required]
        public string KieuGiuong { get; set; } = null!;


        public string? MoTa { get; set; }

        [Required]
        [Url(ErrorMessage = "URL ảnh chính không hợp lệ.")]
        public string UrlAnhChinh { get; set; } = null!;

        public string? MotaPhong { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải lớn hơn hoặc bằng 1.")]
        public int SucChua { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số giường phải lớn hơn hoặc bằng 1.")]
        public int SoGiuong { get; set; }

        [Required]
        public string DonViTinh { get; set; } = null!;

        [Required]
        [Range(0, 5, ErrorMessage = "Số sao trung bình phải nằm trong khoảng từ 0 đến 5.")]
        public decimal SoSaoTrungBinh { get; set; }

        // Danh sách ảnh phụ
        public List<PhongAnhDTO> UrlAnhPhu { get; set; } = new List<PhongAnhDTO>();

        // Danh sách tiện nghi
        public List<TienNghiDTO> TienNghi { get; set; } = new List<TienNghiDTO>();

        // Danh sách giảm giá
        public List<GiamGiaDTO> GiamGia { get; set; } = new List<GiamGiaDTO>();
        // Danh sách feedback
        public List<FeedBackDTO> Feedbacks { get; set; } = new List<FeedBackDTO>();

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phòng sau giảm phải lớn hơn hoặc bằng 0.")]
        public decimal GiaPhongSauGiam { get; set; }
    }
}
