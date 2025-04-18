using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("PhongWithTienNghi")]
public partial class PhongWithTienNghi
{
    [Key]
    [StringLength(10)]
    public string MaPhong { get; set; } = null!;

    [StringLength(50)]
    public string LoaiPhong { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GiaPhong { get; set; }

    [StringLength(1)]
    public string TinhTrang { get; set; } = null!;

    public int SoLuongPhong { get; set; }

    public int Tang { get; set; }

    [StringLength(50)]
    public string KieuGiuong { get; set; } = null!;

    [StringLength(500)]
    public string? MoTa { get; set; }

    [StringLength(255)]
    public string? UrlAnhChinh { get; set; }

    [StringLength(255)]
    public string? UrlAnhPhu1 { get; set; }

    [StringLength(255)]
    public string? UrlAnhPhu2 { get; set; }

    public string? TienNghi { get; set; }

    [InverseProperty("MaPhongNavigation")]
    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
