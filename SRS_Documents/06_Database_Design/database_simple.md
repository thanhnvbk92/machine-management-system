# THIẾT KẾ DATABASE - DỰA TRÊN HỆ THỐNG CÓ SẴN

> **Dựa trên database hiện tại của bạn: hse_pm_database**

## 1. TỔNG QUAN

### Database hiện tại
- **Database**: MySQL `hse_pm_database` 
- **Server**: 10.7.12.236
- **Cấu trúc**: Production management system với machines, lines, stations
- **Mục đích mở rộng**: Thêm log collection và remote control

---

## 2. CÁC BẢNG HIỆN CÓ (Từ file dump của bạn)

### 2.1 Bảng MACHINES (Đã có - chỉ cần mở rộng)

**Hiện tại**: Quản lý thông tin máy móc trong production
```sql
CREATE TABLE `machines` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `status` varchar(20) DEFAULT NULL,
  `MachineTypeId` int DEFAULT NULL,
  `IP` varchar(15) DEFAULT NULL,
  `GMES_Name` varchar(50) DEFAULT NULL,
  `StationID` int DEFAULT NULL,
  `ProgramName` varchar(45) DEFAULT NULL,
  `mac_address` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`ID`)
);
```

**Cần thêm cột** cho log collection system:
```sql
ALTER TABLE machines ADD COLUMN last_log_time DATETIME;
ALTER TABLE machines ADD COLUMN app_version VARCHAR(50);
ALTER TABLE machines ADD COLUMN client_status VARCHAR(20) DEFAULT 'Offline';
ALTER TABLE machines ADD COLUMN last_seen DATETIME;
```

### 2.2 Bảng LOG_DATA (MỚI - Cần tạo)

**Mục đích**: Lưu log từ các máy trong hệ thống production
```sql
CREATE TABLE log_data (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    machine_id INT NOT NULL,                    -- Link với machines.ID
    station_id INT,                             -- Link với stations.ID (optional)
    log_time DATETIME NOT NULL,                 -- Thời gian ghi log gốc
    log_level VARCHAR(20) NOT NULL,             -- Info, Warning, Error, Critical
    message TEXT NOT NULL,                      -- Nội dung log
    source VARCHAR(100),                        -- Program source (GMES_Name, ProgramName)
    raw_data TEXT,                              -- Raw log content
    received_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (machine_id) REFERENCES machines(ID),
    FOREIGN KEY (station_id) REFERENCES stations(ID),
    INDEX idx_machine_time (machine_id, log_time),
    INDEX idx_log_level (log_level, received_time)
);
```

### 2.3 Bảng COMMANDS (MỚI - Cần tạo)

**Mục đích**: Điều khiển máy từ xa thông qua client app
```sql
CREATE TABLE commands (
    command_id INT AUTO_INCREMENT PRIMARY KEY,
    machine_id INT,                             -- NULL = broadcast to all
    station_id INT,                             -- Optional: specific station
    command_type VARCHAR(50) NOT NULL,          -- StartProgram, StopProgram, RestartService
    program_name VARCHAR(100),                  -- Target program (from machines.ProgramName)
    parameters TEXT,                            -- JSON parameters
    status VARCHAR(20) DEFAULT 'Pending',       -- Pending, Sent, Executing, Done, Failed
    priority INT DEFAULT 5,                     -- 1=High, 5=Normal, 10=Low
    created_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    sent_time DATETIME,
    executed_time DATETIME,
    result_message TEXT,
    
    FOREIGN KEY (machine_id) REFERENCES machines(ID),
    FOREIGN KEY (station_id) REFERENCES stations(ID),
    INDEX idx_machine_status (machine_id, status),
    INDEX idx_pending (status, created_time) 
);
```

### 2.4 Bảng CLIENT_CONFIG (MỚI - Cần tạo)

**Mục đích**: Cấu hình cho client apps chạy trên các máy
```sql
CREATE TABLE client_config (
    config_id INT AUTO_INCREMENT PRIMARY KEY,
    machine_id INT,                             -- NULL = global config
    config_key VARCHAR(100) NOT NULL,
    config_value TEXT,
    description VARCHAR(500),
    updated_time DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (machine_id) REFERENCES machines(ID),
    UNIQUE KEY unique_machine_key (machine_id, config_key),
    INDEX idx_config_key (config_key)
);
```

---

## 3. QUAN HỆ VỚI HỆ THỐNG HIỆN TẠI

```
BUYERS ─── MODELGROUPS ─── MODELS
    │           │
    │           └─── MODELPROCESSES ─── STATIONS ─── MACHINES (có sẵn)
    │                                      │           │
LINES ─────────────────────────────────────┘           │
                                                        │
                                                        ├─── LOG_DATA (mới)
                                                        │
                                                        └─── COMMANDS (mới)

CLIENT_CONFIG (độc lập, link với MACHINES)
```

