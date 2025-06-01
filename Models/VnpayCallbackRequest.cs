namespace YourNamespace.Models
{
    public class VnpayCallbackRequest
    {
        public string Vnp_TmnCode { get; set; }
        public string Vnp_Amount { get; set; }
        public string Vnp_OrderInfo { get; set; }
        public string Vnp_TxnRef { get; set; }
        public string Vnp_ResponseCode { get; set; }
        public string Vnp_SecureHash { get; set; }
        // Add other fields as needed
    }
}
