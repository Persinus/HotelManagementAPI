using FluentValidation;
using HotelManagementAPI.DTOs;

public class TatCaBaiVietDTOValidator : AbstractValidator<TatCaBaiVietDTO>
{
    public TatCaBaiVietDTOValidator()
    {
        RuleFor(x => x.MaBaiViet)
            .NotEmpty().WithMessage("Mã bài viết là bắt buộc.");

        RuleFor(x => x.MaNguoiDung)
            .NotEmpty().WithMessage("Mã người dùng là bắt buộc.");

        RuleFor(x => x.TieuDe)
            .NotEmpty().WithMessage("Tiêu đề là bắt buộc.")
            .MaximumLength(200).WithMessage("Tiêu đề tối đa 200 ký tự.");

        RuleFor(x => x.NoiDung)
            .NotEmpty().WithMessage("Nội dung là bắt buộc.");

        // NgayDang, HinhAnhUrl, TrangThai là optional, không cần rule bắt buộc
    }
}