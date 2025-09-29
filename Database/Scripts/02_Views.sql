-- Database Views for Machine Management System
-- These views provide convenient access to commonly used data combinations

USE machine_management_db;

-- =============================================
-- v_machine_status - Current status of all machines
-- =============================================
DROP VIEW IF EXISTS v_machine_status;
CREATE VIEW v_machine_status AS
SELECT 
    m.MachineId,
    m.MachineName,
    m.MachineCode,
    m.MachineType,
    m.Description as MachineDescription,
    s.StationId,
    s.StationName,
    s.StationCode,
    l.LineId,
    l.LineName,
    l.LineCode,
    mp.ProcessName,
    mod.ModelName,
    mg.GroupName,
    b.BuyerName,
    b.BuyerCode,
    -- Log statistics (last 24 hours)
    log_stats.TotalLogs24h,
    log_stats.ErrorLogs24h,
    log_stats.WarningLogs24h,
    log_stats.LastLogTime,
    -- Command statistics (last 24 hours)
    cmd_stats.TotalCommands24h,
    cmd_stats.PendingCommands,
    cmd_stats.FailedCommands,
    cmd_stats.LastCommandTime,
    -- Machine status
    CASE 
        WHEN log_stats.LastLogTime > DATE_SUB(NOW(), INTERVAL 1 HOUR) THEN 'Online'
        WHEN log_stats.LastLogTime > DATE_SUB(NOW(), INTERVAL 24 HOUR) THEN 'Inactive'
        ELSE 'Offline'
    END as ConnectionStatus,
    CASE 
        WHEN log_stats.ErrorLogs24h > 10 THEN 'Critical'
        WHEN log_stats.ErrorLogs24h > 5 THEN 'Warning'
        WHEN log_stats.WarningLogs24h > 20 THEN 'Warning'
        ELSE 'Normal'
    END as HealthStatus,
    m.IsActive,
    m.CreatedAt,
    m.UpdatedAt
FROM MACHINES m
INNER JOIN STATIONS s ON m.StationId = s.StationId
INNER JOIN LINES l ON s.LineId = l.LineId
INNER JOIN MODELPROCESSES mp ON l.ModelProcessId = mp.ModelProcessId
INNER JOIN MODELS mod ON mp.ModelId = mod.ModelId
INNER JOIN MODELGROUPS mg ON mod.ModelGroupId = mg.ModelGroupId
INNER JOIN BUYERS b ON mg.BuyerId = b.BuyerId
LEFT JOIN (
    SELECT 
        MachineId,
        COUNT(*) as TotalLogs24h,
        SUM(CASE WHEN LogLevel = 'Error' THEN 1 ELSE 0 END) as ErrorLogs24h,
        SUM(CASE WHEN LogLevel = 'Warning' THEN 1 ELSE 0 END) as WarningLogs24h,
        MAX(LogTimestamp) as LastLogTime
    FROM LOGDATA 
    WHERE LogTimestamp > DATE_SUB(NOW(), INTERVAL 24 HOUR)
    GROUP BY MachineId
) log_stats ON m.MachineId = log_stats.MachineId
LEFT JOIN (
    SELECT 
        MachineId,
        COUNT(*) as TotalCommands24h,
        SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) as PendingCommands,
        SUM(CASE WHEN Status = 'Failed' THEN 1 ELSE 0 END) as FailedCommands,
        MAX(CreatedAt) as LastCommandTime
    FROM COMMANDS 
    WHERE CreatedAt > DATE_SUB(NOW(), INTERVAL 24 HOUR)
    GROUP BY MachineId
) cmd_stats ON m.MachineId = cmd_stats.MachineId;

-- =============================================
-- v_log_summary - Daily log statistics
-- =============================================
DROP VIEW IF EXISTS v_log_summary;
CREATE VIEW v_log_summary AS
SELECT 
    DATE(ld.LogTimestamp) as LogDate,
    b.BuyerName,
    l.LineName,
    s.StationName,
    m.MachineName,
    m.MachineCode,
    ld.LogLevel,
    COUNT(*) as LogCount,
    COUNT(DISTINCT ld.MachineId) as UniqueMachines,
    MIN(ld.LogTimestamp) as FirstLogTime,
    MAX(ld.LogTimestamp) as LastLogTime
