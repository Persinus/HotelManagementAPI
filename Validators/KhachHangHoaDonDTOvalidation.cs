using FluentValidation;
using HotelManagementAPI.DTOs.KhachHang;

public class KhachHangHoaDonDTOValidator : AbstractValidator<KhachHangHoaDonDTO>
{
    public KhachHangHoaDonDTOValidator()
    {
        RuleFor(x => x.MaHoaDon)
            .NotEmpty().WithMessage("Mã hóa đơn là bắt buộc.");

        RuleFor(x => x.MaNguoiDung)
            .NotEmpty().WithMessage("Mã người dùng là bắt buộc.");

        RuleFor(x => x.MaDatPhong)
            .NotEmpty().WithMessage("Mã đặt phòng là bắt buộc.");

        RuleFor(x => x.TongTien)
            .GreaterThanOrEqualTo(0).WithMessage("Tổng tiền phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.TinhTrangHoaDon)
            .InclusiveBetween((byte)1, (byte)2).WithMessage("Tình trạng hóa đơn phải là 1 (Chưa thanh toán) hoặc 2 (Đã thanh toán).");
    }
}

