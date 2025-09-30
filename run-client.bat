@echo off
cd /d "f:\Dev\Projects\C#\Project .NET\Machine Mangement System\src\ClientApp\MachineClient.WPF"
echo Starting WPF App...
dotnet run
echo App finished with exit code: %ERRORLEVEL%
pause