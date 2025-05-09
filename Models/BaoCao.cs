using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class BaoCao
{
    public string MaBaoCao { get; set; } = null!;

    public string LoaiBaoCao { get; set; } = null!;

    public DateTime? ThoiGian { get; set; }

    public string? NoiDung { get; set; }

    public virtual ICollection<ChiTietBaoCao> ChiTietBaoCaos { get; set; } = new List<ChiTietBaoCao>();
}
