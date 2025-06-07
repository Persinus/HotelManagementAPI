using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienThem1NoiQuyDTOValidator : AbstractValidator<QuanTriVienThem1NoiQuyDTO>
{
    public QuanTriVienThem1NoiQuyDTOValidator()
    {
        RuleFor(x => x.TenNoiQuy)
            .NotEmpty().WithMessage("Tên nội quy là bắt buộc.")
            .MaximumLength(100).WithMessage("Tên nội quy tối đa 100 ký tự.");

        RuleFor(x => x.MoTa)
            .NotEmpty().WithMessage("Mô tả là bắt buộc.")
            .MaximumLength(255).WithMessage("Mô tả tối đa 255 ký tự.");

        RuleFor(x => x.NgayTao)
            .NotEmpty().WithMessage("Ngày tạo là bắt buộc.");

        RuleFor(x => x.NgayCapNhat)
            .NotEmpty().WithMessage("Ngày cập nhật là bắt buộc.");
    }
}