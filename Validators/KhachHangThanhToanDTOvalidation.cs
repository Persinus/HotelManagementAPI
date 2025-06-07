using FluentValidation;
using HotelManagementAPI.DTOs.KhachHang;

public class KhachHangThanhToanDTOValidator : AbstractValidator<KhachHangThanhToanDTO>
{
    public KhachHangThanhToanDTOValidator()
    {
        RuleFor(x => x.MaThanhToan)
            .NotEmpty().WithMessage("Mã thanh toán là bắt buộc.")
            .Length(6).WithMessage("Mã thanh toán phải đúng 6 ký tự.");

        RuleFor(x => x.MaHoaDon)
            .NotEmpty().WithMessage("Mã hóa đơn là bắt buộc.")
            .Length(6).WithMessage("Mã hóa đơn phải đúng 6 ký tự.");

        RuleFor(x => x.SoTienThanhToan)
            .GreaterThanOrEqualTo(0).WithMessage("Số tiền thanh toán phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.NgayThanhToan)
            .NotNull().WithMessage("Ngày thanh toán là bắt buộc.");

        RuleFor(x => x.PhuongThucThanhToan)
            .NotEmpty().WithMessage("Phương thức thanh toán là bắt buộc.");

        RuleFor(x => x.TinhTrangThanhToan)
            .NotEmpty().WithMessage("Tình trạng thanh toán là bắt buộc.")
            .Length(1).WithMessage("Tình trạng thanh toán phải đúng 1 ký tự.");
    }
}