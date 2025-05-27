using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("DatDichVu")]
public partial class DatDichVu
{
    [Key]
    [StringLength(6)]
    public string MaDatDichVu { get; set; } = null!;

    [StringLength(6)]
    public string MaDatPhong { get; set; } = null!;

    [StringLength(6)]
    public string MaDichVu { get; set; } = null!;

    public int SoLuong { get; set; }

 

    [ForeignKey("MaDatPhong")]
    [InverseProperty("DatDichVus")]
    public virtual DatPhong MaDatPhongNavigation { get; set; } = null!;

    [ForeignKey("MaDichVu")]
    [InverseProperty("DatDichVus")]
    public virtual DichVu MaDichVuNavigation { get; set; } = null!;

}
