using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class PhongWithTienNghi
{
    public string MaPhong { get; set; } = null!;

    public string LoaiPhong { get; set; } = null!;

    public decimal GiaPhong { get; set; }

    public string TinhTrang { get; set; } = null!;

    public int SoLuongPhong { get; set; }

    public int Tang { get; set; }

    public string KieuGiuong { get; set; } = null!;

    public string? MoTa { get; set; }

    public string? UrlAnhChinh { get; set; }

    public string? UrlAnhPhu1 { get; set; }

    public string? UrlAnhPhu2 { get; set; }

    public string? TienNghi { get; set; }

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    public virtual ICollection<UserFeedback> UserFeedbacks { get; set; } = new List<UserFeedback>();
}
