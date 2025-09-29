-- Stored Procedures for Machine Management System
-- These procedures handle complex operations efficiently

USE machine_management_db;

DELIMITER //

-- =============================================
-- Get Machine Hierarchy with full production chain
-- =============================================
DROP PROCEDURE IF EXISTS GetMachineHierarchy//
CREATE PROCEDURE GetMachineHierarchy(
    IN p_machine_id INT DEFAULT NULL,
    IN p_buyer_code VARCHAR(100) DEFAULT NULL
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    SELECT 
        b.BuyerId,
        b.BuyerName,
        b.BuyerCode,
        mg.ModelGroupId,
        mg.GroupName,
        mg.GroupCode,
        m.ModelId,
        m.ModelName,
        m.ModelCode,
        mp.ModelProcessId,
        mp.ProcessName,
        mp.ProcessCode,
        l.LineId,
        l.LineName,
        l.LineCode,
        s.StationId,
        s.StationName,
        s.StationCode,
        mc.MachineId,
        mc.MachineName,
        mc.MachineCode,
        mc.MachineType,
        mc.Description,
        mc.CreatedAt,
        mc.UpdatedAt,
        mc.IsActive
    FROM BUYERS b
    LEFT JOIN MODELGROUPS mg ON b.BuyerId = mg.BuyerId
    LEFT JOIN MODELS m ON mg.ModelGroupId = m.ModelGroupId
    LEFT JOIN MODELPROCESSES mp ON m.ModelId = mp.ModelId
    LEFT JOIN LINES l ON mp.ModelProcessId = l.ModelProcessId
    LEFT JOIN STATIONS s ON l.LineId = s.LineId
    LEFT JOIN MACHINES mc ON s.StationId = mc.StationId
    WHERE 
        (p_machine_id IS NULL OR mc.MachineId = p_machine_id) AND
        (p_buyer_code IS NULL OR b.BuyerCode = p_buyer_code) AND
        b.IsActive = 1
    ORDER BY 
        b.BuyerName, mg.GroupName, m.ModelName, 
        mp.ProcessName, l.LineName, s.StationName, mc.MachineName;
END//

-- =============================================
-- Get Logs by Date Range with optional filtering
-- =============================================
DROP PROCEDURE IF EXISTS GetLogsByDateRange//
CREATE PROCEDURE GetLogsByDateRange(
    IN p_start_date DATETIME,
    IN p_end_date DATETIME,
    IN p_machine_id INT DEFAULT NULL,
    IN p_log_level VARCHAR(100) DEFAULT NULL,
    IN p_limit INT DEFAULT 1000,
    IN p_offset INT DEFAULT 0
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    -- Validate date range
    IF p_start_date IS NULL OR p_end_date IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Start date and end date are required';
    END IF;

    IF p_start_date > p_end_date THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Start date cannot be after end date';
    END IF;

    SELECT 
        ld.LogId,
        ld.MachineId,
        m.MachineName,
        m.MachineCode,
        s.StationName,
        l.LineName,
        ld.LogType,
        ld.LogLevel,
        ld.Message,
        ld.Details,
        ld.LogTimestamp,
        ld.Source,
        ld.Category,
        ld.CreatedAt
    FROM LOGDATA ld
    INNER JOIN MACHINES m ON ld.MachineId = m.MachineId
    INNER JOIN STATIONS s ON m.StationId = s.StationId
    INNER JOIN LINES l ON s.LineId = l.LineId
    WHERE 
        ld.LogTimestamp >= p_start_date 
        AND ld.LogTimestamp <= p_end_date
        AND (p_machine_id IS NULL OR ld.MachineId = p_machine_id)
        AND (p_log_level IS NULL OR ld.LogLevel = p_log_level)
    ORDER BY ld.LogTimestamp DESC
    LIMIT p_limit OFFSET p_offset;

    -- Return total count for pagination
    SELECT COUNT(*) as TotalCount
    FROM LOGDATA ld
    WHERE 
        ld.LogTimestamp >= p_start_date 
        AND ld.LogTimestamp <= p_end_date
        AND (p_machine_id IS NULL OR ld.MachineId = p_machine_id)
        AND (p_log_level IS NULL OR ld.LogLevel = p_log_level);
END//

-- =============================================
-- Cleanup Old Logs (maintenance procedure)
-- =============================================
DROP PROCEDURE IF EXISTS CleanupOldLogs//
CREATE PROCEDURE CleanupOldLogs(
    IN p_retention_days INT DEFAULT 90,
    IN p_batch_size INT DEFAULT 1000,
    IN p_dry_run BOOLEAN DEFAULT FALSE
)
BEGIN
    DECLARE v_deleted_count INT DEFAULT 0;
    DECLARE v_total_deleted INT DEFAULT 0;
    DECLARE v_cutoff_date DATETIME;
    DECLARE v_done INT DEFAULT FALSE;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    -- Calculate cutoff date
    SET v_cutoff_date = DATE_SUB(NOW(), INTERVAL p_retention_days DAY);

    -- Start transaction
    START TRANSACTION;

    -- If dry run, just return count
    IF p_dry_run THEN
        SELECT 
            COUNT(*) as LogsToDelete,
            v_cutoff_date as CutoffDate,
            'DRY RUN - No logs will be deleted' as Status;
        ROLLBACK;
    ELSE
        -- Delete in batches to avoid long locks
        delete_loop: LOOP
            DELETE FROM LOGDATA 
            WHERE LogTimestamp < v_cutoff_date 
            ORDER BY LogTimestamp ASC 
            LIMIT p_batch_size;
            
            SET v_deleted_count = ROW_COUNT();
            SET v_total_deleted = v_total_deleted + v_deleted_count;
            
            IF v_deleted_count = 0 THEN
                LEAVE delete_loop;
            END IF;
            
            -- Small delay to avoid overloading the system
            DO SLEEP(0.1);
        END LOOP;

        SELECT 
            v_total_deleted as LogsDeleted,
            v_cutoff_date as CutoffDate,
            'Cleanup completed successfully' as Status;
            
        COMMIT;
    END IF;
END//

-- =============================================
-- Get Machine Statistics
-- =============================================
DROP PROCEDURE IF EXISTS GetMachineStatistics//
CREATE PROCEDURE GetMachineStatistics(
    IN p_start_date DATETIME DEFAULT NULL,
    IN p_end_date DATETIME DEFAULT NULL
)
BEGIN
    DECLARE v_start_date DATETIME;
    DECLARE v_end_date DATETIME;
    
    -- Default to last 7 days if no date range provided
    SET v_start_date = COALESCE(p_start_date, DATE_SUB(NOW(), INTERVAL 7 DAY));
    SET v_end_date = COALESCE(p_end_date, NOW());

    SELECT 
        m.MachineId,
        m.MachineName,
        m.MachineCode,
        s.StationName,
        l.LineName,
        COUNT(ld.LogId) as TotalLogs,
        COUNT(CASE WHEN ld.LogLevel = 'Error' THEN 1 END) as ErrorCount,
        COUNT(CASE WHEN ld.LogLevel = 'Warning' THEN 1 END) as WarningCount,
        COUNT(CASE WHEN ld.LogLevel = 'Information' THEN 1 END) as InfoCount,
        MAX(ld.LogTimestamp) as LastLogTime,
        COUNT(c.CommandId) as TotalCommands,
        COUNT(CASE WHEN c.Status = 'Completed' THEN 1 END) as CompletedCommands,
        COUNT(CASE WHEN c.Status = 'Failed' THEN 1 END) as FailedCommands,
        COUNT(CASE WHEN c.Status = 'Pending' THEN 1 END) as PendingCommands
    FROM MACHINES m
    LEFT JOIN STATIONS s ON m.StationId = s.StationId
    LEFT JOIN LINES l ON s.LineId = l.LineId
    LEFT JOIN LOGDATA ld ON m.MachineId = ld.MachineId 
        AND ld.LogTimestamp BETWEEN v_start_date AND v_end_date
    LEFT JOIN COMMANDS c ON m.MachineId = c.MachineId 
        AND c.CreatedAt BETWEEN v_start_date AND v_end_date
    WHERE m.IsActive = 1
    GROUP BY m.MachineId, m.MachineName, m.MachineCode, s.StationName, l.LineName
    ORDER BY m.MachineName;
END//

DELIMITER ;

-- =============================================
-- Create indexes for better performance
-- =============================================

-- Index for log queries by timestamp
CREATE INDEX IF NOT EXISTS idx_logdata_timestamp_machine 
ON LOGDATA (LogTimestamp DESC, MachineId);

-- Index for log level queries
CREATE INDEX IF NOT EXISTS idx_logdata_level_timestamp 
ON LOGDATA (LogLevel, LogTimestamp DESC);

-- Index for command status queries
CREATE INDEX IF NOT EXISTS idx_commands_status_created 
ON COMMANDS (Status, CreatedAt DESC);

-- Index for machine hierarchy queries
CREATE INDEX IF NOT EXISTS idx_machine_station 
ON MACHINES (StationId, IsActive);

PRINT 'Stored procedures created successfully!';