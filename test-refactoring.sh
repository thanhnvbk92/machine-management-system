#!/bin/bash

echo "=== Testing Clean Architecture Refactoring ==="
echo "Building MachineClient.WPF project..."

# Navigate to project directory
cd "F:\Dev\Projects\C#\Project .NET\Machine Mangement System\src\ClientApp\MachineClient.WPF"

# Build the project
dotnet build --configuration Debug --verbosity normal

if [ $? -eq 0 ]; then
    echo "✅ Build successful!"
    echo ""
    echo "=== Project Structure Validation ==="
    echo "Services created:"
    ls -la Services/ | grep -E "(IMachineConnectionService|IBackupManager|IUIStateManager|IApplicationSettingsService)"
    
    echo ""
    echo "ViewModels:"
    ls -la ViewModels/ | grep -E "(CleanMainViewModel|MainViewModel)"
    
    echo ""
    echo "=== Next Steps ==="
    echo "1. Fix any compilation errors"
    echo "2. Test MAC update workflow"
    echo "3. Test backup functionality"
    echo "4. Test settings persistence"
    echo "5. Validate UI state management"
else
    echo "❌ Build failed. Check compilation errors above."
    exit 1
fi