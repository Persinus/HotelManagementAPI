using System;
using System.Security.Cryptography;
using System.Text;

namespace HotelManagementAPI.Helper
{
    public static class SensitiveDataHelper
    {
        private static readonly string Key = "12345678901234567890123456789012"; // 32 ký tự

        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.GenerateIV();
            var iv = aes.IV;
            using var encryptor = aes.CreateEncryptor(aes.Key, iv);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            var result = new byte[iv.Length + encrypted.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);
            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            var iv = new byte[16];
            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var cipher = new byte[fullCipher.Length - iv.Length];
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
            var decrypted = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}