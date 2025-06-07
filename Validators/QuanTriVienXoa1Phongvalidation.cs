using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienXoa1PhongDTOValidator : AbstractValidator<QuanTriVienXoa1PhongDTO>
{
    public QuanTriVienXoa1PhongDTOValidator()
    {
        RuleFor(x => x.MaPhong)
            .NotEmpty().WithMessage("Mã phòng là bắt buộc.");
    }
}