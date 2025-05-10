using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class Phong
{
    public string MaPhong { get; set; } = null!;

    public string LoaiPhong { get; set; } = null!;

    public decimal GiaPhong { get; set; }

    public string TinhTrang { get; set; } = null!;

    public int SoLuongPhong { get; set; }

    public int Tang { get; set; }

    public string KieuGiuong { get; set; } = null!;

    public string? MoTa { get; set; }

    public string UrlAnhChinh { get; set; } = null!;

    public string? MotaPhong { get; set; }

    public int SucChua { get; set; }

    public int SoGiuong { get; set; }

    public string DonViTinh { get; set; } = null!;

    public decimal SoSaoTrungBinh { get; set; }

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<LichSuTinhTrangPhong> LichSuTinhTrangPhongs { get; set; } = new List<LichSuTinhTrangPhong>();

    public virtual ICollection<PhongAnh> PhongAnhs { get; set; } = new List<PhongAnh>();

    public virtual ICollection<GiamGium> MaGiamGia { get; set; } = new List<GiamGium>();

    public virtual ICollection<TienNghi> MaTienNghis { get; set; } = new List<TienNghi>();
}