FROM LOGDATA ld
INNER JOIN MACHINES m ON ld.MachineId = m.MachineId
INNER JOIN STATIONS s ON m.StationId = s.StationId
INNER JOIN LINES l ON s.LineId = l.LineId
INNER JOIN MODELPROCESSES mp ON l.ModelProcessId = mp.ModelProcessId
INNER JOIN MODELS mod ON mp.ModelId = mod.ModelId
INNER JOIN MODELGROUPS mg ON mod.ModelGroupId = mg.ModelGroupId
INNER JOIN BUYERS b ON mg.BuyerId = b.BuyerId
WHERE ld.LogTimestamp >= DATE_SUB(CURDATE(), INTERVAL 30 DAY)
GROUP BY 
    DATE(ld.LogTimestamp),
    b.BuyerName,
    l.LineName,
    s.StationName,
    m.MachineName,
    m.MachineCode,
    ld.LogLevel
ORDER BY LogDate DESC, LogCount DESC;

-- =============================================
-- v_production_hierarchy - Complete hierarchy view
-- =============================================
DROP VIEW IF EXISTS v_production_hierarchy;
CREATE VIEW v_production_hierarchy AS
SELECT 
    -- Buyer level
    b.BuyerId,
    b.BuyerName,
    b.BuyerCode,
    b.Description as BuyerDescription,
    
    -- Model Group level
    mg.ModelGroupId,
    mg.GroupName,
    mg.GroupCode,
    mg.Description as GroupDescription,
    
    -- Model level
    m.ModelId,
    m.ModelName,
    m.ModelCode,
    m.Description as ModelDescription,
    
    -- Process level
    mp.ModelProcessId,
    mp.ProcessName,
    mp.ProcessCode,
    mp.Description as ProcessDescription,
    
    -- Line level
    l.LineId,
    l.LineName,
    l.LineCode,
    l.Description as LineDescription,
    
    -- Station level
    s.StationId,
    s.StationName,
    s.StationCode,
    s.Description as StationDescription,
    
    -- Machine level
    mc.MachineId,
    mc.MachineName,
    mc.MachineCode,
    mc.MachineType,
    mc.Description as MachineDescription,
    
    -- Hierarchy path for easy navigation
    CONCAT(b.BuyerCode, ' > ', mg.GroupCode, ' > ', m.ModelCode, ' > ', 
           mp.ProcessCode, ' > ', l.LineCode, ' > ', s.StationCode) as HierarchyPath,
    
    -- Machine count per level
    COUNT(mc.MachineId) OVER (PARTITION BY b.BuyerId) as MachinesPerBuyer,
    COUNT(mc.MachineId) OVER (PARTITION BY mg.ModelGroupId) as MachinesPerGroup,
    COUNT(mc.MachineId) OVER (PARTITION BY m.ModelId) as MachinesPerModel,
    COUNT(mc.MachineId) OVER (PARTITION BY mp.ModelProcessId) as MachinesPerProcess,
    COUNT(mc.MachineId) OVER (PARTITION BY l.LineId) as MachinesPerLine,
    COUNT(mc.MachineId) OVER (PARTITION BY s.StationId) as MachinesPerStation,
    
    -- Status flags
    b.IsActive as BuyerActive,
    mg.IsActive as GroupActive,
    m.IsActive as ModelActive,
    mp.IsActive as ProcessActive,
    l.IsActive as LineActive,
    s.IsActive as StationActive,
    mc.IsActive as MachineActive,
    
    -- Timestamps
    mc.CreatedAt as MachineCreatedAt,
    mc.UpdatedAt as MachineUpdatedAt
FROM BUYERS b
LEFT JOIN MODELGROUPS mg ON b.BuyerId = mg.BuyerId
LEFT JOIN MODELS m ON mg.ModelGroupId = m.ModelGroupId  
LEFT JOIN MODELPROCESSES mp ON m.ModelId = mp.ModelId
LEFT JOIN LINES l ON mp.ModelProcessId = l.ModelProcessId
LEFT JOIN STATIONS s ON l.LineId = s.LineId
LEFT JOIN MACHINES mc ON s.StationId = mc.StationId
WHERE b.IsActive = 1;

