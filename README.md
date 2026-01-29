Learning Management System (LMS)
Giới thiệu

Đề tài “Xây dựng hệ thống quản lý học tập trực tuyến (Learning Management System – LMS)” tập trung vào việc phân tích, thiết kế và phát triển một nền tảng học tập trực tuyến theo kiến trúc phần mềm hiện đại. Hệ thống hỗ trợ quản lý khóa học, người học, nội dung giảng dạy và các nghiệp vụ liên quan, đáp ứng nhu cầu học tập trực tuyến trong bối cảnh chuyển đổi số giáo dục.

Công nghệ sử dụng

Backend: ASP.NET Core (.NET 8)

Frontend: Angular

Cơ sở dữ liệu: MySQL

Hạ tầng & triển khai: Docker, Docker Compose

Dịch vụ tích hợp: Thanh toán trực tuyến, gửi email, xác thực người dùng thông qua API

Kiến trúc hệ thống

Hệ thống được xây dựng theo mô hình client–server, trong đó frontend và backend được tách biệt rõ ràng. Backend áp dụng kiến trúc phân tầng nhằm tăng khả năng mở rộng, bảo trì và kiểm thử; frontend đảm nhiệm giao diện và tương tác với người dùng thông qua các API do backend cung cấp.

Chức năng chính

Quản lý người dùng và phân quyền

Quản lý khóa học và nội dung học tập

Đăng ký học và theo dõi quá trình học

Giỏ hàng và thanh toán trực tuyến

Gửi email thông báo và xác thực

Triển khai

Toàn bộ hệ thống được container hóa bằng Docker, giúp đảm bảo tính nhất quán giữa các môi trường phát triển và triển khai, đồng thời thuận tiện cho việc mở rộng và bảo trì hệ thống.

Mục tiêu đề tài

Áp dụng các nguyên lý kiến trúc phần mềm hiện đại vào một hệ thống thực tế

Xây dựng một LMS có tính khả thi và khả năng mở rộng

Làm cơ sở nghiên cứu và phát triển các chức năng nâng cao trong tương lai