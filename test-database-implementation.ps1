# Database Implementation Test Script
# This script tests the database setup and verifies all components

param(
    [Parameter(Mandatory=$false)]
    [string]$Server = "localhost",
    
    [Parameter(Mandatory=$false)]
    [int]$Port = 3306,
    
    [Parameter(Mandatory=$false)]
    [string]$Username = "root",
    
    [Parameter(Mandatory=$false)]
    [string]$Password = "password",
    
    [Parameter(Mandatory=$false)]
    [string]$DatabaseName = "machine_management_db_test"
)

Write-Host "🧪 Database Implementation Test Suite" -ForegroundColor Cyan
Write-Host "====================================="
Write-Host ""

$connectionString = "Server=$Server;Port=$Port;Database=$DatabaseName;Uid=$Username;Pwd=$Password;CharSet=utf8mb4;"

function Test-EFMigrations {
    Write-Host "🔍 Testing EF Core Migrations..." -ForegroundColor Yellow
    
    try {
        $apiProject = Join-Path $PSScriptRoot "src\Backend\MachineManagement.API"
        $infrastructureProject = Join-Path $PSScriptRoot "src\Backend\MachineManagement.Infrastructure"
        
        if (-not (Test-Path $apiProject)) {
            Write-Host "❌ API project not found" -ForegroundColor Red
            return $false
        }
        
        if (-not (Test-Path $infrastructureProject)) {
            Write-Host "❌ Infrastructure project not found" -ForegroundColor Red
            return $false
        }
        
        # Check if migrations exist
        $migrationsPath = Join-Path $infrastructureProject "Migrations"
        if (-not (Test-Path $migrationsPath)) {
            Write-Host "❌ Migrations directory not found" -ForegroundColor Red
            return $false
        }
        
        $migrationFiles = Get-ChildItem $migrationsPath -Filter "*.cs" | Where-Object { $_.Name -like "*InitialCreate*" }
        if ($migrationFiles.Count -eq 0) {
            Write-Host "❌ No initial migration found" -ForegroundColor Red
            return $false
        }
        
        Write-Host "✅ EF Core migrations are properly configured" -ForegroundColor Green
        Write-Host "   - Migration files found: $($migrationFiles.Count)" -ForegroundColor Gray
        return $true
    }
    catch {
        Write-Host "❌ EF Migrations test failed: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Test-ProjectStructure {
    Write-Host "🏗️ Testing Project Structure..." -ForegroundColor Yellow
    
    $requiredPaths = @(
        "src\Backend\MachineManagement.Core\MachineManagement.Core.csproj",
        "src\Backend\MachineManagement.Infrastructure\MachineManagement.Infrastructure.csproj", 
        "src\Backend\MachineManagement.API\MachineManagement.API.csproj",
        "src\Backend\MachineManagement.Core\Entities\Machine.cs",
        "src\Backend\MachineManagement.Infrastructure\Data\ApplicationDbContext.cs",
        "src\Backend\MachineManagement.API\Program.cs"
    )
    
    $missing = @()
    foreach ($path in $requiredPaths) {
        $fullPath = Join-Path $PSScriptRoot $path
        if (-not (Test-Path $fullPath)) {
            $missing += $path
        }
    }
    
    if ($missing.Count -gt 0) {
        Write-Host "❌ Missing required files:" -ForegroundColor Red
        foreach ($file in $missing) {
            Write-Host "   - $file" -ForegroundColor Red
        }
        return $false
    }
    
    Write-Host "✅ All required project files are present" -ForegroundColor Green
    return $true
}

function Test-DatabaseScripts {
    Write-Host "📜 Testing Database Scripts..." -ForegroundColor Yellow
    
    $requiredScripts = @(
        "Database\Scripts\01_StoredProcedures.sql",
        "Database\Scripts\02_Views.sql", 
        "Database\Scripts\03_LogPartitioning.sql",
        "Database\Scripts\04_ExtendedSeedData.sql"
    )
    
    $missing = @()
    foreach ($script in $requiredScripts) {
        $fullPath = Join-Path $PSScriptRoot $script
        if (-not (Test-Path $fullPath)) {
            $missing += $script
        }
    }
    
    if ($missing.Count -gt 0) {
        Write-Host "❌ Missing SQL scripts:" -ForegroundColor Red
        foreach ($script in $missing) {
            Write-Host "   - $script" -ForegroundColor Red
        }
        return $false
    }
    
    # Check script content
    $storedProcScript = Join-Path $PSScriptRoot "Database\Scripts\01_StoredProcedures.sql"
    $content = Get-Content $storedProcScript -Raw
    
    $expectedProcedures = @("GetMachineHierarchy", "GetLogsByDateRange", "CleanupOldLogs", "GetMachineStatistics")
    $foundProcedures = 0
    
    foreach ($proc in $expectedProcedures) {
        if ($content -like "*$proc*") {
            $foundProcedures++
        }
    }
    
    if ($foundProcedures -ne $expectedProcedures.Count) {
        Write-Host "❌ Not all stored procedures found in SQL script" -ForegroundColor Red
        return $false
    }
    
    Write-Host "✅ All database scripts are present and valid" -ForegroundColor Green
    Write-Host "   - Found $foundProcedures/$($expectedProcedures.Count) expected stored procedures" -ForegroundColor Gray
    return $true
}

function Test-EntityModels {
    Write-Host "🎭 Testing Entity Models..." -ForegroundColor Yellow
    
    $entityPath = Join-Path $PSScriptRoot "src\Backend\MachineManagement.Core\Entities"
    
    $requiredEntities = @(
        "BaseEntity.cs",
        "Machine.cs", 
        "LogData.cs",
        "Command.cs",
        "ClientConfig.cs"
    )
    
    $missing = @()
    foreach ($entity in $requiredEntities) {
        $fullPath = Join-Path $entityPath $entity
        if (-not (Test-Path $fullPath)) {
            $missing += $entity
        }
    }
    
    if ($missing.Count -gt 0) {
        Write-Host "❌ Missing entity files:" -ForegroundColor Red
        foreach ($entity in $missing) {
            Write-Host "   - $entity" -ForegroundColor Red
        }
        return $false
    }
    
    # Check Machine.cs contains hierarchical entities
    $machineFile = Join-Path $entityPath "Machine.cs"
    $content = Get-Content $machineFile -Raw
    
    $expectedEntities = @("Buyer", "ModelGroup", "Model", "ModelProcess", "Line", "Station", "Machine")
    $foundEntities = 0
    
    foreach ($entity in $expectedEntities) {
        if ($content -like "*class $entity*") {
            $foundEntities++
        }
    }
    
    if ($foundEntities -ne $expectedEntities.Count) {
        Write-Host "❌ Not all hierarchical entities found" -ForegroundColor Red
        return $false
    }
    
    Write-Host "✅ All entity models are properly configured" -ForegroundColor Green
    Write-Host "   - Found $foundEntities/$($expectedEntities.Count) hierarchical entities" -ForegroundColor Gray
    return $true
}

function Test-ProjectBuild {
    Write-Host "🔨 Testing Project Build..." -ForegroundColor Yellow
    
    try {
        # Test Core project build
        $coreProject = Join-Path $PSScriptRoot "src\Backend\MachineManagement.Core"
        Push-Location $coreProject
        $result = dotnet build --verbosity quiet
        Pop-Location
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "❌ Core project build failed" -ForegroundColor Red
            return $false
        }
        
        # Test Infrastructure project build
        $infraProject = Join-Path $PSScriptRoot "src\Backend\MachineManagement.Infrastructure"
        Push-Location $infraProject
        $result = dotnet build --verbosity quiet
        Pop-Location
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "❌ Infrastructure project build failed" -ForegroundColor Red
            return $false
        }
        
        # Test API project build
        $apiProject = Join-Path $PSScriptRoot "src\Backend\MachineManagement.API"
        Push-Location $apiProject
        $result = dotnet build --verbosity quiet
        Pop-Location
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "❌ API project build failed" -ForegroundColor Red
            return $false
        }
        
        Write-Host "✅ All projects build successfully" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "❌ Build test failed: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Test-SetupScripts {
    Write-Host "⚙️ Testing Setup Scripts..." -ForegroundColor Yellow
    
    $setupScript = Join-Path $PSScriptRoot "setup-database.ps1"
    $runScript = Join-Path $PSScriptRoot "run-backend.ps1"
    
    if (-not (Test-Path $setupScript)) {
        Write-Host "❌ setup-database.ps1 not found" -ForegroundColor Red
        return $false
    }
    
    if (-not (Test-Path $runScript)) {
        Write-Host "❌ run-backend.ps1 not found" -ForegroundColor Red
        return $false
    }
    
    # Check setup script functions
    $content = Get-Content $setupScript -Raw
    $expectedFunctions = @("Create-Database", "Apply-Migrations", "Create-StoredProcedures", "Create-Views")
    $foundFunctions = 0
    
    foreach ($func in $expectedFunctions) {
        if ($content -like "*function $func*") {
            $foundFunctions++
        }
    }
    
    if ($foundFunctions -ne $expectedFunctions.Count) {
        Write-Host "❌ Not all setup functions found" -ForegroundColor Red
        return $false
    }
    
    Write-Host "✅ Setup scripts are properly configured" -ForegroundColor Green
    Write-Host "   - Found $foundFunctions/$($expectedFunctions.Count) setup functions" -ForegroundColor Gray
    return $true
}

