using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienThemMaGiamGiaDTOValidator : AbstractValidator<QuanTriVienThemMaGiamGiaDTO>
{
    public QuanTriVienThemMaGiamGiaDTOValidator()
    {
        RuleFor(x => x.TenGiamGia)
            .NotEmpty().WithMessage("Tên giảm giá là bắt buộc.")
            .MaximumLength(50).WithMessage("Tên giảm giá tối đa 50 ký tự.");

        RuleFor(x => x.LoaiGiamGia)
            .NotEmpty().WithMessage("Loại giảm giá là bắt buộc.")
            .Length(6).WithMessage("Loại giảm giá phải đúng 6 ký tự.");

        RuleFor(x => x.GiaTriGiam)
            .GreaterThanOrEqualTo(0).WithMessage("Giá trị giảm phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.NgayBatDau)
            .NotEmpty().WithMessage("Ngày bắt đầu là bắt buộc.");

        RuleFor(x => x.NgayKetThuc)
            .NotEmpty().WithMessage("Ngày kết thúc là bắt buộc.")
            .GreaterThanOrEqualTo(x => x.NgayBatDau).WithMessage("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");

        RuleFor(x => x.MoTa)
            .MaximumLength(255).WithMessage("Mô tả tối đa 255 ký tự.");
    }
}