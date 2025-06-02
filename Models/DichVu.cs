using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class DichVu
{
    public string MaDichVu { get; set; } = null!;

    public string TenDichVu { get; set; } = null!;

    public decimal DonGia { get; set; }

    public string? MoTaDichVu { get; set; }

    public string HinhAnhDichVu { get; set; } = null!;

    public int SoLuong { get; set; }

    public byte TrangThai { get; set; }

    public string LoaiDichVu { get; set; } = null!;

    public string DonViTinh { get; set; } = null!;

    public virtual ICollection<DatDichVu> DatDichVus { get; set; } = new List<DatDichVu>();
}
