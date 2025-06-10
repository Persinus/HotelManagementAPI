using FluentValidation;
using HotelManagementAPI.DTOs.KhachHang;

public class KhachHangThanhToanDTOValidator : AbstractValidator<KhachHangThanhToanDTO>
{
    public KhachHangThanhToanDTOValidator()
    {
      

        RuleFor(x => x.MaHoaDon)
            .NotEmpty().WithMessage("Mã hóa đơn là bắt buộc.")
            .Length(5).WithMessage("Mã hóa đơn phải đúng 6 ký tự.");

        RuleFor(x => x.SoTienThanhToan)
            .GreaterThanOrEqualTo(0).WithMessage("Số tiền thanh toán phải lớn hơn hoặc bằng 0.");


        RuleFor(x => x.PhuongThucThanhToan)
            .NotEmpty().WithMessage("Phương thức thanh toán là bắt buộc.");

       
    }
}