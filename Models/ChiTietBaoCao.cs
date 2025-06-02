using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class ChiTietBaoCao
{
    public string MaChiTiet { get; set; } = null!;

    public string MaBaoCao { get; set; } = null!;

    public string NoiDungChiTiet { get; set; } = null!;

    public decimal GiaTri { get; set; }

    public virtual BaoCao MaBaoCaoNavigation { get; set; } = null!;
}
