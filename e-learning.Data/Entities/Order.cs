namespace e_learning.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Success, Failed
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        // Mã tham chiếu từ VNPay (Sau khi thanh toán thành công mới có)
        public string? VnPayTransactionId { get; set; } 
        
        // Lưu lại thông tin phản hồi từ VNPay để đối soát nếu cần
        public string? PaymentResponseCode { get; set; } 
    }
}