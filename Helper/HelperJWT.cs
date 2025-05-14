using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HotelManagementAPI.Models; // Added namespace for NguoiDung

namespace HotelManagementAPI.Helper
{
    public static class JwtHelper
    {
        public static string GenerateJwtToken(NguoiDung nguoiDung, string secretKey, string issuer, string audience)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nguoiDung.MaNguoiDung.ToString()),
                new Claim("Vaitro", nguoiDung.Vaitro), // Đảm bảo claim "Vaitro" được thêm
                new Claim(JwtRegisteredClaimNames.Email, nguoiDung.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Ghi log các claims khi tạo token
            Console.WriteLine("Claims khi tạo JWT token:");
            foreach (var claim in claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}