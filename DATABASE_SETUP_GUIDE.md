# 🗄️ HSE_PM_DB Database Setup Guide

## 📋 Prerequisites

### 1. MySQL Server
- **MySQL 8.0+** hoặc **MariaDB 10.5+**
- Có thể sử dụng XAMPP, WAMP, hoặc standalone MySQL

### 2. Database Access
- Username/Password với quyền CREATE DATABASE
- Thường là `root` user

## 🚀 Quick Setup

### Option 1: PowerShell Script (Recommended)
```powershell
# Chạy script tự động
.\setup-database.ps1

# Hoặc với parameters tùy chỉnh
.\setup-database.ps1 -Server "10.7.12.236" -Username "root" -Password "your_password" -Port 3306
```

### Option 2: Manual SQL Execution
```bash
# Sử dụng MySQL command line
mysql -u root -p < setup-database.sql

# Hoặc với remote server
mysql -h 10.7.12.236 -u root -p < setup-database.sql
```

### Option 3: MySQL Workbench
1. Mở MySQL Workbench
2. Kết nối đến server
3. Mở file `setup-database.sql`
4. Execute toàn bộ script

## 📊 Database Structure

Sau khi setup, database `HSE_PM_DB` sẽ có:

### 🔧 Core Tables:
- **`machines`** - Thông tin máy móc (từ hệ thống cũ + mở rộng)
- **`stations`** - Thông tin stations
- **`lines`** - Thông tin production lines

### 📝 New Tables (Log Collection):
- **`log_data`** - Lưu trữ logs từ machines
- **`commands`** - Remote commands cho machines
- **`client_config`** - Configuration cho client apps

### 📈 Views:
- **`v_machine_status`** - Tổng hợp trạng thái machines
- **`v_recent_logs`** - Logs gần đây (24h)

### ⚙️ Stored Procedures:
- **`sp_insert_log`** - Insert log entry
- **`sp_update_machine_status`** - Update machine status
- **`sp_get_pending_commands`** - Get pending commands

## 🔗 Connection Strings

### Development
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HSE_PM_DB;Uid=root;Pwd=your_password;Port=3306;"
  }
}
```

### Production
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=10.7.12.236;Database=HSE_PM_DB;Uid=hse_app_user;Pwd=secure_password;Port=3306;"
  }
}
```

## 🧪 Testing Database

### 1. Verify Tables
```sql
USE HSE_PM_DB;
SHOW TABLES;
```

### 2. Check Sample Data
```sql
SELECT * FROM machines;
SELECT * FROM v_machine_status;
```

### 3. Test Log Insertion
```sql
CALL sp_insert_log(1, 1, 'INFO', 'Test log message', 'TestApp', NULL, NOW());
SELECT * FROM log_data WHERE machine_id = 1;
```

### 4. Test Command Creation
```sql
INSERT INTO commands (machine_id, command_type, program_name, parameters)
VALUES (1, 'START_PROGRAM', 'TestProgram.exe', '{"param1": "value1"}');

CALL sp_get_pending_commands(1);
```

## 🔧 Configuration

### Application User (Production)
Để tăng bảo mật, tạo user riêng cho application:

```sql
-- Chạy với quyền root
CREATE USER 'hse_app_user'@'%' IDENTIFIED BY 'secure_password_here';
GRANT SELECT, INSERT, UPDATE, DELETE ON HSE_PM_DB.* TO 'hse_app_user'@'%';
GRANT EXECUTE ON HSE_PM_DB.* TO 'hse_app_user'@'%';
FLUSH PRIVILEGES;
```

### Index Optimization
Database đã được tối ưu với indexes cho:
- Frequent queries trên `machine_id`, `log_time`
- Command status và priority
- Machine status tracking

## 🔄 Migration từ Database Cũ

Nếu bạn có database `hse_pm_database` hiện tại:

### 1. Backup Database Cũ
```bash
mysqldump -h 10.7.12.236 -u root -p hse_pm_database > hse_pm_database_backup.sql
```

### 2. Migrate Data
```sql
-- Import dữ liệu từ tables hiện có
INSERT INTO HSE_PM_DB.machines (ID, Name, status, IP, StationID, ...)
SELECT ID, Name, status, IP, StationID, ... FROM hse_pm_database.machines;

INSERT INTO HSE_PM_DB.stations (ID, StationName, LineID, ...)
SELECT ID, StationName, LineID, ... FROM hse_pm_database.stations;
```

## 🐛 Troubleshooting

### Connection Issues
```bash
# Test MySQL connection
mysql -h localhost -u root -p -e "SELECT 'Connected!' as Status;"
```

### Permission Issues
```sql
-- Check user permissions
SHOW GRANTS FOR 'root'@'localhost';
```

### Port Issues
```bash
# Check if MySQL is running on port 3306
netstat -an | findstr 3306
```

## 📁 Files Created

- `setup-database.sql` - Main database setup script
- `setup-database.ps1` - PowerShell automation script
- `DATABASE_SETUP_GUIDE.md` - This guide

## 🎯 Next Steps

1. ✅ **Setup Database** - Run setup script
2. 🔧 **Update appsettings.json** - Add connection strings
3. 🧪 **Test Connection** - Verify from .NET app
4. 📝 **Create EF Models** - Generate Entity Framework models
5. 🚀 **Deploy Application** - Run Machine Management System

---

**Database HSE_PM_DB ready for Machine Management System!** 🎉