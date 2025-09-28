# YÊU CẦU CHỨC NĂNG - UPDATED VERSION

> **Cập nhật**: Thêm Web Admin Server cho quản lý database với authentication

## HỆ THỐNG BAO GỒM 4 THÀNH PHẦN:

1. **Database Server** - MySQL với các bảng machines, log_data, commands, client_config  
2. **API Server** - REST API cho clients gửi log và nhận commands
3. **Client Applications** - Apps chạy trên máy production
4. **🆕 Web Admin Server** - Giao diện web quản lý database (yêu cầu đăng nhập)

---

## 1. DATABASE & API SERVER REQUIREMENTS

### 1.1 Thu thập dữ liệu từ clients

**FR-DB-001: Nhận log data từ clients**
- **Priority**: Critical
- **Mô tả**: API endpoint để nhận log data được gửi từ các client applications
- **Input**: 
  - Client ID (auto-generated hoặc machine-based)
  - Timestamp
  - Log content (text, JSON, hoặc structured data)
  - Log level (Info, Warning, Error)
  - Application source
- **Output**: Confirmation response cho client
- **Business Rules**:
  - Validate client ID (auto-register nếu chưa tồn tại)
  - Timestamp phải hợp lệ
  - Giới hạn kích thước log entry (max 10KB)
  - Rate limiting: max 100 requests/minute per client
- **Acceptance Criteria**:
  - ✅ REST API endpoint: POST /api/logs
  - ✅ Handle 1000 concurrent clients
  - ✅ Response time < 500ms
  - ✅ Auto-retry mechanism cho failed requests

**FR-DB-002: Lưu trữ thông tin clients**
- **Priority**: High
- **Mô tả**: Quản lý danh sách các client machines
- **Input**:
  - Client ID (unique identifier)
  - Machine Name/Hostname
  - IP Address
  - OS Information
  - Application Version
  - Last Seen timestamp
  - Status (Online/Offline/Error)
- **Output**: Client registry và status tracking
- **Business Rules**:
  - Auto-register client khi first connect
  - Update last seen mỗi khi nhận data
  - Mark offline sau 5 phút không có data
- **Acceptance Criteria**:
  - ✅ Client registration endpoint
  - ✅ Automatic client discovery
  - ✅ Status monitoring
  - ✅ Client list management

**FR-DB-003: Command queue system**
- **Priority**: High
- **Mô tả**: Lưu trữ và quản lý các lệnh gửi xuống clients
- **Input**:
  - Target Client ID(s)
  - Command Type (Start, Stop, Restart, Configure)
  - Command Parameters (JSON format)
  - Priority Level
  - Created By (manager user)
- **Output**: Command queue cho clients poll
- **Business Rules**:
  - Commands có expiry time (default 1 hour)
  - Support broadcast commands (all clients)
  - Command history tracking
- **Acceptance Criteria**:
  - ✅ Command CRUD operations
  - ✅ Client polling endpoint: GET /api/commands/{clientId}
  - ✅ Command status tracking (Pending, Sent, Executed, Failed)
  - ✅ Command history log

---

## 2. CLIENT APPLICATION REQUIREMENTS

### 2.1 Log file monitoring

**FR-CLIENT-001: File watcher service**
- **Priority**: Critical
- **Mô tả**: Monitor các log files và detect changes
- **Input**: Configuration của log file paths
- **Output**: Log events khi file thay đổi
- **User Story**: "Là một client app, tôi cần tự động detect khi log file được update để gửi data mới lên server"
- **Business Rules**:
  - Support multiple file patterns (*.log, *.txt, *.xml)
  - Handle file rotation (log.1, log.2, etc.)
  - Resume từ last read position sau restart
- **Acceptance Criteria**:
  - ✅ Real-time file change detection
  - ✅ Handle large files (>100MB)
  - ✅ Support file rotation
  - ✅ Configurable file paths

**FR-CLIENT-002: Log parsing và sending**
- **Priority**: Critical
- **Mô tả**: Parse log content và send lên server
- **Input**: Raw log content từ files
- **Output**: Structured data gửi lên API
- **User Story**: "Là một client app, tôi cần parse log content và gửi lên server một cách reliable"
- **Business Rules**:
  - Parse theo configured format (regex patterns)
  - Batch multiple log entries
  - Retry failed sends với exponential backoff
  - Local queue khi server không available
- **Acceptance Criteria**:
  - ✅ Configurable log parsing rules
  - ✅ Batch processing (max 100 entries/request)
  - ✅ Offline queue management
  - ✅ Error handling và retry logic

### 2.2 Command execution

**FR-CLIENT-003: Command polling và execution**
- **Priority**: High
- **Mô tả**: Poll server cho commands và execute chúng
- **Input**: Commands từ server API
- **Output**: Command execution results
- **User Story**: "Là một client app, tôi cần định kỳ check commands từ server và thực hiện chúng"
- **Business Rules**:
  - Poll server mỗi 30 seconds
  - Validate commands trước khi execute
  - Report execution status về server
  - Support command types: Start, Stop, Restart app, Run script
