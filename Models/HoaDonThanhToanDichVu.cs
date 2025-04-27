using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class HoaDonThanhToanDichVu
{
    public string MaHoaDonDichVu { get; set; } = null!;

    public string? MaKhachHang { get; set; }

    public string? MaChiTietDichVu { get; set; }

    public string TrangThaiThanhToan { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal ThanhTien { get; set; }

    public DateTime NgayLapHoaDon { get; set; }

    public virtual DichVu? MaChiTietDichVuNavigation { get; set; }

    public virtual KhachHang? MaKhachHangNavigation { get; set; }
}
