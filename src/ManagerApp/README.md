# Machine Management - Manager Web Application

## 🌐 Overview
Blazor Server web application for centralized machine management dashboard with real-time monitoring, command execution, and analytics.

## ✨ Features

### Dashboard
- **Real-time KPI Cards**: Total machines, online/offline status, pending commands
- **System Health Monitoring**: Database, API, and SignalR connection status
- **Activity Timeline**: Recent machine activities and command executions
- **Alert System**: Active alerts and notifications

### Machine Management
- **Machine Grid View**: Card-based responsive machine display
- **Search & Filtering**: Find machines by name, code, or description
- **Machine Details**: Comprehensive view with logs, commands, and stats
- **Status Monitoring**: Real-time status updates via SignalR

### Command Center
- **Command Creation**: Send commands to individual or multiple machines
- **Command History**: Track all executed commands with status
- **Batch Operations**: Execute commands across multiple machines
- **Priority Management**: Set command execution priority

### Real-time Features
- **Live Updates**: Machine status changes via SignalR
- **Log Streaming**: Real-time log entries display
- **Command Tracking**: Live command execution status
- **Notifications**: Instant alerts and system notifications

## 🛠️ Technical Stack

- **Framework**: ASP.NET Core Blazor Server (.NET 8)
- **UI Library**: MudBlazor (Material Design)
- **Real-time**: SignalR Core
- **Database**: Entity Framework Core (MySQL)
- **Architecture**: Clean Architecture with Repository pattern

## 📁 Project Structure

```
src/ManagerApp/
├── Components/Pages/          # Blazor pages/components
│   ├── Dashboard.razor        # Main dashboard
│   ├── MachineList.razor      # Machine grid view
│   └── MachineDetails.razor   # Machine details page
├── Hubs/                      # SignalR hubs
│   ├── MachineHub.cs         # Machine status updates
│   ├── LogHub.cs             # Log streaming
│   ├── CommandHub.cs         # Command tracking
│   └── NotificationHub.cs    # Alerts & notifications
├── Services/                  # Business services
│   ├── IMachineService.cs    # Machine management
│   ├── ILogService.cs        # Log operations
│   ├── ICommandService.cs    # Command operations
│   └── IDashboardService.cs  # Dashboard metrics
├── Shared/                    # Shared components
│   ├── MainLayout.razor      # Main layout
│   └── NavMenu.razor         # Navigation menu
└── wwwroot/                   # Static assets
    ├── css/site.css          # Custom styles
    └── js/app.js             # JavaScript utilities
```

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- MySQL Server
- Node.js (for npm packages)

### Setup & Run
1. **Configure Database**:
   ```json
   // appsettings.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=machine_management_db;Uid=root;Pwd=your_password;Port=3306;"
     }
   }
   ```

2. **Build & Run**:
   ```bash
   cd src/ManagerApp
   dotnet restore
   dotnet build
   dotnet run
   ```

3. **Access Application**:
   - Open browser to `https://localhost:5001`
   - Dashboard loads automatically

## 🔧 Configuration

### SignalR Settings
```json
{
  "SignalR": {
    "ConnectionTimeout": "00:01:00",
    "KeepAliveInterval": "00:00:15"
  }
}
```

### Dashboard Settings
```json
{
  "Dashboard": {
    "RefreshIntervalSeconds": 30,
    "MaxMachinesPerPage": 50,
    "MaxLogEntriesPerPage": 100
  }
}
```

## 📱 UI Components

### Navigation Structure
- **Dashboard**: Main overview với KPIs và activity
- **Machines**: Machine management và monitoring
  - Machine List: Grid view với search/filter
  - Machine Details: Chi tiết từng machine
  - Add Machine: Thêm machine mới
- **Commands**: Command center và history
  - Command History: Lịch sử commands
  - Create Command: Tạo command mới
  - Batch Commands: Thao tác hàng loạt
- **Logs**: Log viewing và analysis
  - Log Viewer: Xem và search logs
  - Log Analytics: Thống kê và analysis
- **Reports**: Reporting và analytics
- **Administration**: User và system management

### Material Design Theme
- Primary Color: `#1976d2` (Blue)
- Responsive design cho mobile/tablet
- Dark/Light theme support
- Consistent iconography với Material Icons

## 🔄 Real-time Features

### SignalR Hubs

#### MachineHub (`/machineHub`)
- `MachineStatusUpdated(machineId, status)`
- `MachineHeartbeatReceived(machineId, timestamp)`
- `MachineMetricsUpdated(metrics)`

#### LogHub (`/logHub`)
- `NewLogEntry(logEntry)`
- Join/Leave groups: `Machine_{id}_Logs`, `LogLevel_{level}`

#### CommandHub (`/commandHub`)
- `CommandStatusUpdated(commandId, status, result)`
- `CommandCreated(command)`
- `BatchCommandProgress(batchId, completed, total)`

#### NotificationHub (`/notificationHub`)
- `AlertReceived(alert)`
- `SystemNotification(notification)`
- `DashboardUpdated(dashboardData)`

## 📊 API Integration

### Services Layer
```csharp
// Dependency injection setup trong Program.cs
builder.Services.AddScoped<IMachineService, MachineService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<ICommandService, CommandService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
```

### Repository Pattern
- `IUnitOfWork` và `UnitOfWork` implementation
- Generic `IRepository<T>` cho data access
- Clean separation of concerns

## 🎨 Styling

### Custom CSS Classes
```css
/* Machine status styling */
.machine-card.online { border-left: 4px solid #4caf50; }
.machine-card.offline { border-left: 4px solid #ff9800; }
.machine-card.error { border-left: 4px solid #f44336; }

/* Log level styling */
.log-entry.error { border-left: 3px solid #f44336; background: #ffebee; }
.log-entry.warning { border-left: 3px solid #ff9800; background: #fff8e1; }
.log-entry.info { border-left: 3px solid #2196f3; }
```

## 📈 Performance

### Optimization Features
- Server-side rendering với Blazor Server
- SignalR connection pooling
- Pagination cho large datasets
- Lazy loading cho components
- Efficient state management

### Monitoring
- Real-time connection status
- Performance metrics tracking
- Error logging với Serilog
- Health checks integration

## 🚀 Deployment

### Development
```bash
dotnet run --environment Development
```

### Production
```bash
dotnet publish -c Release
# Deploy to IIS, Azure, or Docker container
```

### Docker Support
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY bin/Release/net8.0/publish/ .
ENTRYPOINT ["dotnet", "MachineManagement.ManagerApp.dll"]
```

## 🔐 Security

### Authentication & Authorization
- ASP.NET Core Identity integration (planned)
- Role-based access control
- JWT token support
- Session management

### Data Protection
- HTTPS enforcement
- Input validation
- XSS protection
- CSRF protection

---

## 📝 Development Notes

### Current Status
- ✅ Core foundation implemented
- ✅ Dashboard với real-time features
- ✅ Machine management interface  
- ✅ SignalR hubs configured
- ✅ Material Design theme
- 🔄 Additional pages in development

### Next Steps
- [ ] Complete Log Viewer page
- [ ] Command History implementation
- [ ] Charts và analytics
- [ ] Authentication system
- [ ] Export functionality
- [ ] Mobile responsiveness improvements

### Known Issues
- Some advanced pages có syntax issues (being fixed)
- DateRange picker cần configuration
- Chart integration cần thêm libraries

---

Built with ❤️ using Blazor Server và MudBlazor