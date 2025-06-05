//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementAPI.DTOs.QuanTriVien
{
    public class QuanTriVienSuaNoiQuyDTO
    {
        public int Id { get; set; }
        public string TenNoiQuy { get; set; }
        public string MoTa { get; set; }
        public DateTime NgayCapNhat { get; set; }
    }
}