using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("ThanhToan")]
public partial class ThanhToan
{
    [Key]
    [StringLength(6)]
    public string MaThanhToan { get; set; } = null!;

    [StringLength(6)]
    public string MaHoaDon { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SoTienThanhToan { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayThanhToan { get; set; }

    [StringLength(50)]
    public string PhuongThucThanhToan { get; set; } = null!;

    [StringLength(1)]
    public string TinhTrangThanhToan { get; set; } = null!;

    [ForeignKey("MaHoaDon")]
    [InverseProperty("ThanhToans")]
    public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;
}
