-- Extended Seed Data for Machine Management System
-- This script adds comprehensive sample data for testing and demonstration

USE machine_management_db;

-- =============================================
-- Additional Model Groups for each Buyer
-- =============================================

INSERT IGNORE INTO MODELGROUPS (ModelGroupId, GroupName, GroupCode, BuyerId, Description, CreatedAt, IsActive) VALUES
-- BMW additional groups
(3, '5 Series', 'BMW_5', 1, 'BMW 5 Series luxury sedan lineup', NOW(), 1),
(4, 'X3 Series', 'BMW_X3', 1, 'BMW X3 compact SUV lineup', NOW(), 1),
(5, 'X5 Series', 'BMW_X5', 1, 'BMW X5 mid-size SUV lineup', NOW(), 1),

-- Audi additional groups  
(6, 'A6', 'AUDI_A6', 2, 'Audi A6 executive sedan lineup', NOW(), 1),
(7, 'Q5', 'AUDI_Q5', 2, 'Audi Q5 compact SUV lineup', NOW(), 1),
(8, 'Q7', 'AUDI_Q7', 2, 'Audi Q7 full-size SUV lineup', NOW(), 1),

-- Volkswagen groups
(9, 'Golf', 'VW_GOLF', 3, 'Volkswagen Golf compact car lineup', NOW(), 1),
(10, 'Passat', 'VW_PASSAT', 3, 'Volkswagen Passat mid-size sedan lineup', NOW(), 1),
(11, 'Tiguan', 'VW_TIGUAN', 3, 'Volkswagen Tiguan compact SUV lineup', NOW(), 1),

-- Mercedes-Benz groups
(12, 'C-Class', 'MB_C', 4, 'Mercedes-Benz C-Class compact executive lineup', NOW(), 1),
(13, 'E-Class', 'MB_E', 4, 'Mercedes-Benz E-Class executive sedan lineup', NOW(), 1),
(14, 'GLC', 'MB_GLC', 4, 'Mercedes-Benz GLC compact SUV lineup', NOW(), 1);

-- =============================================
-- Additional Models for each Model Group
-- =============================================

INSERT IGNORE INTO MODELS (ModelId, ModelName, ModelCode, ModelGroupId, Description, CreatedAt, IsActive) VALUES
-- BMW 3 Series additional models
(3, 'BMW 328i', 'BMW_328I', 1, 'BMW 3 Series 2.8L inline-4 model', NOW(), 1),
(4, 'BMW 335i', 'BMW_335I', 1, 'BMW 3 Series 3.5L inline-6 model', NOW(), 1),

-- BMW 5 Series models
(5, 'BMW 520i', 'BMW_520I', 3, 'BMW 5 Series 2.0L base model', NOW(), 1),
(6, 'BMW 530i', 'BMW_530I', 3, 'BMW 5 Series 3.0L standard model', NOW(), 1),
(7, 'BMW 540i', 'BMW_540I', 3, 'BMW 5 Series 4.0L performance model', NOW(), 1),

-- BMW X3 models
(8, 'BMW X3 xDrive20i', 'BMW_X3_20I', 4, 'BMW X3 2.0L AWD base model', NOW(), 1),
(9, 'BMW X3 xDrive30i', 'BMW_X3_30I', 4, 'BMW X3 3.0L AWD standard model', NOW(), 1),

-- Audi A4 additional models
(10, 'Audi A4 1.8T', 'A4_18T', 2, 'Audi A4 1.8L turbo base model', NOW(), 1),
(11, 'Audi A4 3.2', 'A4_32', 2, 'Audi A4 3.2L V6 performance model', NOW(), 1),

-- Audi A6 models
(12, 'Audi A6 2.0T', 'A6_20T', 6, 'Audi A6 2.0L turbo base model', NOW(), 1),
(13, 'Audi A6 3.0T', 'A6_30T', 6, 'Audi A6 3.0L supercharged model', NOW(), 1),

-- Volkswagen models
(14, 'VW Golf 1.4T', 'GOLF_14T', 9, 'Volkswagen Golf 1.4L turbo base model', NOW(), 1),
(15, 'VW Golf GTI', 'GOLF_GTI', 9, 'Volkswagen Golf GTI performance model', NOW(), 1),
(16, 'VW Passat 2.0T', 'PASSAT_20T', 10, 'Volkswagen Passat 2.0L turbo model', NOW(), 1),

