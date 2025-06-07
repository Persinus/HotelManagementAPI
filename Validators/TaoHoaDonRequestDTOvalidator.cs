using FluentValidation;
using HotelManagementAPI.DTOs.KhachHang;

public class TaoHoaDonRequestDTOValidator : AbstractValidator<TaoHoaDonRequestDTO>
{
    public TaoHoaDonRequestDTOValidator()
    {
        RuleFor(x => x.MaDatPhong)
            .NotEmpty().WithMessage("Mã đặt phòng là bắt buộc.");

        RuleFor(x => x.GiaTriGiam)
            .GreaterThanOrEqualTo(0).WithMessage("Giá trị giảm phải lớn hơn hoặc bằng 0.");
    }
}