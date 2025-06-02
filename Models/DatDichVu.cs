using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class DatDichVu
{
    public string MaDatDichVu { get; set; } = null!;

    public string MaDatPhong { get; set; } = null!;

    public string MaDichVu { get; set; } = null!;

    public int SoLuong { get; set; }

    public virtual DatPhong MaDatPhongNavigation { get; set; } = null!;

    public virtual DichVu MaDichVuNavigation { get; set; } = null!;
}
