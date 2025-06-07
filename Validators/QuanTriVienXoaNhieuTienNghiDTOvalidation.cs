using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienXoaNhieuTienNghiDTOValidator : AbstractValidator<QuanTriVienXoaNhieuTienNghiDTO>
{
    public QuanTriVienXoaNhieuTienNghiDTOValidator()
    {
        RuleFor(x => x.DanhSachMaTienNghi)
            .NotNull().WithMessage("Danh sách mã tiện nghi là bắt buộc.")
            .Must(list => list.Count > 0).WithMessage("Danh sách mã tiện nghi không được để trống.");
    }
}