-- Mercedes-Benz models
(17, 'Mercedes C200', 'MB_C200', 12, 'Mercedes-Benz C200 base model', NOW(), 1),
(18, 'Mercedes C300', 'MB_C300', 12, 'Mercedes-Benz C300 standard model', NOW(), 1),
(19, 'Mercedes E250', 'MB_E250', 13, 'Mercedes-Benz E250 base executive model', NOW(), 1);

-- =============================================
-- Model Processes for each Model
-- =============================================

INSERT IGNORE INTO MODELPROCESSES (ModelProcessId, ProcessName, ProcessCode, ModelId, Description, CreatedAt, IsActive) VALUES
-- BMW 320i processes
(3, 'Welding', 'WLD', 1, 'Body welding and assembly process', NOW(), 1),
(4, 'Quality Control', 'QC', 1, 'Final quality inspection process', NOW(), 1),

-- BMW 530i processes
(5, 'Assembly', 'ASM', 6, 'Main vehicle assembly process', NOW(), 1),
(6, 'Paint', 'PNT', 6, 'Vehicle painting process', NOW(), 1),
(7, 'Interior', 'INT', 6, 'Interior fitting and trim process', NOW(), 1),

-- Audi A4 processes
(8, 'Welding', 'WLD', 2, 'Body welding and joining process', NOW(), 1),
(9, 'Electronics', 'ELC', 2, 'Electronic systems integration', NOW(), 1),

-- VW Golf processes
(10, 'Stamping', 'STP', 14, 'Body panel stamping process', NOW(), 1),
(11, 'Assembly', 'ASM', 14, 'Final vehicle assembly', NOW(), 1),

-- Mercedes C200 processes
(12, 'Paint', 'PNT', 17, 'Premium paint finishing process', NOW(), 1),
(13, 'Assembly', 'ASM', 17, 'Luxury vehicle assembly process', NOW(), 1);

-- =============================================
-- Production Lines for each Process
-- =============================================

INSERT IGNORE INTO LINES (LineId, LineName, LineCode, ModelProcessId, Description, CreatedAt, IsActive) VALUES
-- Assembly lines
(3, 'Assembly Line 2', 'ASM_L2', 1, 'Secondary assembly line for BMW 320i', NOW(), 1),
(4, 'Assembly Line 3', 'ASM_L3', 5, 'Primary assembly line for BMW 530i', NOW(), 1),
(5, 'Assembly Line 4', 'ASM_L4', 11, 'VW Golf assembly line', NOW(), 1),

-- Paint lines
(6, 'Paint Line 2', 'PNT_L2', 2, 'Secondary paint line for BMW 320i', NOW(), 1),
(7, 'Paint Line 3', 'PNT_L3', 6, 'BMW 530i premium paint line', NOW(), 1),
(8, 'Paint Line 4', 'PNT_L4', 12, 'Mercedes premium paint line', NOW(), 1),

-- Welding lines
(9, 'Welding Line 1', 'WLD_L1', 3, 'BMW 320i body welding line', NOW(), 1),
(10, 'Welding Line 2', 'WLD_L2', 8, 'Audi A4 welding line', NOW(), 1),

-- Quality control lines
(11, 'QC Line 1', 'QC_L1', 4, 'BMW 320i quality control line', NOW(), 1),

-- Electronics lines
(12, 'Electronics Line 1', 'ELC_L1', 9, 'Audi A4 electronics integration line', NOW(), 1);

-- =============================================
-- Work Stations for each Line
-- =============================================

INSERT IGNORE INTO STATIONS (StationId, StationName, StationCode, LineId, Description, CreatedAt, IsActive) VALUES
-- Assembly Line 1 additional stations
(3, 'Assembly Station C', 'ASM_S3', 1, 'Engine installation station', NOW(), 1),
(4, 'Assembly Station D', 'ASM_S4', 1, 'Transmission installation station', NOW(), 1),
(5, 'Assembly Station E', 'ASM_S5', 1, 'Final assembly checkout station', NOW(), 1),

