using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienThem1PhongDTOValidator : AbstractValidator<QuanTriVienThem1PhongDTO>
{
    public QuanTriVienThem1PhongDTOValidator()
    {
        RuleFor(x => x.LoaiPhong)
            .NotEmpty().WithMessage("Loại phòng là bắt buộc.");

        RuleFor(x => x.GiaPhong)
            .GreaterThanOrEqualTo(0).WithMessage("Giá phòng phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.TinhTrang)
            .InclusiveBetween((byte)0, (byte)2).WithMessage("Tình trạng phải là 0, 1 hoặc 2.");

        RuleFor(x => x.SoLuongPhong)
            .GreaterThanOrEqualTo(1).WithMessage("Số lượng phòng phải lớn hơn hoặc bằng 1.");

        RuleFor(x => x.Tang)
            .GreaterThanOrEqualTo(0).WithMessage("Tầng phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.KieuGiuong)
            .NotEmpty().WithMessage("Kiểu giường là bắt buộc.");

        RuleFor(x => x.MoTa)
            .MaximumLength(255).WithMessage("Mô tả tối đa 255 ký tự.");

        RuleFor(x => x.SucChua)
            .GreaterThanOrEqualTo(1).WithMessage("Sức chứa phải lớn hơn hoặc bằng 1.");

        RuleFor(x => x.SoGiuong)
            .GreaterThanOrEqualTo(1).WithMessage("Số giường phải lớn hơn hoặc bằng 1.");
    }
}