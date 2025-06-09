using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using HotelManagementAPI.Models;
using HotelManagementAPI.Services;
using VNPAY.NET.Enums;
namespace MyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnpayController : ControllerBase
    {
        private readonly IVnpayService _vnpayService;

        public VnpayController(IVnpayService vnpayService)
        {
            _vnpayService = vnpayService;
        }

        [HttpGet("CreatePaymentUrl")]
        public IActionResult CreatePaymentUrl(double moneyToPay, string description)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var uniqueId = DateTime.UtcNow.Ticks.ToString() + new Random().Next(1000, 9999).ToString();
                var nowUtcPlus7 = DateTime.UtcNow.AddHours(7); // Giờ VN (UTC+7)

                var paymentRequest = new PaymentRequest
                {
                    PaymentId = uniqueId,  // Lưu dưới dạng chuỗi
                    Money = moneyToPay,
                    Description = description,
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY.ToString(),
                    CreatedDate = nowUtcPlus7,
                    ExpireDate = nowUtcPlus7.AddMinutes(15), // Thời gian hết hạn sau 15 phút

                    Currency = Currency.VND.ToString(),
                    Language = DisplayLanguage.Vietnamese.ToString()
                    // Nếu có ExpireDate, set ví dụ: ExpireDate = nowUtcPlus7.AddMinutes(15)
                };

                var paymentUrl = _vnpayService.CreatePaymentUrl(paymentRequest);
                return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("IpnAction")]
        public IActionResult IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var queryParams = new Dictionary<string, string>();
                    foreach (var key in Request.Query.Keys)
                    {
                        queryParams[key] = Request.Query[key];
                    }

                    var paymentResult = _vnpayService.HandleIpn(queryParams);

                    if (paymentResult.IsSuccess)
                    {
                        return Ok("Payment succeeded");
                    }

                    return BadRequest("Payment failed");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("No query string found");
        }
        [HttpGet("Callback")]
        public IActionResult Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var queryParams = new Dictionary<string, string>();
                    foreach (var key in Request.Query.Keys)
                    {
                        queryParams[key] = Request.Query[key];
                    }

                    // Process query params as needed for callback
                    return Ok("Callback handled successfully");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("No query string found");
        }
       
    }
}
