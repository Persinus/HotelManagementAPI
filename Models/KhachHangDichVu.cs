using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[PrimaryKey("MaKhachHang", "MaChiTietDichVu")]
[Table("KhachHang_DichVu")]
public partial class KhachHangDichVu
{
    [Key]
    [StringLength(10)]
    public string MaKhachHang { get; set; } = null!;

    [Key]
    [StringLength(10)]
    public string MaChiTietDichVu { get; set; } = null!;

    public int SoLuong { get; set; }

    [ForeignKey("MaChiTietDichVu")]
    [InverseProperty("KhachHangDichVus")]
    public virtual DichVu MaChiTietDichVuNavigation { get; set; } = null!;

    [ForeignKey("MaKhachHang")]
    [InverseProperty("KhachHangDichVus")]
    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;
}
