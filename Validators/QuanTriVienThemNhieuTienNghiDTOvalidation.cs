using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienThemNhieuTienNghiDTOValidator : AbstractValidator<QuanTriVienThemNhieuTienNghiDTO>
{
    public QuanTriVienThemNhieuTienNghiDTOValidator()
    {
        RuleFor(x => x.DanhSachTienNghi)
            .NotNull().WithMessage("Danh sách tiện nghi là bắt buộc.")
            .Must(list => list.Count > 0).WithMessage("Danh sách tiện nghi không được để trống.")
            .ForEach(tn => tn.SetValidator(new QuanTriVienThem1TienNghiDTOValidator()));
    }
}