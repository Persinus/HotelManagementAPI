using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class LichSuGiaoDich
{
    public string MaGiaoDich { get; set; } = null!;

    public string MaNguoiDung { get; set; } = null!;

    public string LoaiGiaoDich { get; set; } = null!;

    public DateTime? ThoiGianGiaoDich { get; set; }

    public string? MoTa { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
