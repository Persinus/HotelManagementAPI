using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class NoiQuy
{
    public int Id { get; set; }

    public int SoThuTu { get; set; }

    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public string? HinhAnh { get; set; }
}
