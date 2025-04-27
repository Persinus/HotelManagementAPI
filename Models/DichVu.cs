using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class DichVu
{
    public string MaChiTietDichVu { get; set; } = null!;

    public string TenDichVu { get; set; } = null!;

    public decimal DonGia { get; set; }

    public string? MoTaDichVu { get; set; }

    public string? UrlAnh { get; set; }

    public virtual ICollection<HoaDonThanhToanDichVu> HoaDonThanhToanDichVus { get; set; } = new List<HoaDonThanhToanDichVu>();

    public virtual ICollection<KhachHangDichVu> KhachHangDichVus { get; set; } = new List<KhachHangDichVu>();
}
