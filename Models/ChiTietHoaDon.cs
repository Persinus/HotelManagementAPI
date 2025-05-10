using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("ChiTietHoaDon")]
public partial class ChiTietHoaDon
{
    [Key]
    [StringLength(6)]
    public string MaChiTiet { get; set; } = null!;

    [StringLength(6)]
    public string MaHoaDon { get; set; } = null!;

    [StringLength(12)]
    public string LoaiKhoanMuc { get; set; } = null!;

    [StringLength(12)]
    public string MaKhoanMuc { get; set; } = null!;

    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ThanhTien { get; set; }

    [ForeignKey("MaHoaDon")]
    [InverseProperty("ChiTietHoaDons")]
    public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;
}
