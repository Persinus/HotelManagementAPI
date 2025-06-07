using FluentValidation;
using HotelManagementAPI.DTOs.NhanVien;

public class NhanVienThemBaiVietDTOValidator : AbstractValidator<NhanVienThemBaiVietDTO>
{
    public NhanVienThemBaiVietDTOValidator()
    {
        RuleFor(x => x.TieuDe)
            .NotEmpty().WithMessage("Tiêu đề là bắt buộc.")
            .MaximumLength(200).WithMessage("Tiêu đề tối đa 200 ký tự.");

        RuleFor(x => x.NoiDung)
            .NotEmpty().WithMessage("Nội dung là bắt buộc.");
    }
}