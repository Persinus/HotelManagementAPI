using FluentValidation;
using HotelManagementAPI.DTOs.QuanTriVien;

public class QuanTriVienSuaRoleDTOValidator : AbstractValidator<QuanTriVienSuaRoleDTO>
{
    public QuanTriVienSuaRoleDTOValidator()
    {
        RuleFor(x => x.VaiTroMoi)
            .NotEmpty().WithMessage("Vai trò mới là bắt buộc.");
    }
}