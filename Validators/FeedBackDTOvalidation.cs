using FluentValidation;
using HotelManagementAPI.DTOs;

public class FeedBackDTOvalidation : AbstractValidator<FeedBackDTO>
{
    public FeedBackDTOvalidation()
    {
        RuleFor(x => x.MaPhong)
            .NotEmpty().WithMessage("Mã phòng là bắt buộc.");

        RuleFor(x => x.SoSao)
            .InclusiveBetween(1, 5).WithMessage("Số sao phải từ 1 đến 5.");

        RuleFor(x => x.BinhLuan)
            .MaximumLength(500).WithMessage("Bình luận tối đa 500 ký tự.");

        RuleFor(x => x.PhanLoai)
            .NotNull().WithMessage("Phân loại là bắt buộc.");
    }
}