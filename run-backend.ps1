# Backend API Run Script
# This script starts the Machine Management Backend API

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Development", "Production", "Staging")]
    [string]$Environment = "Development",
    
    [Parameter(Mandatory=$false)]
    [string]$Port = "5000",
    
    [Parameter(Mandatory=$false)]
    [string]$Urls = "",
    
    [Parameter(Mandatory=$false)]
    [switch]$Build = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$Watch = $false
)

Write-Host "🚀 Machine Management System - Backend API" -ForegroundColor Cyan
Write-Host "==========================================="
Write-Host ""

# Set environment variables
$env:ASPNETCORE_ENVIRONMENT = $Environment
if ($Urls) {
    $env:ASPNETCORE_URLS = $Urls
} else {
    $env:ASPNETCORE_URLS = "http://localhost:$Port;https://localhost:$([int]$Port + 1)"
}

# API project path
$apiProject = Join-Path $PSScriptRoot "src\Backend\MachineManagement.API"

# Check if API project exists
if (-not (Test-Path $apiProject)) {
    Write-Host "❌ API project not found at: $apiProject" -ForegroundColor Red
    Write-Host "Please ensure the project structure is correct." -ForegroundColor Yellow
    exit 1
}

Write-Host "📋 Configuration:" -ForegroundColor Cyan
Write-Host "  Environment: $Environment"
Write-Host "  URLs: $($env:ASPNETCORE_URLS)"
Write-Host "  Project: $apiProject"
Write-Host ""

# Build if requested
if ($Build) {
    Write-Host "🔨 Building backend API..." -ForegroundColor Yellow
    Push-Location $apiProject
    $buildResult = dotnet build --configuration Release
    Pop-Location
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Build failed!" -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ Build completed successfully" -ForegroundColor Green
    Write-Host ""
}

# Start the API
Write-Host "🌐 Starting Backend API..." -ForegroundColor Yellow
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Gray
Write-Host ""

Push-Location $apiProject

try {
    if ($Watch) {
        Write-Host "👀 Running in watch mode (auto-restart on changes)..." -ForegroundColor Green
        dotnet watch run --environment $Environment
    } else {
        dotnet run --environment $Environment
    }
} catch {
    Write-Host "❌ Failed to start API: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    Pop-Location
}

Write-Host ""
Write-Host "👋 Backend API stopped" -ForegroundColor Gray