using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class NhanVien
{
    public string MaNhanVien { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string CanCuocCongDan { get; set; } = null!;

    public string? HinhAnh { get; set; }

    public DateOnly NgayVaoLam { get; set; }

    public string? TrangThai { get; set; }

    public string Email { get; set; } = null!;

    public string TenTaiKhoan { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? Jwk { get; set; }

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    public virtual ICollection<HoaDonThanhToanPhong> HoaDonThanhToanPhongs { get; set; } = new List<HoaDonThanhToanPhong>();
}
