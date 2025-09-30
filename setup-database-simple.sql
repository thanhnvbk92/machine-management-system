-- ===============================================
-- SIMPLE DATABASE SETUP SCRIPT - HSE_PM_DB
-- Machine Management System - Basic Version
-- ===============================================

-- Tạo database HSE_PM_DB
CREATE DATABASE IF NOT EXISTS HSE_PM_DB 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE HSE_PM_DB;

-- ===============================================
-- 1. TẠO CÁC BẢNG CƠ BẢN (đơn giản)
-- ===============================================

-- Bảng Machines (từ hệ thống hiện có, được mở rộng)
CREATE TABLE IF NOT EXISTS `machines` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `status` varchar(20) DEFAULT NULL,
  `MachineTypeId` int DEFAULT NULL,
  `IP` varchar(15) DEFAULT NULL,
  `GMES_Name` varchar(50) DEFAULT NULL,
  `StationID` int DEFAULT NULL,
  `ProgramName` varchar(45) DEFAULT NULL,
  `mac_address` varchar(50) DEFAULT NULL,
  -- Cột mới cho log collection system
  `last_log_time` DATETIME DEFAULT NULL,
  `app_version` VARCHAR(50) DEFAULT NULL,
  `client_status` VARCHAR(20) DEFAULT 'Offline',
  `last_seen` DATETIME DEFAULT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`ID`),
  INDEX `idx_ip` (`IP`),
  INDEX `idx_station` (`StationID`),
  INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Bảng Stations (từ hệ thống hiện có)
