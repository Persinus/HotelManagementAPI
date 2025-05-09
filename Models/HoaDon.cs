using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class HoaDon
{
    public string MaHoaDon { get; set; } = null!;

    public string MaNguoiDung { get; set; } = null!;

    public string MaDatPhong { get; set; } = null!;

    public decimal TongTien { get; set; }

    public DateTime? NgayTaoHoaDon { get; set; }

    public DateTime? NgayThanhToan { get; set; }

    public string TinhTrangHoaDon { get; set; } = null!;

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual ICollection<DatDichVu> DatDichVus { get; set; } = new List<DatDichVu>();

    public virtual DatPhong MaDatPhongNavigation { get; set; } = null!;

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
