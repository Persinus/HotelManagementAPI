using FluentValidation;
using HotelManagementAPI.DTOs;

public class LoginDTOValidator : AbstractValidator<LoginDTO>
{
    public LoginDTOValidator()
    {
        RuleFor(x => x.TenTaiKhoan)
            .NotEmpty().WithMessage("Tên tài khoản không được để trống.");

        RuleFor(x => x.MatKhau)
            .NotEmpty().WithMessage("Mật khẩu không được để trống.");
    }
}