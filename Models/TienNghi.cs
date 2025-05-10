using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("TienNghi")]
public partial class TienNghi
{
    [Key]
    [StringLength(6)]
    public string MaTienNghi { get; set; } = null!;

    [StringLength(100)]
    public string TenTienNghi { get; set; } = null!;

    [StringLength(500)]
    public string? MoTa { get; set; }

    [ForeignKey("MaTienNghi")]
    [InverseProperty("MaTienNghis")]
    public virtual ICollection<Phong> MaPhongs { get; set; } = new List<Phong>();
}
