-- =====================================================================
-- SCRIPT TẠO DATABASE HSE_PM_DB THEO SRS DOCUMENT SECTION 5
-- Machine Management System - Database Creation Script
-- =====================================================================

-- Drop và tạo lại database
DROP DATABASE IF EXISTS HSE_PM_DB;
CREATE DATABASE HSE_PM_DB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE HSE_PM_DB;

-- =====================================================================
-- 1. TẠO CÁC BẢNG HỆ THỐNG CỐ SẴN (theo SRS - tái sử dụng)
-- =====================================================================

-- Bảng BUYERS
CREATE TABLE buyers (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Code VARCHAR(50) UNIQUE,
    Description TEXT,
    created_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_time DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Bảng LINES  
CREATE TABLE lines (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    buyer_id INT,
    Description TEXT,
    created_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (buyer_id) REFERENCES buyers(ID),
    INDEX idx_buyer (buyer_id)
);

-- Bảng MODELGROUPS
CREATE TABLE modelgroups (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    buyer_id INT,
    Description TEXT,
    created_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (buyer_id) REFERENCES buyers(ID),
    INDEX idx_buyer (buyer_id)
);

-- Bảng MODELS
CREATE TABLE models (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    modelgroup_id INT,
    part_number VARCHAR(50),
    Description TEXT,
    created_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (modelgroup_id) REFERENCES modelgroups(ID),
    INDEX idx_modelgroup (modelgroup_id)
);

-- Bảng MODELPROCESSES
CREATE TABLE modelprocesses (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    model_id INT,
    process_order INT,
    Description TEXT,
    created_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (model_id) REFERENCES models(ID),
    INDEX idx_model (model_id)
);

-- Bảng STATIONS
CREATE TABLE stations (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    LineId INT,
    modelprocess_id INT,
    station_type VARCHAR(50),
    Description TEXT,
    created_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (LineId) REFERENCES lines(ID),
    FOREIGN KEY (modelprocess_id) REFERENCES modelprocesses(ID),
    INDEX idx_line (LineId),
    INDEX idx_modelprocess (modelprocess_id)
);

-- Bảng MACHINES (Core table - theo SRS design)
CREATE TABLE machines (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    status VARCHAR(20) DEFAULT 'Offline',                -- Running, Offline, Error, Maintenance
    MachineTypeId INT,
    IP VARCHAR(15),
    GMES_Name VARCHAR(100),                              -- Tên trong hệ thống GMES
    StationID INT,
    ProgramName VARCHAR(100),                            -- Program đang chạy trên máy
    mac_address VARCHAR(17),                             -- MAC address để identify
    
    -- Thêm cột cho client app tracking (theo SRS section 5.2)
    last_log_time DATETIME,
    app_version VARCHAR(50),
    client_status VARCHAR(20) DEFAULT 'Offline',         -- Online, Offline
    last_seen DATETIME,
    
    created_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_time DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (StationID) REFERENCES stations(ID),
    INDEX idx_station (StationID),
    INDEX idx_ip (IP),
    INDEX idx_client_status (client_status, last_seen),
    INDEX idx_gmes_name (GMES_Name)
);

-- =====================================================================
-- 2. TẠO CÁC BẢNG MỚI (theo SRS section 5.1)
-- =====================================================================

-- Bảng LOG_FILE (quản lý file log)
CREATE TABLE log_file (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    file_name VARCHAR(100) NOT NULL,
    machine_id INT NOT NULL,
    date_created DATE NOT NULL,
    file_size BIGINT DEFAULT 0,
    status VARCHAR(20) DEFAULT 'Processing',              -- Processing, Completed, Error
    created_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (machine_id) REFERENCES machines(ID),
    INDEX idx_machine_date (machine_id, date_created),
    INDEX idx_file_name (file_name)
);

-- Bảng LOG_DATA (lưu chi tiết log data)
CREATE TABLE log_data (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    file_id INT NOT NULL,
    machine_id INT NOT NULL,
    station_id INT,
    
    -- Log content fields (theo SRS format)
    EQPINFO VARCHAR(50),
    PROCINFO VARCHAR(10), 
    PID VARCHAR(22) NOT NULL,
    FID VARCHAR(22),
    part_no VARCHAR(11),
    variant VARCHAR(20),
    result VARCHAR(15) NOT NULL,
    jobfile VARCHAR(100) NOT NULL,
    gmes_status VARCHAR(10) NOT NULL,
    start_time DATETIME NOT NULL,
    end_time DATETIME,
    stepNG VARCHAR(255),
    measure VARCHAR(255),
    spec_min VARCHAR(255),
    spec_max VARCHAR(255),
    
    -- System fields
    log_level VARCHAR(20) DEFAULT 'Info',                -- Info, Warning, Error, Critical
    source VARCHAR(100),                                 -- Program source (GMES_Name, ProgramName)
    raw_data TEXT,                                       -- Raw log content
    received_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (file_id) REFERENCES log_file(ID),
    FOREIGN KEY (machine_id) REFERENCES machines(ID),
    FOREIGN KEY (station_id) REFERENCES stations(ID),
    INDEX idx_machine_time (machine_id, start_time),
    INDEX idx_log_level (log_level, received_time),
    INDEX idx_result (result, received_time)
);

-- Bảng COMMANDS (điều khiển máy từ xa - theo SRS section 2.3)
CREATE TABLE commands (
    command_id INT AUTO_INCREMENT PRIMARY KEY,
    machine_id INT,                                      -- NULL = broadcast to all
    station_id INT,                                      -- Optional: specific station
    command_type VARCHAR(50) NOT NULL,                   -- StartProgram, StopProgram, RestartService
    program_name VARCHAR(100),                           -- Target program (from machines.ProgramName)
    parameters TEXT,                                     -- JSON parameters
    status VARCHAR(20) DEFAULT 'Pending',                -- Pending, Sent, Executing, Done, Failed
    priority INT DEFAULT 5,                              -- 1=High, 5=Normal, 10=Low
    created_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    sent_time DATETIME,
    executed_time DATETIME,
    result_message TEXT,
    
    FOREIGN KEY (machine_id) REFERENCES machines(ID),
    FOREIGN KEY (station_id) REFERENCES stations(ID),
    INDEX idx_machine_status (machine_id, status),
    INDEX idx_pending (status, created_time)
);

-- Bảng CLIENT_CONFIG (cấu hình cho client apps - theo SRS section 2.4)
CREATE TABLE client_config (
    config_id INT AUTO_INCREMENT PRIMARY KEY,
    machine_id INT,                                      -- NULL = global config
    config_key VARCHAR(100) NOT NULL,
    config_value TEXT,
    description VARCHAR(500),
    updated_time DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (machine_id) REFERENCES machines(ID),
    UNIQUE KEY unique_machine_key (machine_id, config_key),
    INDEX idx_config_key (config_key)
);

-- =====================================================================
-- 3. DỮ LIỆU MẪU (theo SRS requirements)
-- =====================================================================

-- Insert Buyers
INSERT INTO buyers (Name, Code, Description) VALUES
('Honda Vietnam', 'HVN', 'Honda Vietnam Assembly'),
('Toyota Vietnam', 'TVN', 'Toyota Vietnam Motors'),
('Hyundai Vietnam', 'HYU', 'Hyundai Thanh Cong Vietnam');

-- Insert Lines  
INSERT INTO lines (Name, buyer_id, Description) VALUES
('Line 01 - Engine Assembly', 1, 'Honda Engine Assembly Line'),
('Line 02 - Body Welding', 1, 'Honda Body Welding Line'),
('Line 03 - Paint Shop', 2, 'Toyota Paint Shop Line'),
('Line 04 - Final Assembly', 3, 'Hyundai Final Assembly Line');

-- Insert Model Groups
INSERT INTO modelgroups (Name, buyer_id, Description) VALUES
('Honda Civic Series', 1, 'Civic model group'),
('Honda CR-V Series', 1, 'CR-V model group'),
('Toyota Camry Series', 2, 'Camry model group'),
('Hyundai Accent Series', 3, 'Accent model group');

-- Insert Models
INSERT INTO models (Name, modelgroup_id, part_number, Description) VALUES
('Civic 2024', 1, 'HC2024', 'Honda Civic 2024 model'),
('Civic Sport 2024', 1, 'HCS2024', 'Honda Civic Sport 2024'),
('CR-V 2024', 2, 'CRV2024', 'Honda CR-V 2024 model'),
('Camry 2024', 3, 'CAM2024', 'Toyota Camry 2024 model'),
('Accent 2024', 4, 'ACC2024', 'Hyundai Accent 2024 model');

-- Insert Model Processes
INSERT INTO modelprocesses (Name, model_id, process_order, Description) VALUES
('Engine Install', 1, 1, 'Install engine for Civic'),
('Transmission Mount', 1, 2, 'Mount transmission'),
('Body Welding', 2, 1, 'Body welding process'),
('Paint Application', 3, 1, 'Paint application process'),
('Final Assembly', 4, 1, 'Final assembly process'),
('Quality Check', 5, 1, 'Quality inspection process');

-- Insert Stations
INSERT INTO stations (Name, LineId, modelprocess_id, station_type, Description) VALUES
('Engine Station 01', 1, 1, 'Assembly', 'Engine installation station'),
('Engine Station 02', 1, 2, 'Assembly', 'Transmission mounting station'),
('Welding Station 01', 2, 3, 'Welding', 'Body welding station'),
('Paint Station 01', 3, 4, 'Painting', 'Paint application station'),
('Assembly Station 01', 4, 5, 'Assembly', 'Final assembly station'),
('QC Station 01', 4, 6, 'Quality', 'Quality control station');

-- Insert Machines (theo SRS design với đầy đủ thông tin)
INSERT INTO machines (Name, status, IP, GMES_Name, StationID, ProgramName, mac_address, client_status) VALUES
('Machine_ENG_01', 'Running', '192.168.1.101', 'ENG01_CTRL', 1, 'EngineInstaller.exe', '00:1B:44:11:3A:B1', 'Online'),
('Machine_ENG_02', 'Running', '192.168.1.102', 'ENG02_CTRL', 2, 'TransmissionMounter.exe', '00:1B:44:11:3A:B2', 'Online'),
('Machine_WLD_01', 'Offline', '192.168.1.201', 'WLD01_CTRL', 3, 'WeldingController.exe', '00:1B:44:11:3A:C1', 'Offline'),
('Machine_PNT_01', 'Running', '192.168.1.301', 'PNT01_CTRL', 4, 'PaintController.exe', '00:1B:44:11:3A:D1', 'Online'),
('Machine_ASM_01', 'Running', '192.168.1.401', 'ASM01_CTRL', 5, 'AssemblyController.exe', '00:1B:44:11:3A:E1', 'Online'),
('Machine_QC_01', 'Running', '192.168.1.501', 'QC01_CTRL', 6, 'QualityChecker.exe', '00:1B:44:11:3A:F1', 'Online');

-- Insert Log Files (sample data)
INSERT INTO log_file (file_name, machine_id, date_created, file_size, status) VALUES
('ENG01_20240930.log', 1, '2024-09-30', 1024000, 'Completed'),
('ENG02_20240930.log', 2, '2024-09-30', 856000, 'Completed'),
('PNT01_20240930.log', 4, '2024-09-30', 2048000, 'Processing'),
('ASM01_20240930.log', 5, '2024-09-30', 1536000, 'Completed'),
('QC01_20240930.log', 6, '2024-09-30', 768000, 'Completed');

-- Insert Sample Log Data
INSERT INTO log_data (file_id, machine_id, station_id, EQPINFO, PROCINFO, PID, result, jobfile, gmes_status, start_time, end_time, log_level, source) VALUES
(1, 1, 1, 'ENG01', 'INSTALL', 'PID_001', 'PASS', 'engine_install_job01.xml', 'OK', '2024-09-30 08:00:00', '2024-09-30 08:15:00', 'Info', 'EngineInstaller.exe'),
(1, 1, 1, 'ENG01', 'INSTALL', 'PID_002', 'FAIL', 'engine_install_job02.xml', 'NG', '2024-09-30 08:16:00', '2024-09-30 08:20:00', 'Error', 'EngineInstaller.exe'),
(2, 2, 2, 'ENG02', 'MOUNT', 'PID_003', 'PASS', 'transmission_mount_job01.xml', 'OK', '2024-09-30 08:30:00', '2024-09-30 08:45:00', 'Info', 'TransmissionMounter.exe'),
(4, 5, 5, 'ASM01', 'ASSEMBLY', 'PID_004', 'PASS', 'final_assembly_job01.xml', 'OK', '2024-09-30 09:00:00', '2024-09-30 09:30:00', 'Info', 'AssemblyController.exe'),
(5, 6, 6, 'QC01', 'CHECK', 'PID_005', 'PASS', 'quality_check_job01.xml', 'OK', '2024-09-30 10:00:00', '2024-09-30 10:15:00', 'Info', 'QualityChecker.exe');

-- Insert Sample Commands
INSERT INTO commands (machine_id, station_id, command_type, program_name, parameters, status, priority) VALUES
(1, 1, 'StartProgram', 'EngineInstaller.exe', '{"job_file": "engine_install_job03.xml"}', 'Done', 1),
(2, 2, 'RestartService', 'TransmissionMounter.exe', '{"delay_seconds": 30}', 'Pending', 5),
(NULL, NULL, 'StopProgram', 'All', '{}', 'Pending', 10),
(4, 4, 'StartProgram', 'PaintController.exe', '{"color": "red", "thickness": "medium"}', 'Executing', 3);

-- Insert Global Configuration (theo SRS section 5.3)
INSERT INTO client_config (machine_id, config_key, config_value, description) VALUES
(NULL, 'log_retention_days', '90', 'Số ngày giữ log data'),
(NULL, 'command_timeout_minutes', '60', 'Timeout cho commands'),
(NULL, 'client_polling_interval', '30', 'Client polling interval (seconds)'),
(NULL, 'log_batch_size', '100', 'Số log entries per batch'),
(NULL, 'max_file_size_mb', '50', 'Maximum log file size in MB'),
(1, 'custom_engine_config', 'turbo_mode=true', 'Custom config for Engine Machine 01'),
(4, 'paint_pressure', '2.5', 'Paint pressure setting for Paint Machine 01');

-- =====================================================================
-- 4. CẬP NHẬT TIMESTAMPS CHO SAMPLE DATA
-- =====================================================================

-- Cập nhật last_seen và last_log_time cho machines online
UPDATE machines 
SET last_seen = NOW(),
    last_log_time = NOW(),
    app_version = 'v1.0.0'
WHERE client_status = 'Online';

-- =====================================================================
-- 5. TẠO VIEWS HỖ TRỢ (theo SRS recommendations)
-- =====================================================================

-- View: Machine status with station and line info
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
JOIN stations s ON m.StationID = s.ID
JOIN lines l ON s.LineId = l.ID;

-- View: Recent logs summary
CREATE VIEW v_recent_logs AS
SELECT 
    m.Name as machine_name,
    s.Name as station_name,
    l.Name as line_name,
    ld.log_level,
    ld.result,
    ld.start_time,
    ld.source,
    ld.PID
FROM log_data ld
JOIN machines m ON ld.machine_id = m.ID
JOIN stations s ON ld.station_id = s.ID
JOIN lines l ON s.LineId = l.ID
WHERE ld.received_time >= DATE_SUB(NOW(), INTERVAL 1 DAY)
ORDER BY ld.received_time DESC;

-- View: Pending commands
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

-- =====================================================================
-- HOÀN THÀNH TẠO DATABASE THEO SRS SECTION 5
-- =====================================================================

SELECT 'Database HSE_PM_DB đã được tạo thành công theo SRS Document Section 5!' as message;
SELECT 'Machines count:' as info, COUNT(*) as count FROM machines;
SELECT 'Log data count:' as info, COUNT(*) as count FROM log_data;  
SELECT 'Commands count:' as info, COUNT(*) as count FROM commands;
SELECT 'Config count:' as info, COUNT(*) as count FROM client_config;