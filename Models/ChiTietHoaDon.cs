using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class ChiTietHoaDon
{
    public string MaChiTiet { get; set; } = null!;

    public string MaHoaDon { get; set; } = null!;

    public string LoaiKhoanMuc { get; set; } = null!;

    public string MaKhoanMuc { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal ThanhTien { get; set; }

    public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;
}
