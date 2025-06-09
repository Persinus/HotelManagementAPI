using HotelManagementAPI.Models;
using System.Collections.Generic;

namespace HotelManagementAPI.Services
{
    public interface IVnpayService
    {
        string CreatePaymentUrl(PaymentRequest request);
        PaymentResult HandleIpn(Dictionary<string, string> queryParams);
        bool ValidateCallback(VnpayCallbackRequest callbackRequest);
    }
}
