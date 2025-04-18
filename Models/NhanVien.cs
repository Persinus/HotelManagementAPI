using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("NhanVien")]
[Index("SoDienThoai", Name = "UQ__NhanVien__0389B7BDF1BB0F64", IsUnique = true)]
[Index("CanCuocCongDan", Name = "UQ__NhanVien__9A7D3DD72123CDF2", IsUnique = true)]
[Index("MaNguoiDung", Name = "UQ__NhanVien__C539D76307FED8AF", IsUnique = true)]
public partial class NhanVien
{
    [Key]
    [StringLength(10)]
    public string MaNhanVien { get; set; } = null!;

    [StringLength(10)]
    public string MaNguoiDung { get; set; } = null!;

    [StringLength(50)]
    public string HoTen { get; set; } = null!;

    [StringLength(15)]
    public string SoDienThoai { get; set; } = null!;

    [StringLength(20)]
    public string CanCuocCongDan { get; set; } = null!;

    [StringLength(255)]
    public string? HinhAnh { get; set; }

    public DateOnly NgayVaoLam { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [InverseProperty("MaNhanVienNavigation")]
    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    [InverseProperty("MaNhanVienNavigation")]
    public virtual ICollection<HoaDonThanhToanPhong> HoaDonThanhToanPhongs { get; set; } = new List<HoaDonThanhToanPhong>();

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("NhanVien")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
