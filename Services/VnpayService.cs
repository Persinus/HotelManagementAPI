using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using HotelManagementAPI.Models;

using PaymentRequest = HotelManagementAPI.Models.PaymentRequest;



namespace HotelManagementAPI.Services
{
    public class VnpayService : IVnpayService
    {
        private readonly IConfiguration _configuration;

        public VnpayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(HotelManagementAPI.Models.PaymentRequest request)
        {
            var vnpayConfig = _configuration.GetSection("Vnpay");
            var tmnCode = vnpayConfig["TmnCode"];
            var hashSecret = vnpayConfig["HashSecret"];
            var baseUrl = vnpayConfig["BaseUrl"];
            var returnUrl = vnpayConfig["ReturnUrl"];

            var queryParams = new SortedDictionary<string, string>
            {
                {"vnp_Version", "2.1.0"},
                {"vnp_Command", "pay"},
                {"vnp_TmnCode", tmnCode},
                {"vnp_Amount", ((long)(request.Money * 100)).ToString()},
            {"vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss")},

                {"vnp_CurrCode", "VND"},
                {"vnp_IpAddr", "127.0.0.1"},
                {"vnp_ExpireDate", DateTime.UtcNow.AddHours(7).AddMinutes(15).ToString("yyyyMMddHHmmss")},

                {"vnp_Locale", "vn"},
                {"vnp_OrderInfo", request.Description},
                {"vnp_OrderType", "billpayment"},
                {"vnp_ReturnUrl", returnUrl},
                {"vnp_TxnRef", request.PaymentId.ToString()}
            };

            var hashData = new StringBuilder();
            foreach (var kvp in queryParams)
            {
                if (hashData.Length > 0)
                    hashData.Append('&');
                hashData.Append($"{kvp.Key}={kvp.Value}");
            }

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hashSecret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashData.ToString()));
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return $"{baseUrl}?{hashData}&vnp_SecureHash={hash}";
        }
          public PaymentResult HandleIpn(Dictionary<string, string> queryParams)
        {
            // Xử lý logic IPN (Instant Payment Notification) ở đây
            // Ví dụ giả lập thành công
            return new PaymentResult
            {
                IsSuccess = true,
                Message = "IPN processed successfully"
            };
        }
        public bool ValidateCallback(HotelManagementAPI.Models.VnpayCallbackRequest callbackRequest)
        {
            // Implement callback validation based on VNPAY documentation
            return true;
        }
    }
}
