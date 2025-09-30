# SRS - Database Design Document
## Machine Management System Database Specification

### Phiên bản: 2.0
### Database: **hse_pm_db** (MySQL 8.0)
### Ngày cập nhật: 30/09/2025

---

## 1. TỔNG QUAN DATABASE

### 1.1 Thông tin cơ bản
- **Database Name**: `hse_pm_db`
- **Database Engine**: MySQL 8.0
- **Character Set**: `utf8mb4`
- **Collation**: `utf8mb4_0900_ai_ci`
- **Total Tables**: 12 bảng chính + 3 views

### 1.2 Kiến trúc hệ thống
Database được thiết kế theo mô hình **hybrid**:
- **Phần gốc**: Tái sử dụng cấu trúc từ `hse_pm_database` hiện có
- **Phần mở rộng**: Thêm 4 bảng mới cho Machine Management System

```
BUYERS ─── MODELGROUPS ─── MODELS
    │           │
    │           └─── MODELPROCESSES ─── STATIONS ─── MACHINES
    │                                      │           │
LINES ─────────────────────────────────────┘           │
                                                        │
                                                        ├─── LOG_FILE ─── LOG_DATA
                                                        │
                                                        ├─── COMMANDS
                                                        │
                                                        └─── CLIENT_CONFIG
```

---

## 2. CẤU TRÚC CÁC BẢNG HIỆN CÓ (Từ hse_pm_database)

### 2.1 Bảng `buyers`
**Mục đích**: Quản lý khách hàng/đối tác
```sql
CREATE TABLE `buyers` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(10) NOT NULL,
  `Name` varchar(50) NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `Code` (`Code`),
  UNIQUE KEY `Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

### 2.2 Bảng `lines`
**Mục đích**: Quản lý các line sản xuất
```sql
CREATE TABLE `lines` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(20) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

### 2.3 Bảng `machinetypes`
**Mục đích**: Phân loại các loại máy
```sql
CREATE TABLE `machinetypes` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

### 2.4 Bảng `modelgroups`
**Mục đích**: Nhóm các model sản phẩm
```sql
CREATE TABLE `modelgroups` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `BuyerId` int NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `Name` (`Name`),
  KEY `BuyerId` (`BuyerId`),
  CONSTRAINT `modelgroups_ibfk_1` FOREIGN KEY (`BuyerId`) REFERENCES `buyers` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

### 2.5 Bảng `models`
**Mục đích**: Chi tiết các model sản phẩm
```sql
CREATE TABLE `models` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `ModelGroupID` int NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `Name` (`Name`),
  KEY `ModelGroupID` (`ModelGroupID`),
  CONSTRAINT `models_ibfk_1` FOREIGN KEY (`ModelGroupID`) REFERENCES `modelgroups` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

### 2.6 Bảng `modelprocesses`
**Mục đích**: Quy trình sản xuất cho từng model
```sql
CREATE TABLE `modelprocesses` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `ModelGroupID` int NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `ModelGroupID` (`ModelGroupID`),
  CONSTRAINT `modelprocesses_ibfk_1` FOREIGN KEY (`ModelGroupID`) REFERENCES `modelgroups` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

### 2.7 Bảng `stations`
**Mục đích**: Các trạm làm việc trong line sản xuất
```sql
CREATE TABLE `stations` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `ModelProcessId` int NOT NULL,
  `LineId` int NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `LineId` (`LineId`),
  KEY `stations_ibfk_1` (`ModelProcessId`),
  CONSTRAINT `stations_ibfk_1` FOREIGN KEY (`ModelProcessId`) REFERENCES `modelprocesses` (`ID`),
  CONSTRAINT `stations_ibfk_2` FOREIGN KEY (`LineId`) REFERENCES `lines` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

---

## 3. BẢNG MACHINES (Đã mở rộng)

