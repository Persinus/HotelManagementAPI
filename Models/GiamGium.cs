using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

public partial class GiamGium
{
    [Key]
    [StringLength(6)]
    public string MaGiamGia { get; set; } = null!;

    [StringLength(50)]
    public string TenGiamGia { get; set; } = null!;

    [StringLength(6)]
    public string? LoaiGiamGia { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GiaTriGiam { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayBatDau { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayKetThuc { get; set; }

    [StringLength(255)]
    public string? MoTa { get; set; }

    [ForeignKey("MaGiamGia")]
    [InverseProperty("MaGiamGia")]
    public virtual ICollection<Phong> MaPhongs { get; set; } = new List<Phong>();
}
