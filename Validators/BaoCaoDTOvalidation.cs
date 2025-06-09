using FluentValidation;
using HotelManagementAPI.DTOs;

public class BaoCaoDTOValidator : AbstractValidator<BaoCaoDTO>
{
    public BaoCaoDTOValidator()
    {
        RuleFor(x => x.MaBaoCao)
            .NotEmpty().WithMessage("Mã báo cáo là bắt buộc.")
            .Length(6).WithMessage("Mã báo cáo phải đúng 6 ký tự.");

        RuleFor(x => x.LoaiBaoCao)
            .NotEmpty().WithMessage("Loại báo cáo là bắt buộc.");

        RuleFor(x => x.ThoiGian)
            .NotNull().WithMessage("Thời gian là bắt buộc.");

        RuleFor(x => x.NoiDung)
            .NotEmpty().WithMessage("Nội dung là bắt buộc.");
    }
}