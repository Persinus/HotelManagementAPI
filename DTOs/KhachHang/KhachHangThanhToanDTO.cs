using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.KhachHang
{
    public class KhachHangThanhToanDTO
    {


        [Required]
        [StringLength(6)]
        public string MaHoaDon { get; set; } = null!;

     
        public decimal SoTienThanhToan { get; set; }

     

        [Required]
        public string PhuongThucThanhToan { get; set; } = null!;

     
    }}