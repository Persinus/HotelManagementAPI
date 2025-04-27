using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class QuanTriVien
{
    public string MaQuanTri { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string TenTaiKhoan { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string TenAdmin { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string? Jwk { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }
}
