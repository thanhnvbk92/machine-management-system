# FlaUI.Automation.Extensions

A comprehensive UI Automation library built on top of FlaUI, providing easy-to-use services for automated UI testing and interaction in WPF applications.

## Features

- **Core UI Automation**: Button clicking, text reading, form interaction
- **Element Monitoring**: Real-time monitoring of UI element changes
- **Demo Services**: Comprehensive demonstration of all features
- **Dependency Injection**: Easy integration with .NET DI container
- **Logging**: Built-in logging support with Microsoft.Extensions.Logging
- **Thread-Safe**: Proper handling of UI thread operations

## Quick Start

### 1. Installation

```xml
<PackageReference Include="FlaUI.Automation.Extensions" Version="1.0.0" />
```

### 2. Service Registration

```csharp
// In your Program.cs or Startup.cs
services.AddUIAutomation(); // Add all services

// Or add specific services
services.AddUIAutomationCore(); // Core only
services.AddUIAutomationWithMonitoring(); // Core + Monitoring
services.AddUIAutomationWithDemo(); // All services
```

### 3. Basic Usage

```csharp
public class MyService
{
    private readonly IUIAutomationService _automation;
    
    public MyService(IUIAutomationService automation)
    {
        _automation = automation;
    }
    
    public async Task DoAutomationAsync()
    {
        // Initialize (usually done once)
        await _automation.InitializeAsync("MyApp");
        
        // Click a button
        await _automation.ClickButtonAsync("MyButtonId");
        
        // Read text from element
        var text = await _automation.ReadTextAsync("MyTextBlockId");
        
        // Set TextBox value
        await _automation.SetTextBoxValueAsync("MyTextBoxId", "Hello World");
        
        // Monitor element changes
        await _automation.StartTextBlockMonitoringAsync("StatusTextId", 
            newText => Console.WriteLine($"Status changed: {newText}"));
    }
}
```

## Services Overview

### IUIAutomationService
Core service providing fundamental automation operations:
- Button clicking
- Text reading/writing
- ComboBox interaction
- Menu navigation
- Element monitoring

### IElementMonitoringService
Specialized service for monitoring UI element changes:
- Real-time value monitoring
- Event-driven notifications
- Configurable polling intervals

### IAutomationDemoService
Demonstration service showcasing all features:
- Comprehensive feature testing
- Progress reporting
- Debug information

## Element Identification

Elements can be identified by AutomationId or Name:

```xml
<!-- Recommended: Use AutomationId -->
<Button AutomationProperties.AutomationId="MyButtonId"
        AutomationProperties.Name="My Button"
        Content="Click Me"/>
```

```csharp
// Find by AutomationId (recommended)
await automation.ClickButtonAsync("MyButtonId", useAutomationId: true);

// Find by Name (fallback)
await automation.ClickButtonAsync("My Button", useAutomationId: false);
```

## Advanced Usage

### Element Monitoring

```csharp
// Subscribe to element changes
monitoringService.ElementChanged += (sender, e) => {
    Console.WriteLine($"Element {e.ElementIdentifier} changed from '{e.PreviousValue}' to '{e.NewValue}'");
};

// Start monitoring
await monitoringService.StartMonitoringAsync("StatusTextId");

// Stop monitoring
await monitoringService.StopMonitoringAsync();
```

### Demo Service

```csharp
// Subscribe to demo progress
demoService.DemoProgress += (sender, e) => {
    Console.WriteLine($"{e.Timestamp}: {e.Message}");
};

// Run comprehensive demo
await demoService.InitializeAndTestAsync("MyApp");

// Get debug information
var debugInfo = await demoService.GetDebugInfoAsync();
```

## Error Handling

All services include comprehensive error handling and logging:

```csharp
try
{
    var success = await automation.ClickButtonAsync("MyButton");
    if (!success)
    {
        // Button not found or not clickable
        logger.LogWarning("Button operation failed");
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "Automation operation failed");
}
```

## Best Practices

1. **Initialize Once**: Call `InitializeAsync()` once per application session
2. **Use AutomationIds**: Always prefer AutomationId over Name for element identification
3. **Handle Errors**: Check return values and handle exceptions appropriately
4. **Dispose Resources**: Ensure services are properly disposed when done
5. **Thread Safety**: All operations are designed to be thread-safe
6. **Logging**: Use the built-in logging for debugging and monitoring

## Configuration

### Monitoring Intervals

```csharp
// Custom monitoring interval (default: 500ms)
await monitoringService.StartMonitoringAsync("ElementId", useAutomationId: true, intervalMs: 1000);
```

### Application Targeting

```csharp
// Target by application title
await automation.InitializeAsync("MyApplication");

// Target by process ID
await automation.InitializeAsync(processId: 1234);

// Auto-detect current process
await automation.InitializeAsync();
```

## Troubleshooting

### Element Not Found
- Verify AutomationId is set correctly
- Check if element is visible and loaded
- Try using Name instead of AutomationId
- Use debug info to inspect available elements

### Performance Issues
- Increase monitoring intervals for less frequent updates
- Optimize element finding by using specific identifiers
- Limit concurrent monitoring operations

### Threading Issues
- All operations are designed for UI thread safety
- Events are raised on appropriate threads
- Use async/await patterns consistently

## Sample Projects

Check the source repository for complete sample applications demonstrating:
- WPF application integration
- Console automation tools
- Unit testing with automation
- Advanced monitoring scenarios

## License

This library is part of the Machine Management System project and follows the same licensing terms.

## Contributing

Contributions are welcome! Please follow the coding standards and include appropriate tests for new features.