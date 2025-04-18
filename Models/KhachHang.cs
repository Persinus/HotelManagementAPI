using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("KhachHang")]
[Index("SoDienThoai", Name = "UQ__KhachHan__0389B7BDB9FC1CCF", IsUnique = true)]
[Index("CanCuocCongDan", Name = "UQ__KhachHan__9A7D3DD775B63E1A", IsUnique = true)]
[Index("MaNguoiDung", Name = "UQ__KhachHan__C539D76313D448ED", IsUnique = true)]
public partial class KhachHang
{
    [Key]
    [StringLength(10)]
    public string MaKhachHang { get; set; } = null!;

    [StringLength(10)]
    public string MaNguoiDung { get; set; } = null!;

    [StringLength(50)]
    public string HoTen { get; set; } = null!;

    [StringLength(20)]
    public string CanCuocCongDan { get; set; } = null!;

    [StringLength(15)]
    public string SoDienThoai { get; set; } = null!;

    [StringLength(100)]
    public string? DiaChi { get; set; }

    public DateOnly? NgaySinh { get; set; }

    [StringLength(10)]
    public string? GioiTinh { get; set; }

    [StringLength(50)]
    public string? NgheNghiep { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [StringLength(255)]
    public string? HinhAnh { get; set; }

    [InverseProperty("MaKhachHangNavigation")]
    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    [InverseProperty("MaKhachHangNavigation")]
    public virtual ICollection<HoaDonThanhToanDichVu> HoaDonThanhToanDichVus { get; set; } = new List<HoaDonThanhToanDichVu>();

    [InverseProperty("MaKhachHangNavigation")]
    public virtual ICollection<KhachHangDichVu> KhachHangDichVus { get; set; } = new List<KhachHangDichVu>();

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("KhachHang")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
