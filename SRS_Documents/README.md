# HỆ THỐNG THU THẬP LOG & ĐIỀU KHIỂN TỪ XA

> **🎯 Version: Đơn giản cho <1000 clients - Dễ hiểu, dễ sửa đổi**

## ⚙️ CẤU TRÚC TÀI LIỆU

### ✅ **Tài liệu chính (Đã hoàn thành)**

#### 📋 1. Giới thiệu dự án
- [`overview.md`](01_Introduction/overview.md) - Mục đích và phạm vi hệ thống
- [`definitions.md`](01_Introduction/definitions.md) - Thuật ngữ và định nghĩa
- [`references.md`](01_Introduction/references.md) - Tài liệu tham khảo

#### 📋 2. Yêu cầu hệ thống  
- [`functional_requirements_simple.md`](03_Requirements/functional_requirements_simple.md) - Yêu cầu chức năng
- [`non_functional_requirements_simple.md`](03_Requirements/non_functional_requirements_simple.md) - Yêu cầu kỹ thuật

#### 📋 3. Thiết kế database
- [`database_simple.md`](06_Database_Design/database_simple.md) - **🆕 ĐƠN GIẢN** - 4 bảng cơ bản, SQL đơn giản

#### 📋 4. Quản lý dự án
- [`timeline_simple.md`](08_Project_Management/timeline_simple.md) - Timeline 12 tuần

---

## 🚀 **HỆ THỐNG CỦA BẠN**

### Mô tả ngắn gọn:
```
[Client Apps]  →  Gửi Log  →  [Server]  ←  Xem & Điều khiển  ←  [Manager Web]
      ↓                         ↓                                      ↓
  [Log Files]              [Database]                           [Commands] 
  [App khác]                                                        
```

### 3 thành phần chính:
1. **Client App** (Windows Service):
   - Tự động đọc log files 
   - Gửi data lên server
   - Nhận lệnh và điều khiển app khác

2. **Server API** (.NET Core):
   - Nhận log từ clients
   - Lưu vào MySQL database  
   - Quản lý commands

3. **Manager Web** (React/Vue):
   - Dashboard xem clients
   - Xem log data
   - Gửi lệnh điều khiển

---

## 📊 **DATABASE ĐƠN GIẢN** 

### 4 bảng chính:
| Bảng | Mục đích | Ví dụ dữ liệu |
|------|----------|---------------|
| **clients** | Danh sách máy client | PC001, 192.168.1.100, Online |
| **log_data** | Log gửi từ clients | "App started", Info, 14:30 |
| **commands** | Lệnh gửi đến clients | StartApp, notepad.exe, Pending |
| **settings** | Cài đặt hệ thống | log_retention_days = 90 |

### Ưu điểm:
- ✅ Dễ hiểu (không cần biết SQL phức tạp)
- ✅ Dễ sửa đổi (thêm/bớt cột đơn giản)  
- ✅ MySQL miễn phí
- ✅ Có thể dùng phpMyAdmin

---

## ⚙️ **TECHNICAL SPECS**

### Performance:
- **Clients**: Hỗ trợ 500-1000 máy
- **Log volume**: ~1000 entries/phút
- **Response time**: < 1 giây
- **Storage**: ~5GB/năm

### Technology Stack:
- **Database**: MySQL
- **Server**: .NET Core API
- **Client**: C# Windows Service  
- **Web**: React/Vue.js
- **Timeline**: 12 tuần

---

## 📝 **FILES ĐÃ XÓA** (Quá phức tạp)
- ~~database_schema.md~~ - SQL phức tạp với 10+ tables
- ~~functional_requirements.md~~ - Enterprise features
- ~~non_functional_requirements.md~~ - Enterprise specs
- ~~timeline.md~~ - 18 tuần timeline
- ~~SRS_Template.md~~ - File gốc quá lớn

---

## 🛠️ **CÁCH Sử DỤNG TÀI LIỆU**

### 1. Đọc hiểu hệ thống:
```
overview.md → functional_requirements_simple.md → database_simple.md
```

### 2. Chỉnh sửa dễ dàng:
- **Database**: Mở `database_simple.md` → Sửa bảng/cột
- **Features**: Mở `functional_requirements_simple.md` → Thêm/bớt tính năng
- **Timeline**: Mở `timeline_simple.md` → Điều chỉnh thời gian

### 3. SQL đơn giản:
```sql
-- Thêm client mới
INSERT INTO clients (machine_name, ip_address) VALUES ('PC001', '192.168.1.100');

-- Xem log gần đây  
SELECT * FROM log_data ORDER BY log_time DESC LIMIT 50;

-- Tạo lệnh mới
INSERT INTO commands (client_id, command_type) VALUES (1, 'StartApp');
```

---

## 📞 **SUPPORT**

### Files quan trọng nhất:
1. **`database_simple.md`** - Thiết kế database (dễ hiểu nhất)
2. **`functional_requirements_simple.md`** - Tính năng hệ thống  
3. **`timeline_simple.md`** - Lịch trình 12 tuần

### Nếu cần thay đổi:
- Database: Sửa trực tiếp SQL trong `database_simple.md`
- Features: Thêm/bớt requirements
- Timeline: Điều chỉnh theo team size

---

**🎉 Hoàn thành**: Tài liệu đã được đơn giản hóa tối đa cho dễ hiểu và chỉnh sửa!  
**📅 Cập nhật**: 28/09/2025  
**✨ Trạng thái**: Ready để bắt đầu development