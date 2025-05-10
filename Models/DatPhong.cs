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
    [StringLength(6)]
    public string MaDatPhong { get; set; } = null!;

    [StringLength(6)]
    public string MaNguoiDung { get; set; } = null!;

    [StringLength(6)]
    public string MaPhong { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? NgayDat { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCheckIn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCheckOut { get; set; }

    public byte TinhTrangDatPhong { get; set; }

    [InverseProperty("MaDatPhongNavigation")]
    public virtual ICollection<DatDichVu> DatDichVus { get; set; } = new List<DatDichVu>();

    [InverseProperty("MaDatPhongNavigation")]
    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("DatPhongs")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    [ForeignKey("MaPhong")]
    [InverseProperty("DatPhongs")]
    public virtual Phong MaPhongNavigation { get; set; } = null!;
}
