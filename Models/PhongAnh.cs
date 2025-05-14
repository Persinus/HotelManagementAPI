using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("PhongAnh")]
public partial class PhongAnh
{
    [Key]
    [StringLength(6)]
    public string MaAnh { get; set; } = null!;

    [StringLength(6)]
    public string MaPhong { get; set; } = null!;

    [StringLength(255)]
    public string UrlAnh { get; set; } = null!;

    [ForeignKey("MaPhong")]
    [InverseProperty("PhongAnhs")]
    public virtual Phong MaPhongNavigation { get; set; } = null!;
}
