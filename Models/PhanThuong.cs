using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class PhanThuong
{
    public string MaPhanThuong { get; set; } = null!;

    public string TenPhanThuong { get; set; } = null!;

    public string? MoTa { get; set; }

    public string? UrlHinhAnh { get; set; }

    public virtual ICollection<KhachHangPhanThuong> KhachHangPhanThuongs { get; set; } = new List<KhachHangPhanThuong>();
}
