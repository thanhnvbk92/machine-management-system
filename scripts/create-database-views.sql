-- =====================================================================
-- SCRIPT TẠO CÁC DATABASE VIEWS CHO HSE_PM_DATABASE
-- Machine Management System - Database Views Creation Script
-- =====================================================================

USE hse_pm_database;

-- =====================================================================
-- 1. VIEW CHO MACHINE DETAILS (bao gồm thông tin từ nhiều bảng liên quan)
-- =====================================================================
DROP VIEW IF EXISTS v_machine_details;
CREATE VIEW v_machine_details AS
SELECT 
    m.ID as machine_id,
    m.Name as machine_name,
    m.status as machine_status,
    m.IP as machine_ip,
    m.GMES_Name as gmes_name,
    m.ProgramName as program_name,
    m.mac_address,
    
    -- Machine Type Info
    mt.Id as machine_type_id,
    mt.Name as machine_type_name,
    
    -- Station Info
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
    b.Name as buyer_name

FROM machines m
    LEFT JOIN machinetypes mt ON m.MachineTypeId = mt.Id
    LEFT JOIN stations s ON m.StationID = s.ID
    LEFT JOIN `lines` l ON s.LineId = l.ID
    LEFT JOIN modelprocesses mp ON s.ModelProcessId = mp.ID
    LEFT JOIN modelgroups mg ON mp.ModelGroupID = mg.ID
    LEFT JOIN buyers b ON mg.BuyerId = b.ID;

-- =====================================================================
-- 2. VIEW CHO STATION HIERARCHY (thứ bậc từ Buyer -> Line -> Station)
-- =====================================================================
DROP VIEW IF EXISTS v_station_hierarchy;
CREATE VIEW v_station_hierarchy AS
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
    
    -- Machine Count in this station
    (SELECT COUNT(*) FROM machines WHERE StationID = s.ID) as machine_count

FROM stations s
    INNER JOIN `lines` l ON s.LineId = l.ID
    INNER JOIN modelprocesses mp ON s.ModelProcessId = mp.ID
    INNER JOIN modelgroups mg ON mp.ModelGroupID = mg.ID
    INNER JOIN buyers b ON mg.BuyerId = b.ID;

-- =====================================================================
-- 3. VIEW CHO MODEL RELATIONSHIP (mối quan hệ Model -> ModelGroup -> Buyer)
-- =====================================================================
DROP VIEW IF EXISTS v_model_relationships;
CREATE VIEW v_model_relationships AS
SELECT 
    m.ID as model_id,
    m.Name as model_name,
    
    -- Model Group Info
    mg.ID as model_group_id,
    mg.Name as model_group_name,
    
    -- Buyer Info
    b.ID as buyer_id,
    b.Code as buyer_code,
    b.Name as buyer_name,
    
    -- Related Model Processes
    (SELECT COUNT(*) FROM modelprocesses WHERE ModelGroupID = mg.ID) as process_count

FROM models m
    INNER JOIN modelgroups mg ON m.ModelGroupID = mg.ID
    INNER JOIN buyers b ON mg.BuyerId = b.ID;

-- =====================================================================
-- 4. VIEW CHO MACHINE STATISTICS BY BUYER
-- =====================================================================
DROP VIEW IF EXISTS v_machine_stats_by_buyer;
CREATE VIEW v_machine_stats_by_buyer AS
SELECT 
    b.ID as buyer_id,
    b.Code as buyer_code,
    b.Name as buyer_name,
    
    COUNT(DISTINCT mg.ID) as model_group_count,
    COUNT(DISTINCT m.ID) as model_count,
    COUNT(DISTINCT mp.ID) as process_count,
    COUNT(DISTINCT l.ID) as line_count,
    COUNT(DISTINCT s.ID) as station_count,
    COUNT(DISTINCT machines.ID) as machine_count,
    
    -- Machine Status Counts
    SUM(CASE WHEN machines.status = 'Online' THEN 1 ELSE 0 END) as machines_online,
    SUM(CASE WHEN machines.status = 'Offline' THEN 1 ELSE 0 END) as machines_offline,
    SUM(CASE WHEN machines.status = 'Maintenance' THEN 1 ELSE 0 END) as machines_maintenance

