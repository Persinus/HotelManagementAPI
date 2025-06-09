using System;

namespace HotelManagementAPI.Models
{
    public class PaymentRequest
    {
        public string PaymentId { get; set; }
        public double Money { get; set; }
        public string Description { get; set; }

        // Thêm các trường mở rộng cho VNPAY
        public string IpAddress { get; set; }
        public string BankCode { get; set; } // Có thể dùng enum nếu muốn
        public DateTime CreatedDate { get; set; }
        public DateTime ExpireDate { get; set; } // <-- thêm cái này
        public string Currency { get; set; }
        public string Language { get; set; }


        
    }
}
