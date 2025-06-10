using FluentValidation;
using HotelManagementAPI.DTOs.KhachHang;

public class ThanhToanDTOValidator : AbstractValidator<KhachHangThanhToanDTO>
{
    public ThanhToanDTOValidator()
    {
       

        RuleFor(x => x.MaHoaDon)
            .NotEmpty().WithMessage("Mã hóa đơn là bắt buộc.")
            .Length(5).WithMessage("Mã hóa đơn phải đúng 6 ký tự.");

        RuleFor(x => x.SoTienThanhToan)
            .GreaterThanOrEqualTo(0).WithMessage("Số tiền thanh toán phải lớn hơn hoặc bằng 0.");

       
    }
}