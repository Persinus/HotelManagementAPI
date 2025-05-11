using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class Feedback
{
    public string MaFeedback { get; set; } = null!;

    public string MaPhong { get; set; } = null!;

    public string MaNguoiDung { get; set; } = null!;

    public int SoSao { get; set; }

    public string? BinhLuan { get; set; }

    public DateTime? NgayFeedback { get; set; }

    public string PhanLoai { get; set; } = null!;

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
    public virtual Phong MaPhongNavigation { get; set; } = null!;
}
