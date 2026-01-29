namespace e_learning.Services.DTOs
{
    public class VnPayResultDto
    {
        public bool IsSuccess { get; set; }
        public int OrderId { get; set; }
        public string Message { get; set; }
        public string ResponseCode { get; set; } // Mã lỗi từ VNPay (ví dụ: 00, 07, 09...)
    }
}