# 1. TỔNG QUAN DỰ ÁN

## 1.1 Mục đích

Tài liệu Software Requirements Specification (SRS) này mô tả các yêu cầu chức năng và phi chức năng cho **Hệ thống Thu thập dữ liệu và Điều khiển từ xa** (Data Collection & Remote Control System).

### Các thành phần chính:

1. **Database Server**: 
   - Lưu trữ dữ liệu log từ các client
   - Quản lý thông tin client và lệnh điều khiển
   - API server để nhận dữ liệu và gửi lệnh

2. **Client Application**: 
   - Ứng dụng chạy tự động trên máy client (không cần đăng nhập)
   - Đọc log file từ ứng dụng khác
   - Gửi dữ liệu lên server
   - Nhận lệnh từ manager và điều khiển ứng dụng thứ 3

3. **Manager Application**: 
   - Ứng dụng web để giám sát các client
   - Xem dữ liệu log được gửi lên
   - Gửi lệnh điều khiển xuống các client
   - Dashboard đơn giản để theo dõi trạng thái

## 1.2 Phạm vi dự án

### Bao gồm:
- ✅ Client application tự động đọc log files
- ✅ API server nhận dữ liệu từ clients
- ✅ Database lưu trữ log data và client info
- ✅ Manager web app để giám sát và điều khiển
- ✅ Command system để gửi lệnh xuống clients
- ✅ Basic monitoring và alerting

### Không bao gồm:
- ❌ User authentication system (clients chạy tự động)
- ❌ Complex reporting và analytics
- ❌ Real-time machine monitoring
- ❌ Mobile application
- ❌ Integration với ERP/MES systems

## 1.3 Đối tượng sử dụng tài liệu

- **Project Manager**: Hiểu tổng quan và quản lý tiến độ
- **Business Analyst**: Đảm bảo requirements đáp ứng nhu cầu business
- **System Architect**: Thiết kế kiến trúc hệ thống
- **Developers**: Tham khảo để implement
- **Testers**: Tạo test cases và test plans
- **End Users**: Hiểu chức năng và cách sử dụng hệ thống
- **Stakeholders**: Review và approve requirements

## 1.4 Mục tiêu dự án

### Mục tiêu chính:
1. **Thu thập dữ liệu tự động**: Collect log data từ <1000 clients
2. **Điều khiển từ xa**: Gửi lệnh và điều khiển ứng dụng trên client
3. **Giám sát tập trung**: Dashboard để theo dõi trạng thái các clients
4. **Đơn giản hóa**: Hệ thống dễ deploy và maintain

### Key Performance Indicators (KPIs):
- **Client Capacity**: Support <1000 clients đồng thời
- **Data Collection**: Thu thập log trong vòng 30 giây
- **Command Delivery**: Gửi lệnh trong vòng 10 giây
- **System Uptime**: > 99% (less critical than enterprise systems)

## 1.5 Thành công của dự án

Dự án được coi là thành công khi:
- ✅ Hệ thống hoạt động ổn định trong 3 tháng đầu
- ✅ Tất cả user stories được implement và test pass
- ✅ Performance requirements được đáp ứng
- ✅ End users được training và sử dụng thành thạo
- ✅ System documentation hoàn chỉnh
- ✅ Stakeholders sign-off và accept hệ thống