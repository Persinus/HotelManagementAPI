using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienThemAnhDTOValidator : AbstractValidator<QuanTriVienThemAnhDTO>
{
    public QuanTriVienThemAnhDTOValidator()
    {
        RuleFor(x => x.MaPhong)
            .NotEmpty().WithMessage("Mã phòng là bắt buộc.");
    }
}