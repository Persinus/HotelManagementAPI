using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("HoaDon")]
public partial class HoaDon
{
    [Key]
    [StringLength(6)]
    public string MaHoaDon { get; set; } = null!;

    [StringLength(6)]
    public string MaNguoiDung { get; set; } = null!;

    [StringLength(6)]
    public string MaDatPhong { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TongTien { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTaoHoaDon { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayThanhToan { get; set; }

    public byte TinhTrangHoaDon { get; set; }

    [InverseProperty("MaHoaDonNavigation")]
    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    [InverseProperty("MaHoaDonNavigation")]
    public virtual ICollection<DatDichVu> DatDichVus { get; set; } = new List<DatDichVu>();

    [ForeignKey("MaDatPhong")]
    [InverseProperty("HoaDons")]
    public virtual DatPhong MaDatPhongNavigation { get; set; } = null!;

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("HoaDons")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    [InverseProperty("MaHoaDonNavigation")]
    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
