# Machine Management System

> **Hệ thống quản lý máy móc sản xuất với thu thập log và điều khiển từ xa**

## 🚀 Tổng quan hệ thống

**✅ BACKEND API HOÀN THÀNH - READY FOR TESTING**
**✅ WPF CLIENT APP HOÀN THÀNH - WITH UI AUTOMATION LIBRARY**
**✅ FLAUI AUTOMATION LIBRARY HOÀN THÀNH - REUSABLE ACROSS PROJECTS**

### Mô tả kiến trúc:
```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   CLIENT APP    │    │   BACKEND API    │    │  MANAGER WEB    │
│   (WPF Desktop) │◄──►│  (.NET 8 Core)   │◄──►│ (Blazor Server) │
│        ✅       │    │       ✅         │    │   (Coming)      │
│ - Log Collection│    │ - REST APIs      │    │ - Dashboard     │
│ - Command Exec  │    │ - Business Logic │    │ - Management    │
│ - Config Sync   │    │ - Data Access    │    │ - Reporting     │
│ - UI Automation │    │ - Health Checks  │    │ - Real-time     │
│ - Backup System │    │ - Swagger UI     │    │   Monitoring    │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                                │
                                ▼
                       ┌──────────────────┐
                       │   MySQL DATABASE │
                       │ machine_mgmt_db  │
                       │       ✅         │
                       └──────────────────┘
                                │
                                ▼
                    ┌─────────────────────────┐
                    │ FLAUI AUTOMATION LIB    │
                    │      ✅ REUSABLE        │
                    │ - Button Clicking       │
                    │ - Text Reading          │
                    │ - Element Monitoring    │
                    │ - Cross-Project Support │
                    └─────────────────────────┘
```

### 4 thành phần chính:

#### 1. **✅ Backend API** (.NET 8 Core Web API) - **HOÀN THÀNH**
   - **RESTful APIs**: Machine management, log collection, command execution
   - **Clean Architecture**: Repository pattern, service layer, dependency injection
   - **Database**: MySQL với EF Core, hierarchical production structure
   - **Features**: Swagger UI, health checks, structured logging, CORS
   - **Endpoints**: 
     - `/api/machines` - Quản lý machines (CRUD, heartbeat, registration)
     - `/api/logs` - Thu thập và truy vấn logs (batch upload, filtering)
   - **Tech Stack**: ASP.NET Core 8, EF Core, MySQL, Serilog, AutoMapper

#### 2. **✅ Client App** (Windows Desktop - WPF) - **HOÀN THÀNH**
   - **Material Design 3 UI**: Modern, beautiful interface với MaterialDesignThemes
   - **Machine Info Management**: Real-time machine status và configuration
   - **Log Collection**: Tự động đọc và upload log files
   - **Backup System**: Advanced backup với FTP, scheduling, date filtering
   - **UI Automation**: Integration với FlaUI library cho automated testing
   - **Pin Count Monitoring**: Real-time pin usage tracking với warnings
   - **Tech Stack**: WPF, MVVM, CommunityToolkit.Mvvm, Material Design, FluentFTP, FlaUI

#### 3. **✅ FlaUI Automation Library** - **HOÀN THÀNH & REUSABLE**
   - **Core Automation**: Button clicking, text reading, form interaction
   - **Element Monitoring**: Real-time UI element change detection
   - **Demo Services**: Comprehensive automation demonstrations
   - **Cross-Project Support**: Reusable trong WPF, Console, Web, Windows Service
   - **Dependency Injection**: Easy integration với Microsoft DI container
   - **Tech Stack**: FlaUI.Core, FlaUI.UIA3, Microsoft.Extensions.*

#### 4. **🌐 Manager Web** (Blazor Server) - **PLANNED**
   - Dashboard xem clients và log data
   - Gửi lệnh điều khiển từ xa
   - **Tech Stack**: Blazor Server, SignalR
   - Quản lý cấu hình hệ thống

## 🎯 Features đã hoàn thành

### WPF Client App:
- ✅ **Modern UI**: Material Design 3 với navigation improvements
- ✅ **Machine Information**: 3-column compact layout với real-time updates
- ✅ **Pin Count Table**: DataGrid với status indicators và reset functionality
- ✅ **Backup System**: FTP integration với progress tracking
- ✅ **Advanced Backup**: Time scheduling, date filtering, next backup calculation
- ✅ **UI Automation Demo**: Complete integration với FlaUI library
- ✅ **Element Monitoring**: Real-time tracking của UI element changes

