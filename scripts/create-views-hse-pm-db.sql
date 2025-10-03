-- =====================================================================
-- SCRIPT TẠO CÁC DATABASE VIEWS CHO HSE_PM_DB
-- Machine Management System - Database Views Creation Script
-- Updated for HSE_PM_DB with complete structure
-- =====================================================================

USE HSE_PM_DB;

-- =====================================================================
-- 1. VIEW CHO COMPLETE MACHINE DETAILS (bao gồm tất cả thông tin liên quan)
-- =====================================================================
DROP VIEW IF EXISTS v_machine_complete_details;
CREATE VIEW v_machine_complete_details AS
SELECT 
    m.ID as machine_id,
    m.Name as machine_name,
    m.status as machine_status,
    m.IP as machine_ip,
    m.GMES_Name as gmes_name,
    m.ProgramName as program_name,
    m.mac_address,
    m.last_log_time,
    m.app_version,
    m.client_status,
    m.last_seen,
    
    -- Machine Type Info
    mt.Id as machine_type_id,
    mt.Name as machine_type_name,
    
    -- Station Info
    s.ID as station_id,
    s.Name as station_name,
    
    -- Line Info
    l.ID as line_id,
    l.Name as line_name,
    
    -- Model Process Info (through stations.ModelProcessId)
    mp.ID as model_process_id,
    mp.Name as model_process_name,
    
    -- Model Group Info
    mg.ID as model_group_id,
    mg.Name as model_group_name,
    
    -- Buyer Info
    b.ID as buyer_id,
    b.Code as buyer_code,
    b.Name as buyer_name,
    
    -- Recent command count
    (SELECT COUNT(*) FROM commands WHERE machine_id = m.ID AND status = 'Pending') as pending_commands,
    (SELECT COUNT(*) FROM log_data WHERE machine_id = m.ID AND DATE(received_time) = CURDATE()) as today_logs

FROM machines m
    LEFT JOIN machinetypes mt ON m.MachineTypeId = mt.Id
    LEFT JOIN stations s ON m.StationID = s.ID
    LEFT JOIN `lines` l ON s.LineId = l.ID
    LEFT JOIN modelprocesses mp ON s.ModelProcessId = mp.ID
    LEFT JOIN modelgroups mg ON mp.ModelGroupID = mg.ID
    LEFT JOIN buyers b ON mg.BuyerId = b.ID;

-- =====================================================================
-- 2. VIEW CHO STATION MANAGEMENT (quản lý station với hierarchy)
-- =====================================================================
DROP VIEW IF EXISTS v_station_management;
CREATE VIEW v_station_management AS
SELECT 
    s.ID as station_id,
    s.Name as station_name,
    
    -- Line Info
    l.ID as line_id,
    l.Name as line_name,
    
    -- Model Process Info
    mp.ID as model_process_id,
    mp.Name as model_process_name,
    
    -- Model Group Info
    mg.ID as model_group_id,
    mg.Name as model_group_name,
    
    -- Buyer Info
    b.ID as buyer_id,
    b.Code as buyer_code,
    b.Name as buyer_name,
    
    -- Machine Statistics
    COUNT(DISTINCT m.ID) as total_machines,
    SUM(CASE WHEN m.client_status = 'Online' THEN 1 ELSE 0 END) as machines_online,
    SUM(CASE WHEN m.client_status = 'Offline' THEN 1 ELSE 0 END) as machines_offline,
    SUM(CASE WHEN m.client_status = 'Maintenance' THEN 1 ELSE 0 END) as machines_maintenance,
    
    -- Recent Activity
    MAX(m.last_seen) as last_machine_activity,
    COUNT(DISTINCT CASE WHEN DATE(ld.received_time) = CURDATE() THEN ld.log_id END) as today_log_count

FROM stations s
    INNER JOIN `lines` l ON s.LineId = l.ID
    INNER JOIN modelprocesses mp ON s.ModelProcessId = mp.ID
    INNER JOIN modelgroups mg ON mp.ModelGroupID = mg.ID
    INNER JOIN buyers b ON mg.BuyerId = b.ID
    LEFT JOIN machines m ON s.ID = m.StationID
    LEFT JOIN log_data ld ON m.ID = ld.machine_id

GROUP BY s.ID, s.Name, l.ID, l.Name, mp.ID, mp.Name, mg.ID, mg.Name, b.ID, b.Code, b.Name;

