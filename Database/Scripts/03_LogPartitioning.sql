-- Log Partitioning Setup for Machine Management System
-- This script creates partitions for LOGDATA table to improve query performance

USE machine_management_db;

-- =============================================
-- Create Log Partitioning
-- =============================================

-- First, we need to check if the table is already partitioned
SET @partition_check = (
    SELECT COUNT(*) 
    FROM information_schema.PARTITIONS 
    WHERE table_schema = 'machine_management_db' 
    AND table_name = 'LOGDATA' 
    AND partition_name IS NOT NULL
);

-- Only proceed if table is not already partitioned
SET @sql_partition = 
CASE 
    WHEN @partition_check = 0 THEN
        'ALTER TABLE LOGDATA PARTITION BY RANGE (YEAR(LogTimestamp) * 100 + MONTH(LogTimestamp)) (
            PARTITION p_2024_01 VALUES LESS THAN (202402),
            PARTITION p_2024_02 VALUES LESS THAN (202403),
            PARTITION p_2024_03 VALUES LESS THAN (202404),
            PARTITION p_2024_04 VALUES LESS THAN (202405),
            PARTITION p_2024_05 VALUES LESS THAN (202406),
            PARTITION p_2024_06 VALUES LESS THAN (202407),
            PARTITION p_2024_07 VALUES LESS THAN (202408),
            PARTITION p_2024_08 VALUES LESS THAN (202409),
            PARTITION p_2024_09 VALUES LESS THAN (202410),
            PARTITION p_2024_10 VALUES LESS THAN (202411),
            PARTITION p_2024_11 VALUES LESS THAN (202412),
            PARTITION p_2024_12 VALUES LESS THAN (202501),
            PARTITION p_2025_01 VALUES LESS THAN (202502),
            PARTITION p_2025_02 VALUES LESS THAN (202503),
            PARTITION p_2025_03 VALUES LESS THAN (202504),
            PARTITION p_2025_04 VALUES LESS THAN (202505),
            PARTITION p_2025_05 VALUES LESS THAN (202506),
            PARTITION p_2025_06 VALUES LESS THAN (202507),
            PARTITION p_2025_07 VALUES LESS THAN (202508),
            PARTITION p_2025_08 VALUES LESS THAN (202509),
            PARTITION p_2025_09 VALUES LESS THAN (202510),
            PARTITION p_2025_10 VALUES LESS THAN (202511),
            PARTITION p_2025_11 VALUES LESS THAN (202512),
            PARTITION p_2025_12 VALUES LESS THAN (202601),
            PARTITION p_2026_01 VALUES LESS THAN (202602),
            PARTITION p_2026_02 VALUES LESS THAN (202603),
            PARTITION p_2026_03 VALUES LESS THAN (202604),
            PARTITION p_2026_04 VALUES LESS THAN (202605),
            PARTITION p_2026_05 VALUES LESS THAN (202606),
            PARTITION p_2026_06 VALUES LESS THAN (202607),
            PARTITION p_2026_07 VALUES LESS THAN (202608),
            PARTITION p_2026_08 VALUES LESS THAN (202609),
            PARTITION p_2026_09 VALUES LESS THAN (202610),
            PARTITION p_2026_10 VALUES LESS THAN (202611),
            PARTITION p_2026_11 VALUES LESS THAN (202612),
            PARTITION p_2026_12 VALUES LESS THAN (202701),
            PARTITION p_future VALUES LESS THAN MAXVALUE
        );'
    ELSE 
        'SELECT "Table LOGDATA is already partitioned" as Status;'
END;

PREPARE stmt FROM @sql_partition;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- =============================================
-- Stored Procedure for Automatic Partition Management
-- =============================================

DELIMITER //

DROP PROCEDURE IF EXISTS ManageLogPartitions//
CREATE PROCEDURE ManageLogPartitions()
BEGIN
    DECLARE v_year INT;
    DECLARE v_month INT;
    DECLARE v_next_year INT;
    DECLARE v_next_month INT;
    DECLARE v_partition_name VARCHAR(20);
    DECLARE v_partition_value INT;
    DECLARE v_partition_exists INT DEFAULT 0;
    DECLARE v_sql TEXT;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    -- Get next month values
    SET v_year = YEAR(DATE_ADD(CURDATE(), INTERVAL 2 MONTH));
    SET v_month = MONTH(DATE_ADD(CURDATE(), INTERVAL 2 MONTH));
    
    -- Calculate next month after that
    IF v_month = 12 THEN
        SET v_next_year = v_year + 1;
        SET v_next_month = 1;
    ELSE
        SET v_next_year = v_year;
        SET v_next_month = v_month + 1;
    END IF;

    -- Create partition name and value
    SET v_partition_name = CONCAT('p_', v_year, '_', LPAD(v_month, 2, '0'));
    SET v_partition_value = v_next_year * 100 + v_next_month;

    -- Check if partition already exists
    SELECT COUNT(*) INTO v_partition_exists
    FROM information_schema.PARTITIONS 
    WHERE table_schema = 'machine_management_db' 
      AND table_name = 'LOGDATA' 
      AND partition_name = v_partition_name;

    -- If partition doesn't exist, create it
    IF v_partition_exists = 0 THEN
        -- First, split the future partition
        SET v_sql = CONCAT('ALTER TABLE LOGDATA REORGANIZE PARTITION p_future INTO (',
            'PARTITION ', v_partition_name, ' VALUES LESS THAN (', v_partition_value, '),',
            'PARTITION p_future VALUES LESS THAN MAXVALUE',
        ');');
        
        SET @sql = v_sql;
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
        
        SELECT CONCAT('Created partition: ', v_partition_name) as Status;
    ELSE
        SELECT CONCAT('Partition already exists: ', v_partition_name) as Status;
    END IF;
