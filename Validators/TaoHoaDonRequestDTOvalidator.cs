using FluentValidation;
using HotelManagementAPI.DTOs.KhachHang;

public class TaoHoaDonRequestDTOValidator : AbstractValidator<TaoHoaDonRequestDTO>
{
    public TaoHoaDonRequestDTOValidator()
    {
        RuleFor(x => x.MaDatPhong)
            .NotEmpty().WithMessage("Mã đặt phòng là bắt buộc.");

     
    }
}