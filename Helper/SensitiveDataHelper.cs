using System;
using System.Security.Cryptography;
using System.Text;

namespace HotelManagementAPI.Helper
{
    /// <summary>
    /// Lớp hỗ trợ mã hóa và giải mã dữ liệu nhạy cảm (như mã đặt phòng, CCCD, email, ...)
    /// Sử dụng AES (Advanced Encryption Standard) với khóa 256-bit và IV ngẫu nhiên.
    /// 
    /// 📚 Giải thích thuật ngữ:
    /// - 🔐 AES: Thuật toán mã hóa đối xứng, an toàn, thường dùng trong thực tế (256-bit là mạnh nhất).
    /// - 🔑 Key (Khóa mã hóa): Chuỗi bí mật (32 byte ~ 256-bit) dùng để mã hóa/giải mã.
    /// - 🧊 IV (Initialization Vector): Mỗi lần mã hóa sẽ tạo ngẫu nhiên 16 byte, giúp kết quả khác nhau dù nội dung giống nhau.
    /// - 🔄 Encrypt/Decrypt: Mã hóa chuyển chuỗi bình thường → khó đọc (Base64); giải mã thì ngược lại.
    /// - 🧷 Base64: Dạng mã hóa để lưu dữ liệu nhị phân như chuỗi văn bản (dễ lưu DB, truyền qua API).
    /// 
    /// ⚠️ Lưu ý: Đây là mã hóa đối xứng → cần giữ bí mật khóa `Key`. Nếu bị lộ → có thể giải mã toàn bộ dữ liệu!
    /// </summary>
    public static class SensitiveDataHelper
    {
        // Khóa mã hóa 32 ký tự (256-bit), dùng trong AES-256
        private static readonly string Key = "12345678901234567890123456789012";

        /// <summary>
        /// Mã hóa một chuỗi văn bản (plain text) thành dạng chuỗi base64 (cipher text)
        /// </summary>
        /// <param name="plainText">Chuỗi cần mã hóa</param>
        /// <returns>Chuỗi mã hóa dạng Base64 chứa IV + dữ liệu mã hóa</returns>
        public static string Encrypt(string plainText)
        {
            // 1. Tạo đối tượng AES
            using var aes = Aes.Create();

            // 2. Thiết lập khóa mã hóa
            aes.Key = Encoding.UTF8.GetBytes(Key);

            // 3. Sinh IV ngẫu nhiên mỗi lần gọi Encrypt (IV: 16 byte)
            aes.GenerateIV();
            var iv = aes.IV;

            // 4. Tạo encryptor với Key + IV
            using var encryptor = aes.CreateEncryptor(aes.Key, iv);

            // 5. Chuyển chuỗi thành mảng byte
            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            // 6. Mã hóa dữ liệu byte
            var encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // 7. Ghép IV và dữ liệu mã hóa → kết quả cuối cùng
            var result = new byte[iv.Length + encrypted.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length); // IV ở đầu
            Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length); // Dữ liệu mã hóa theo sau

            // 8. Chuyển sang chuỗi base64 để dễ lưu trữ hoặc truyền API
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Giải mã một chuỗi base64 đã được mã hóa bởi hàm Encrypt
        /// </summary>
        /// <param name="cipherText">Chuỗi mã hóa dạng base64</param>
        /// <returns>Chuỗi văn bản gốc</returns>
        public static string Decrypt(string cipherText)
        {
            // 1. Chuyển chuỗi base64 về mảng byte
            var fullCipher = Convert.FromBase64String(cipherText);

            // 2. Tạo đối tượng AES và thiết lập khóa
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);

            // 3. Tách IV từ phần đầu của chuỗi byte (luôn là 16 byte)
            var iv = new byte[16];
            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            // 4. Lấy phần còn lại là dữ liệu mã hóa
            var cipher = new byte[fullCipher.Length - iv.Length];
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            // 5. Tạo decryptor và giải mã dữ liệu
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var decrypted = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            // 6. Trả về chuỗi văn bản gốc
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
