# Script tạo lại database HSE_PM_DB theo SRS Document Section 5
param(
    [string]$Server = "localhost",
    [string]$Username = "root", 
    [string]$Password = "Anduongb67",
    [string]$SqlFile = ""
)

# Tạo đường dẫn SQL file nếu không được cung cấp
if ([string]::IsNullOrEmpty($SqlFile)) {
    $SqlFile = Join-Path $PSScriptRoot "create-database-srs.sql"
}

Write-Host "Dang tao lai database HSE_PM_DB theo SRS Document..." -ForegroundColor Yellow
Write-Host "SQL Script: $SqlFile" -ForegroundColor Green

try {
    # Kiểm tra file SQL script
    if (!(Test-Path $SqlFile)) {
        throw "Không tìm thấy file SQL script: $SqlFile"
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
        throw "Không tìm thấy mysql.exe. Vui lòng cài đặt MySQL hoặc cập nhật đường dẫn."
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
    
    Write-Host "CANH BAO: Script nay se XOA va TAO LAI database HSE_PM_DB!" -ForegroundColor Red
    Write-Host "Bat dau thuc thi SQL script..." -ForegroundColor Yellow
    
    # Chạy mysql với SQL script
    $process = Start-Process -FilePath $mysqlPath -ArgumentList $arguments -RedirectStandardInput $SqlFile -RedirectStandardOutput "sql_output.log" -RedirectStandardError "sql_error.log" -Wait -PassThru -NoNewWindow
    
    if ($process.ExitCode -eq 0) {
        Write-Host "Database HSE_PM_DB da duoc tao thanh cong theo SRS!" -ForegroundColor Green
        
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
        
        Write-Host "Database structure da duoc tao theo SRS Document Section 5:" -ForegroundColor Green
        Write-Host "   + Tai su dung he thong co san (buyers, lines, stations, machines)" -ForegroundColor Green
        Write-Host "   + Them bang moi (log_data, commands, client_config)" -ForegroundColor Green
        Write-Host "   + Mo rong bang machines voi client tracking" -ForegroundColor Green
        Write-Host "   + Du lieu mau cho testing" -ForegroundColor Green
        Write-Host "   + Views ho tro query" -ForegroundColor Green
        
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
    Write-Host "Loi tao database: $($_.Exception.Message)" -ForegroundColor Red
    
    # Hiển thị lỗi chi tiết nếu có
    if (Test-Path "sql_error.log") {
        Write-Host "Chi tiet loi:" -ForegroundColor Yellow
        Get-Content "sql_error.log" | ForEach-Object { Write-Host "   $_" -ForegroundColor Red }
    }
    
    return $false
}