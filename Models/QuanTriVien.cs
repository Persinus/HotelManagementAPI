using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

[Table("QuanTriVien")]
[Index("SoDienThoai", Name = "UQ__QuanTriV__0389B7BDBD6EBD0F", IsUnique = true)]
[Index("MaNguoiDung", Name = "UQ__QuanTriV__C539D7635FE8785B", IsUnique = true)]
public partial class QuanTriVien
{
    [Key]
    [StringLength(10)]
    public string MaQuanTri { get; set; } = null!;

    [StringLength(10)]
    public string MaNguoiDung { get; set; } = null!;

    [StringLength(50)]
    public string TenAdmin { get; set; } = null!;

    [StringLength(15)]
    public string SoDienThoai { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("QuanTriVien")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