CREATE TABLE IF NOT EXISTS `stations` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `StationName` varchar(100) NOT NULL,
  `LineID` int DEFAULT NULL,
  `StationType` varchar(50) DEFAULT NULL,
  `Sequence` int DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT 1,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`ID`),
  INDEX `idx_line` (`LineID`),
  INDEX `idx_active` (`IsActive`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===============================================
-- 2. TẠO CÁC BẢNG MỚI CHO LOG COLLECTION
-- ===============================================

-- Bảng Log Data
CREATE TABLE IF NOT EXISTS `log_data` (
    `log_id` INT AUTO_INCREMENT PRIMARY KEY,
    `machine_id` INT NOT NULL,
    `station_id` INT DEFAULT NULL,
    `log_time` DATETIME NOT NULL,
    `log_level` VARCHAR(20) NOT NULL,
    `message` TEXT NOT NULL,
    `source` VARCHAR(100) DEFAULT NULL,
    `raw_data` TEXT DEFAULT NULL,
    `received_time` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `processed` TINYINT(1) DEFAULT 0,
    `file_path` VARCHAR(500) DEFAULT NULL,
    INDEX `idx_machine_time` (`machine_id`, `log_time`),
    INDEX `idx_station_time` (`station_id`, `log_time`),
    INDEX `idx_log_level` (`log_level`),
    INDEX `idx_received_time` (`received_time`),
    INDEX `idx_processed` (`processed`),
    FOREIGN KEY (`machine_id`) REFERENCES `machines`(`ID`) ON DELETE CASCADE,
    FOREIGN KEY (`station_id`) REFERENCES `stations`(`ID`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Bảng Commands (cho remote control)
CREATE TABLE IF NOT EXISTS `commands` (
    `command_id` INT AUTO_INCREMENT PRIMARY KEY,
    `machine_id` INT DEFAULT NULL,
    `station_id` INT DEFAULT NULL,
    `command_type` VARCHAR(50) NOT NULL,
    `program_name` VARCHAR(100) DEFAULT NULL,
    `parameters` TEXT DEFAULT NULL,
    `status` VARCHAR(20) DEFAULT 'Pending',
    `priority` INT DEFAULT 5,
    `created_time` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `sent_time` DATETIME DEFAULT NULL,
    `executed_time` DATETIME DEFAULT NULL,
    `result_message` TEXT DEFAULT NULL,
    `timeout_seconds` INT DEFAULT 30,
    `retry_count` INT DEFAULT 0,
    `max_retries` INT DEFAULT 3,
    INDEX `idx_machine_status` (`machine_id`, `status`),
    INDEX `idx_station_status` (`station_id`, `status`),
    INDEX `idx_created_time` (`created_time`),
    INDEX `idx_priority` (`priority`),
    FOREIGN KEY (`machine_id`) REFERENCES `machines`(`ID`) ON DELETE CASCADE,
    FOREIGN KEY (`station_id`) REFERENCES `stations`(`ID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Bảng Client Config
CREATE TABLE IF NOT EXISTS `client_config` (
    `config_id` INT AUTO_INCREMENT PRIMARY KEY,
    `machine_id` INT DEFAULT NULL,
    `config_key` VARCHAR(100) NOT NULL,
    `config_value` TEXT DEFAULT NULL,
    `description` VARCHAR(500) DEFAULT NULL,
    `updated_time` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `is_active` TINYINT(1) DEFAULT 1,
    INDEX `idx_machine_key` (`machine_id`, `config_key`),
    INDEX `idx_config_key` (`config_key`),
    INDEX `idx_active` (`is_active`),
    UNIQUE KEY `unique_machine_key` (`machine_id`, `config_key`),
    FOREIGN KEY (`machine_id`) REFERENCES `machines`(`ID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===============================================
-- 3. INSERT SAMPLE DATA
-- ===============================================

-- Insert sample stations
INSERT IGNORE INTO `stations` (`ID`, `StationName`, `LineID`, `StationType`, `Sequence`, `IsActive`) VALUES
(1, 'Station_01', 1, 'Assembly', 1, 1),
(2, 'Station_02', 1, 'Testing', 2, 1),
(3, 'Station_03', 1, 'Packaging', 3, 1);

-- Insert sample machines
INSERT IGNORE INTO `machines` (`ID`, `Name`, `status`, `IP`, `StationID`, `client_status`) VALUES
(1, 'Machine_A01', 'Running', '192.168.1.101', 1, 'Online'),
(2, 'Machine_A02', 'Running', '192.168.1.102', 2, 'Online'),
(3, 'Machine_B01', 'Stopped', '192.168.1.201', 3, 'Offline');

-- Insert sample configuration
INSERT IGNORE INTO `client_config` (`machine_id`, `config_key`, `config_value`, `description`) VALUES
(1, 'log_upload_interval', '300', 'Log upload interval in seconds'),
(1, 'backup_enabled', 'true', 'Enable automatic backup'),
(1, 'ftp_server', '192.168.1.100', 'FTP server for backup'),
(2, 'log_upload_interval', '300', 'Log upload interval in seconds'),
(2, 'backup_enabled', 'true', 'Enable automatic backup'),
(3, 'log_upload_interval', '600', 'Log upload interval in seconds');

-- ===============================================
-- 4. CREATE SIMPLE STORED PROCEDURES
-- ===============================================

DELIMITER $$

-- Procedure: Insert Log Entry
CREATE PROCEDURE `sp_insert_log`(
    IN p_machine_id INT,
    IN p_station_id INT,
    IN p_log_level VARCHAR(20),
    IN p_message TEXT,
    IN p_source VARCHAR(100),
    IN p_raw_data TEXT,
    IN p_log_time DATETIME
)
BEGIN
    INSERT INTO log_data (machine_id, station_id, log_level, message, source, raw_data, log_time)
    VALUES (p_machine_id, p_station_id, p_log_level, p_message, p_source, p_raw_data, p_log_time);
    
    -- Update machine last_log_time
    UPDATE machines 
    SET last_log_time = p_log_time, last_seen = NOW()
    WHERE ID = p_machine_id;
END$$

-- Procedure: Update Machine Status
CREATE PROCEDURE `sp_update_machine_status`(
    IN p_machine_id INT,
    IN p_status VARCHAR(20),
    IN p_client_status VARCHAR(20),
    IN p_app_version VARCHAR(50)
)
BEGIN
    UPDATE machines 
    SET status = COALESCE(p_status, status),
        client_status = COALESCE(p_client_status, client_status),
        app_version = COALESCE(p_app_version, app_version),
        last_seen = NOW()
    WHERE ID = p_machine_id;
END$$

DELIMITER ;

-- ===============================================
-- SETUP COMPLETE
-- ===============================================

SELECT 'HSE_PM_DB Database setup completed successfully!' as Status;
SELECT COUNT(*) as TotalTables FROM information_schema.tables WHERE table_schema = 'HSE_PM_DB';
SELECT COUNT(*) as TotalProcedures FROM information_schema.routines WHERE routine_schema = 'HSE_PM_DB';