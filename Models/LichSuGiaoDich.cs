using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("LichSuGiaoDich")]
public partial class LichSuGiaoDich
{
    [Key]
    [StringLength(6)]
    public string MaGiaoDich { get; set; } = null!;

    [StringLength(6)]
    public string MaNguoiDung { get; set; } = null!;

    [StringLength(50)]
    public string LoaiGiaoDich { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianGiaoDich { get; set; }

    [StringLength(255)]
    public string? MoTa { get; set; }

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("LichSuGiaoDiches")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