-- Paint Line 1 additional stations
(6, 'Paint Prep Station', 'PNT_P1', 2, 'Surface preparation station', NOW(), 1),
(7, 'Base Coat Station', 'PNT_B1', 2, 'Base coat application station', NOW(), 1),
(8, 'Clear Coat Station', 'PNT_C1', 2, 'Clear coat application station', NOW(), 1),

-- Welding Line 1 stations
(9, 'Spot Welding Station', 'WLD_SW1', 9, 'Automated spot welding station', NOW(), 1),
(10, 'Arc Welding Station', 'WLD_AW1', 9, 'Manual arc welding station', NOW(), 1),
(11, 'Welding QC Station', 'WLD_QC1', 9, 'Welding quality control station', NOW(), 1),

-- Quality Control stations
(12, 'Visual Inspection', 'QC_VI1', 11, 'Visual quality inspection station', NOW(), 1),
(13, 'Dimensional Check', 'QC_DC1', 11, 'Dimensional accuracy check station', NOW(), 1),
(14, 'Electronic Test', 'QC_ET1', 11, 'Electronic systems test station', NOW(), 1),

-- Electronics stations
(15, 'ECU Installation', 'ELC_ECU1', 12, 'Engine control unit installation', NOW(), 1),
(16, 'Wiring Harness', 'ELC_WH1', 12, 'Main wiring harness installation', NOW(), 1),
(17, 'System Test', 'ELC_ST1', 12, 'Electronic systems testing', NOW(), 1);

-- =============================================
-- Machines for each Station
-- =============================================

INSERT IGNORE INTO MACHINES (MachineId, MachineName, MachineCode, MachineType, StationId, Description, CreatedAt, IsActive) VALUES
-- Assembly stations machines
(3, 'Engine Crane 1', 'ASM_EC001', 'Crane', 3, 'Hydraulic engine lifting crane', NOW(), 1),
(4, 'Engine Crane 2', 'ASM_EC002', 'Crane', 3, 'Backup hydraulic engine crane', NOW(), 1),
(5, 'Transmission Lift', 'ASM_TL001', 'Lift', 4, 'Transmission installation lift', NOW(), 1),
(6, 'Torque Wrench Station', 'ASM_TW001', 'Tool', 4, 'Automated torque wrench system', NOW(), 1),
(7, 'Final Test Console', 'ASM_FT001', 'Console', 5, 'Final assembly test console', NOW(), 1),

-- Paint stations machines
(8, 'Surface Cleaner', 'PNT_SC001', 'Cleaner', 6, 'Automated surface cleaning system', NOW(), 1),
(9, 'Primer Booth', 'PNT_PB001', 'Booth', 6, 'Automated primer application booth', NOW(), 1),
(10, 'Base Coat Robot', 'PNT_BR001', 'Robot', 7, 'Robotic base coat sprayer', NOW(), 1),
(11, 'Base Coat Robot 2', 'PNT_BR002', 'Robot', 7, 'Secondary base coat robot', NOW(), 1),
(12, 'Clear Coat Robot', 'PNT_CR001', 'Robot', 8, 'Automated clear coat application', NOW(), 1),
(13, 'Drying Oven', 'PNT_DO001', 'Oven', 8, 'Paint curing oven system', NOW(), 1),

-- Welding stations machines
(14, 'Spot Welder 1', 'WLD_SW001', 'Welder', 9, 'Automated spot welding robot', NOW(), 1),
(15, 'Spot Welder 2', 'WLD_SW002', 'Welder', 9, 'Secondary spot welding robot', NOW(), 1),
(16, 'Arc Welder 1', 'WLD_AW001', 'Welder', 10, 'Manual arc welding station', NOW(), 1),
(17, 'Welding Fixture', 'WLD_FX001', 'Fixture', 10, 'Component holding fixture', NOW(), 1),
(18, 'Weld Inspector', 'WLD_IN001', 'Inspector', 11, 'Automated weld inspection system', NOW(), 1),

