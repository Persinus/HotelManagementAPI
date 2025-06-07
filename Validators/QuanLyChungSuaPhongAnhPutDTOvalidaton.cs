using FluentValidation;
using HotelManagementAPI.DTOs.QuanLyChung;

public class QuanLyChungSuaPhongAnhDTOValidator : AbstractValidator<QuanLyChungSuaPhongAnhDTO>
{
    public QuanLyChungSuaPhongAnhDTOValidator()
    {
        RuleFor(x => x.UrlAnh)
            .NotEmpty().WithMessage("URL ảnh là bắt buộc.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                         && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            .WithMessage("URL ảnh không hợp lệ.");
    }
}