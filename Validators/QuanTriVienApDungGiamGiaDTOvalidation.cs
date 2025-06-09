using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienApDungGiamGiaDTOValidator : AbstractValidator<QuanTriVienApDungGiamGiaDTO>
{
    public QuanTriVienApDungGiamGiaDTOValidator()
    {
        RuleFor(x => x.MaGiamGia)
            .NotEmpty().WithMessage("Mã giảm giá là bắt buộc.");

        RuleFor(x => x.TenGiamGia)
            .NotEmpty().WithMessage("Tên giảm giá là bắt buộc.");

        RuleFor(x => x.GiaTriGiam)
            .GreaterThanOrEqualTo(0).WithMessage("Giá trị giảm phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.NgayBatDau)
            .NotEmpty().WithMessage("Ngày bắt đầu là bắt buộc.");

        RuleFor(x => x.NgayKetThuc)
            .NotEmpty().WithMessage("Ngày kết thúc là bắt buộc.")
            .GreaterThanOrEqualTo(x => x.NgayBatDau).WithMessage("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");

        RuleFor(x => x.MoTa)
            .MaximumLength(255).WithMessage("Mô tả tối đa 255 ký tự.");

        RuleFor(x => x.DanhSachMaPhong)
            .NotNull().WithMessage("Danh sách mã phòng là bắt buộc.")
            .Must(list => list.Count > 0).WithMessage("Danh sách mã phòng không được để trống.");
    }
}