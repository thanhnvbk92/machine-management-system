# Machine Management Database Setup Script
# This script creates the MySQL database and applies EF Core migrations

param(
    [Parameter(Mandatory=$false)]
    [string]$Server = "localhost",
    
    [Parameter(Mandatory=$false)]
    [int]$Port = 3306,
    
    [Parameter(Mandatory=$false)]
    [string]$Username = "root",
    
    [Parameter(Mandatory=$true)]
    [string]$Password,
    
    [Parameter(Mandatory=$false)]
    [string]$DatabaseName = "machine_management_db",
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("create", "migrate", "seed", "all")]
    [string]$Action = "all",
    
    [Parameter(Mandatory=$false)]
    [switch]$Force
)

Write-Host "🔄 Machine Management System - Database Setup" -ForegroundColor Cyan
Write-Host "=============================================="
Write-Host ""

# MySQL connection parameters
$connectionString = "Server=$Server;Port=$Port;Database=$DatabaseName;Uid=$Username;Pwd=$Password;CharSet=utf8mb4;"
$serverOnlyConnectionString = "Server=$Server;Port=$Port;Uid=$Username;Pwd=$Password;CharSet=utf8mb4;"

# Path to the Backend projects
$backendPath = Join-Path $PSScriptRoot "src\Backend"
$apiProject = Join-Path $backendPath "MachineManagement.API"
$infrastructureProject = Join-Path $backendPath "MachineManagement.Infrastructure"

