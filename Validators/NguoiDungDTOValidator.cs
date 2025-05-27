using FluentValidation;
using HotelManagementAPI.DTOs;

public class NguoiDungDTOValidator : AbstractValidator<NguoiDungDTO>
{
    public NguoiDungDTOValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email là bắt buộc.")
            .EmailAddress().WithMessage("Email không hợp lệ.");

        RuleFor(x => x.TenTaiKhoan)
            .NotEmpty().WithMessage("Tên tài khoản là bắt buộc.")
            .MaximumLength(50).WithMessage("Tên tài khoản không được vượt quá 50 ký tự.");

        RuleFor(x => x.MatKhau)
            .NotEmpty().WithMessage("Mật khẩu là bắt buộc.")
            .MaximumLength(100).WithMessage("Mật khẩu không được vượt quá 100 ký tự.");

        RuleFor(x => x.HoTen)
            .MaximumLength(100).WithMessage("Họ tên không được vượt quá 100 ký tự.");

        RuleFor(x => x.SoDienThoai)
            .NotEmpty().WithMessage("Số điện thoại là bắt buộc.");

        RuleFor(x => x.DiaChi)
            .NotEmpty().WithMessage("Địa chỉ là bắt buộc.");

        RuleFor(x => x.CanCuocCongDan)
            .NotEmpty().WithMessage("Căn cước công dân là bắt buộc.")
            .MaximumLength(12).WithMessage("Căn cước công dân không được vượt quá 12 ký tự.");
    }
}