using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienThem1DichVuDTOValidator : AbstractValidator<QuanTriVienThem1DichVuDTO>
{
    public QuanTriVienThem1DichVuDTOValidator()
    {
        RuleFor(x => x.TenDichVu)
            .NotEmpty().WithMessage("Tên dịch vụ là bắt buộc.")
            .MaximumLength(100).WithMessage("Tên dịch vụ tối đa 100 ký tự.");

        RuleFor(x => x.DonGia)
            .GreaterThanOrEqualTo(0).WithMessage("Đơn giá phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.MoTaDichVu)
            .NotEmpty().WithMessage("Mô tả dịch vụ là bắt buộc.");

        RuleFor(x => x.SoLuong)
            .GreaterThanOrEqualTo(0).WithMessage("Số lượng phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.LoaiDichVu)
            .NotEmpty().WithMessage("Loại dịch vụ là bắt buộc.")
            .MaximumLength(50).WithMessage("Loại dịch vụ tối đa 50 ký tự.");

        RuleFor(x => x.DonViTinh)
            .NotEmpty().WithMessage("Đơn vị tính là bắt buộc.")
            .MaximumLength(50).WithMessage("Đơn vị tính tối đa 50 ký tự.");
    }
}