# Run all tests
Write-Host "Running comprehensive database implementation tests..." -ForegroundColor Cyan
Write-Host ""

$tests = @(
    @{ Name = "Project Structure"; Function = { Test-ProjectStructure } },
    @{ Name = "Entity Models"; Function = { Test-EntityModels } },
    @{ Name = "EF Migrations"; Function = { Test-EFMigrations } },
    @{ Name = "Database Scripts"; Function = { Test-DatabaseScripts } },
    @{ Name = "Setup Scripts"; Function = { Test-SetupScripts } },
    @{ Name = "Project Build"; Function = { Test-ProjectBuild } }
)

$passed = 0
$failed = 0

foreach ($test in $tests) {
    $result = & $test.Function
    if ($result) {
        $passed++
    } else {
        $failed++
    }
    Write-Host ""
}

# Summary
Write-Host "📊 Test Results Summary" -ForegroundColor Cyan
Write-Host "======================"
Write-Host "✅ Passed: $passed" -ForegroundColor Green
Write-Host "❌ Failed: $failed" -ForegroundColor Red
Write-Host "📈 Success Rate: $([math]::Round(($passed / ($passed + $failed)) * 100, 1))%" -ForegroundColor Cyan
Write-Host ""

if ($failed -eq 0) {
    Write-Host "🎉 All tests passed! Database implementation is ready." -ForegroundColor Green
    Write-Host ""
    Write-Host "📝 Next steps:" -ForegroundColor Cyan
    Write-Host "  1. Setup MySQL server"
    Write-Host "  2. Run: .\setup-database.ps1 -Password 'your_password'"
    Write-Host "  3. Run: .\run-backend.ps1"
    Write-Host ""
} else {
    Write-Host "⚠️ Some tests failed. Please fix the issues above." -ForegroundColor Yellow
    exit 1
}