# DATABASE NAME CHANGES - HSE_PM_DB

## ✅ ĐÃ THAY ĐỔI THÀNH CÔNG

Tên database đã được thay đổi từ `machine_mgmt_db`/`hse_pm_database` thành **`HSE_PM_DB`** trong tất cả các file:

### 📁 Files đã cập nhật:

#### 1. **README.md**
- ✅ Diagram database name: `machine_mgmt_db` → `HSE_PM_DB`

#### 2. **CI/CD Workflows**
- ✅ `.github/workflows/ci-cd.yml`:
  - Test database: `machine_management_test` → `HSE_PM_DB_test`
  - Staging database: `machine_management_staging` → `HSE_PM_DB_staging`
  - Connection string updated
  
- ✅ `.github/workflows/release.yml`:
  - Production database: `machine_management_prod` → `HSE_PM_DB_prod`
  - Volume name updated

#### 3. **Database Design Documentation**
- ✅ `SRS_Documents/06_Database_Design/database_simple.md`:
  - Database overview: `hse_pm_database` → `HSE_PM_DB`
  - USE statements: `USE hse_pm_database;` → `USE HSE_PM_DB;`
  - Backup commands: database name updated
  - Restore commands: database name updated

## 🗄️ DATABASE ENVIRONMENTS

Sau khi thay đổi, hệ thống sẽ sử dụng:

```yaml
Development:   HSE_PM_DB_dev
Testing:       HSE_PM_DB_test  
Staging:       HSE_PM_DB_staging
Production:    HSE_PM_DB_prod
```

## 🔧 NEXT STEPS

### 1. Cập nhật Connection Strings
Cần tạo/cập nhật `appsettings.json` files với connection strings mới:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=10.7.12.236;Database=HSE_PM_DB;Uid=username;Pwd=password;Port=3306;",
    "LoggingConnection": "Server=10.7.12.236;Database=HSE_PM_DB;Uid=username;Pwd=password;Port=3306;"
  }
}
```

### 2. Backend API Configuration
Cần tạo:
- `src/Backend/MachineManagement.API/appsettings.json`
- `src/Backend/MachineManagement.API/appsettings.Development.json`
- `src/Backend/MachineManagement.API/appsettings.Production.json`

### 3. Database Migration
Khi deploy, cần chạy:
```sql
-- Tạo database mới (nếu chưa có)
CREATE DATABASE HSE_PM_DB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Hoặc rename database hiện tại
RENAME DATABASE hse_pm_database TO HSE_PM_DB;
```

### 4. Update Application Code
Kiểm tra và cập nhật bất kỳ hard-coded database names nào trong:
- Entity Framework configurations
- Repository implementations
- Service classes
- Test files

## ✨ SUMMARY

**Database name đã được chuẩn hóa thành `HSE_PM_DB`** để:
- ✅ Phù hợp với tên project
- ✅ Dễ nhận diện trong production environment  
- ✅ Consistent across all environments
- ✅ Follow naming conventions

Tất cả files configuration và documentation đã được cập nhật để reflect tên database mới.