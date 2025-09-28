# ĐỊNH NGHĨA VÀ TỪ VIẾT TẮT

## Thuật ngữ chuyên môn

### A
- **API (Application Programming Interface)**: Giao diện lập trình ứng dụng
- **Audit Trail**: Nhật ký kiểm toán, ghi lại tất cả hoạt động trong hệ thống

### C  
- **CRUD**: Create, Read, Update, Delete - Các thao tác cơ bản với dữ liệu
- **Client App**: Ứng dụng chạy tự động trên client machine, đọc log và nhận lệnh

### D
- **Dashboard**: Bảng điều khiển hiển thị thông tin tổng quan
- **Database (DB)**: Cơ sở dữ liệu
- **Downtime**: Thời gian hệ thống không hoạt động
- **Data Integrity**: Tính toàn vẹn dữ liệu

### E
- **ERD (Entity Relationship Diagram)**: Sơ đồ thực thể kết hợp
- **ERP (Enterprise Resource Planning)**: Hệ thống hoạch định tài nguyên doanh nghiệp

### H
- **HTTPS**: HyperText Transfer Protocol Secure
- **Hardware Interface**: Giao diện phần cứng

### J
- **JSON**: JavaScript Object Notation - Định dạng trao đổi dữ liệu

### K
- **KPI (Key Performance Indicator)**: Chỉ số đánh giá hiệu suất chủ chốt

### L
- **Log File**: Tập tin ghi lại hoạt động của ứng dụng khác trên client machine
- **Log Parser**: Module đọc và phân tích nội dung log file

### M
- **Manager App**: Ứng dụng web để giám sát clients và gửi lệnh điều khiển
- **MES (Manufacturing Execution System)**: Hệ thống thực thi sản xuất
- **Modbus**: Giao thức truyền thông công nghiệp
- **Multi-factor Authentication**: Xác thực đa yếu tố

### P
- **POC (Proof of Concept)**: Bằng chứng khái niệm
- **Predictive Maintenance**: Bảo trì dự báo

### R
- **Real-time**: Thời gian thực
- **REST**: Representational State Transfer
- **RS485**: Chuẩn truyền thông nối tiếp

### S
- **SRS (Software Requirements Specification)**: Đặc tả yêu cầu phần mềm
- **SSL (Secure Sockets Layer)**: Lớp ổ cắc bảo mật
- **Stakeholder**: Bên liên quan

### T
- **TCP/IP**: Transmission Control Protocol/Internet Protocol
- **Third-party Application**: Ứng dụng thứ 3 cài đặt trên client, được điều khiển bởi Client App

### U
- **UI (User Interface)**: Giao diện người dùng  
- **UX (User Experience)**: Trải nghiệm người dùng
- **Uptime**: Thời gian hệ thống hoạt động bình thường

### W
- **WebSocket**: Giao thức truyền thông hai chiều qua web
- **Workflow**: Luồng công việc

## Định nghĩa Domain-specific

### Machine States (Trạng thái máy)
- **Running**: Máy đang hoạt động bình thường
- **Idle**: Máy đang chờ (sẵn sàng hoạt động)
- **Stopped**: Máy đã dừng
- **Error**: Máy gặp lỗi, cần can thiệp
- **Maintenance**: Máy đang được bảo trì

### Alert Types (Loại cảnh báo)
- **Critical**: Cảnh báo nghiêm trọng, cần xử lý ngay
- **Warning**: Cảnh báo, cần chú ý
- **Info**: Thông tin, không cần hành động
- **Maintenance Due**: Đến hạn bảo trì

### User Roles (Vai trò người dùng)
- **Operator**: Người vận hành máy
- **Supervisor**: Giám sát viên
- **Manager**: Quản lý
- **Administrator**: Quản trị hệ thống
- **Maintenance**: Nhân viên bảo trì

### Data Types (Loại dữ liệu)
- **Operational Data**: Dữ liệu vận hành (nhiệt độ, áp suất, tốc độ)
- **Historical Data**: Dữ liệu lịch sử
- **Configuration Data**: Dữ liệu cấu hình
- **User Data**: Dữ liệu người dùng
- **System Data**: Dữ liệu hệ thống

## Đơn vị đo lường

### Performance
- **Response Time**: Thời gian phản hồi (giây)
- **Throughput**: Thông lượng (transactions/second)
- **Concurrent Users**: Số người dùng đồng thời
- **Uptime**: Thời gian hoạt động (%)

### Hardware
- **CPU**: Central Processing Unit (cores, GHz)
- **RAM**: Random Access Memory (GB)
- **Storage**: Dung lượng lưu trữ (GB, TB)
- **Network**: Băng thông mạng (Mbps, Gbps)

### Machine Parameters
- **Temperature**: Nhiệt độ (°C)
- **Pressure**: Áp suất (bar, psi)
- **Speed**: Tốc độ (RPM)
- **Power**: Công suất (kW)
- **Voltage**: Điện áp (V)
- **Current**: Dòng điện (A)