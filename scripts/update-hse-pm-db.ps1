# Script cập nhật database hse_pm_db với cấu trúc từ hse_pm_database + 4 bảng mới
param(
    [string]$Server = "localhost",
    [string]$Username = "root", 
    [string]$Password = "Anduongb67",
    [string]$SqlFile = ""
)

# Tạo đường dẫn SQL file nếu không được cung cấp
if ([string]::IsNullOrEmpty($SqlFile)) {
    $SqlFile = Join-Path $PSScriptRoot "update-hse-pm-db.sql"
}

Write-Host "Cap nhat database hse_pm_db voi cau truc tu hse_pm_database..." -ForegroundColor Yellow
Write-Host "SQL Script: $SqlFile" -ForegroundColor Green

try {
    # Kiểm tra file SQL script
    if (!(Test-Path $SqlFile)) {
        throw "Khong tim thay file SQL script: $SqlFile"
    }
    
    # Tìm đường dẫn MySQL
    $mysqlPaths = @(
        "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe",
        "C:\Program Files\MySQL\MySQL Server 8.4\bin\mysql.exe",
        "C:\MySQL\bin\mysql.exe", 
        "C:\xampp\mysql\bin\mysql.exe"
    )
    
    $mysqlPath = $null
    foreach ($path in $mysqlPaths) {
        if (Test-Path $path) {
            $mysqlPath = $path
            break
        }
    }
    
    if ($null -eq $mysqlPath) {
        throw "Khong tim thay mysql.exe. Vui long cai dat MySQL hoac cap nhat duong dan."
    }
    
    Write-Host "Su dung MySQL: $mysqlPath" -ForegroundColor Cyan
    
    # Tạo arguments cho mysql command
    $arguments = @(
        "--host=$Server",
        "--user=$Username",
        "--password=$Password",
        "--force",
        "--verbose"
    )
    
    Write-Host "CANH BAO: Script nay se XOA va TAO LAI database hse_pm_db!" -ForegroundColor Red
    Write-Host "Bat dau thuc thi SQL script..." -ForegroundColor Yellow
    
    # Chạy mysql với SQL script
    $process = Start-Process -FilePath $mysqlPath -ArgumentList $arguments -RedirectStandardInput $SqlFile -RedirectStandardOutput "sql_output.log" -RedirectStandardError "sql_error.log" -Wait -PassThru -NoNewWindow
    
    if ($process.ExitCode -eq 0) {
        Write-Host "Database hse_pm_db da duoc cap nhat thanh cong!" -ForegroundColor Green
        
        # Hiển thị thống kê
        Write-Host "Kiem tra ket qua:" -ForegroundColor Cyan
        if (Test-Path "sql_output.log") {
            $output = Get-Content "sql_output.log" -Tail 10
            $output | ForEach-Object { 
                if ($_ -like "*count*" -or $_ -like "*message*") {
                    Write-Host "   $_" -ForegroundColor White
                }
            }
        }
        
        Write-Host "Database structure da duoc cap nhat:" -ForegroundColor Green
        Write-Host "   + Sao chep cau truc tu hse_pm_database" -ForegroundColor Green
        Write-Host "   + Them bang machines voi client tracking" -ForegroundColor Green
        Write-Host "   + Them 4 bang moi: log_file, log_data, commands, client_config" -ForegroundColor Green
        Write-Host "   + Sao chep du lieu tu database goc" -ForegroundColor Green
        Write-Host "   + Tao views ho tro query" -ForegroundColor Green
        
        # Cleanup log files
        if (Test-Path "sql_output.log") { Remove-Item "sql_output.log" -Force }
        if (Test-Path "sql_error.log") { Remove-Item "sql_error.log" -Force }
        
        return $true
    } else {
        $errorContent = ""
        if (Test-Path "sql_error.log") {
            $errorContent = Get-Content "sql_error.log" -Raw
        }
        throw "MySQL failed with exit code: $($process.ExitCode). Error: $errorContent"
    }
}
catch {
    Write-Host "Loi cap nhat database: $($_.Exception.Message)" -ForegroundColor Red
    
    # Hiển thị lỗi chi tiết nếu có
    if (Test-Path "sql_error.log") {
        Write-Host "Chi tiet loi:" -ForegroundColor Yellow
        Get-Content "sql_error.log" | ForEach-Object { Write-Host "   $_" -ForegroundColor Red }
    }
    
    return $false
}