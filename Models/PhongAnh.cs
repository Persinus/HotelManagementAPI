using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class PhongAnh
{
    public int MaAnh { get; set; }

    public string? MaPhong { get; set; }

    public string UrlAnh { get; set; } = null!;

    public virtual Phong? MaPhongNavigation { get; set; }
}
