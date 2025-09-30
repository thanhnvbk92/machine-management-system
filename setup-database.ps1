# ===============================================
# HSE_PM_DB Database Setup Script
# Machine Management System
# ===============================================

param(
    [string]$Server = "localhost",
    [string]$Username = "root",
    [string]$Password = "",
    [int]$Port = 3306,
    [switch]$SkipSampleData
)

Write-Host "==> Machine Management System - Database Setup" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# Check if MySQL is available
Write-Host "Checking MySQL connection..." -ForegroundColor Yellow

$mysqlPath = ""
$possiblePaths = @(
    "mysql",
    "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe",
    "C:\Program Files\MySQL\MySQL Server 8.4\bin\mysql.exe",
    "C:\xampp\mysql\bin\mysql.exe",
    "C:\wamp64\bin\mysql\mysql8.0.39\bin\mysql.exe"
)

foreach ($path in $possiblePaths) {
    try {
        $result = & $path --version 2>$null
        if ($LASTEXITCODE -eq 0) {
            $mysqlPath = $path
            Write-Host "SUCCESS: Found MySQL at: $mysqlPath" -ForegroundColor Green
            break
        }
    }
    catch {
        continue
    }
}

if (-not $mysqlPath) {
    Write-Host "ERROR: MySQL not found. Please install MySQL or add it to PATH." -ForegroundColor Red
    Write-Host "   Download from: https://dev.mysql.com/downloads/mysql/" -ForegroundColor Yellow
    exit 1
}

# Get connection details if not provided
if (-not $Password) {
    $securePassword = Read-Host "Enter MySQL password for user '$Username'" -AsSecureString
    $Password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword))
}

Write-Host "Connecting to MySQL Server..." -ForegroundColor Yellow
Write-Host "   Server: $Server" -ForegroundColor Gray
Write-Host "   Port: $Port" -ForegroundColor Gray
Write-Host "   Username: $Username" -ForegroundColor Gray

# Test connection
$testConnection = @"
SELECT 'Connection successful!' as Status;
"@

try {
    $testResult = $testConnection | & $mysqlPath -h $Server -P $Port -u $Username --password=$Password 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Connection failed"
    }
    Write-Host "SUCCESS: Connected to MySQL successfully!" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: Failed to connect to MySQL:" -ForegroundColor Red
    Write-Host "   $testResult" -ForegroundColor Red
    exit 1
}

# Create backup of existing database (if exists)
Write-Host "Creating backup of existing database..." -ForegroundColor Yellow

$backupFile = "HSE_PM_DB_backup_$(Get-Date -Format 'yyyyMMdd_HHmmss').sql"
$checkDbQuery = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'HSE_PM_DB';"

try {
    $dbExists = $checkDbQuery | & $mysqlPath -h $Server -P $Port -u $Username --password=$Password -s -N 2>$null
    if ($dbExists -eq "HSE_PM_DB") {
        Write-Host "   Database HSE_PM_DB exists, creating backup..." -ForegroundColor Yellow
        & mysqldump -h $Server -P $Port -u $Username --password=$Password HSE_PM_DB > $backupFile 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "SUCCESS: Backup created: $backupFile" -ForegroundColor Green
        }
    } else {
        Write-Host "   No existing database found." -ForegroundColor Gray
    }
}
catch {
    Write-Host "WARNING: Could not create backup, continuing..." -ForegroundColor Yellow
}

# Run database setup script
Write-Host "Running database setup script..." -ForegroundColor Yellow

$scriptPath = Join-Path $PSScriptRoot "setup-database.sql"
if (-not (Test-Path $scriptPath)) {
    Write-Host "ERROR: Setup script not found: $scriptPath" -ForegroundColor Red
    exit 1
}

try {
    $result = Get-Content $scriptPath | & $mysqlPath -h $Server -P $Port -u $Username --password=$Password 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Setup script failed"
    }
    Write-Host "SUCCESS: Database setup completed successfully!" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: Database setup failed:" -ForegroundColor Red
    Write-Host "$result" -ForegroundColor Red
    exit 1
}

# Verify setup
Write-Host "Verifying database setup..." -ForegroundColor Yellow

$verifyQuery = @"
USE HSE_PM_DB;
SELECT 
    'Tables' as Type, COUNT(*) as Count 
FROM information_schema.tables 
WHERE table_schema = 'HSE_PM_DB'
UNION ALL
SELECT 
    'Views' as Type, COUNT(*) as Count 
FROM information_schema.views 
WHERE table_schema = 'HSE_PM_DB'
UNION ALL
SELECT 
    'Procedures' as Type, COUNT(*) as Count 
FROM information_schema.routines 
WHERE routine_schema = 'HSE_PM_DB';
"@

try {
    $verifyResult = $verifyQuery | & $mysqlPath -h $Server -P $Port -u $Username --password=$Password
    Write-Host "SUCCESS: Database verification:" -ForegroundColor Green
    Write-Host "$verifyResult" -ForegroundColor Gray
}
catch {
    Write-Host "WARNING: Could not verify setup" -ForegroundColor Yellow
}

# Show sample connection string
Write-Host ""
Write-Host "Connection String for .NET Applications:" -ForegroundColor Cyan
Write-Host "   Server=$Server;Database=HSE_PM_DB;Uid=$Username;Pwd=****;Port=$Port;" -ForegroundColor Yellow

# Show next steps
Write-Host ""
Write-Host "==> Next Steps:" -ForegroundColor Cyan
Write-Host "   1. Update appsettings.json with connection string" -ForegroundColor White
Write-Host "   2. Test connection from .NET application" -ForegroundColor White
Write-Host "   3. Run EF Core migrations (if using)" -ForegroundColor White
Write-Host "   4. Test log insertion and command execution" -ForegroundColor White

Write-Host ""
Write-Host "SUCCESS: HSE_PM_DB Database setup completed successfully!" -ForegroundColor Green
Write-Host "   Database: HSE_PM_DB" -ForegroundColor Gray
Write-Host "   Server: $Server" -ForegroundColor Gray
Write-Host "   Ready for Machine Management System!" -ForegroundColor Gray