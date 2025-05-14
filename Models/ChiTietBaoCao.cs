using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("ChiTietBaoCao")]
public partial class ChiTietBaoCao
{
    [Key]
    [StringLength(6)]
    public string MaChiTiet { get; set; } = null!;

    [StringLength(6)]
    public string MaBaoCao { get; set; } = null!;

    [StringLength(255)]
    public string NoiDungChiTiet { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GiaTri { get; set; }

    [ForeignKey("MaBaoCao")]
    [InverseProperty("ChiTietBaoCaos")]
    public virtual BaoCao MaBaoCaoNavigation { get; set; } = null!;
}
