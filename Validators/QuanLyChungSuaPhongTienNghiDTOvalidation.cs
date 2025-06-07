using FluentValidation;
using HotelManagementAPI.DTOs.QuanLyChung;

public class QuanLyChungSuaPhongTienNghiDTOValidator : AbstractValidator<QuanLyChungSuaPhongTienNghiDTO>
{
    public QuanLyChungSuaPhongTienNghiDTOValidator()
    {
        RuleFor(x => x.TenTienNghi)
            .NotEmpty().WithMessage("Tên tiện nghi là bắt buộc.")
            .MaximumLength(100).WithMessage("Tên tiện nghi tối đa 100 ký tự.");

        RuleFor(x => x.MoTa)
            .MaximumLength(255).WithMessage("Mô tả tối đa 255 ký tự.");
    }
}