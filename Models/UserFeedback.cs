using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class UserFeedback
{
    public string MaKhachHang { get; set; } = null!;

    public string MaPhong { get; set; } = null!;

    public double SoSao { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? NgayFeedback { get; set; }

    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;

    public virtual PhongWithTienNghi MaPhongNavigation { get; set; } = null!;
}
