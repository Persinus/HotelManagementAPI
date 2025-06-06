using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs.KhachHang
{
    /// <summary>
    /// DTO để tạo hóa đơn từ yêu cầu của khách hàng.
    /// </summary>
    /// <remarks>
    /// Chứa thông tin cần thiết để tạo hóa đơn, bao gồm mã đặt phòng và giá trị giảm giá.
    /// </remarks>
public class TaoHoaDonRequestDTO
{
    public string MaDatPhong { get; set; } = null!;
    public decimal GiaTriGiam { get; set; } // Thêm dòng này
    // ... các trường khác nếu có ...
} 
}