### FlaUI Automation Library:
- ✅ **IUIAutomationService**: Core automation với button clicking, text reading
- ✅ **IElementMonitoringService**: Real-time element monitoring với events
- ✅ **IAutomationDemoService**: Comprehensive demo với progress reporting
- ✅ **ServiceCollectionExtensions**: Easy DI registration
- ✅ **Cross-Project Compatible**: Console, WPF, Web applications
- ✅ **NuGet Package Ready**: Configured để distribute

## 🚀 Quick Start

### Prerequisites:
- .NET 8 SDK
- MySQL Server
- Visual Studio 2022 hoặc VS Code

### 1. Clone Repository:
```bash
git clone https://github.com/thanhnvbk92/machine-management-system.git
cd machine-management-system
```

### 2. Database Setup:
```bash
# Run setup script
.\setup-database.ps1
```

### 3. Run Backend API:
```bash
# Start API server
.\run-backend.ps1
```

### 4. Run WPF Client:
```bash
cd src\ClientApp\MachineClient.WPF
dotnet run
```

### 5. Test UI Automation:
```bash
cd src\Demos\UIAutomationConsoleDemo
dotnet run
```

## 📁 Project Structure

```
src/
├── Libraries/
│   └── FlaUI.Automation.Extensions/     # 🔥 Reusable UI Automation Library
├── ClientApp/
│   └── MachineClient.WPF/               # 🖥️ WPF Desktop Application
├── Demos/
│   └── UIAutomationConsoleDemo/         # 🔧 Console Demo Project
├── Backend/                             # 🌐 API Server (separate repo)
└── ManagerApp/                          # 📊 Web Dashboard (planned)
```

## 🔧 Technology Stack

### Frontend (WPF):
- **.NET 8** - Latest framework
- **WPF** - Windows desktop UI
- **MaterialDesignThemes 5.1.0** - Modern Material Design 3
- **CommunityToolkit.Mvvm** - MVVM framework
- **FluentFTP** - FTP client cho backup
- **FlaUI** - UI Automation framework

### Library (FlaUI.Automation.Extensions):
- **FlaUI.Core & FlaUI.UIA3** - Core automation
- **Microsoft.Extensions.*** - DI, Logging, Hosting
- **Cross-Platform Compatible** - Windows applications

### Backend (API):
- **ASP.NET Core 8** - Web API framework
- **Entity Framework Core** - ORM
- **MySQL** - Database
- **Serilog** - Structured logging
- **AutoMapper** - Object mapping
- **Swagger** - API documentation

## 📚 Documentation

- **[Library Reuse Guide](LIBRARY_REUSE_GUIDE.md)** - How to use FlaUI library in other projects
- **[UI Automation Guide](src/ClientApp/MachineClient.WPF/UI_AUTOMATION_GUIDE.md)** - Complete UI automation documentation
- **[SRS Documents](SRS_Documents/)** - System requirements và specifications
- **[GitHub Instructions](.github/copilot-instructions.md)** - Development guidelines

## 🔄 CI/CD

- **GitHub Actions**: Automated build, test, và deployment
- **Multi-Project Build**: Libraries và applications
- **NuGet Package**: Automated library packaging
- **Quality Gates**: Code analysis và testing

## 🎯 Roadmap

### Phase 1: ✅ COMPLETED
- [x] Backend API với MySQL
- [x] WPF Client với Material Design
- [x] FlaUI Automation Library
- [x] Basic UI Automation integration
- [x] Backup system với FTP
- [x] Advanced backup scheduling

### Phase 2: 🔄 IN PROGRESS
- [ ] Blazor Manager Web App
- [ ] Real-time SignalR communication
- [ ] Advanced reporting dashboard
- [ ] NuGet package distribution

### Phase 3: 📋 PLANNED
- [ ] Multi-language support
- [ ] Advanced analytics
- [ ] Mobile companion app
- [ ] Cloud deployment options

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Thanh Nguyen**
- GitHub: [@thanhnvbk92](https://github.com/thanhnvbk92)
- Email: thanhnvbk92@gmail.com

---

⭐ **Star this repo if you find it helpful!** ⭐