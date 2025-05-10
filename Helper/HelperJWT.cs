using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HotelManagementAPI.Helper
{
    public static class JwtHelper
    {
        public static string GenerateJwtToken(NguoiDungDTO nguoiDung, string secretKey, string issuer, string audience)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, nguoiDung.MaNguoiDung),
                new Claim(ClaimTypes.Email, nguoiDung.Email),
                new Claim("VaiTro", nguoiDung.Vaitro) // Thêm claim VaiTro
            };

            var token = new JwtSecurityToken(
                issuer: "your-issuer",
                audience: "your-audience",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_super_secret_key_1234567890")),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}