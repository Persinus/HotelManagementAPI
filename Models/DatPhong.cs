using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class DatPhong
{
    public string MaDatPhong { get; set; } = null!;

    public string MaKhachHang { get; set; } = null!;

    public string MaNhanVien { get; set; } = null!;

    public string MaPhong { get; set; } = null!;

    public DateTime NgayDat { get; set; }

    public DateTime NgayNhanPhong { get; set; }

    public DateTime NgayTraPhong { get; set; }

    public int TrangThai { get; set; }

    public virtual ICollection<HoaDonThanhToanPhong> HoaDonThanhToanPhongs { get; set; } = new List<HoaDonThanhToanPhong>();

    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;

    public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;

    public virtual PhongWithTienNghi MaPhongNavigation { get; set; } = null!;
}
