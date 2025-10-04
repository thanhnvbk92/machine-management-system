@echo off
echo Starting Machine Manager WPF Application...
cd /d "f:\Dev\Projects\Big-Project\machine-management-system\src\ManagerApp\WPF"
dotnet build
if %ERRORLEVEL% EQU 0 (
    echo Build successful. Starting application...
    start "" ".\bin\Debug\net8.0-windows\MachineManagerApp.exe"
) else (
    echo Build failed!
    pause
)