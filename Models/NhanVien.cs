using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("NhanVien")]
public partial class NhanVien
{
    [Key]
    [StringLength(6)]
    public string MaNhanVien { get; set; } = null!;

    [StringLength(6)]
    public string MaNguoiDung { get; set; } = null!;

    [StringLength(50)]
    public string ChucVu { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Luong { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayVaoLam { get; set; }

    [StringLength(50)]
    public string? CaLamViec { get; set; }

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("NhanViens")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