---

## 4. THAO TÁC VỚI HỆ THỐNG CÓ SẴN

### 4.1 Lấy danh sách machines đang hoạt động
```sql
SELECT 
    m.ID,
    m.Name,
    m.IP,
    m.status,
    m.GMES_Name,
    m.ProgramName,
    s.Name AS StationName,
    l.Name AS LineName
FROM machines m
JOIN stations s ON m.StationID = s.ID  
JOIN lines l ON s.LineId = l.ID
WHERE m.status = 'Running';
```

### 4.2 Thêm log từ máy cụ thể
```sql
INSERT INTO log_data (machine_id, station_id, log_time, log_level, message, source)
SELECT 
    m.ID,
    m.StationID, 
    NOW(),
    'Info',
    'Machine started successfully',
    m.ProgramName
FROM machines m 
WHERE m.IP = '192.168.1.100';
```

### 4.3 Gửi lệnh đến máy theo line
```sql
INSERT INTO commands (machine_id, command_type, program_name, parameters)
SELECT 
    m.ID,
    'RestartProgram',
    m.ProgramName,
    '{"delay_seconds": 30}'
FROM machines m
JOIN stations s ON m.StationID = s.ID
JOIN lines l ON s.LineId = l.ID  
WHERE l.Name = 'Line 01';
```

### 4.4 Xem log theo station
```sql
SELECT 
    m.Name AS MachineName,
    s.Name AS StationName,
    l.log_time,
    l.log_level,
    l.message,
    l.source
FROM log_data l
JOIN machines m ON l.machine_id = m.ID
JOIN stations s ON l.station_id = s.ID
WHERE s.Name = 'Assembly Station'
ORDER BY l.log_time DESC
LIMIT 100;
```

---

## 5. CÀI ĐẶT CHO HỆ THỐNG HIỆN TẠI

### 5.1 Tạo các bảng mới (chạy trên database hiện tại)
```sql
USE hse_pm_database;

-- Tạo bảng log_data
CREATE TABLE log_data (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    machine_id INT NOT NULL,
    station_id INT,
    log_time DATETIME NOT NULL,
    log_level VARCHAR(20) NOT NULL,
    message TEXT NOT NULL,
    source VARCHAR(100),
    raw_data TEXT,
    received_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (machine_id) REFERENCES machines(ID),
    FOREIGN KEY (station_id) REFERENCES stations(ID)
);

-- Tạo bảng commands  
CREATE TABLE commands (
    command_id INT AUTO_INCREMENT PRIMARY KEY,
    machine_id INT,
    station_id INT,
    command_type VARCHAR(50) NOT NULL,
    program_name VARCHAR(100),
    parameters TEXT,
    status VARCHAR(20) DEFAULT 'Pending',
    priority INT DEFAULT 5,
    created_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    sent_time DATETIME,
    executed_time DATETIME,
    result_message TEXT,
    FOREIGN KEY (machine_id) REFERENCES machines(ID),
    FOREIGN KEY (station_id) REFERENCES stations(ID)
);

-- Tạo bảng client_config
CREATE TABLE client_config (
    config_id INT AUTO_INCREMENT PRIMARY KEY,
    machine_id INT,
    config_key VARCHAR(100) NOT NULL,
    config_value TEXT,
    description VARCHAR(500),
    updated_time DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (machine_id) REFERENCES machines(ID),
    UNIQUE KEY unique_machine_key (machine_id, config_key)
);
```

### 5.2 Cập nhật bảng machines hiện tại
```sql
-- Thêm cột cho client app tracking
ALTER TABLE machines 
ADD COLUMN last_log_time DATETIME,
ADD COLUMN app_version VARCHAR(50),
ADD COLUMN client_status VARCHAR(20) DEFAULT 'Offline',
ADD COLUMN last_seen DATETIME;

-- Tạo index cho performance
CREATE INDEX idx_machines_client_status ON machines(client_status, last_seen);
CREATE INDEX idx_machines_ip ON machines(IP);
```

### 5.3 Dữ liệu cấu hình mặc định
```sql
INSERT INTO client_config (machine_id, config_key, config_value, description) VALUES
(NULL, 'log_retention_days', '90', 'Số ngày giữ log data'),
(NULL, 'command_timeout_minutes', '60', 'Timeout cho commands'),
(NULL, 'client_polling_interval', '30', 'Client polling interval (seconds)'),
(NULL, 'log_batch_size', '100', 'Số log entries per batch');
```

---

## 6. CLIENT APP INTEGRATION

### 6.1 Client app sẽ identify máy qua IP hoặc MAC
```sql
-- Tìm machine bằng IP
SELECT ID, Name, GMES_Name, ProgramName, StationID 
FROM machines 
WHERE IP = '192.168.1.100';

-- Tìm machine bằng MAC address  
SELECT ID, Name, GMES_Name, ProgramName, StationID 
FROM machines 
WHERE mac_address = '00:1B:44:11:3A:B7';
```

