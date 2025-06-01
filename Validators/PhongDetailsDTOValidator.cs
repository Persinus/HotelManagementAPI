using FluentValidation;
using HotelManagementAPI.DTOs;

public class PhongDetailsDTOValidator : AbstractValidator<PhongDetailsDTO>
{
    public PhongDetailsDTOValidator()
    {
        RuleFor(x => x.MaPhong).NotEmpty();
        RuleFor(x => x.LoaiPhong).NotEmpty();
        RuleFor(x => x.GiaPhong).GreaterThanOrEqualTo(0).WithMessage("Giá phòng phải lớn hơn hoặc bằng 0.");
        RuleFor(x => x.TinhTrang).NotEmpty();
        RuleFor(x => x.Tang).GreaterThanOrEqualTo(1).WithMessage("Tầng phải lớn hơn hoặc bằng 1.");
        RuleFor(x => x.KieuGiuong).NotEmpty();
        RuleFor(x => x.UrlAnhChinh).NotEmpty().Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("URL ảnh chính không hợp lệ.");
        RuleFor(x => x.SucChua).GreaterThanOrEqualTo(1).WithMessage("Sức chứa phải lớn hơn hoặc bằng 1.");
        RuleFor(x => x.SoGiuong).GreaterThanOrEqualTo(1).WithMessage("Số giường phải lớn hơn hoặc bằng 1.");
        RuleFor(x => x.DonViTinh).NotEmpty();
        RuleFor(x => x.SoSaoTrungBinh).InclusiveBetween(0, 5).WithMessage("Số sao trung bình phải nằm trong khoảng từ 0 đến 5.");
       
    }
}