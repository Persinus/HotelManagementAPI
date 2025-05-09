using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class DatPhong
{
    public string MaDatPhong { get; set; } = null!;

    public string MaNguoiDung { get; set; } = null!;

    public string MaPhong { get; set; } = null!;

    public DateTime? NgayDat { get; set; }

    public DateTime? NgayCheckIn { get; set; }

    public DateTime? NgayCheckOut { get; set; }

    public string TinhTrangDatPhong { get; set; } = null!;

    public virtual ICollection<DatDichVu> DatDichVus { get; set; } = new List<DatDichVu>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    public virtual Phong MaPhongNavigation { get; set; } = null!;
}
