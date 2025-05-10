using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("Feedback")]
public partial class Feedback
{
    [Key]
    [StringLength(6)]
    public string MaFeedback { get; set; } = null!;

    [StringLength(6)]
    public string MaPhong { get; set; } = null!;

    [StringLength(6)]
    public string MaNguoiDung { get; set; } = null!;

    public int SoSao { get; set; }

    [StringLength(500)]
    public string? BinhLuan { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayFeedback { get; set; }

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("Feedbacks")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    [ForeignKey("MaPhong")]
    [InverseProperty("Feedbacks")]
    public virtual Phong MaPhongNavigation { get; set; } = null!;
}
