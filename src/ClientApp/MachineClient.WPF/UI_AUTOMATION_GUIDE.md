# UI Automation Integration Guide for MachineClient.WPF

This document explains how UI Automation has been integrated into the MachineClient.WPF application using the FlaUI.Automation.Extensions library.

## Overview

The WPF application now includes comprehensive UI automation capabilities that allow:
- External applications to control the WPF interface
- Automated testing and monitoring
- Cross-application integration
- Real-time element monitoring

## Integration Details

### 1. Library Reference

The project references the FlaUI.Automation.Extensions library:

```xml
<ProjectReference Include="..\..\Libraries\FlaUI.Automation.Extensions\FlaUI.Automation.Extensions.csproj" />
```

### 2. Service Registration (App.xaml.cs)

```csharp
// UI Automation Services (from library)
services.AddUIAutomation();
```

This registers all automation services:
- `IUIAutomationService` - Core automation operations
- `IElementMonitoringService` - Real-time element monitoring
- `IAutomationDemoService` - Comprehensive demo capabilities

### 3. ViewModel Integration (MainViewModel.cs)

```csharp
public partial class MainViewModel : ObservableObject
{
    private readonly IUIAutomationService _uiAutomationService;
    private readonly IElementMonitoringService _elementMonitoringService;
    private readonly IAutomationDemoService _automationDemoService;

    public MainViewModel(
        /* other services */,
        IUIAutomationService uiAutomationService,
        IElementMonitoringService elementMonitoringService,
        IAutomationDemoService automationDemoService)
    {
        _uiAutomationService = uiAutomationService;
        _elementMonitoringService = elementMonitoringService;
        _automationDemoService = automationDemoService;
        
        // Subscribe to automation events
        _automationDemoService.DemoProgress += OnAutomationDemoProgress;
    }
}
```

### 4. AutomationId Properties

All interactive UI elements have AutomationId set for reliable automation:

```xml
<!-- Buttons -->
<Button AutomationProperties.AutomationId="StartBackupButton" />
<Button AutomationProperties.AutomationId="StopBackupButton" />
<Button AutomationProperties.AutomationId="DemoAllFeaturesButton" />

<!-- Text Elements -->
<TextBlock AutomationProperties.AutomationId="MachineIdTextBlock" />
<TextBlock AutomationProperties.AutomationId="BackupStatusTextBlock" />
<TextBlock AutomationProperties.AutomationId="BackupProgressTextBlock" />

<!-- Progress Elements -->
<ProgressBar AutomationProperties.AutomationId="BackupProgressBar" />
```

## Available Automation Commands

### Button Automation
```csharp
// Click start backup button
var clicked = await _uiAutomationService.ClickButtonAsync("StartBackupButton");

// Click stop backup button
var stopped = await _uiAutomationService.ClickButtonAsync("StopBackupButton");

// Run demo features
var demo = await _uiAutomationService.ClickButtonAsync("DemoAllFeaturesButton");
```

### Text Reading
```csharp
// Read machine information
var machineId = await _uiAutomationService.ReadTextAsync("MachineIdTextBlock");
var status = await _uiAutomationService.ReadTextAsync("BackupStatusTextBlock");
var progress = await _uiAutomationService.ReadTextAsync("BackupProgressTextBlock");
```

### Element Monitoring
```csharp
// Start monitoring backup status changes
_elementMonitoringService.ElementChanged += (sender, e) => {
    Console.WriteLine($"Status changed: {e.NewValue}");
};

await _elementMonitoringService.StartMonitoringAsync("BackupStatusTextBlock");
```

### Demo Features
```csharp
// Run comprehensive demo
await _automationDemoService.InitializeAndTestAsync();

// Get debug information
var debugInfo = await _automationDemoService.GetDebugInfoAsync();
```

## External Integration Examples

### Console Application
See `UIAutomationConsoleDemo` project for complete example of controlling the WPF app from a console application.

### PowerShell Integration
```powershell
# Example PowerShell script to interact with the WPF app
# (Would require .NET interop setup)
```

### Web API Integration
```csharp
// Example: Web API controller that controls WPF app
[HttpPost("backup/start")]
public async Task<IActionResult> StartBackup()
{
    var success = await _uiAutomationService.ClickButtonAsync("StartBackupButton");
    return Ok(new { Success = success });
}
```

## Testing Integration

### Unit Testing
```csharp
[Test]
public async Task CanClickStartBackupButton()
{
    // Arrange
    var mockAutomation = new Mock<IUIAutomationService>();
    mockAutomation.Setup(x => x.ClickButtonAsync("StartBackupButton", true))
              .ReturnsAsync(true);
    
    // Act
    var result = await mockAutomation.Object.ClickButtonAsync("StartBackupButton");
    
    // Assert
    Assert.IsTrue(result);
}
```

### Integration Testing
```csharp
[Test]
public async Task CanReadMachineId()
{
    // Arrange
    await _automation.InitializeAsync("MachineClient.WPF");
    
    // Act
    var machineId = await _automation.ReadTextAsync("MachineIdTextBlock");
    
    // Assert
    Assert.IsNotEmpty(machineId);
}
```

## Best Practices

1. **Use AutomationId**: Always set AutomationId for elements that need automation
2. **Error Handling**: Check return values from automation methods
3. **Initialization**: Initialize automation service once per session
4. **Resource Cleanup**: Dispose services properly when done
5. **Thread Safety**: All automation operations are thread-safe

## Troubleshooting

### Element Not Found
- Verify AutomationId is set correctly
- Check element is visible and loaded
- Try using Name fallback: `useAutomationId: false`

### Connection Issues
- Ensure WPF application is running
- Check process name matches exactly
- Verify UI automation is initialized

### Performance Issues
- Reduce monitoring intervals if needed
- Limit concurrent automation operations
- Use specific element identifiers

## Architecture Benefits

✅ **Separation of Concerns** - Automation logic in separate library  
✅ **Reusable Components** - Library can be used in any .NET project  
✅ **Testable Code** - Easy to mock interfaces for testing  
✅ **Maintainable** - Single place for automation logic updates  
✅ **Extensible** - Easy to add new automation capabilities  
✅ **Cross-Project Usage** - Same library works for console, web, desktop apps  

This integration makes the WPF application fully controllable from external applications while maintaining clean separation between UI and automation concerns.