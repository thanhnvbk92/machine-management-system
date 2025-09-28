# Machine Management System

> Hệ thống quản lý máy móc với Desktop Client (WPF), Web Manager (Blazor) và Backend API (ASP.NET Core)

## 🚀 Tổng quan hệ thống

### Mô tả ngắn gọn:
```
[Client Apps]  →  Gửi Log  →  [Server API]  ←  Xem & Điều khiển  ←  [Manager Web]
      ↓                         ↓                                      ↓
  [Log Files]              [Database]                           [Commands] 
  [App khác]                                                        
```

### 3 thành phần chính:
1. **Client App** (Windows Desktop - WPF):
   - Tự động đọc log files từ máy production
   - Gửi data lên server API
   - Nhận lệnh và điều khiển ứng dụng khác
   - Sử dụng: WPF, MVVM, Material Design

2. **Backend API** (.NET Core Web API):
   - Nhận log từ clients
   - Lưu vào MySQL database  
   - Quản lý commands và client registry
   - Sử dụng: ASP.NET Core, EF Core, Repository Pattern

3. **Manager Web** (Blazor Server):
   - Dashboard xem clients và log data
   - Gửi lệnh điều khiển từ xa
   - Quản lý cấu hình hệ thống
   - Sử dụng: Blazor Server, Material Design

## 📁 Cấu trúc dự án

```
📦 machine-management-system/
├── 📁 .github/                    # GitHub Actions workflows & instructions
├── 📁 SRS_Documents/              # Software Requirements Specification
├── 📁 src/
│   ├── 📁 ClientApp/              # WPF Desktop Client
│   ├── 📁 Backend/                # ASP.NET Core Web API  
│   ├── 📁 ManagerApp/             # Blazor Server Web App
│   └── 📁 Database/               # SQL Scripts & Migrations
├── 📁 docs/                       # Technical documentation
├── 📁 tests/                      # Unit & Integration tests
└── 📄 README.md
```

## 🛠️ Công nghệ sử dụng

### Backend & API
- **.NET 8** - Framework chính
- **ASP.NET Core Web API** - REST API services
- **Entity Framework Core** - ORM và database migrations
- **MySQL** - Database chính
- **Serilog** - Logging framework
- **AutoMapper** - DTO mapping

### Desktop Client
- **WPF** - Windows Presentation Foundation
- **MVVM Pattern** - với CommunityToolkit.Mvvm
- **Material Design** - MaterialDesignInXamlToolkit
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection

### Web Manager
- **Blazor Server** - Server-side Blazor
- **MudBlazor** - Material Design UI components
- **SignalR** - Real-time communication

### DevOps
- **GitHub Actions** - CI/CD pipeline
- **Docker** - Containerization
- **xUnit** - Unit testing framework

## 📋 Yêu cầu hệ thống

### Development
- Visual Studio 2022 hoặc VS Code
- .NET 8 SDK
- MySQL Server 8.0+
- Git

### Production
- Windows Server 2019+ (cho Client Apps)
- Linux/Windows Server (cho Backend API)
- MySQL Server 8.0+
- IIS hoặc Docker

## 🚀 Bắt đầu phát triển

### 1. Clone repository
```bash
git clone https://github.com/thanhnvbk92/machine-management-system.git
cd machine-management-system
```

### 2. Setup Database
```bash
# Tạo database từ script
mysql -u root -p < src/Database/init_database.sql

# Hoặc dùng EF migrations
cd src/Backend
dotnet ef database update
```

### 3. Chạy Backend API
```bash
cd src/Backend
dotnet run
```

### 4. Chạy Manager Web
```bash
cd src/ManagerApp  
dotnet run
```

### 5. Build Client App
```bash
cd src/ClientApp
dotnet build
```

## 📚 Tài liệu

- [📖 Software Requirements](SRS_Documents/README.md)
- [🏗️ Architecture Overview](docs/architecture.md)
- [📋 API Documentation](docs/api.md)
- [🎨 UI Design Guidelines](docs/ui-guidelines.md)
- [⚙️ Deployment Guide](docs/deployment.md)

## 🧪 Testing

```bash
# Chạy tất cả tests
dotnet test

# Test coverage report
dotnet test --collect:"XPlat Code Coverage"
```

## 🔄 CI/CD Pipeline

GitHub Actions workflow bao gồm:
1. **Build** - Build tất cả projects
2. **Test** - Chạy unit tests & integration tests  
3. **Database Migration** - EF Core migrations
4. **Deploy** - Docker containers deployment

## 🤝 Đóng góp

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Tạo Pull Request

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

## 📞 Contact

**Developer**: thanhnvbk92
**Project Link**: https://github.com/thanhnvbk92/machine-management-system

---
*Được tạo với ❤️ bởi GitHub Copilot*