### 3.1 Cấu trúc bảng `machines`
**Mục đích**: Quản lý máy móc trong hệ thống (đã thêm client tracking)
```sql
CREATE TABLE `machines` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `status` varchar(20) DEFAULT NULL,              -- Running, Offline, Error, Maintenance
  `MachineTypeId` int DEFAULT NULL,
  `IP` varchar(15) DEFAULT NULL,
  `GMES_Name` varchar(50) DEFAULT NULL,           -- Tên trong hệ thống GMES
  `StationID` int DEFAULT NULL,
  `ProgramName` varchar(45) DEFAULT NULL,         -- Program đang chạy
  `mac_address` varchar(50) DEFAULT NULL,         -- MAC address
  
  -- ✅ CÁC CỘT MỚI CHO CLIENT TRACKING
  `last_log_time` datetime DEFAULT NULL,          -- Thời gian log cuối
  `app_version` varchar(50) DEFAULT NULL,         -- Version của client app
  `client_status` varchar(20) DEFAULT 'Offline',  -- Online, Offline
  `last_seen` datetime DEFAULT NULL,              -- Lần cuối client ping
  
  PRIMARY KEY (`ID`),
  UNIQUE KEY `IP_UNIQUE` (`IP`),
  UNIQUE KEY `idx_machines_mac` (`mac_address`),
  KEY `StationID` (`StationID`),
  KEY `machines_machinetypes_idx` (`MachineTypeId`),
  KEY `idx_client_status` (`client_status`, `last_seen`),
  CONSTRAINT `machines_ibfk_1` FOREIGN KEY (`StationID`) REFERENCES `stations` (`ID`),
  CONSTRAINT `machines_machinetypes` FOREIGN KEY (`MachineTypeId`) REFERENCES `machinetypes` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

### 3.2 Ý nghĩa các cột mới
- `last_log_time`: Timestamp log cuối cùng nhận từ máy
- `app_version`: Version của client application
- `client_status`: Trạng thái kết nối của client (Online/Offline)
- `last_seen`: Thời điểm cuối client ping server

---

## 4. CÁC BẢNG MỚI CHO MACHINE MANAGEMENT SYSTEM

### 4.1 Bảng `log_file`
**Mục đích**: Quản lý các file log từ máy
```sql
CREATE TABLE `log_file` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `file_name` varchar(100) NOT NULL,              -- Tên file log
  `machine_id` int NOT NULL,                      -- Máy tạo file
  `date_created` date NOT NULL,                   -- Ngày tạo file
  `file_size` bigint DEFAULT 0,                   -- Kích thước file (bytes)
  `status` varchar(20) DEFAULT 'Processing',      -- Processing, Completed, Error
  `created_time` datetime DEFAULT CURRENT_TIMESTAMP,
  
  PRIMARY KEY (`ID`),
  KEY `idx_machine_date` (`machine_id`, `date_created`),
  KEY `idx_file_name` (`file_name`),
  CONSTRAINT `log_file_machine_fk` FOREIGN KEY (`machine_id`) REFERENCES `machines` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