-- =============================================
-- v_command_queue - Command execution queue view
-- =============================================
DROP VIEW IF EXISTS v_command_queue;
CREATE VIEW v_command_queue AS
SELECT 
    c.CommandId,
    c.CommandType,
    c.CommandData,
    c.Status,
    c.Priority,
    c.CreatedAt,
    c.ExecutedAt,
    c.ScheduledAt,
    c.Response,
    c.ErrorMessage,
    
    -- Machine information
    m.MachineId,
    m.MachineName,
    m.MachineCode,
    s.StationName,
    l.LineName,
    b.BuyerName,
    
    -- Execution time calculation
    CASE 
        WHEN c.ExecutedAt IS NOT NULL THEN 
            TIMESTAMPDIFF(SECOND, c.CreatedAt, c.ExecutedAt)
        ELSE NULL 
    END as ExecutionTimeSeconds,
    
    -- Queue position (for pending commands)
    CASE 
        WHEN c.Status = 'Pending' THEN
            ROW_NUMBER() OVER (
                PARTITION BY c.MachineId, c.Status 
                ORDER BY c.Priority ASC, c.CreatedAt ASC
            )
        ELSE NULL 
    END as QueuePosition,
    
    -- Age of command
    TIMESTAMPDIFF(MINUTE, c.CreatedAt, NOW()) as AgeMinutes
    
FROM COMMANDS c
INNER JOIN MACHINES m ON c.MachineId = m.MachineId
INNER JOIN STATIONS s ON m.StationId = s.StationId
INNER JOIN LINES l ON s.LineId = l.LineId
INNER JOIN MODELPROCESSES mp ON l.ModelProcessId = mp.ModelProcessId
INNER JOIN MODELS mod ON mp.ModelId = mod.ModelId
INNER JOIN MODELGROUPS mg ON mod.ModelGroupId = mg.ModelGroupId
INNER JOIN BUYERS b ON mg.BuyerId = b.BuyerId
ORDER BY 
    c.Status = 'Pending' DESC,
    c.Priority ASC,
    c.CreatedAt ASC;

-- =============================================
-- v_error_analysis - Error pattern analysis
-- =============================================
DROP VIEW IF EXISTS v_error_analysis;
CREATE VIEW v_error_analysis AS
SELECT 
    DATE(ld.LogTimestamp) as ErrorDate,
    HOUR(ld.LogTimestamp) as ErrorHour,
    b.BuyerName,
    l.LineName,
    s.StationName,
    m.MachineName,
    m.MachineCode,
    ld.LogLevel,
    ld.Category,
    ld.Source,
    
    -- Error count and patterns
    COUNT(*) as ErrorCount,
    COUNT(DISTINCT m.MachineId) as AffectedMachines,
    
    -- Most common error message
    SUBSTRING(
        GROUP_CONCAT(ld.Message ORDER BY ld.LogTimestamp DESC SEPARATOR ' | '), 
        1, 500
    ) as RecentMessages,
    
    MIN(ld.LogTimestamp) as FirstOccurrence,
    MAX(ld.LogTimestamp) as LastOccurrence,
    
    -- Error frequency
    COUNT(*) / 
    GREATEST(TIMESTAMPDIFF(HOUR, MIN(ld.LogTimestamp), MAX(ld.LogTimestamp)), 1) 
    as ErrorsPerHour

FROM LOGDATA ld
INNER JOIN MACHINES m ON ld.MachineId = m.MachineId
INNER JOIN STATIONS s ON m.StationId = s.StationId
INNER JOIN LINES l ON s.LineId = l.LineId
INNER JOIN MODELPROCESSES mp ON l.ModelProcessId = mp.ModelProcessId
INNER JOIN MODELS mod ON mp.ModelId = mod.ModelId
INNER JOIN MODELGROUPS mg ON mod.ModelGroupId = mg.ModelGroupId
INNER JOIN BUYERS b ON mg.BuyerId = b.BuyerId
WHERE ld.LogLevel IN ('Error', 'Critical', 'Warning')
  AND ld.LogTimestamp >= DATE_SUB(NOW(), INTERVAL 7 DAY)
GROUP BY 
    DATE(ld.LogTimestamp),
    HOUR(ld.LogTimestamp),
    b.BuyerName,
    l.LineName,
    s.StationName,
    m.MachineName,
    m.MachineCode,
    ld.LogLevel,
    ld.Category,
    ld.Source
HAVING ErrorCount > 1
ORDER BY ErrorDate DESC, ErrorHour DESC, ErrorCount DESC;

SELECT 'Database views created successfully!' as Status;