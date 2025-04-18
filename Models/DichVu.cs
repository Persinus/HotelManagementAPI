using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("DichVu")]
public partial class DichVu
{
    [Key]
    [StringLength(10)]
    public string MaChiTietDichVu { get; set; } = null!;

    [StringLength(100)]
    public string TenDichVu { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DonGia { get; set; }

    [StringLength(300)]
    public string? MoTaDichVu { get; set; }

    [StringLength(255)]
    public string? UrlAnh { get; set; }

    [InverseProperty("MaChiTietDichVuNavigation")]
    public virtual ICollection<HoaDonThanhToanDichVu> HoaDonThanhToanDichVus { get; set; } = new List<HoaDonThanhToanDichVu>();

    [InverseProperty("MaChiTietDichVuNavigation")]
    public virtual ICollection<KhachHangDichVu> KhachHangDichVus { get; set; } = new List<KhachHangDichVu>();
}
