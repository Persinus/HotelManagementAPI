using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class PhongAnh
{
    public string MaAnh { get; set; } = null!;

    public string MaPhong { get; set; } = null!;

    public string UrlAnh { get; set; } = null!;

    public virtual Phong MaPhongNavigation { get; set; } = null!;
}
