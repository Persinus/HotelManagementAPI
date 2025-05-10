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
    [StringLength(6)]
    public string MaDichVu { get; set; } = null!;

    [StringLength(100)]
    public string TenDichVu { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DonGia { get; set; }

    [StringLength(300)]
    public string? MoTaDichVu { get; set; }

    [StringLength(255)]
    public string HinhAnhDichVu { get; set; } = null!;

    public int SoLuong { get; set; }

    public byte TrangThai { get; set; }

    [StringLength(50)]
    public string LoaiDichVu { get; set; } = null!;

    [StringLength(50)]
    public string DonViTinh { get; set; } = null!;

    [InverseProperty("MaDichVuNavigation")]
    public virtual ICollection<DatDichVu> DatDichVus { get; set; } = new List<DatDichVu>();
}