-- =====================================================================
-- 3. VIEW CHO BUYER DASHBOARD (dashboard cho buyer)
-- =====================================================================
DROP VIEW IF EXISTS v_buyer_dashboard;
CREATE VIEW v_buyer_dashboard AS
SELECT 
    b.ID as buyer_id,
    b.Code as buyer_code,
    b.Name as buyer_name,
    
    -- Hierarchical Counts
    COUNT(DISTINCT mg.ID) as model_groups,
    COUNT(DISTINCT m.ID) as total_models,
    COUNT(DISTINCT mp.ID) as model_processes,
    COUNT(DISTINCT l.ID) as production_lines,
    COUNT(DISTINCT s.ID) as stations,
    COUNT(DISTINCT machines.ID) as total_machines,
    
    -- Machine Status Distribution
    SUM(CASE WHEN machines.client_status = 'Online' THEN 1 ELSE 0 END) as machines_online,
    SUM(CASE WHEN machines.client_status = 'Offline' THEN 1 ELSE 0 END) as machines_offline,
    SUM(CASE WHEN machines.client_status = 'Maintenance' THEN 1 ELSE 0 END) as machines_maintenance,
    
    -- Production Activity
    COUNT(DISTINCT CASE WHEN DATE(ld.received_time) = CURDATE() THEN ld.log_id END) as today_logs,
    COUNT(DISTINCT CASE WHEN ld.result = 'OK' AND DATE(ld.received_time) = CURDATE() THEN ld.log_id END) as today_ok_parts,
    COUNT(DISTINCT CASE WHEN ld.result = 'NG' AND DATE(ld.received_time) = CURDATE() THEN ld.log_id END) as today_ng_parts,
    
    -- Command Management
    COUNT(DISTINCT CASE WHEN cmd.status = 'Pending' THEN cmd.command_id END) as pending_commands,
    MAX(machines.last_seen) as last_activity

FROM buyers b
    LEFT JOIN modelgroups mg ON b.ID = mg.BuyerId
    LEFT JOIN models m ON mg.ID = m.ModelGroupID
    LEFT JOIN modelprocesses mp ON mg.ID = mp.ModelGroupID
    LEFT JOIN stations s ON mp.ID = s.ModelProcessId
    LEFT JOIN `lines` l ON s.LineId = l.ID
    LEFT JOIN machines ON s.ID = machines.StationID
    LEFT JOIN log_data ld ON machines.ID = ld.machine_id
    LEFT JOIN commands cmd ON machines.ID = cmd.machine_id

GROUP BY b.ID, b.Code, b.Name;

-- =====================================================================
-- 4. VIEW CHO LOG ANALYSIS (phân tích log chi tiết)
-- =====================================================================
DROP VIEW IF EXISTS v_log_analysis;
CREATE VIEW v_log_analysis AS
SELECT 
    ld.log_id,
    ld.received_time,
    ld.start_time,
    ld.end_time,
    ld.result,
    ld.log_level,
    ld.PID,
    ld.FID,
    ld.part_no,
    ld.variant,
    ld.jobfile,
    ld.gmes_status,
    
    -- Machine Info
    m.ID as machine_id,
    m.Name as machine_name,
    m.IP as machine_ip,
    m.client_status,
    
    -- Station and Line Info
    s.ID as station_id,
    s.Name as station_name,
    l.ID as line_id,
    l.Name as line_name,
    
    -- Model Info
    model.ID as model_id,
    model.Name as model_name,
    mg.Name as model_group_name,
    b.Name as buyer_name,
    
    -- Analysis Fields
    TIMEDIFF(ld.end_time, ld.start_time) as processing_time,
    CASE 
        WHEN ld.result = 'OK' THEN 'Pass'
        WHEN ld.result = 'NG' THEN 'Fail'
        ELSE 'Unknown'
    END as test_result

FROM log_data ld
    LEFT JOIN machines m ON ld.machine_id = m.ID
    LEFT JOIN stations s ON m.StationID = s.ID
    LEFT JOIN `lines` l ON s.LineId = l.ID
    LEFT JOIN models model ON ld.model_id = model.ID
    LEFT JOIN modelgroups mg ON model.ModelGroupID = mg.ID
    LEFT JOIN buyers b ON mg.BuyerId = b.ID;