### 6.2 Update machine status khi client connect
```sql
UPDATE machines 
SET client_status = 'Online',
    last_seen = NOW(),
    app_version = 'v1.0.0'
WHERE IP = '192.168.1.100';
```

### 6.3 Lấy pending commands cho machine
```sql
SELECT command_id, command_type, program_name, parameters
FROM commands 
WHERE machine_id = 1 
  AND status = 'Pending'
ORDER BY priority ASC, created_time ASC;
```

---

## 7. BẢO TRÌ VÀ MONITORING

### 7.1 Cleanup scripts (chạy hàng ngày)
```sql
-- Xóa log cũ hơn 90 ngày
DELETE FROM log_data 
WHERE received_time < DATE_SUB(NOW(), INTERVAL 90 DAY);

-- Đánh dấu machines offline (không ping trong 5 phút)
UPDATE machines 
SET client_status = 'Offline'
WHERE last_seen < DATE_SUB(NOW(), INTERVAL 5 MINUTE)
  AND client_status = 'Online';

-- Xóa commands cũ đã hoàn thành
DELETE FROM commands 
WHERE status IN ('Done', 'Failed')
  AND executed_time < DATE_SUB(NOW(), INTERVAL 30 DAY);
```

### 7.2 Monitoring queries
```sql
-- Machines online/offline count by line
SELECT 
    l.Name AS LineName,
    COUNT(CASE WHEN m.client_status = 'Online' THEN 1 END) AS OnlineCount,
    COUNT(CASE WHEN m.client_status = 'Offline' THEN 1 END) AS OfflineCount
FROM lines l
JOIN stations s ON l.ID = s.LineId
JOIN machines m ON s.ID = m.StationID
GROUP BY l.ID, l.Name;

-- Recent error logs by station
SELECT 
    s.Name AS Station,
    COUNT(*) AS ErrorCount,
    MAX(l.log_time) AS LastError
FROM log_data l
JOIN machines m ON l.machine_id = m.ID  
JOIN stations s ON m.StationID = s.ID
WHERE l.log_level = 'Error'
  AND l.log_time >= DATE_SUB(NOW(), INTERVAL 1 DAY)
GROUP BY s.ID, s.Name
ORDER BY ErrorCount DESC;
```

---

## 8. ƯU ĐIỂM CỦA THIẾT KẾ NÀY

**✅ TÁI SỬ DỤNG HỆ THỐNG CÓ SẴN:**
- Không cần tạo lại database từ đầu
- Sử dụng được cấu trúc machines, stations, lines hiện tại
- Tích hợp seamless với GMES system
- Backup procedures đã có sẵn

**✅ MỞ RỘNG ĐƠN GIẢN:**
- Chỉ cần thêm 3 bảng mới (log_data, commands, client_config)
- Modify bảng machines với vài cột thêm
- Không ảnh hưởng đến hệ thống production hiện tại
- Có thể rollback dễ dàng nếu cần

**✅ TƯƠNG THÍCH PRODUCTION:**
- Sử dụng đúng naming convention hiện tại
- Foreign keys link với bảng stations và machines
- Query được log theo line, station, machine hierarchy
- Hỗ trợ GMES_Name và ProgramName có sẵn

**📝 LỢI ÍCH CHO CLIENT APPS:**
- Client app identify máy qua IP address (đã có sẵn)
- Có thể điều khiển program theo ProgramName
- Log được phân loại theo station và line
- Commands có thể target specific machine hoặc broadcast

---

## 9. DEPLOYMENT PLAN

### Phase 1: Preparation (1 ngày)
```sql
-- 1. Backup database hiện tại
mysqldump -h 10.7.12.236 -u username -p hse_pm_database > backup_before_upgrade.sql

-- 2. Test trên development database
mysql -u username -p test_hse_pm_database < backup_before_upgrade.sql
```

### Phase 2: Add new tables (30 phút - không downtime)
```sql
USE hse_pm_database;
-- Chạy tất cả CREATE TABLE statements từ section 5.1
```

### Phase 3: Modify existing table (10 phút - có thể có short downtime)
```sql
-- Thêm cột vào bảng machines
ALTER TABLE machines ADD COLUMN last_log_time DATETIME;
ALTER TABLE machines ADD COLUMN app_version VARCHAR(50);
ALTER TABLE machines ADD COLUMN client_status VARCHAR(20) DEFAULT 'Offline';
ALTER TABLE machines ADD COLUMN last_seen DATETIME;
```

### Phase 4: Initialize config data (5 phút)
```sql
-- Insert default configuration
INSERT INTO client_config (machine_id, config_key, config_value, description) VALUES...
```

**📊 TOTAL DOWNTIME: < 15 phút (chỉ khi modify bảng machines)**