using FluentValidation;
using HotelManagementAPI.DTOs.KhachHang;

public class HoaDonDTOValidator : AbstractValidator<KhachHangHoaDonDTO>
{
    public HoaDonDTOValidator()
    {
        RuleFor(x => x.MaHoaDon)
            .NotEmpty().WithMessage("Mã hóa đơn là bắt buộc.")
            .Length(6).WithMessage("Mã hóa đơn phải đúng 6 ký tự.");

        RuleFor(x => x.MaNguoiDung)
            .NotEmpty().WithMessage("Mã người dùng là bắt buộc.")
            .Length(6).WithMessage("Mã người dùng phải đúng 6 ký tự.");

        RuleFor(x => x.MaDatPhong)
            .NotEmpty().WithMessage("Mã đặt phòng là bắt buộc.")
            .Length(6).WithMessage("Mã đặt phòng phải đúng 6 ký tự.");

        RuleFor(x => x.TongTien)
            .GreaterThanOrEqualTo(0).WithMessage("Tổng tiền phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.TinhTrangHoaDon)
            .NotEmpty().WithMessage("Tình trạng hóa đơn là bắt buộc.");
    }
}