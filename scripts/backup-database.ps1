# Script backup database HSE_PM_DB trước khi rebuild theo SRS
param(
    [string]$Server = "localhost",
    [string]$Database = "HSE_PM_DB",
    [string]$Username = "root",
    [string]$Password = "Anduongb67",
    [string]$BackupPath = ""
)

# Tạo đường dẫn backup nếu không được cung cấp
if ([string]::IsNullOrEmpty($BackupPath)) {
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $BackupPath = Join-Path $PSScriptRoot "..\backup_HSE_PM_DB_before_srs_$timestamp.sql"
}

Write-Host "Đang backup database $Database..." -ForegroundColor Yellow
Write-Host "Backup file: $BackupPath" -ForegroundColor Green

try {
    # Tìm đường dẫn MySQL
    $mysqlPaths = @(
        "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe",
        "C:\Program Files\MySQL\MySQL Server 8.4\bin\mysqldump.exe", 
        "C:\MySQL\bin\mysqldump.exe",
        "C:\xampp\mysql\bin\mysqldump.exe"
    )
    
    $mysqldumpPath = $null
    foreach ($path in $mysqlPaths) {
        if (Test-Path $path) {
            $mysqldumpPath = $path
            break
        }
    }
    
    if ($null -eq $mysqldumpPath) {
        throw "Không tìm thấy mysqldump.exe. Vui lòng cài đặt MySQL hoặc cập nhật đường dẫn."
    }
    
    Write-Host "Sử dụng mysqldump: $mysqldumpPath" -ForegroundColor Cyan
    
    # Tạo thư mục backup nếu chưa có
    $backupDir = Split-Path $BackupPath -Parent
    if (!(Test-Path $backupDir)) {
        New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
    }
    
    # Chạy mysqldump với connection string an toàn
    $arguments = @(
        "--host=$Server",
        "--user=$Username", 
        "--password=$Password",
        "--single-transaction",
        "--routines",
        "--triggers",
        "--databases",
        $Database
    )
    
    & $mysqldumpPath $arguments | Out-File -FilePath $BackupPath -Encoding UTF8
    
    if ($LASTEXITCODE -eq 0) {
        $fileSize = (Get-Item $BackupPath).Length / 1KB
        Write-Host "✅ Backup thành công! File size: $([math]::Round($fileSize, 2)) KB" -ForegroundColor Green
        Write-Host "📁 Backup saved to: $BackupPath" -ForegroundColor Green
        return $true
    } else {
        throw "mysqldump failed with exit code: $LASTEXITCODE"
    }
}
catch {
    Write-Host "❌ Lỗi backup database: $($_.Exception.Message)" -ForegroundColor Red
    return $false
}