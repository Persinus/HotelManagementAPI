using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HotelManagementAPI.Models; // Thêm namespace để truy cập class NguoiDung

namespace HotelManagementAPI.Helper
{
    /// <summary>
    /// Lớp hỗ trợ tạo JWT token từ thông tin người dùng.
    /// 
    /// 📚 Giải thích thuật ngữ:
    /// - 🔐 JWT (JSON Web Token): Chuỗi mã hóa gồm 3 phần: Header, Payload, Signature. Dùng để xác thực người dùng (stateless).
    /// - 🧾 Claim: Là một "thông tin khai báo" như Id, email, vai trò,... được nhúng trong Token.
    /// - 🔑 Secret key: Chuỗi bí mật (server giữ kín), dùng để ký token, giúp xác minh tính hợp lệ.
    /// - ⏳ Expire: Token có hạn sử dụng, hết hạn thì buộc đăng nhập lại.
    /// - 🛡️ SigningCredentials: Thông tin dùng để ký token với thuật toán (vd: HMAC SHA256).
    /// - 📤 Issuer: Ai phát hành token | 📥 Audience: Ai được phép nhận token.
    /// 
    /// ⚠️ Token không mã hóa dữ liệu, chỉ **ký** để đảm bảo tính toàn vẹn. KHÔNG nên nhét dữ liệu nhạy cảm vào JWT.
    /// </summary>
    public static class JwtHelper
    {
        /// <summary>
        /// Tạo JWT token từ thông tin người dùng, sử dụng secret key, issuer và audience
        /// </summary>
        /// <param name="nguoiDung">Thông tin người dùng để tạo claims</param>
        /// <param name="secretKey">Chuỗi bí mật dùng để ký token</param>
        /// <param name="issuer">Ai phát hành token (vd: hệ thống bạn)</param>
        /// <param name="audience">Ai sẽ sử dụng token này</param>
        /// <returns>Chuỗi JWT token hợp lệ</returns>
        public static string GenerateJwtToken(NguoiDung nguoiDung, string secretKey, string issuer, string audience)
        {
            // 1. Tạo danh sách claims (thông tin sẽ được nhúng vào token)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nguoiDung.MaNguoiDung.ToString()), // Mã người dùng
                new Claim("Vaitro", nguoiDung.Vaitro), // Vai trò: Admin, LeTan, KhachHang,...
                new Claim(JwtRegisteredClaimNames.Email, nguoiDung.Email), // Email người dùng
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Mã định danh token (giúp chống trùng lặp)
            };

            // 2. (Tuỳ chọn) Ghi log ra console để kiểm tra các claims đã nhúng
            Console.WriteLine("Claims khi tạo JWT token:");
            foreach (var claim in claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            // 3. Tạo đối tượng chứa key ký token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // 4. Tạo thông tin ký với thuật toán SHA256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 5. Tạo JWT token hoàn chỉnh
            var token = new JwtSecurityToken(
                issuer: issuer,         // Ai phát hành (vd: api.hotel.com)
                audience: audience,     // Ai dùng (vd: frontend.hotel.com)
                claims: claims,         // Dữ liệu trong token
                expires: DateTime.Now.AddHours(1), // Token sống 1 tiếng
                signingCredentials: creds // Ký token để tránh bị giả mạo
            );

            // 6. Trả về chuỗi token (dạng .xxxx.yyyy.zzzz)
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
