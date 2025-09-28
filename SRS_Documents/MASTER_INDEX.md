# DATA COLLECTION & REMOTE CONTROL SYSTEM - SRS MASTER INDEX

> **🔄 SCALED VERSION**: Đây là phiên bản scaled down cho <1000 clients, không cần authentication phức tạp

## 📁 Cấu trúc tài liệu (Updated)

### 🔹 01_Introduction/
- [`overview.md`](01_Introduction/overview.md) - ✅ **Updated** - Mục đích và phạm vi cho hệ thống đơn giản
- [`definitions.md`](01_Introduction/definitions.md) - ✅ **Updated** - Thuật ngữ phù hợp với log collection system
- [`references.md`](01_Introduction/references.md) - ✅ Tài liệu tham khảo

### 🔹 03_Requirements/
- [`functional_requirements_simple.md`](03_Requirements/functional_requirements_simple.md) - ✅ **NEW** - Yêu cầu chức năng đơn giản hóa
- [`non_functional_requirements_simple.md`](03_Requirements/non_functional_requirements_simple.md) - ✅ **NEW** - NFR scaled cho <1000 clients
- ~~`functional_requirements.md`~~ - **Deprecated** - Phiên bản phức tạp
- ~~`non_functional_requirements.md`~~ - **Deprecated** - Phiên bản enterprise

### 🔹 06_Database_Design/
- [`database_schema_simple.md`](06_Database_Design/database_schema_simple.md) - ✅ **NEW** - Database design đơn giản (4 core tables)
- ~~`database_schema.md`~~ - **Deprecated** - Phiên bản phức tạp

### 🔹 08_Project_Management/
- [`timeline_simple.md`](08_Project_Management/timeline_simple.md) - ✅ **NEW** - Timeline 12 tuần thay vì 18 tuần
- ~~`timeline.md`~~ - **Deprecated** - Timeline phức tạp

## 🎯 **Hệ thống mới (Simplified)**:

### Core Components:
1. **API Server**: Thu thập log từ clients, distribute commands
2. **Client Service**: Đọc log files, gửi data, nhận commands, control apps
3. **Manager Web App**: Dashboard để monitor và send commands

### Key Features:
- ✅ Support <1000 clients (không cần authentication)
- ✅ Automatic log file monitoring và parsing
- ✅ Remote command execution
- ✅ Third-party application control
- ✅ Basic web dashboard
- ✅ Simple deployment và maintenance

### 🔹 01_Introduction/
- [`overview.md`](01_Introduction/overview.md) - Mục đích, phạm vi và mục tiêu dự án
- [`definitions.md`](01_Introduction/definitions.md) - Thuật ngữ và từ viết tắt
- [`references.md`](01_Introduction/references.md) - Tài liệu tham khảo và standards

### 🔹 02_System_Overview/
- `product_perspective.md` - *[Cần tạo]* - Góc nhìn tổng quan sản phẩm
- `product_functions.md` - *[Cần tạo]* - Chức năng chính của hệ thống  
- `user_characteristics.md` - *[Cần tạo]* - Đặc điểm người dùng
- `constraints.md` - *[Cần tạo]* - Ràng buộc và giả định

### 🔹 03_Requirements/
- [`functional_requirements_simple.md`](03_Requirements/functional_requirements_simple.md) - Yêu cầu chức năng cơ bản
- [`non_functional_requirements_simple.md`](03_Requirements/non_functional_requirements_simple.md) - Yêu cầu kỹ thuật

### 🔹 04_Interfaces/
- `external_interfaces.md` - *[Cần tạo]* - Giao diện bên ngoài
- `hardware_interfaces.md` - *[Cần tạo]* - Giao diện phần cứng
- `software_interfaces.md` - *[Cần tạo]* - Giao diện phần mềm
- `communication_interfaces.md` - *[Cần tạo]* - Giao diện truyền thông

### 🔹 05_System_Requirements/
- `logical_database.md` - *[Cần tạo]* - Yêu cầu database logic
- `design_constraints.md` - *[Cần tạo]* - Ràng buộc thiết kế
- `software_attributes.md` - *[Cần tạo]* - Thuộc tính phần mềm

### 🔹 06_Database_Design/
- [`database_simple.md`](06_Database_Design/database_simple.md) - Thiết kế database đơn giản

### 🔹 07_Architecture/
- `system_architecture.md` - *[Cần tạo]* - Kiến trúc hệ thống
- `deployment_diagram.md` - *[Cần tạo]* - Sơ đồ triển khai

### 🔹 08_Project_Management/
- [`timeline_simple.md`](08_Project_Management/timeline_simple.md) - Lịch trình dự án

### 🔹 09_Appendices/
- `glossary.md` - *[Cần tạo]* - Thuật ngữ
- `analysis_models.md` - *[Cần tạo]* - Mô hình phân tích

---

## 📊 **TIẺ N ĐỘ HOÀN THÀNH**

### ✅ **Đã hoàn thành** (7/15 files)
- 01_Introduction/* (3 files)
- 03_Requirements/* (2 files)
- 06_Database_Design/database_simple.md (1 file)
- 08_Project_Management/timeline_simple.md (1 file)

### 🕑 **Cần hoàn thành** (8/15 files)
- 02_System_Overview/* (4 files)
- 04_Interfaces/* (4 files)

### ❓ **Tùy chọn** (Optional)
- 05_System_Requirements/*
- 07_Architecture/*
- 09_Appendices/*

---

## 🎯 **MỤC TIÊU SRS**

1. ✅ **Simplified System**: Chỉ các tính năng cốt lõi
2. ✅ **< 1000 Clients**: Không cần enterprise features
3. ✅ **Easy to Understand**: Developer có thể đọc và implement ngay
4. ✅ **12-week Timeline**: Thời gian hợp lý cho small team
5. ✅ **MySQL Simple**: 4 bảng chính, không cần stored procedures phức tạp

---

**🔄 Cập nhật**: 28/09/2025  
**✨ Trạng thái**: Core documents hoàn thành, ready for implementation