- **Acceptance Criteria**:
  - ✅ Configurable polling interval
  - ✅ Command validation
  - ✅ Execution status reporting
  - ✅ Support multiple command types

**FR-CLIENT-004: Third-party app control**
- **Priority**: High
- **Mô tả**: Điều khiển ứng dụng thứ 3 trên client machine
- **Input**: Control commands (start, stop, restart process, UI automation)
- **Output**: Process control actions
- **User Story**: "Là một client app, tôi cần có thể start/stop các ứng dụng khác theo lệnh từ manager"
- **Business Rules**:
  - Configurable executable paths
  - Process monitoring
  - Safety checks (prevent system process kill)
  - User permission validation
- **Acceptance Criteria**:
  - ✅ Process start/stop/restart
  - ✅ Process status monitoring
  - ✅ Configuration management
  - ✅ Safety constraints

---

## 3. MANAGER APPLICATION REQUIREMENTS

### 3.1 Client monitoring

**FR-MANAGER-001: Client dashboard**
- **Priority**: High
- **Mô tả**: Hiển thị tổng quan tất cả clients
- **Input**: Client data từ database
- **Output**: Dashboard với client status
- **User Story**: "Là một manager, tôi muốn thấy tổng quan tất cả clients và status của chúng"
- **Business Rules**:
  - Real-time status updates
  - Filter theo status, location, etc.
  - Search clients by name/IP
- **Acceptance Criteria**:
  - ✅ Grid view với client information
  - ✅ Status indicators (Online/Offline/Error)
  - ✅ Last seen timestamps
  - ✅ Client count summary

**FR-MANAGER-002: Log data viewer**
- **Priority**: Medium
- **Mô tả**: Xem log data đã được collect từ clients
- **Input**: Log data từ database
- **Output**: Searchable log interface
- **User Story**: "Là một manager, tôi muốn có thể xem và search trong log data từ các clients"
- **Business Rules**:
  - Pagination cho large datasets
  - Filter theo client, time range, log level
  - Export functionality
- **Acceptance Criteria**:
  - ✅ Searchable log interface
  - ✅ Date/time filtering
  - ✅ Export to CSV/Excel
  - ✅ Pagination

### 3.2 Command management

**FR-MANAGER-003: Send commands to clients**
- **Priority**: High
- **Mô tả**: Gửi commands đến một hoặc nhiều clients
- **Input**: Command parameters và target clients
- **Output**: Command creation và tracking
- **User Story**: "Là một manager, tôi muốn gửi lệnh đến clients để control applications"
- **Business Rules**:
  - Support single client và broadcast commands
  - Command templates cho common operations
  - Confirmation prompts cho critical commands
- **Acceptance Criteria**:
  - ✅ Command creation interface
  - ✅ Client selection (single/multiple/all)
  - ✅ Command templates
  - ✅ Confirmation dialogs

**FR-MANAGER-004: Command status tracking**
- **Priority**: Medium
- **Mô tả**: Theo dõi execution status của commands
- **Input**: Command execution reports từ clients
- **Output**: Command status dashboard
- **User Story**: "Là một manager, tôi muốn biết commands đã được execute thành công hay chưa"
- **Business Rules**:
  - Real-time status updates
  - Command history retention (30 days)
  - Failed command retry options
- **Acceptance Criteria**:
  - ✅ Command status dashboard
  - ✅ Execution timeline
  - ✅ Error reporting
  - ✅ Retry functionality

---

## 4. 🆕 WEB ADMIN SERVER REQUIREMENTS

### 4.1 Authentication & Authorization

**FR-ADMIN-001: User Authentication System**
- **Priority**: Critical
- **Mô tả**: Hệ thống đăng nhập bảo mật cho web admin
- **Input**: Username, Password
- **Output**: Session token và access permissions
- **User Story**: "Là một admin, tôi cần đăng nhập để truy cập vào hệ thống quản lý database"
- **Business Rules**:
  - Username/password validation
  - Session timeout sau 8 giờ không hoạt động
  - Khóa account sau 5 lần đăng nhập sai
  - Minimum password: 8 ký tự, có số và ký tự đặc biệt
- **Acceptance Criteria**:
  - ✅ Login form với validation
  - ✅ Session management
  - ✅ Remember me option (7 ngày)
  - ✅ Logout functionality

**FR-ADMIN-002: Role-based Access Control**
- **Priority**: High
- **Mô tả**: Phân quyền truy cập theo role của user
- **Input**: User role information
- **Output**: Access permissions to different modules
- **User Story**: "Là một admin, tôi muốn có các level quyền khác nhau cho từng người dùng"
- **Business Rules**:
  - **Super Admin**: Full access tất cả bảng và functions
  - **Admin**: CRUD trên machines, stations, commands (không được xóa data quan trọng)
  - **Operator**: Chỉ xem data và tạo commands
  - **Viewer**: Chỉ xem, không chỉnh sửa
- **Acceptance Criteria**:
  - ✅ Role assignment interface
  - ✅ Permission matrix display
  - ✅ Access control trên từng page
  - ✅ Audit log cho permission changes

---

*File tiếp tục với các requirements khác...*