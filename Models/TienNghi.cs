using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class TienNghi
{
    public string MaTienNghi { get; set; } = null!;

    public string TenTienNghi { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<Phong> MaPhongs { get; set; } = new List<Phong>();
}
