using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class NhanVien
{
    public string MaNhanVien { get; set; } = null!;

    public string MaNguoiDung { get; set; } = null!;

    public string ChucVu { get; set; } = null!;

    public decimal Luong { get; set; }

    public DateTime? NgayVaoLam { get; set; }

    public string? CaLamViec { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