### 4.2 Bảng `log_data`
**Mục đích**: Lưu chi tiết dữ liệu log từ máy (theo format GMES)
```sql
CREATE TABLE `log_data` (
  `log_id` int NOT NULL AUTO_INCREMENT,
  `file_id` int NOT NULL,                         -- Link đến log_file
  `machine_id` int NOT NULL,                      -- Máy tạo log
  `station_id` int DEFAULT NULL,                  -- Station tương ứng
  `model_id` int DEFAULT NULL,                    -- Model sản phẩm đang xử lý
  
  -- ✅ CÁC TRƯỜNG THEO FORMAT GMES/PRODUCTION
  `EQPINFO` varchar(50) DEFAULT NULL,             -- Thông tin thiết bị
  `PROCINFO` varchar(10) DEFAULT NULL,            -- Thông tin process
  `PID` varchar(22) NOT NULL,                     -- Process ID
  `FID` varchar(22) DEFAULT NULL,                 -- File ID
  `part_no` varchar(11) DEFAULT NULL,             -- Số part
  `variant` varchar(20) DEFAULT NULL,             -- Biến thể sản phẩm
  `result` varchar(15) NOT NULL,                  -- PASS/FAIL/NG
  `jobfile` varchar(100) NOT NULL,                -- File job thực thi
  `gmes_status` varchar(10) NOT NULL,             -- OK/NG từ GMES
  `start_time` datetime NOT NULL,                 -- Thời gian bắt đầu
  `end_time` datetime DEFAULT NULL,               -- Thời gian kết thúc
  `stepNG` varchar(255) DEFAULT NULL,             -- Bước lỗi (nếu có)
  `measure` varchar(255) DEFAULT NULL,            -- Giá trị đo
  `spec_min` varchar(255) DEFAULT NULL,           -- Spec tối thiểu
  `spec_max` varchar(255) DEFAULT NULL,           -- Spec tối đa
  
  -- ✅ CÁC TRƯỜNG HỆ THỐNG
  `log_level` varchar(20) DEFAULT 'Info',         -- Info, Warning, Error, Critical
  `source` varchar(100) DEFAULT NULL,             -- Nguồn tạo log
  `raw_data` text,                                -- Raw content
  `received_time` datetime DEFAULT CURRENT_TIMESTAMP,
  
  PRIMARY KEY (`log_id`),
  KEY `idx_machine_time` (`machine_id`, `start_time`),
  KEY `idx_log_level` (`log_level`, `received_time`),
  KEY `idx_result` (`result`, `received_time`),
  KEY `idx_model_time` (`model_id`, `start_time`),
  CONSTRAINT `log_data_file_fk` FOREIGN KEY (`file_id`) REFERENCES `log_file` (`ID`),
  CONSTRAINT `log_data_machine_fk` FOREIGN KEY (`machine_id`) REFERENCES `machines` (`ID`),
  CONSTRAINT `log_data_station_fk` FOREIGN KEY (`station_id`) REFERENCES `stations` (`ID`),
  CONSTRAINT `log_data_model_fk` FOREIGN KEY (`model_id`) REFERENCES `models` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

### 4.3 Bảng `commands`
**Mục đích**: Điều khiển máy từ xa thông qua client app
```sql
CREATE TABLE `commands` (
  `command_id` int NOT NULL AUTO_INCREMENT,
  `machine_id` int DEFAULT NULL,                  -- NULL = broadcast all
  `station_id` int DEFAULT NULL,                  -- Station cụ thể
  `command_type` varchar(50) NOT NULL,            -- StartProgram, StopProgram, RestartService
  `program_name` varchar(100) DEFAULT NULL,       -- Tên program target
  `parameters` text,                              -- JSON parameters
  `status` varchar(20) DEFAULT 'Pending',         -- Pending, Sent, Executing, Done, Failed
  `priority` int DEFAULT 5,                       -- 1=High, 5=Normal, 10=Low
  `created_time` datetime DEFAULT CURRENT_TIMESTAMP,
  `sent_time` datetime DEFAULT NULL,              -- Thời gian gửi lệnh
  `executed_time` datetime DEFAULT NULL,          -- Thời gian thực thi
  `result_message` text,                          -- Kết quả thực thi
  
  PRIMARY KEY (`command_id`),
  KEY `idx_machine_status` (`machine_id`, `status`),
  KEY `idx_pending` (`status`, `created_time`),
  CONSTRAINT `commands_machine_fk` FOREIGN KEY (`machine_id`) REFERENCES `machines` (`ID`),
  CONSTRAINT `commands_station_fk` FOREIGN KEY (`station_id`) REFERENCES `stations` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```



























































































### 4.4 Bảng `client_config`
**Mục đích**: Cấu hình cho client applications
```sql
CREATE TABLE `client_config` (
  `config_id` int NOT NULL AUTO_INCREMENT,
  `machine_id` int DEFAULT NULL,                  -- NULL = global config
  `config_key` varchar(100) NOT NULL,             -- Tên config
  `config_value` text,                            -- Giá trị config
  `description` varchar(500) DEFAULT NULL,        -- Mô tả config
  `updated_time` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  
  PRIMARY KEY (`config_id`),
  UNIQUE KEY `unique_machine_key` (`machine_id`, `config_key`),
  KEY `idx_config_key` (`config_key`),
  CONSTRAINT `client_config_machine_fk` FOREIGN KEY (`machine_id`) REFERENCES `machines` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

---

## 5. VIEWS HỖ TRỢ

### 5.1 View `v_machine_status`
**Mục đích**: Tổng hợp trạng thái máy với thông tin station và line
```sql
CREATE VIEW v_machine_status AS
SELECT 
    m.ID as machine_id,
    m.Name as machine_name,
    m.status as machine_status,
    m.client_status,
    m.IP,
    m.GMES_Name,
    m.ProgramName,
    s.Name as station_name,
    l.Name as line_name,
    m.last_seen,
    m.app_version
FROM machines m
LEFT JOIN stations s ON m.StationID = s.ID
LEFT JOIN `lines` l ON s.LineId = l.ID;
```

### 5.2 View `v_recent_logs`
**Mục đích**: Logs gần đây theo machine/station/line/model
```sql
CREATE VIEW v_recent_logs AS
SELECT 
    m.Name as machine_name,
    s.Name as station_name,
    l.Name as line_name,
    md.Name as model_name,
    ld.log_level,
    ld.result,
    ld.start_time,
    ld.source,
    ld.PID
FROM log_data ld
JOIN machines m ON ld.machine_id = m.ID
LEFT JOIN stations s ON ld.station_id = s.ID
LEFT JOIN `lines` l ON s.LineId = l.ID
LEFT JOIN models md ON ld.model_id = md.ID
WHERE ld.received_time >= DATE_SUB(NOW(), INTERVAL 1 DAY)
ORDER BY ld.received_time DESC;
```

### 5.3 View `v_pending_commands`
**Mục đích**: Commands đang chờ thực thi
```sql
CREATE VIEW v_pending_commands AS
SELECT 
    c.command_id,
    m.Name as machine_name,
    s.Name as station_name,
    c.command_type,
    c.program_name,
    c.status,
    c.priority,
    c.created_time
FROM commands c
LEFT JOIN machines m ON c.machine_id = m.ID
LEFT JOIN stations s ON c.station_id = s.ID
WHERE c.status IN ('Pending', 'Sent', 'Executing')
ORDER BY c.priority ASC, c.created_time ASC;
```

---

## 6. DỮ LIỆU MẪU

### 6.1 Dữ liệu từ hse_pm_database
- ✅ **Đã sao chép**: buyers, lines, machinetypes, modelgroups, models, modelprocesses, stations, machines
- ✅ **Đã cập nhật**: machines với client tracking fields

### 6.2 Dữ liệu mẫu cho bảng mới
```sql
-- Global configurations
INSERT INTO client_config (machine_id, config_key, config_value, description) VALUES
(NULL, 'log_retention_days', '90', 'Số ngày giữ log data'),
(NULL, 'command_timeout_minutes', '60', 'Timeout cho commands'),
(NULL, 'client_polling_interval', '30', 'Client polling interval (seconds)'),
(NULL, 'log_batch_size', '100', 'Số log entries per batch'),
(NULL, 'max_file_size_mb', '50', 'Maximum log file size in MB');
```

---

## 7. INDEXES VÀ PERFORMANCE

### 7.1 Indexes chính
- `machines`: IP, mac_address, client_status + last_seen
- `log_data`: machine_id + start_time, log_level + received_time, result + received_time, model_id + start_time
- `commands`: machine_id + status, status + created_time
- `client_config`: config_key, machine_id + config_key (unique)

### 7.2 Foreign Keys
- Tất cả relationships đều có foreign key constraints
- Cascade actions được thiết lập phù hợp

---

## 8. INTEGRATION VỚI CLIENT APPS

### 8.1 Client App Workflow
1. **Machine Identification**: Sử dụng IP hoặc MAC address
2. **Status Update**: Cập nhật client_status, last_seen, app_version
3. **Log Submission**: Gửi log data theo format GMES
4. **Command Polling**: Check pending commands định kỳ
5. **Config Retrieval**: Lấy configuration settings

### 8.2 API Endpoints Integration
- `GET /api/machines/{ip}/status` - Machine status
- `POST /api/logs/upload` - Upload log data
- `GET /api/commands/pending/{machineId}` - Get pending commands
- `PUT /api/commands/{commandId}/result` - Update command result
- `GET /api/config/{machineId}` - Get machine configurations

---

## 9. BACKUP VÀ MAINTENANCE

### 9.1 Retention Policies
- **Log Data**: 90 ngày (configurable)
- **Commands**: 30 ngày sau khi Done/Failed
- **Machine Status**: Realtime, archive monthly

### 9.2 Monitoring Queries
```sql
-- Machine online/offline by line
SELECT l.Name, COUNT(CASE WHEN m.client_status='Online' THEN 1 END) as Online,
       COUNT(CASE WHEN m.client_status='Offline' THEN 1 END) as Offline
FROM machines m 
JOIN stations s ON m.StationID = s.ID 
JOIN `lines` l ON s.LineId = l.ID 
GROUP BY l.ID, l.Name;

-- Error logs last 24h
SELECT COUNT(*) as ErrorCount FROM log_data 
WHERE log_level = 'Error' AND received_time >= DATE_SUB(NOW(), INTERVAL 1 DAY);
```

---

## 10. KẾT LUẬN

✅ **Database Structure**: Hoàn chình và ready for production  
✅ **Data Migration**: Đã sao chép từ hse_pm_database  
✅ **Client Integration**: Support đầy đủ cho WPF và Web clients  
✅ **Performance**: Indexes và views được optimize  
✅ **Monitoring**: Views và queries hỗ trợ real-time monitoring  

**Database Name**: `hse_pm_db`  
**Total Tables**: 12 + 3 views  
**Ready for**: Backend API, WPF Client, ManagerApp  