-- Quality Control machines
(19, 'Vision System 1', 'QC_VS001', 'Vision', 12, 'Automated visual inspection camera', NOW(), 1),
(20, 'Measuring Arm', 'QC_MA001', 'Measurement', 13, 'Coordinate measuring machine', NOW(), 1),
(21, 'Laser Scanner', 'QC_LS001', 'Scanner', 13, '3D laser scanning system', NOW(), 1),
(22, 'Test Bench', 'QC_TB001', 'Tester', 14, 'Electronic systems test bench', NOW(), 1),

-- Electronics machines
(23, 'ECU Programmer', 'ELC_EP001', 'Programmer', 15, 'ECU programming station', NOW(), 1),
(24, 'Harness Tester', 'ELC_HT001', 'Tester', 16, 'Wiring harness continuity tester', NOW(), 1),
(25, 'System Simulator', 'ELC_SS001', 'Simulator', 17, 'Vehicle systems simulator', NOW(), 1);

-- =============================================
-- Sample Log Data for testing
-- =============================================

-- Insert sample logs for the past few days
INSERT INTO LOGDATA (MachineId, LogType, LogLevel, Message, Details, LogTimestamp, Source, Category, CreatedAt) VALUES
-- Recent logs (last 24 hours)
(1, 'Operation', 'Information', 'Welding operation started', 'Welding sequence 001 initiated for part BMW_320I_001', DATE_SUB(NOW(), INTERVAL 2 HOUR), 'WeldingController', 'Production', NOW()),
(1, 'Operation', 'Information', 'Welding operation completed', 'Welding sequence 001 completed successfully', DATE_SUB(NOW(), INTERVAL 1 HOUR), 'WeldingController', 'Production', NOW()),
(2, 'Operation', 'Information', 'Paint spray started', 'Base coat application started', DATE_SUB(NOW(), INTERVAL 3 HOUR), 'PaintController', 'Production', NOW()),
(2, 'Operation', 'Warning', 'Paint pressure low', 'Paint pressure dropped to 85% of normal', DATE_SUB(NOW(), INTERVAL 2 HOUR), 'PaintController', 'Maintenance', NOW()),
(2, 'Operation', 'Information', 'Paint pressure normalized', 'Paint pressure restored to normal levels', DATE_SUB(NOW(), INTERVAL 1 HOUR), 'PaintController', 'Maintenance', NOW()),

-- Yesterday's logs
(3, 'Operation', 'Information', 'Engine installation started', 'BMW 320i engine installation sequence', DATE_SUB(NOW(), INTERVAL 1 DAY), 'CraneController', 'Production', NOW()),
(4, 'Maintenance', 'Warning', 'Hydraulic pressure alert', 'Hydraulic system pressure below optimal range', DATE_SUB(NOW(), INTERVAL 1 DAY), 'HydraulicSystem', 'Maintenance', NOW()),
(5, 'Operation', 'Information', 'Transmission installed', 'Automatic transmission installation completed', DATE_SUB(NOW(), INTERVAL 1 DAY), 'TransController', 'Production', NOW()),

-- Last week's logs
(14, 'Operation', 'Information', 'Spot welding cycle started', 'Automated spot welding cycle 1001', DATE_SUB(NOW(), INTERVAL 3 DAY), 'WeldRobot', 'Production', NOW()),
(14, 'Alarm', 'Error', 'Electrode wear detected', 'Welding electrode wear exceeds threshold', DATE_SUB(NOW(), INTERVAL 3 DAY), 'WeldRobot', 'Maintenance', NOW()),
(15, 'Maintenance', 'Information', 'Electrode replacement completed', 'Welding electrodes replaced and calibrated', DATE_SUB(NOW(), INTERVAL 2 DAY), 'Maintenance', 'Maintenance', NOW()),

-- Quality control logs
(19, 'Quality', 'Information', 'Visual inspection passed', 'All visual inspection points passed', DATE_SUB(NOW(), INTERVAL 4 HOUR), 'VisionSystem', 'Quality', NOW()),
(20, 'Quality', 'Warning', 'Dimensional variance detected', 'Part dimension 0.2mm outside tolerance', DATE_SUB(NOW(), INTERVAL 6 HOUR), 'CMM', 'Quality', NOW()),
(21, 'Quality', 'Information', 'Laser scan completed', '3D scan completed, all points within tolerance', DATE_SUB(NOW(), INTERVAL 8 HOUR), 'LaserScanner', 'Quality', NOW());

