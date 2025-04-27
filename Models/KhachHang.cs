using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class KhachHang
{
    public string MaKhachHang { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string CanCuocCongDan { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string? DiaChi { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? NgheNghiep { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public string? HinhAnh { get; set; }

    public string Email { get; set; } = null!;

    public string TenTaiKhoan { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? Jwk { get; set; }

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    public virtual ICollection<HoaDonThanhToanDichVu> HoaDonThanhToanDichVus { get; set; } = new List<HoaDonThanhToanDichVu>();

    public virtual ICollection<KhachHangDichVu> KhachHangDichVus { get; set; } = new List<KhachHangDichVu>();

    public virtual ICollection<KhachHangPhanThuong> KhachHangPhanThuongs { get; set; } = new List<KhachHangPhanThuong>();

    public virtual ICollection<UserFeedback> UserFeedbacks { get; set; } = new List<UserFeedback>();

    public virtual ICollection<VeGiamGium> VeGiamGia { get; set; } = new List<VeGiamGium>();
}
