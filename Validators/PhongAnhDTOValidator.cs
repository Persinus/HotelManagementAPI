using FluentValidation;
using HotelManagementAPI.DTOs;

public class PhongAnhDTOValidator : AbstractValidator<PhongAnhDTO>
{
    public PhongAnhDTOValidator()
    {
        RuleFor(x => x.MaAnh)
            .NotEmpty().WithMessage("Mã ảnh không được để trống.")
            .Length(6).WithMessage("Mã ảnh phải đúng 6 ký tự.");

        RuleFor(x => x.MaPhong)
            .NotEmpty().WithMessage("Mã phòng không được để trống.")
            .Length(6).WithMessage("Mã phòng phải đúng 6 ký tự.");

        RuleFor(x => x.UrlAnh)
            .NotEmpty().WithMessage("Url ảnh không được để trống.");
    }
}