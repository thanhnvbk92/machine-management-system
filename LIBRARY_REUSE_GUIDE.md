# FlaUI.Automation.Extensions - Library Reuse Guide

Hướng dẫn chi tiết cách sử dụng FlaUI.Automation.Extensions library trong nhiều projects khác nhau.

## 1. Tổng quan về Library

### Mục đích
FlaUI.Automation.Extensions được thiết kế để cung cấp:
- UI Automation capabilities cho Windows applications
- Reusable services và interfaces
- Easy integration với dependency injection
- Consistent API across multiple projects

### Architecture
```
FlaUI.Automation.Extensions/
├── Interfaces/
│   ├── IUIAutomationService.cs
│   ├── IElementMonitoringService.cs
│   └── IAutomationDemoService.cs
├── Services/
│   ├── UIAutomationService.cs
│   ├── ElementMonitoringService.cs
│   └── AutomationDemoService.cs
├── Extensions/
│   └── ServiceCollectionExtensions.cs
└── FlaUI.Automation.Extensions.csproj
```

### Core Features
- **UI Element Automation** - Click buttons, read text, interact with controls
- **Element Monitoring** - Real-time change detection
- **Application Connectivity** - Connect to running applications
- **Comprehensive Demo** - Built-in demonstration capabilities

## 2. Installation và Setup

### A. Từ Source Code (Development)

1. **Add Project Reference**:
```xml
<ProjectReference Include="..\path\to\FlaUI.Automation.Extensions\FlaUI.Automation.Extensions.csproj" />
```

2. **Register Services trong Startup**:
```csharp
using FlaUI.Automation.Extensions.Extensions;

// Program.cs hoặc App.xaml.cs
services.AddFlaUIAutomation();
```

### B. Từ NuGet Package (Production)

```powershell
Install-Package FlaUI.Automation.Extensions
# hoặc
dotnet add package FlaUI.Automation.Extensions
```

## 3. Usage Examples

### A. WPF Application

#### App.xaml.cs
```csharp
using Microsoft.Extensions.DependencyInjection;
using FlaUI.Automation.Extensions.Extensions;

public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        
        // Register FlaUI services
        services.AddFlaUIAutomation();
        
        // Register your ViewModels
        services.AddTransient<MainViewModel>();
        
        _serviceProvider = services.BuildServiceProvider();
        
        var mainWindow = new MainWindow
        {
            DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
        };
        mainWindow.Show();
        
        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
```

#### MainViewModel.cs
```csharp
using FlaUI.Automation.Extensions.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class MainViewModel : ObservableObject
{
    private readonly IUIAutomationService _automationService;
    private readonly IElementMonitoringService _monitoringService;
    private readonly IAutomationDemoService _demoService;

    public MainViewModel(
        IUIAutomationService automationService,
        IElementMonitoringService monitoringService,
        IAutomationDemoService demoService)
    {
        _automationService = automationService;
        _monitoringService = monitoringService;
        _demoService = demoService;
    }

    [RelayCommand]
    private async Task ConnectToApplicationAsync()
    {
        var connected = await _automationService.ConnectToApplicationAsync("Notepad");
        if (connected)
        {
            Status = "Connected to Notepad successfully!";
        }
    }

    [RelayCommand]
    private async Task StartMonitoringAsync()
    {
        await _monitoringService.StartMonitoringAsync("TextBlock", "Status");
        Status = "Monitoring started...";
    }

    [ObservableProperty]
    private string status = "Ready";
}
```

### B. Console Application

#### Program.cs
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FlaUI.Automation.Extensions.Extensions;
using FlaUI.Automation.Extensions.Interfaces;

class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Add FlaUI Automation services
                services.AddFlaUIAutomation();
                
                // Add hosted service
                services.AddHostedService<AutomationWorker>();
            })
            .Build();

        await host.RunAsync();
    }
}

public class AutomationWorker : BackgroundService
{
    private readonly IUIAutomationService _automationService;
    private readonly IElementMonitoringService _monitoringService;
    private readonly ILogger<AutomationWorker> _logger;

    public AutomationWorker(
        IUIAutomationService automationService,
        IElementMonitoringService monitoringService,
        ILogger<AutomationWorker> logger)
    {
        _automationService = automationService;
        _monitoringService = monitoringService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Automation worker started");

        // Connect to target application
        var connected = await _automationService.ConnectToApplicationAsync("Calculator");
        if (!connected)
        {
            _logger.LogError("Failed to connect to Calculator");
            return;
        }

        // Perform automation tasks
        await _automationService.ClickElementAsync("Button", "1");
        await Task.Delay(500, stoppingToken);
        
        await _automationService.ClickElementAsync("Button", "+");
        await Task.Delay(500, stoppingToken);
        
        await _automationService.ClickElementAsync("Button", "2");
        await Task.Delay(500, stoppingToken);
        
        await _automationService.ClickElementAsync("Button", "=");
        
        // Read result
        var result = await _automationService.ReadTextAsync("Text", "CalculatorResults");
        _logger.LogInformation($"Calculation result: {result}");
    }
}
```

### C. ASP.NET Core Web Application

#### Program.cs (ASP.NET Core)
```csharp
using FlaUI.Automation.Extensions.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddFlaUIAutomation();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();
app.MapControllers();

