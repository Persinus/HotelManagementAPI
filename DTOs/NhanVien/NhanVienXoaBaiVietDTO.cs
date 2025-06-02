//DTO xóa bài viết 
using System.ComponentModel.DataAnnotations;
namespace HotelManagementAPI.DTOs.NhanVien
{
    public class NhanVienXoaBaiVietDTO
    {
        [Required]
        [StringLength(6, ErrorMessage = "Mã bài viết phải có độ dài từ 1 đến 6 ký tự.")]
        public string MaBaiViet { get; set; } = null!;


    }
}