-- =====================================================================
-- 5. VIEW CHO COMMAND TRACKING (theo dõi lệnh)
-- =====================================================================
DROP VIEW IF EXISTS v_command_tracking;
CREATE VIEW v_command_tracking AS
SELECT 
    cmd.command_id,
    cmd.command_type,
    cmd.program_name,
    cmd.status as command_status,
    cmd.priority,
    cmd.created_time,
    cmd.sent_time,
    cmd.executed_time,
    cmd.result_message,
    
    -- Machine Info
    m.ID as machine_id,
    m.Name as machine_name,
    m.IP as machine_ip,
    m.client_status as machine_status,
    
    -- Station Info
    s.ID as station_id,
    s.Name as station_name,
    l.Name as line_name,
    
    -- Timing Analysis
    CASE 
        WHEN cmd.status = 'Pending' THEN TIMESTAMPDIFF(MINUTE, cmd.created_time, NOW())
        WHEN cmd.sent_time IS NOT NULL THEN TIMESTAMPDIFF(MINUTE, cmd.created_time, cmd.sent_time)
        ELSE NULL
    END as wait_time_minutes,
    
    CASE 
        WHEN cmd.executed_time IS NOT NULL AND cmd.sent_time IS NOT NULL 
        THEN TIMESTAMPDIFF(SECOND, cmd.sent_time, cmd.executed_time)
        ELSE NULL
    END as execution_time_seconds

FROM commands cmd
    LEFT JOIN machines m ON cmd.machine_id = m.ID
    LEFT JOIN stations s ON cmd.station_id = s.ID OR m.StationID = s.ID
    LEFT JOIN `lines` l ON s.LineId = l.ID;

-- =====================================================================
-- 6. VIEW CHO PRODUCTION SUMMARY (tóm tắt sản xuất)
-- =====================================================================
DROP VIEW IF EXISTS v_production_summary;
CREATE VIEW v_production_summary AS
SELECT 
    DATE(ld.received_time) as production_date,
    l.ID as line_id,
    l.Name as line_name,
    b.Name as buyer_name,
    s.Name as station_name,
    m.Name as machine_name,
    
    -- Production Counts
    COUNT(*) as total_parts,
    SUM(CASE WHEN ld.result = 'OK' THEN 1 ELSE 0 END) as ok_parts,
    SUM(CASE WHEN ld.result = 'NG' THEN 1 ELSE 0 END) as ng_parts,
    
    -- Quality Metrics
    ROUND((SUM(CASE WHEN ld.result = 'OK' THEN 1 ELSE 0 END) * 100.0 / COUNT(*)), 2) as yield_percentage,
    
    -- Timing
    MIN(ld.start_time) as first_part_time,
    MAX(ld.end_time) as last_part_time,
    
    -- Average processing time
    SEC_TO_TIME(AVG(TIME_TO_SEC(TIMEDIFF(ld.end_time, ld.start_time)))) as avg_processing_time

FROM log_data ld
    LEFT JOIN machines m ON ld.machine_id = m.ID
    LEFT JOIN stations s ON m.StationID = s.ID
    LEFT JOIN `lines` l ON s.LineId = l.ID
    LEFT JOIN modelprocesses mp ON s.ModelProcessId = mp.ID
    LEFT JOIN modelgroups mg ON mp.ModelGroupID = mg.ID
    LEFT JOIN buyers b ON mg.BuyerId = b.ID

WHERE ld.received_time >= DATE_SUB(CURDATE(), INTERVAL 30 DAY)

GROUP BY DATE(ld.received_time), l.ID, l.Name, b.Name, s.Name, m.Name
ORDER BY production_date DESC, line_name, station_name;

-- =====================================================================
-- THÊM INDEXES CHO PERFORMANCE (sử dụng procedure để check tồn tại)
-- =====================================================================

-- Index cho commands table
CREATE INDEX idx_commands_machine ON commands(machine_id);
CREATE INDEX idx_commands_status ON commands(status);
CREATE INDEX idx_commands_created_time ON commands(created_time);

-- Index cho log_data table  
CREATE INDEX idx_log_machine_received ON log_data(machine_id, received_time);
CREATE INDEX idx_log_result_date ON log_data(result, received_time);
CREATE INDEX idx_log_start_time ON log_data(start_time);

-- Index cho machines table
CREATE INDEX idx_machines_client_status ON machines(client_status);
CREATE INDEX idx_machines_last_seen ON machines(last_seen);

COMMIT;

-- =====================================================================
-- SCRIPT HOÀN THÀNH - CÁC VIEWS ĐÃ ĐƯỢC TẠO CHO HSE_PM_DB
-- =====================================================================
-- Các views đã tạo:
-- 1. v_machine_complete_details - Chi tiết máy hoàn chỉnh
-- 2. v_station_management - Quản lý station với thống kê
-- 3. v_buyer_dashboard - Dashboard cho buyer
-- 4. v_log_analysis - Phân tích log chi tiết
-- 5. v_command_tracking - Theo dõi lệnh
-- 6. v_production_summary - Tóm tắt sản xuất theo ngày
-- =====================================================================