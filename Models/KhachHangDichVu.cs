using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class KhachHangDichVu
{
    public string MaKhachHang { get; set; } = null!;

    public string MaChiTietDichVu { get; set; } = null!;

    public int SoLuong { get; set; }

    public virtual DichVu MaChiTietDichVuNavigation { get; set; } = null!;

    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;
}