-- =============================================
-- Sample Commands for testing
-- =============================================

INSERT INTO COMMANDS (MachineId, CommandType, CommandData, Status, Priority, CreatedAt, ScheduledAt) VALUES
-- Pending commands
(1, 'START_WELD', '{"sequence": "002", "part_id": "BMW_320I_002", "weld_points": 15}', 'Pending', 1, NOW(), DATE_ADD(NOW(), INTERVAL 30 MINUTE)),
(2, 'CHANGE_COLOR', '{"color_code": "BMW_ALPINE_WHITE", "batch_id": "PNT_001"}', 'Pending', 2, NOW(), DATE_ADD(NOW(), INTERVAL 1 HOUR)),
(14, 'REPLACE_ELECTRODE', '{"electrode_type": "TYPE_A", "position": "LEFT"}', 'Pending', 1, NOW(), DATE_ADD(NOW(), INTERVAL 15 MINUTE)),

-- Completed commands
(3, 'LIFT_ENGINE', '{"engine_type": "BMW_B48", "target_height": "1200mm"}', 'Completed', 1, DATE_SUB(NOW(), INTERVAL 2 HOUR), DATE_SUB(NOW(), INTERVAL 2 HOUR)),
(5, 'INSTALL_TRANS', '{"trans_type": "8AT", "bolt_torque": "65Nm"}', 'Completed', 1, DATE_SUB(NOW(), INTERVAL 1 DAY), DATE_SUB(NOW(), INTERVAL 1 DAY)),

-- Failed commands
(4, 'HYDRAULIC_TEST', '{"test_pressure": "200bar", "duration": "30s"}', 'Failed', 2, DATE_SUB(NOW(), INTERVAL 1 DAY), DATE_SUB(NOW(), INTERVAL 1 DAY));

-- =============================================
-- Sample Client Configurations
-- =============================================

INSERT INTO CLIENT_CONFIGS (MachineId, ConfigKey, ConfigValue, DataType, Description, CreatedAt) VALUES
-- Welding machine configs
(1, 'weld_current', '180', 'Integer', 'Welding current in amperes', NOW()),
(1, 'weld_voltage', '22.5', 'Float', 'Welding voltage in volts', NOW()),
(1, 'electrode_life', '5000', 'Integer', 'Expected electrode life in cycles', NOW()),

-- Paint machine configs  
(2, 'spray_pressure', '3.5', 'Float', 'Paint spray pressure in bar', NOW()),
(2, 'booth_temperature', '23', 'Integer', 'Paint booth temperature in Celsius', NOW()),
(2, 'color_changeover_time', '15', 'Integer', 'Color changeover time in minutes', NOW()),

-- Crane configs
(3, 'max_load', '500', 'Integer', 'Maximum load capacity in kg', NOW()),
(3, 'safety_factor', '2.0', 'Float', 'Safety factor for load calculations', NOW()),

-- General machine configs
(14, 'maintenance_interval', '168', 'Integer', 'Maintenance interval in hours', NOW()),
(15, 'backup_frequency', '60', 'Integer', 'Configuration backup frequency in minutes', NOW());

-- Show summary of inserted data
SELECT 
    'Data seeding completed successfully!' as Status,
    (SELECT COUNT(*) FROM BUYERS) as Buyers,
    (SELECT COUNT(*) FROM MODELGROUPS) as ModelGroups,  
    (SELECT COUNT(*) FROM MODELS) as Models,
    (SELECT COUNT(*) FROM MODELPROCESSES) as ModelProcesses,
    (SELECT COUNT(*) FROM LINES) as ProductionLines,
    (SELECT COUNT(*) FROM STATIONS) as WorkStations,
    (SELECT COUNT(*) FROM MACHINES) as Machines,
    (SELECT COUNT(*) FROM LOGDATA) as LogEntries,
    (SELECT COUNT(*) FROM COMMANDS) as Commands,
    (SELECT COUNT(*) FROM CLIENT_CONFIGS) as ClientConfigs;