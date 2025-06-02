using System;
using System.Text.Json.Serialization;

namespace HotelManagementAPI.DTOs.NhanVien
{
   public class NhanVienThemBaiVietDTO
{
     public string MaBaiViet { get; set; } = null!;
    public string TieuDe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;


    public string? TrangThai { get; set; }
}

}