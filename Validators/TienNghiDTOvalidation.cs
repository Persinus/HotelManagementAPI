using FluentValidation;
using HotelManagementAPI.DTOs;

public class TienNghiDTOValidator : AbstractValidator<TienNghiDTO>
{
    public TienNghiDTOValidator()
    {
        RuleFor(x => x.MaTienNghi)
            .NotEmpty().WithMessage("Mã tiện nghi là bắt buộc.")
            .Length(6).WithMessage("Mã tiện nghi phải đúng 6 ký tự.");

        RuleFor(x => x.TenTienNghi)
            .NotEmpty().WithMessage("Tên tiện nghi là bắt buộc.")
            .MaximumLength(100).WithMessage("Tên tiện nghi tối đa 100 ký tự.");

        // MoTa là optional, không cần rule bắt buộc
    }
}