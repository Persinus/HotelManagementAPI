using System;
using System.Collections.Generic;

namespace HotelManagementAPI.Models;

public partial class PhongYeuThich
{
    public int Id { get; set; }

    public string MaPhong { get; set; } = null!;

    public string MaNguoiDung { get; set; } = null!;

    public DateTime? NgayYeuThich { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    public virtual Phong MaPhongNavigation { get; set; } = null!;
}
