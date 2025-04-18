using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("NguoiDung")]
[Index("Email", Name = "UQ__NguoiDun__A9D105349E3B3289", IsUnique = true)]
public partial class NguoiDung
{
    [Key]
    [StringLength(10)]
    public string MaNguoiDung { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(50)]
    public string TenTaiKhoan { get; set; } = null!;

    [StringLength(255)]
    public string MatKhau { get; set; } = null!;

    [StringLength(20)]
    public string VaiTro { get; set; } = null!;

    [Column("JWK")]
    public string? Jwk { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual KhachHang? KhachHang { get; set; }

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual NhanVien? NhanVien { get; set; }

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual QuanTriVien? QuanTriVien { get; set; }
}
