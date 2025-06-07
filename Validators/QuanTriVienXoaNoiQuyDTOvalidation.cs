using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienXoaNoiQuyDTOValidator : AbstractValidator<QuanTriVienXoaNoiQuyDTO>
{
    public QuanTriVienXoaNoiQuyDTOValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id phải lớn hơn 0.");
    }
}