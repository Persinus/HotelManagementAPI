using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("HoaDonThanhToanDichVu")]
public partial class HoaDonThanhToanDichVu
{
    [Key]
    [StringLength(10)]
    public string MaHoaDonDichVu { get; set; } = null!;

    [StringLength(10)]
    public string? MaKhachHang { get; set; }

    [StringLength(10)]
    public string? MaChiTietDichVu { get; set; }

    [StringLength(1)]
    public string TrangThaiThanhToan { get; set; } = null!;

    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ThanhTien { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayLapHoaDon { get; set; }

    [ForeignKey("MaChiTietDichVu")]
    [InverseProperty("HoaDonThanhToanDichVus")]
    public virtual DichVu? MaChiTietDichVuNavigation { get; set; }

    [ForeignKey("MaKhachHang")]
    [InverseProperty("HoaDonThanhToanDichVus")]
    public virtual KhachHang? MaKhachHangNavigation { get; set; }
}
