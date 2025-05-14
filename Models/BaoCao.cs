using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("BaoCao")]
public partial class BaoCao
{
    [Key]
    [StringLength(6)]
    public string MaBaoCao { get; set; } = null!;

    [StringLength(50)]
    public string LoaiBaoCao { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGian { get; set; }

    [Column(TypeName = "text")]
    public string? NoiDung { get; set; }

    [InverseProperty("MaBaoCaoNavigation")]
    public virtual ICollection<ChiTietBaoCao> ChiTietBaoCaos { get; set; } = new List<ChiTietBaoCao>();
}