END//

-- =============================================
-- Stored Procedure for Dropping Old Partitions
-- =============================================

DROP PROCEDURE IF EXISTS DropOldLogPartitions//
CREATE PROCEDURE DropOldLogPartitions(
    IN p_retention_months INT DEFAULT 12
)
BEGIN
    DECLARE v_cutoff_year INT;
    DECLARE v_cutoff_month INT;
    DECLARE v_cutoff_value INT;
    DECLARE v_partition_name VARCHAR(50);
    DECLARE v_sql TEXT;
    DECLARE done INT DEFAULT FALSE;
    
    DECLARE partition_cursor CURSOR FOR
        SELECT partition_name 
        FROM information_schema.PARTITIONS 
        WHERE table_schema = 'machine_management_db' 
          AND table_name = 'LOGDATA' 
          AND partition_name IS NOT NULL
          AND partition_name != 'p_future'
          AND CAST(SUBSTRING(partition_name, 3, 4) AS UNSIGNED) * 100 + 
              CAST(SUBSTRING(partition_name, 8, 2) AS UNSIGNED) < v_cutoff_value;
    
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    -- Calculate cutoff date
    SET v_cutoff_year = YEAR(DATE_SUB(CURDATE(), INTERVAL p_retention_months MONTH));
    SET v_cutoff_month = MONTH(DATE_SUB(CURDATE(), INTERVAL p_retention_months MONTH));
    SET v_cutoff_value = v_cutoff_year * 100 + v_cutoff_month;

    START TRANSACTION;

    OPEN partition_cursor;
    
    drop_loop: LOOP
        FETCH partition_cursor INTO v_partition_name;
        IF done THEN
            LEAVE drop_loop;
        END IF;
        
        -- Drop the partition
        SET v_sql = CONCAT('ALTER TABLE LOGDATA DROP PARTITION ', v_partition_name, ';');
        
        SET @sql = v_sql;
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
        
        SELECT CONCAT('Dropped old partition: ', v_partition_name) as Status;
    END LOOP;
    
    CLOSE partition_cursor;
    
    COMMIT;
    
    SELECT CONCAT('Partition cleanup completed. Cutoff: ', v_cutoff_year, '-', LPAD(v_cutoff_month, 2, '0')) as Summary;
END//

-- =============================================
-- Stored Procedure to Show Partition Information
-- =============================================

DROP PROCEDURE IF EXISTS ShowPartitionInfo//
CREATE PROCEDURE ShowPartitionInfo()
BEGIN
    SELECT 
        partition_name as PartitionName,
        CASE 
            WHEN partition_name = 'p_future' THEN 'Future (MAXVALUE)'
            ELSE CONCAT(
                SUBSTRING(partition_name, 3, 4), '-',
                SUBSTRING(partition_name, 8, 2)
            )
        END as Period,
        table_rows as EstimatedRows,
        ROUND(data_length / 1024 / 1024, 2) as DataSizeMB,
        ROUND(index_length / 1024 / 1024, 2) as IndexSizeMB,
        ROUND((data_length + index_length) / 1024 / 1024, 2) as TotalSizeMB,
        partition_description as PartitionRule
    FROM information_schema.PARTITIONS 
    WHERE table_schema = 'machine_management_db' 
      AND table_name = 'LOGDATA' 
      AND partition_name IS NOT NULL
    ORDER BY 
        CASE 
            WHEN partition_name = 'p_future' THEN 999999
            ELSE CAST(SUBSTRING(partition_name, 3, 4) AS UNSIGNED) * 100 + 
                 CAST(SUBSTRING(partition_name, 8, 2) AS UNSIGNED)
        END;
END//

DELIMITER ;

-- =============================================
-- Event Scheduler for Automatic Partition Management
-- =============================================

-- Enable event scheduler if not already enabled
SET GLOBAL event_scheduler = ON;

-- Create event to automatically manage partitions monthly
DROP EVENT IF EXISTS evt_manage_log_partitions;
CREATE EVENT evt_manage_log_partitions
ON SCHEDULE EVERY 1 MONTH
STARTS '2024-01-01 02:00:00'
DO
BEGIN
    CALL ManageLogPartitions();
END;

-- Create event to cleanup old partitions every 3 months
DROP EVENT IF EXISTS evt_cleanup_old_partitions;
CREATE EVENT evt_cleanup_old_partitions
ON SCHEDULE EVERY 3 MONTH  
STARTS '2024-01-15 03:00:00'
DO
BEGIN
    CALL DropOldLogPartitions(12); -- Keep 12 months of data
END;

-- Show partition status
CALL ShowPartitionInfo();

SELECT 'Log partitioning setup completed successfully!' as Status;
SELECT 'Automatic partition management events created' as Info;
SELECT 'Use CALL ManageLogPartitions() to manually create next month partition' as Tip;