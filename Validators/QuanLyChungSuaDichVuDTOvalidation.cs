using FluentValidation;
using HotelManagementAPI.DTOs.QuanLyChung;

public class QuanLyChungSuaDichVuDTOValidator : AbstractValidator<QuanLyChungSuaDichVuDTO>
{
    public QuanLyChungSuaDichVuDTOValidator()
    {
        RuleFor(x => x.TenDichVu)
            .NotEmpty().WithMessage("Tên dịch vụ là bắt buộc.")
            .MaximumLength(100).WithMessage("Tên dịch vụ tối đa 100 ký tự.");

        RuleFor(x => x.DonGia)
            .GreaterThanOrEqualTo(0).WithMessage("Đơn giá phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.MoTaDichVu)
            .NotEmpty().WithMessage("Mô tả dịch vụ là bắt buộc.");

        RuleFor(x => x.HinhAnhDichVu)
            .NotEmpty().WithMessage("Hình ảnh dịch vụ là bắt buộc.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                         && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            .WithMessage("URL hình ảnh không hợp lệ.");

        RuleFor(x => x.SoLuong)
            .GreaterThanOrEqualTo(0).WithMessage("Số lượng phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.TrangThai)
            .InclusiveBetween((byte)0, (byte)1).WithMessage("Trạng thái phải là 0 (không hoạt động) hoặc 1 (hoạt động).");

        RuleFor(x => x.LoaiDichVu)
            .NotEmpty().WithMessage("Loại dịch vụ là bắt buộc.")
            .MaximumLength(50).WithMessage("Loại dịch vụ tối đa 50 ký tự.");

        RuleFor(x => x.DonViTinh)
            .NotEmpty().WithMessage("Đơn vị tính là bắt buộc.")
            .MaximumLength(50).WithMessage("Đơn vị tính tối đa 50 ký tự.");
    }
}