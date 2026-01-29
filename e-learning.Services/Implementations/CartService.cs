using e_learning.Data.Entities;
using e_learning.Data.Helpers;
using e_learning.infrastructure.Repositories;
using e_learning.Services.Abstructs;
using e_learning.Services.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace e_learning.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IConfiguration _config;
        public CartService(ICartRepository cartRepository, IStudentRepository studentRepository, ICourseRepository courseRepository, IOrderRepository orderRepository, IEnrollmentService enrollmentService, IConfiguration config)
        {
            _cartRepository = cartRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _orderRepository = orderRepository;
            _enrollmentService = enrollmentService;
            _config = config;
        }

        public async Task<string> CheckoutAsync(int studentId, string ipAddress)
        {
            var cart = await _cartRepository.GetCartAsync(studentId);
            if (cart == null || !cart.Courses.Any())
                throw new Exception("Cart is empty");

            var total = cart.Courses.Sum(c => c.Price);

            var order = new Order
            {
                StudentId = studentId,
                TotalAmount = total,
                PaymentStatus = "Pending",
                CreatedDate = DateTime.UtcNow
            };

            var savedOrder = await _orderRepository.CreateOrderAsync(order);

            var vnpay = new VnPayLibrary();

            string vnp_Url = _config["Vnpay:BaseUrl"];
            string vnp_HashSecret = _config["Vnpay:HashSecret"];
            string vnp_TmnCode = _config["Vnpay:TmnCode"];
            string vnp_ReturnUrl = _config["Vnpay:ReturnUrl"];

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);

            vnpay.AddRequestData("vnp_Amount", ((long)(total * 100)).ToString());

            vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", ipAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang #{savedOrder.Id}");            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);

            vnpay.AddRequestData("vnp_TxnRef", savedOrder.Id.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return paymentUrl;
        }

        public async Task<VnPayResultDto> ProcessVnPayReturnAsync(IQueryCollection query)
{
    var vnpay = new VnPayLibrary();

    // 1. Lấy dữ liệu từ URL VNPay gửi về
    foreach (var (key, value) in query)
    {
        if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
        {
            vnpay.AddResponseData(key, value);
        }
    }

    // 2. Trích xuất các tham số quan trọng
    // vnp_TxnRef chính là OrderId (ví dụ của bạn là 13)
    if (!int.TryParse(vnpay.GetResponseData("vnp_TxnRef"), out int orderId))
    {
        return new VnPayResultDto { IsSuccess = false, Message = "Mã đơn hàng không hợp lệ" };
    }

    string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
    string vnp_SecureHash = query["vnp_SecureHash"];
    string hashSecret = _config["Vnpay:HashSecret"]; // Lấy từ appsettings.json

    // 3. Kiểm tra chữ ký bảo mật
    bool isValidSignature = vnpay.ValidateSignature(vnp_SecureHash, hashSecret);

    if (isValidSignature && vnp_ResponseCode == "00")
    {
        // THÀNH CÔNG: Cập nhật DB
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order != null && order.PaymentStatus != "Success")
        {
            order.PaymentStatus = "Success";
            await _orderRepository.UpdateOrderAsync(order);
            // Ghi danh học viên bằng EnrollmentService đã tiêm vào
            await HandlePostPaymentAsync(order.StudentId);
        }
        return new VnPayResultDto { IsSuccess = true, OrderId = orderId };
    }

    // THẤT BẠI
    return new VnPayResultDto { IsSuccess = false, Message = "Thanh toán không thành công hoặc chữ ký sai" };
}
private async Task HandlePostPaymentAsync(int studentId)
{
    var cart = await _cartRepository.GetCartAsync(studentId);
    if (cart != null)
    {
        foreach (var course in cart.Courses)
        {
            // Gọi EnrollmentService để ghi danh
            await _enrollmentService.EnrollStudentInCourseAsync(studentId, course.CourseId);
        }
        // Xóa giỏ hàng sau khi đã ghi danh xong
        await _cartRepository.DeleteCartAsync(studentId);
    }
}

        public async Task<CartDto> AddToCartAsync(int studentId, int courseId)
        {

            var cart = await _cartRepository.GetCartAsync(studentId)
                ?? new CartDto
                {
                    StudentId = studentId,
                    CartId = Guid.NewGuid()
                };

            if (!cart.Courses.Any(x => x.CourseId == courseId))
            {
                var course = await _courseRepository.GetCourseByIdAsync(courseId);
                var newCartItem = new CartItemDto
                {

                    CourseId = course.Id,
                    CourseTitle = course.Title,
                    Price = course.Price

                };
                cart.Courses.Add(newCartItem);
                await _cartRepository.SaveCartAsync(cart);
            }

            return cart;
        }

        public async Task<CartDto?> GetCartAsync(int studentId)
        {
            return await _cartRepository.GetCartAsync(studentId);
        }


        public async Task RemoveFromCartAsync(int studentId, int courseId)
        {
            var cart = await _cartRepository.GetCartAsync(studentId);
            if (cart == null) return;

            var item = cart.Courses.FirstOrDefault(x => x.CourseId == courseId);
            if (item != null)
            {
                cart.Courses.Remove(item);
                await _cartRepository.SaveCartAsync(cart);
            }
        }

    }
}