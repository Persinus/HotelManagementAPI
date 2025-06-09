using FluentValidation;
using HotelManagementAPI.DTOs.NhanVien;

public class ChiTietBaoCaoDTOValidator : AbstractValidator<ChiTietBaoCaoDTO>
{
    public ChiTietBaoCaoDTOValidator()
    {
        RuleFor(x => x.MaChiTiet)
            .NotEmpty().WithMessage("Mã chi tiết là bắt buộc.")
            .Length(6).WithMessage("Mã chi tiết phải đúng 6 ký tự.");

        RuleFor(x => x.MaBaoCao)
            .NotEmpty().WithMessage("Mã báo cáo là bắt buộc.")
            .Length(6).WithMessage("Mã báo cáo phải đúng 6 ký tự.");

        RuleFor(x => x.NoiDungChiTiet)
            .NotEmpty().WithMessage("Nội dung chi tiết là bắt buộc.");

        RuleFor(x => x.GiaTri)
            .GreaterThanOrEqualTo(0).WithMessage("Giá trị phải lớn hơn hoặc bằng 0.");
    }
}