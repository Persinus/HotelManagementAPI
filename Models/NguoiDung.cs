using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("NguoiDung")]
[Index("CanCuocCongDan", Name = "UQ_CanCuocCongDan", IsUnique = true)]
[Index("SoDienThoai", Name = "UQ_SoDienThoai", IsUnique = true)]
[Index("Email", Name = "UQ__NguoiDun__A9D10534910DC410", IsUnique = true)]
public partial class NguoiDung
{
    [Key]
    [StringLength(6)]
    public string MaNguoiDung { get; set; } = null!;

    [StringLength(15)]
    public string? Vaitro { get; set; }

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(50)]
    public string? TenTaiKhoan { get; set; }

    [StringLength(255)]
    public string? MatKhau { get; set; }

    [StringLength(50)]
    public string? HoTen { get; set; }

    [StringLength(20)]
    public string? CanCuocCongDan { get; set; }

    [StringLength(15)]
    public string? SoDienThoai { get; set; }

    [StringLength(100)]
    public string? DiaChi { get; set; }

    public DateTime? NgaySinh { get; set; }

    [StringLength(10)]
    public string? GioiTinh { get; set; }

    [Column("HinhAnhURL")]
    [StringLength(255)]
    public string HinhAnhUrl { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<LichSuGiaoDich> LichSuGiaoDiches { get; set; } = new List<LichSuGiaoDich>();

   
   
}
