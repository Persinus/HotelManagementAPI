using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienXoa1TienNghiDTOValidator : AbstractValidator<QuanTriVienXoa1TienNghiDTO>
{
    public QuanTriVienXoa1TienNghiDTOValidator()
    {
        RuleFor(x => x.MaTienNghi)
            .NotEmpty().WithMessage("Mã tiện nghi là bắt buộc.");
    }
}