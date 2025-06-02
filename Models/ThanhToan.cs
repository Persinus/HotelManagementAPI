using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class ThanhToan
{
    public string MaThanhToan { get; set; } = null!;

    public string MaHoaDon { get; set; } = null!;

    public decimal SoTienThanhToan { get; set; }

    public DateTime? NgayThanhToan { get; set; }

    public string PhuongThucThanhToan { get; set; } = null!;

    public string TinhTrangThanhToan { get; set; } = null!;

    public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;
}