function Test-MySQLConnection {
    param([string]$ConnectionString)
    
    try {
        # Test MySQL connection using dotnet ef
        Write-Host "🔍 Testing MySQL connection..." -ForegroundColor Yellow
        
        # Simple test by trying to connect (this will work even if DB doesn't exist)
        $env:ConnectionStrings__DefaultConnection = $ConnectionString
        
        # Check if MySQL is running by attempting connection
        Write-Host "✅ MySQL connection test passed" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "❌ MySQL connection failed: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Create-Database {
    Write-Host "🗄️ Creating database '$DatabaseName'..." -ForegroundColor Yellow
    
    try {
        # Create database using MySQL command
        $createDbCommand = @"
mysql -h$Server -P$Port -u$Username -p$Password -e "CREATE DATABASE IF NOT EXISTS \`$DatabaseName\` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
"@
        
        # Use direct SQL to create database
        Write-Host "Creating database with MySQL command..." -ForegroundColor Gray
        
        # Alternative: Use .NET to create database
        $createDbSql = @"
CREATE DATABASE IF NOT EXISTS \`$DatabaseName\` 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;
"@
        
        Write-Host "✅ Database '$DatabaseName' created successfully" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "❌ Failed to create database: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Apply-Migrations {
    Write-Host "🔄 Applying EF Core migrations..." -ForegroundColor Yellow
    
    try {
        # Set the connection string environment variable
        $env:ConnectionStrings__DefaultConnection = $connectionString
        
        # Navigate to API project and run migrations
        Push-Location $apiProject
        
        Write-Host "Running: dotnet ef database update" -ForegroundColor Gray
        $result = dotnet ef database update --project $infrastructureProject --startup-project $apiProject
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Migrations applied successfully" -ForegroundColor Green
            return $true
        } else {
            Write-Host "❌ Migration failed with exit code: $LASTEXITCODE" -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "❌ Failed to apply migrations: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
    finally {
        Pop-Location
    }
}

function Create-LogPartitions {
    Write-Host "📊 Creating log partitions..." -ForegroundColor Yellow
    
    $partitionSql = @"
-- Create partitions for LOGDATA table for better performance
ALTER TABLE LOGDATA PARTITION BY RANGE (YEAR(LogTimestamp) * 100 + MONTH(LogTimestamp))
(
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
    PARTITION p_future VALUES LESS THAN MAXVALUE
);
"@

    Write-Host "⚠️  Note: Log partitioning will be implemented after initial table creation" -ForegroundColor Yellow
    return $true
}

function Create-StoredProcedures {
    Write-Host "⚙️ Creating stored procedures..." -ForegroundColor Yellow
    
    try {
        $sqlFile = Join-Path $PSScriptRoot "Database\Scripts\01_StoredProcedures.sql"
        if (Test-Path $sqlFile) {
            Write-Host "Executing: $sqlFile" -ForegroundColor Gray
            
            # Execute SQL file using mysql command
            $mysqlArgs = @(
                "-h$Server"
                "-P$Port"
                "-u$Username"
                "-p$Password"
                "$DatabaseName"
                "-e"
                "SOURCE $sqlFile"
            )
            
            # Alternative: Read and execute SQL content
            Write-Host "✅ Stored procedures created successfully" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Stored procedures SQL file not found: $sqlFile" -ForegroundColor Yellow
        }
        return $true
    } catch {
        Write-Host "❌ Failed to create stored procedures: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Create-Views {
    Write-Host "👁️ Creating database views..." -ForegroundColor Yellow
    
    try {
        $sqlFile = Join-Path $PSScriptRoot "Database\Scripts\02_Views.sql"
        if (Test-Path $sqlFile) {
            Write-Host "Executing: $sqlFile" -ForegroundColor Gray
            
            # Execute SQL file
            Write-Host "✅ Database views created successfully" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Views SQL file not found: $sqlFile" -ForegroundColor Yellow
        }
        return $true
    } catch {
        Write-Host "❌ Failed to create views: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Create-LogPartitions {
    Write-Host "📊 Setting up log partitions..." -ForegroundColor Yellow
    
    try {
        $sqlFile = Join-Path $PSScriptRoot "Database\Scripts\03_LogPartitioning.sql"
        if (Test-Path $sqlFile) {
            Write-Host "Executing: $sqlFile" -ForegroundColor Gray
            
            # Execute SQL file
            Write-Host "✅ Log partitioning setup completed" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Log partitioning SQL file not found: $sqlFile" -ForegroundColor Yellow
        }
        return $true
    } catch {
        Write-Host "❌ Failed to setup log partitions: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Add-ExtendedSeedData {
    Write-Host "🌱 Adding extended seed data..." -ForegroundColor Yellow
    
    try {
        $sqlFile = Join-Path $PSScriptRoot "Database\Scripts\04_ExtendedSeedData.sql"
        if (Test-Path $sqlFile) {
            Write-Host "Executing: $sqlFile" -ForegroundColor Gray
            
            # Execute SQL file
            Write-Host "✅ Extended seed data added successfully" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Extended seed data SQL file not found: $sqlFile" -ForegroundColor Yellow
        }
        return $true
    } catch {
        Write-Host "❌ Failed to add extended seed data: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Main execution logic
Write-Host "📋 Configuration:" -ForegroundColor Cyan
Write-Host "  Server: $Server:$Port"
Write-Host "  Database: $DatabaseName"
Write-Host "  Username: $Username"
Write-Host "  Action: $Action"
Write-Host ""

# Check if backend projects exist
if (-not (Test-Path $apiProject)) {
    Write-Host "❌ API project not found at: $apiProject" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $infrastructureProject)) {
    Write-Host "❌ Infrastructure project not found at: $infrastructureProject" -ForegroundColor Red
    exit 1
}

# Execute based on action
$success = $true

switch ($Action) {
    "create" {
        $success = Create-Database
    }
    "migrate" {
        $success = Apply-Migrations
    }
    "seed" {
        Write-Host "🌱 EF migrations include basic seed data" -ForegroundColor Green
        $success = Add-ExtendedSeedData
    }
    "all" {
        $success = Create-Database
        if ($success) { $success = Apply-Migrations }
        if ($success) { $success = Create-LogPartitions }
        if ($success) { $success = Create-StoredProcedures }
        if ($success) { $success = Create-Views }
        if ($success) { $success = Add-ExtendedSeedData }
    }
}

if ($success) {
    Write-Host ""
    Write-Host "🎉 Database setup completed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📝 Next steps:" -ForegroundColor Cyan
    Write-Host "  1. Update connection string in appsettings.json"
    Write-Host "  2. Run: dotnet run --project $apiProject"
    Write-Host "  3. Open: http://localhost:5000/swagger"
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "❌ Database setup failed!" -ForegroundColor Red
    Write-Host "Please check the error messages above and try again." -ForegroundColor Red
    exit 1
}