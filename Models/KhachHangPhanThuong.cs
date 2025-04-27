using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class KhachHangPhanThuong
{
    public string MaKhachHang { get; set; } = null!;

    public string MaPhanThuong { get; set; } = null!;

    public DateTime? NgayNhan { get; set; }

    public int? SoLuong { get; set; }

    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;

    public virtual PhanThuong MaPhanThuongNavigation { get; set; } = null!;
}
