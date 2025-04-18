using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("DatPhong")]
public partial class DatPhong
{
    [Key]
    [StringLength(10)]
    public string MaDatPhong { get; set; } = null!;

    [StringLength(10)]
    public string MaKhachHang { get; set; } = null!;

    [StringLength(10)]
    public string MaNhanVien { get; set; } = null!;

    [StringLength(10)]
    public string MaPhong { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime NgayDat { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayNhanPhong { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayTraPhong { get; set; }

    [StringLength(1)]
    public string TrangThai { get; set; } = null!;

    [InverseProperty("MaDatPhongNavigation")]
    public virtual ICollection<HoaDonThanhToanPhong> HoaDonThanhToanPhongs { get; set; } = new List<HoaDonThanhToanPhong>();

    [ForeignKey("MaKhachHang")]
    [InverseProperty("DatPhongs")]
    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;

    [ForeignKey("MaNhanVien")]
    [InverseProperty("DatPhongs")]
    public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;

    [ForeignKey("MaPhong")]
    [InverseProperty("DatPhongs")]
    public virtual PhongWithTienNghi MaPhongNavigation { get; set; } = null!;
}
