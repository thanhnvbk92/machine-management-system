-- Cập nhật view v_recent_logs để bao gồm model_name
USE hse_pm_db;

DROP VIEW IF EXISTS v_recent_logs;

CREATE VIEW v_recent_logs AS
SELECT 
    m.Name as machine_name,
    s.Name as station_name,
    l.Name as line_name,
    md.Name as model_name,
    ld.log_level,
    ld.result,
    ld.start_time,
    ld.source,
    ld.PID
FROM log_data ld
JOIN machines m ON ld.machine_id = m.ID
LEFT JOIN stations s ON ld.station_id = s.ID
LEFT JOIN `lines` l ON s.LineId = l.ID
LEFT JOIN models md ON ld.model_id = md.ID
WHERE ld.received_time >= DATE_SUB(NOW(), INTERVAL 1 DAY)
ORDER BY ld.received_time DESC;

SELECT 'View v_recent_logs đã được cập nhật với model_name!' as message;