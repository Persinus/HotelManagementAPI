using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienThemNhieuDichVuDTOValidator : AbstractValidator<QuanTriVienThemNhieuDichVuDTO>
{
    public QuanTriVienThemNhieuDichVuDTOValidator()
    {
        RuleFor(x => x.DanhSachDichVu)
            .NotNull().WithMessage("Danh sách dịch vụ là bắt buộc.")
            .Must(list => list.Count > 0).WithMessage("Danh sách dịch vụ không được để trống.")
            .ForEach(dv => dv.SetValidator(new QuanTriVienThem1DichVuDTOValidator()));
    }
}