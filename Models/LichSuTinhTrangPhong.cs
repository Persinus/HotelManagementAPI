using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class LichSuTinhTrangPhong
{
    public string MaLichSu { get; set; } = null!;

    public string MaPhong { get; set; } = null!;

    public string TinhTrangCu { get; set; } = null!;

    public string TinhTrangMoi { get; set; } = null!;

    public DateTime? ThoiGianThayDoi { get; set; }

    public virtual Phong MaPhongNavigation { get; set; } = null!;
}
