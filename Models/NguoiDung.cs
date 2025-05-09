using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class NguoiDung
{
    public string MaNguoiDung { get; set; } = null!;

    public string Vaitro { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? TenTaiKhoan { get; set; }

    public string? MatKhau { get; set; }

    public string? HoTen { get; set; }

    public string? CanCuocCongDan { get; set; }

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string HinhAnhUrl { get; set; } = null!;

    public string LoaiDangNhap { get; set; } = null!;

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ICollection<LichSuGiaoDich> LichSuGiaoDiches { get; set; } = new List<LichSuGiaoDich>();

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
