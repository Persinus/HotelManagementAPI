using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienThem1TienNghiDTOValidator : AbstractValidator<QuanTriVienThem1TienNghiDTO>
{
    public QuanTriVienThem1TienNghiDTOValidator()
    {
        RuleFor(x => x.TenTienNghi)
            .NotEmpty().WithMessage("Tên tiện nghi là bắt buộc.")
            .MaximumLength(100).WithMessage("Tên tiện nghi tối đa 100 ký tự.");

        RuleFor(x => x.MoTa)
            .MaximumLength(500).WithMessage("Mô tả tối đa 500 ký tự.");
    }
}