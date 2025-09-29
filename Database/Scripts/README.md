# Database Scripts for Machine Management System

This directory contains SQL scripts for setting up and managing the MySQL database for the Machine Management System.

## 📁 Script Files

### 01_StoredProcedures.sql
**Purpose**: Creates stored procedures for complex database operations

**Procedures Created**:
- `GetMachineHierarchy(machine_id, buyer_code)` - Retrieves complete production hierarchy
- `GetLogsByDateRange(start_date, end_date, machine_id, log_level, limit, offset)` - Efficient log querying with pagination
- `CleanupOldLogs(retention_days, batch_size, dry_run)` - Maintenance procedure for log cleanup
- `GetMachineStatistics(start_date, end_date)` - Machine performance statistics

**Usage**:
```sql
-- Get all machines for BMW
CALL GetMachineHierarchy(NULL, 'BMW');

-- Get error logs from last 24 hours
CALL GetLogsByDateRange(DATE_SUB(NOW(), INTERVAL 1 DAY), NOW(), NULL, 'Error');

-- Get machine statistics for last week
CALL GetMachineStatistics(DATE_SUB(NOW(), INTERVAL 7 DAY), NOW());
```

### 02_Views.sql
**Purpose**: Creates database views for common data access patterns

**Views Created**:
- `v_machine_status` - Current status of all machines with health indicators
- `v_log_summary` - Daily log statistics grouped by machine and level
- `v_production_hierarchy` - Complete production hierarchy with machine counts
- `v_command_queue` - Command execution queue with position and timing
- `v_error_analysis` - Error pattern analysis for troubleshooting

**Usage**:
```sql
-- Check machine health status
SELECT * FROM v_machine_status WHERE HealthStatus = 'Critical';

-- View recent log summary
SELECT * FROM v_log_summary WHERE LogDate = CURDATE();

-- Monitor command queue
SELECT * FROM v_command_queue WHERE Status = 'Pending' ORDER BY QueuePosition;
```

### 03_LogPartitioning.sql
**Purpose**: Implements table partitioning for LOGDATA table to improve query performance

**Features**:
- Monthly partitions for LOGDATA table (2024-2026 + future partition)
- Automatic partition management with stored procedures
- Scheduled events for partition maintenance
- Partition information and management procedures

**Procedures Created**:
- `ManageLogPartitions()` - Creates next month's partition automatically
- `DropOldLogPartitions(retention_months)` - Removes old partitions
- `ShowPartitionInfo()` - Displays partition information and sizes

**Automatic Events**:
- `evt_manage_log_partitions` - Runs monthly to create new partitions
- `evt_cleanup_old_partitions` - Runs quarterly to cleanup old partitions

### 04_ExtendedSeedData.sql
**Purpose**: Provides comprehensive sample data for testing and demonstration

**Data Included**:
- **4 Buyers**: BMW, Audi, Volkswagen, Mercedes-Benz
- **14 Model Groups**: 3-5 groups per buyer (3 Series, A4, Golf, C-Class, etc.)
- **19 Models**: Multiple variants per model group
- **13 Model Processes**: Assembly, Paint, Welding, QC, Electronics
- **12 Production Lines**: Various manufacturing lines
- **17 Work Stations**: Specific work stations per line
- **25+ Machines**: Robots, welders, paint booths, test equipment
- **Sample Log Data**: Recent logs with various levels and categories
- **Sample Commands**: Pending, completed, and failed commands
- **Client Configurations**: Machine-specific settings

## 🚀 Execution Order

1. **First**: Run EF Core migrations to create base tables
   ```bash
   dotnet ef database update
   ```

2. **Then**: Execute scripts in order:
   ```sql
   -- 1. Create stored procedures
   SOURCE Database/Scripts/01_StoredProcedures.sql;
   
   -- 2. Create views
   SOURCE Database/Scripts/02_Views.sql;
   
   -- 3. Setup partitioning (optional but recommended)
   SOURCE Database/Scripts/03_LogPartitioning.sql;
   
   -- 4. Add extended sample data
   SOURCE Database/Scripts/04_ExtendedSeedData.sql;
   ```

## 📊 Database Schema Overview

```
BUYERS (4 records)
├── MODELGROUPS (14 records)
    ├── MODELS (19 records)
        ├── MODELPROCESSES (13 records)
            ├── LINES (12 records)
                ├── STATIONS (17 records)
                    ├── MACHINES (25+ records)
                        ├── LOGDATA (partitioned)
                        ├── COMMANDS
                        └── CLIENT_CONFIGS
```

## 🔧 Performance Features

### Indexes Created
- `idx_logdata_timestamp_machine` - For efficient log queries by time and machine
- `idx_logdata_level_timestamp` - For log level filtering
- `idx_commands_status_created` - For command queue operations
- `idx_machine_station` - For hierarchy queries

### Partitioning Benefits
- **Query Performance**: Partition pruning for date-range queries
- **Maintenance**: Easy cleanup of old data by dropping partitions
- **Storage**: Better disk I/O distribution
- **Scalability**: Handles millions of log records efficiently

### View Performance
- Pre-calculated aggregations for dashboard queries
- Optimized joins for hierarchy navigation
- Real-time status indicators

## 🏥 Health Monitoring

### Machine Status Indicators
- **Online**: Last log within 1 hour
- **Inactive**: Last log within 24 hours
- **Offline**: No logs in 24+ hours

### Health Status Indicators
- **Critical**: >10 errors in 24 hours
- **Warning**: 5-10 errors OR >20 warnings in 24 hours  
- **Normal**: Below warning thresholds

### Command Queue Monitoring
- Queue position for pending commands
- Execution time tracking
- Failed command analysis

## 📈 Usage Examples

### Dashboard Queries
```sql
-- Machine health overview
SELECT 
    BuyerName,
    COUNT(*) as TotalMachines,
    SUM(CASE WHEN ConnectionStatus = 'Online' THEN 1 ELSE 0 END) as OnlineMachines,
    SUM(CASE WHEN HealthStatus = 'Critical' THEN 1 ELSE 0 END) as CriticalMachines
FROM v_machine_status 
GROUP BY BuyerName;
```

### Maintenance Queries
```sql
-- Find machines needing attention
SELECT MachineName, MachineCode, HealthStatus, LastLogTime, ErrorLogs24h
FROM v_machine_status 
WHERE HealthStatus IN ('Critical', 'Warning')
ORDER BY ErrorLogs24h DESC;
```

### Production Monitoring
```sql
-- Production line status
SELECT 
    LineName,
    COUNT(DISTINCT MachineId) as MachineCount,
    AVG(CASE WHEN ConnectionStatus = 'Online' THEN 1 ELSE 0 END) * 100 as OnlinePercentage
FROM v_machine_status
GROUP BY LineName;
```

## 🔒 Security Notes

- All stored procedures include error handling and transaction management
- Input validation prevents SQL injection
- Partition maintenance runs with minimal privileges
- Sensitive configuration data can be encrypted using `IsEncrypted` flag in CLIENT_CONFIGS

## 📝 Maintenance

### Regular Tasks
1. **Daily**: Monitor partition sizes and performance
2. **Weekly**: Review error patterns using `v_error_analysis`
3. **Monthly**: Check partition creation and cleanup
4. **Quarterly**: Analyze storage usage and optimize indexes

### Performance Tuning
- Monitor slow query log for optimization opportunities
- Review partition effectiveness with `ShowPartitionInfo()`
- Update table statistics regularly
- Consider archiving very old log data

---

**Note**: These scripts are designed to work with the Entity Framework Core migrations and should be executed after the initial database schema is created.