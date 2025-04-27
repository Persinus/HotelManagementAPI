using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class HoaDonThanhToanPhong
{
    public string MaHoaDon { get; set; } = null!;

    public string MaDatPhong { get; set; } = null!;

    public string MaNhanVien { get; set; } = null!;

    public DateTime NgayLapHoaDon { get; set; }

    public decimal TongTien { get; set; }

    public string TrangThaiThanhToan { get; set; } = null!;

    public int HinhThucThanhToan { get; set; }

    public virtual DatPhong MaDatPhongNavigation { get; set; } = null!;

    public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;
}
