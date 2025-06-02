using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class BaiViet
{
    public string MaBaiViet { get; set; } = null!;

    public string MaNguoiDung { get; set; } = null!;

    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public DateTime? NgayDang { get; set; }

    public string? HinhAnhUrl { get; set; }

    public string? TrangThai { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