app.Run();
```

#### AutomationController.cs
```csharp
using Microsoft.AspNetCore.Mvc;
using FlaUI.Automation.Extensions.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AutomationController : ControllerBase
{
    private readonly IUIAutomationService _automationService;
    private readonly ILogger<AutomationController> _logger;

    public AutomationController(
        IUIAutomationService automationService,
        ILogger<AutomationController> logger)
    {
        _automationService = automationService;
        _logger = logger;
    }

    [HttpPost("connect/{applicationName}")]
    public async Task<IActionResult> ConnectToApplication(string applicationName)
    {
        try
        {
            var connected = await _automationService.ConnectToApplicationAsync(applicationName);
            return Ok(new { Connected = connected, Application = applicationName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to application {ApplicationName}", applicationName);
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("click")]
    public async Task<IActionResult> ClickElement([FromBody] ClickElementRequest request)
    {
        try
        {
            var result = await _automationService.ClickElementAsync(request.ElementType, request.ElementName);
            return Ok(new { Success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to click element {ElementType}:{ElementName}", 
                request.ElementType, request.ElementName);
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("read/{elementType}/{elementName}")]
    public async Task<IActionResult> ReadElement(string elementType, string elementName)
    {
        try
        {
            var text = await _automationService.ReadTextAsync(elementType, elementName);
            return Ok(new { Text = text });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read element {ElementType}:{ElementName}", 
                elementType, elementName);
            return BadRequest(new { Error = ex.Message });
        }
    }
}

public class ClickElementRequest
{
    public string ElementType { get; set; } = string.Empty;
    public string ElementName { get; set; } = string.Empty;
}
```

## 4. Advanced Usage Patterns

### A. Custom Service Implementation

```csharp
// Create your own service that uses FlaUI services
public interface IBusinessAutomationService
{
    Task<bool> PerformComplexWorkflowAsync();
}

public class BusinessAutomationService : IBusinessAutomationService
{
    private readonly IUIAutomationService _automationService;
    private readonly IElementMonitoringService _monitoringService;
    private readonly ILogger<BusinessAutomationService> _logger;

    public BusinessAutomationService(
        IUIAutomationService automationService,
        IElementMonitoringService monitoringService,
        ILogger<BusinessAutomationService> logger)
    {
        _automationService = automationService;
        _monitoringService = monitoringService;
        _logger = logger;
    }

    public async Task<bool> PerformComplexWorkflowAsync()
    {
        try
        {
            // Connect to application
            if (!await _automationService.ConnectToApplicationAsync("MyBusinessApp"))
            {
                return false;
            }

            // Start monitoring for changes
            await _monitoringService.StartMonitoringAsync("StatusBar", "Progress");

            // Perform business logic
            await _automationService.ClickElementAsync("Button", "StartProcess");
            await _automationService.SetTextAsync("TextBox", "InputField", "Business Data");
            await _automationService.ClickElementAsync("Button", "Submit");

            // Wait for completion
            await Task.Delay(5000);

            // Verify result
            var result = await _automationService.ReadTextAsync("Label", "Result");
            return result.Contains("Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Complex workflow failed");
            return false;
        }
    }
}

// Register in DI container
services.AddFlaUIAutomation();
services.AddScoped<IBusinessAutomationService, BusinessAutomationService>();
```

### B. Configuration-Based Automation

```csharp
// appsettings.json
{
  "Automation": {
    "TargetApplication": "Notepad",
    "DefaultTimeout": 5000,
    "RetryAttempts": 3
  }
}

// Configuration model
public class AutomationOptions
{
    public string TargetApplication { get; set; } = string.Empty;
    public int DefaultTimeout { get; set; } = 5000;
    public int RetryAttempts { get; set; } = 3;
}

// Configured service
public class ConfiguredAutomationService
{
    private readonly IUIAutomationService _automationService;
    private readonly AutomationOptions _options;
    private readonly ILogger<ConfiguredAutomationService> _logger;

    public ConfiguredAutomationService(
        IUIAutomationService automationService,
        IOptions<AutomationOptions> options,
        ILogger<ConfiguredAutomationService> logger)
    {
        _automationService = automationService;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> ConnectWithConfigAsync()
    {
        var attempts = 0;
        while (attempts < _options.RetryAttempts)
        {
            try
            {
                var result = await _automationService.ConnectToApplicationAsync(_options.TargetApplication);
                if (result)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Connection attempt {Attempt} failed", attempts + 1);
            }

            attempts++;
            await Task.Delay(_options.DefaultTimeout);
        }

        return false;
    }
}
```

## 5. Testing với Library

### A. Unit Testing với Mocked Services

```csharp
using Moq;
using Microsoft.Extensions.Logging;
using FlaUI.Automation.Extensions.Interfaces;

[TestClass]
public class BusinessAutomationServiceTests
{
    private Mock<IUIAutomationService> _mockAutomationService;
    private Mock<IElementMonitoringService> _mockMonitoringService;
    private Mock<ILogger<BusinessAutomationService>> _mockLogger;
    private BusinessAutomationService _service;

    [TestInitialize]
    public void Setup()
    {
        _mockAutomationService = new Mock<IUIAutomationService>();
        _mockMonitoringService = new Mock<IElementMonitoringService>();
        _mockLogger = new Mock<ILogger<BusinessAutomationService>>();
        
        _service = new BusinessAutomationService(
            _mockAutomationService.Object,
            _mockMonitoringService.Object,
            _mockLogger.Object);
    }

    [TestMethod]
    public async Task PerformComplexWorkflowAsync_WhenConnectionFails_ReturnsFalse()
    {
        // Arrange
        _mockAutomationService
            .Setup(x => x.ConnectToApplicationAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.PerformComplexWorkflowAsync();

        // Assert
        Assert.IsFalse(result);
        _mockAutomationService.Verify(x => x.ConnectToApplicationAsync("MyBusinessApp"), Times.Once);
    }

    [TestMethod]
    public async Task PerformComplexWorkflowAsync_WhenSuccessful_ReturnsTrue()
    {
        // Arrange
        _mockAutomationService
            .Setup(x => x.ConnectToApplicationAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _mockAutomationService
            .Setup(x => x.ReadTextAsync("Label", "Result"))
            .ReturnsAsync("Success: Operation completed");

        // Act
        var result = await _service.PerformComplexWorkflowAsync();

        // Assert
        Assert.IsTrue(result);
        _mockAutomationService.Verify(x => x.ClickElementAsync("Button", "StartProcess"), Times.Once);
        _mockAutomationService.Verify(x => x.SetTextAsync("TextBox", "InputField", "Business Data"), Times.Once);
    }
}
```

### B. Integration Testing

```csharp
[TestClass]
public class FlaUIIntegrationTests
{
    private ServiceProvider _serviceProvider;

    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddFlaUIAutomation();
        services.AddLogging(builder => builder.AddConsole());
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _serviceProvider?.Dispose();
    }

    [TestMethod]
    public async Task UIAutomationService_CanConnectToNotepad()
    {
        // Arrange
        var automationService = _serviceProvider.GetRequiredService<IUIAutomationService>();
        
        // Start Notepad for testing
        Process.Start("notepad.exe");
        await Task.Delay(2000); // Wait for startup

        try
        {
            // Act
            var connected = await automationService.ConnectToApplicationAsync("Notepad");

            // Assert
            Assert.IsTrue(connected, "Should be able to connect to Notepad");
        }
        finally
        {
            // Cleanup - close Notepad
            var notepadProcesses = Process.GetProcessesByName("notepad");
            foreach (var process in notepadProcesses)
            {
                process.CloseMainWindow();
                process.WaitForExit(2000);
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
        }
    }
}
```

## 6. Deployment Strategies

### A. Single Library Approach

```
📦 FlaUI.Automation.Extensions.nupkg
├── All projects reference same version
├── Centralized updates
└── Consistent API across projects
```

### B. Versioned Libraries

```
📦 FlaUI.Automation.Extensions.Core.1.0.0.nupkg
📦 FlaUI.Automation.Extensions.WPF.1.0.0.nupkg
📦 FlaUI.Automation.Extensions.Web.1.0.0.nupkg
```

### C. Monorepo Structure

```
MachineManagementSystem/
├── src/
│   ├── Libraries/
│   │   └── FlaUI.Automation.Extensions/
│   ├── Applications/
│   │   ├── WPF.Client/
│   │   ├── Web.Dashboard/
│   │   └── Windows.Service/
│   └── Tools/
│       └── Automation.Console/
└── MachineManagementSystem.sln
```

## 7. Lợi ích của Library Approach

✅ **Code Reusability** - Viết 1 lần, sử dụng nhiều nơi  
✅ **Centralized Maintenance** - Bug fixes và improvements ở 1 chỗ  
✅ **Consistent API** - Cùng interface cho tất cả projects  
✅ **Easy Testing** - Mock interfaces dễ dàng  
✅ **Versioning** - Control updates across projects  
✅ **Documentation** - Tập trung documentation  
✅ **Team Collaboration** - Multiple teams có thể sử dụng  
✅ **Performance** - Optimizations benefit all consumers  

## 8. Migration Guide

Để migrate existing UI automation code sang library:

1. **Extract Interfaces** - Tạo interfaces từ existing services
2. **Move Implementations** - Di chuyển implementations vào library project
3. **Update Dependencies** - Thay direct references bằng library reference
4. **Register Services** - Sử dụng extension methods cho DI registration
5. **Update Usages** - Cập nhật code để sử dụng interfaces từ library
6. **Test Integration** - Verify functionality không thay đổi

Với approach này, UI Automation system trở thành một **reusable asset** có thể được sử dụng trong bất kỳ .NET project nào trong organization!