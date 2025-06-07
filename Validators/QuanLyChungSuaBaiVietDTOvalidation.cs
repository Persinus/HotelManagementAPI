using FluentValidation;
using HotelManagementAPI.DTOs.QuanLyChung;

public class QuanLyChungSuaBaiVietDTOValidator : AbstractValidator<QuanLyChungSuaBaiVietDTO>
{
    public QuanLyChungSuaBaiVietDTOValidator()
    {
        RuleFor(x => x.MaBaiViet)
            .NotEmpty().WithMessage("Mã bài viết là bắt buộc.");

        RuleFor(x => x.TieuDe)
            .NotEmpty().WithMessage("Tiêu đề là bắt buộc.")
            .MaximumLength(100).WithMessage("Tiêu đề tối đa 100 ký tự.");

        RuleFor(x => x.NoiDung)
            .NotEmpty().WithMessage("Nội dung là bắt buộc.");
    }
}