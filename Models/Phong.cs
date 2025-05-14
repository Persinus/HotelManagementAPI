using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("Phong")]
public partial class Phong
{
    [Key]
    [StringLength(6)]
    public string MaPhong { get; set; } = null!;

    [StringLength(50)]
    public string LoaiPhong { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GiaPhong { get; set; }

    public byte TinhTrang { get; set; }

    public int SoLuongPhong { get; set; }

    public int Tang { get; set; }

    [StringLength(50)]
    public string KieuGiuong { get; set; } = null!;

    [StringLength(500)]
    public string? MoTa { get; set; }

    [StringLength(255)]
    public string UrlAnhChinh { get; set; } = null!;

    public int SucChua { get; set; }

    public int SoGiuong { get; set; }

    [StringLength(50)]
    public string DonViTinh { get; set; } = null!;

    [Column(TypeName = "decimal(3, 2)")]
    public decimal SoSaoTrungBinh { get; set; }

    [InverseProperty("MaPhongNavigation")]
    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    [InverseProperty("MaPhongNavigation")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("MaPhongNavigation")]
    public virtual ICollection<PhongAnh> PhongAnhs { get; set; } = new List<PhongAnh>();

    [ForeignKey("MaPhong")]
    [InverseProperty("MaPhongs")]
    public virtual ICollection<GiamGium> MaGiamGia { get; set; } = new List<GiamGium>();

    [ForeignKey("MaPhong")]
    [InverseProperty("MaPhongs")]
    public virtual ICollection<TienNghi> MaTienNghis { get; set; } = new List<TienNghi>();
}