FROM buyers b
    LEFT JOIN modelgroups mg ON b.ID = mg.BuyerId
    LEFT JOIN models m ON mg.ID = m.ModelGroupID
    LEFT JOIN modelprocesses mp ON mg.ID = mp.ModelGroupID
    LEFT JOIN stations s ON mp.ID = s.ModelProcessId
    LEFT JOIN `lines` l ON s.LineId = l.ID
    LEFT JOIN machines ON s.ID = machines.StationID

GROUP BY b.ID, b.Code, b.Name;

-- =====================================================================
-- 5. VIEW CHO LINE SUMMARY (tóm tắt thông tin line)
-- =====================================================================
DROP VIEW IF EXISTS v_line_summary;
CREATE VIEW v_line_summary AS
SELECT 
    l.ID as line_id,
    l.Name as line_name,
    
    COUNT(DISTINCT s.ID) as station_count,
    COUNT(DISTINCT machines.ID) as machine_count,
    
    -- Machine Status in this line
    SUM(CASE WHEN machines.status = 'Online' THEN 1 ELSE 0 END) as machines_online,
    SUM(CASE WHEN machines.status = 'Offline' THEN 1 ELSE 0 END) as machines_offline,
    SUM(CASE WHEN machines.status = 'Maintenance' THEN 1 ELSE 0 END) as machines_maintenance,
    
    -- Unique buyers in this line
    GROUP_CONCAT(DISTINCT b.Name ORDER BY b.Name SEPARATOR ', ') as buyers

FROM `lines` l
    LEFT JOIN stations s ON l.ID = s.LineId
    LEFT JOIN machines ON s.ID = machines.StationID
    LEFT JOIN modelprocesses mp ON s.ModelProcessId = mp.ID
    LEFT JOIN modelgroups mg ON mp.ModelGroupID = mg.ID
    LEFT JOIN buyers b ON mg.BuyerId = b.ID

GROUP BY l.ID, l.Name;

-- =====================================================================
-- 6. VIEW CHO RECENT ACTIVITIES sẽ được tạo sau khi có bảng log_data
-- =====================================================================
-- DROP VIEW IF EXISTS v_recent_activities;
-- Bảng log_data chưa tồn tại trong hse_pm_database
-- VIEW này sẽ được tạo sau

-- =====================================================================
-- THÊM INDEXES CHO PERFORMANCE
-- =====================================================================

-- Index cho các foreign keys nếu chưa có
CREATE INDEX IF NOT EXISTS idx_machines_station ON machines(StationID);
CREATE INDEX IF NOT EXISTS idx_machines_type ON machines(MachineTypeId);
CREATE INDEX IF NOT EXISTS idx_stations_line ON stations(LineId);
CREATE INDEX IF NOT EXISTS idx_stations_process ON stations(ModelProcessId);
CREATE INDEX IF NOT EXISTS idx_modelprocesses_group ON modelprocesses(ModelGroupID);
CREATE INDEX IF NOT EXISTS idx_modelgroups_buyer ON modelgroups(BuyerId);
CREATE INDEX IF NOT EXISTS idx_models_group ON models(ModelGroupID);

-- Index cho log_data sẽ được tạo sau khi có bảng log_data
-- CREATE INDEX IF NOT EXISTS idx_log_timestamp ON log_data(timestamp);
-- CREATE INDEX IF NOT EXISTS idx_log_machine_name ON log_data(machine_name);

COMMIT;

-- =====================================================================
-- SCRIPT HOÀN THÀNH - CÁC VIEWS ĐÃ ĐƯỢC TẠO
-- =====================================================================
-- Các views đã tạo:
-- 1. v_machine_details - Chi tiết máy với tất cả thông tin liên quan
-- 2. v_station_hierarchy - Cấu trúc phân cấp station
-- 3. v_model_relationships - Mối quan hệ model
-- 4. v_machine_stats_by_buyer - Thống kê máy theo buyer
-- 5. v_line_summary - Tóm tắt thông tin line
-- 6. v_recent_activities - Hoạt động gần đây
-- =====================================================================