using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class GiamGium
{
    public string MaGiamGia { get; set; } = null!;

    public string TenGiamGia { get; set; } = null!;

    public string? LoaiGiamGia { get; set; }

    public decimal GiaTriGiam { get; set; }

    public DateTime NgayBatDau { get; set; }

    public DateTime NgayKetThuc { get; set; }

    public string? MoTa { get; set; }

    public virtual ICollection<Phong> MaPhongs { get; set; } = new List<Phong>();
}
