using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("HoaDonThanhToanPhong")]
public partial class HoaDonThanhToanPhong
{
    [Key]
    [StringLength(10)]
    public string MaHoaDon { get; set; } = null!;

    [StringLength(10)]
    public string MaDatPhong { get; set; } = null!;

    [StringLength(10)]
    public string MaNhanVien { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime NgayLapHoaDon { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TongTien { get; set; }

    [StringLength(1)]
    public string TrangThaiThanhToan { get; set; } = null!;

    public int HinhThucThanhToan { get; set; }

    [ForeignKey("MaDatPhong")]
    [InverseProperty("HoaDonThanhToanPhongs")]
    public virtual DatPhong MaDatPhongNavigation { get; set; } = null!;

    [ForeignKey("MaNhanVien")]
    [InverseProperty("HoaDonThanhToanPhongs")]
    public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;
}
