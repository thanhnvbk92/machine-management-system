-- =====================================================================
-- SCRIPT CẬP NHẬT DATABASE HSE_PM_DB 
-- Sao chép cấu trúc từ hse_pm_database + Thêm 4 bảng mới theo SRS
-- =====================================================================

-- Sử dụng database hse_pm_db
USE hse_pm_db;

-- Drop và tạo lại database để đảm bảo clean
DROP DATABASE IF EXISTS hse_pm_db;
CREATE DATABASE hse_pm_db CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE hse_pm_db;

-- =====================================================================
-- 1. TẠO CÁC BẢNG TỪHSE_PM_DATABASE (cấu trúc hiện có)
-- =====================================================================

-- Bảng buyers
CREATE TABLE `buyers` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(10) NOT NULL,
  `Name` varchar(50) NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `Code` (`Code`),
  UNIQUE KEY `Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Bảng lines
CREATE TABLE `lines` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(20) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Bảng machinetypes
CREATE TABLE `machinetypes` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Bảng modelgroups
CREATE TABLE `modelgroups` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `BuyerId` int NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `Name` (`Name`),
  KEY `BuyerId` (`BuyerId`),
  CONSTRAINT `modelgroups_ibfk_1` FOREIGN KEY (`BuyerId`) REFERENCES `buyers` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Bảng modelprocesses
CREATE TABLE `modelprocesses` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `ModelGroupID` int NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `ModelGroupID` (`ModelGroupID`),
  CONSTRAINT `modelprocesses_ibfk_1` FOREIGN KEY (`ModelGroupID`) REFERENCES `modelgroups` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Bảng models
CREATE TABLE `models` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `ModelGroupID` int NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `Name` (`Name`),
  KEY `ModelGroupID` (`ModelGroupID`),
  CONSTRAINT `models_ibfk_1` FOREIGN KEY (`ModelGroupID`) REFERENCES `modelgroups` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Bảng stations
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

-- Bảng machines (cấu trúc gốc + thêm client tracking columns)
CREATE TABLE `machines` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `status` varchar(20) DEFAULT NULL,
  `MachineTypeId` int DEFAULT NULL,
  `IP` varchar(15) DEFAULT NULL,
  `GMES_Name` varchar(50) DEFAULT NULL,
  `StationID` int DEFAULT NULL,
  `ProgramName` varchar(45) DEFAULT NULL,
  `mac_address` varchar(50) DEFAULT NULL COMMENT 'MAC address of the machine',
  
  -- Thêm client tracking fields (theo SRS section 5.2)
  `last_log_time` DATETIME DEFAULT NULL,
  `app_version` VARCHAR(50) DEFAULT NULL,
  `client_status` VARCHAR(20) DEFAULT 'Offline',         -- Online, Offline
  `last_seen` DATETIME DEFAULT NULL,
  
  PRIMARY KEY (`ID`),
  UNIQUE KEY `IP_UNIQUE` (`IP`),
  UNIQUE KEY `idx_machines_mac` (`mac_address`),
  KEY `StationID` (`StationID`),
  KEY `machines_machinetypes_idx` (`MachineTypeId`),
  KEY `idx_client_status` (`client_status`, `last_seen`),
  CONSTRAINT `machines_ibfk_1` FOREIGN KEY (`StationID`) REFERENCES `stations` (`ID`),
  CONSTRAINT `machines_machinetypes` FOREIGN KEY (`MachineTypeId`) REFERENCES `machinetypes` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- =====================================================================
-- 2. TẠO 4 BẢNG MỚI THEO SRS
-- =====================================================================

-- Bảng log_file (quản lý file log)
CREATE TABLE `log_file` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `file_name` varchar(100) NOT NULL,
  `machine_id` int NOT NULL,
  `date_created` date NOT NULL,
  `file_size` bigint DEFAULT 0,
  `status` varchar(20) DEFAULT 'Processing',              -- Processing, Completed, Error
  `created_time` datetime DEFAULT CURRENT_TIMESTAMP,
  
  PRIMARY KEY (`ID`),
  KEY `idx_machine_date` (`machine_id`, `date_created`),
  KEY `idx_file_name` (`file_name`),
  CONSTRAINT `log_file_machine_fk` FOREIGN KEY (`machine_id`) REFERENCES `machines` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Bảng log_data (lưu chi tiết log data theo SRS format)
CREATE TABLE `log_data` (
  `log_id` int NOT NULL AUTO_INCREMENT,
  `file_id` int NOT NULL,
  `machine_id` int NOT NULL,
  `station_id` int DEFAULT NULL,
  `model_id` int DEFAULT NULL,                    -- Model sản phẩm đang xử lý
  
  -- Log content fields (theo SRS format)
  `EQPINFO` varchar(50) DEFAULT NULL,
  `PROCINFO` varchar(10) DEFAULT NULL,
  `PID` varchar(22) NOT NULL,
  `FID` varchar(22) DEFAULT NULL,
  `part_no` varchar(11) DEFAULT NULL,
  `variant` varchar(20) DEFAULT NULL,
  `result` varchar(15) NOT NULL,
  `jobfile` varchar(100) NOT NULL,
  `gmes_status` varchar(10) NOT NULL,
  `start_time` datetime NOT NULL,
  `end_time` datetime DEFAULT NULL,
  `stepNG` varchar(255) DEFAULT NULL,
  `measure` varchar(255) DEFAULT NULL,
  `spec_min` varchar(255) DEFAULT NULL,
  `spec_max` varchar(255) DEFAULT NULL,
  
  -- System fields
  `log_level` varchar(20) DEFAULT 'Info',                -- Info, Warning, Error, Critical
  `source` varchar(100) DEFAULT NULL,                    -- Program source (GMES_Name, ProgramName)
  `raw_data` text,                                       -- Raw log content
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

-- Bảng commands (điều khiển máy từ xa theo SRS section 2.3)
CREATE TABLE `commands` (
  `command_id` int NOT NULL AUTO_INCREMENT,
  `machine_id` int DEFAULT NULL,                          -- NULL = broadcast to all
  `station_id` int DEFAULT NULL,                          -- Optional: specific station
  `command_type` varchar(50) NOT NULL,                    -- StartProgram, StopProgram, RestartService
  `program_name` varchar(100) DEFAULT NULL,               -- Target program (from machines.ProgramName)
  `parameters` text,                                      -- JSON parameters
  `status` varchar(20) DEFAULT 'Pending',                 -- Pending, Sent, Executing, Done, Failed
  `priority` int DEFAULT 5,                               -- 1=High, 5=Normal, 10=Low
  `created_time` datetime DEFAULT CURRENT_TIMESTAMP,
  `sent_time` datetime DEFAULT NULL,
  `executed_time` datetime DEFAULT NULL,
  `result_message` text,
  
  PRIMARY KEY (`command_id`),
  KEY `idx_machine_status` (`machine_id`, `status`),
  KEY `idx_pending` (`status`, `created_time`),
  CONSTRAINT `commands_machine_fk` FOREIGN KEY (`machine_id`) REFERENCES `machines` (`ID`),
  CONSTRAINT `commands_station_fk` FOREIGN KEY (`station_id`) REFERENCES `stations` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Bảng client_config (cấu hình cho client apps theo SRS section 2.4)
CREATE TABLE `client_config` (
  `config_id` int NOT NULL AUTO_INCREMENT,
  `machine_id` int DEFAULT NULL,                          -- NULL = global config
  `config_key` varchar(100) NOT NULL,
  `config_value` text,
  `description` varchar(500) DEFAULT NULL,
  `updated_time` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  
  PRIMARY KEY (`config_id`),
  UNIQUE KEY `unique_machine_key` (`machine_id`, `config_key`),
  KEY `idx_config_key` (`config_key`),
  CONSTRAINT `client_config_machine_fk` FOREIGN KEY (`machine_id`) REFERENCES `machines` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- =====================================================================
-- 3. SAO CHÉP DỮ LIỆU TỪ HSE_PM_DATABASE
-- =====================================================================

-- Copy data từ hse_pm_database sang hse_pm_db
INSERT INTO buyers SELECT * FROM hse_pm_database.buyers;
INSERT INTO `lines` SELECT * FROM hse_pm_database.`lines`;
INSERT INTO machinetypes SELECT * FROM hse_pm_database.machinetypes;
INSERT INTO modelgroups SELECT * FROM hse_pm_database.modelgroups;
INSERT INTO modelprocesses SELECT * FROM hse_pm_database.modelprocesses;
INSERT INTO models SELECT * FROM hse_pm_database.models;
INSERT INTO stations SELECT * FROM hse_pm_database.stations;

-- Copy machines data với các cột cũ, cột mới sẽ có default values
INSERT INTO machines (ID, Name, status, MachineTypeId, IP, GMES_Name, StationID, ProgramName, mac_address, client_status)
SELECT ID, Name, status, MachineTypeId, IP, GMES_Name, StationID, ProgramName, mac_address, 'Offline'
FROM hse_pm_database.machines;

-- =====================================================================
-- 4. DỮ LIỆU MẪU CHO CÁC BẢNG MỚI
-- =====================================================================

-- Sample data cho global configuration
INSERT INTO client_config (machine_id, config_key, config_value, description) VALUES
(NULL, 'log_retention_days', '90', 'Số ngày giữ log data'),
(NULL, 'command_timeout_minutes', '60', 'Timeout cho commands'),
(NULL, 'client_polling_interval', '30', 'Client polling interval (seconds)'),
(NULL, 'log_batch_size', '100', 'Số log entries per batch'),
(NULL, 'max_file_size_mb', '50', 'Maximum log file size in MB');

-- Sample log files (nếu có máy trong database)
INSERT INTO log_file (file_name, machine_id, date_created, file_size, status)
SELECT 
    CONCAT(GMES_Name, '_', DATE_FORMAT(CURDATE(), '%Y%m%d'), '.log'),
    ID,
    CURDATE(),
    1024000,
    'Completed'
FROM machines 
WHERE client_status = 'Online' OR client_status IS NULL
LIMIT 5;

-- Sample commands
INSERT INTO commands (machine_id, command_type, program_name, status, priority)
SELECT 
    ID,
    'StartProgram',
    ProgramName,
    'Pending',
    5
FROM machines 
WHERE ProgramName IS NOT NULL
LIMIT 3;

-- Cập nhật timestamps cho machines
UPDATE machines 
SET last_seen = NOW(),
    last_log_time = NOW(),
    app_version = 'v1.0.0',
    client_status = 'Online'
WHERE IP IS NOT NULL;

-- =====================================================================
-- 5. TẠO VIEWS HỖ TRỢ
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
LEFT JOIN stations s ON m.StationID = s.ID
LEFT JOIN `lines` l ON s.LineId = l.ID;

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
LEFT JOIN stations s ON ld.station_id = s.ID
LEFT JOIN `lines` l ON s.LineId = l.ID
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
-- HOÀN THÀNH
-- =====================================================================

SELECT 'Database hse_pm_db đã được cập nhật với cấu trúc từ hse_pm_database + 4 bảng mới!' as message;
SELECT 'Machines count:' as info, COUNT(*) as count FROM machines;
SELECT 'Log data count:' as info, COUNT(*) as count FROM log_data;  
SELECT 'Commands count:' as info, COUNT(*) as count FROM commands;
SELECT 'Config count:' as info, COUNT(*) as count FROM client_config;