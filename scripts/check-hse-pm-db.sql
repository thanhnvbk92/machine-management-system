-- =====================================================================
-- SCRIPT KIỂM TRA CẤU TRÚC DATABASE HSE_PM_DB
-- =====================================================================

USE HSE_PM_DB;

-- Kiểm tra cấu trúc các bảng
DESCRIBE buyers;
DESCRIBE `lines`;
DESCRIBE modelgroups;
DESCRIBE models;
DESCRIBE stations;
DESCRIBE machinetypes;
DESCRIBE machines;
DESCRIBE commands;
DESCRIBE log_data;
DESCRIBE client_config;

-- Hiển thị các views hiện có
SHOW FULL TABLES WHERE Table_type = 'VIEW';