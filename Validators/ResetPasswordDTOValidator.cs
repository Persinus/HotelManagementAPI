using FluentValidation;
using HotelManagementAPI.DTOs;

public class ResetPasswordDTOValidator : AbstractValidator<ResetPasswordDTO>
{
    public ResetPasswordDTOValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email là bắt buộc.")
            .EmailAddress().WithMessage("Email không hợp lệ.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Mật khẩu mới là bắt buộc.")
            .MaximumLength(100).WithMessage("Mật khẩu không được vượt quá 100 ký tự.");
    }
}