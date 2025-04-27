using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class VeGiamGium
{
    public string MaVe { get; set; } = null!;

    public string MaKhachHang { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal PhanTramGiam { get; set; }

    public DateOnly NgayBatDau { get; set; }

    public DateOnly NgayHetHan { get; set; }

    public string? GhiChu { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;
}
