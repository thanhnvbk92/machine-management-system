# Machine Management System

> **🏭 Hệ thống quản lý máy móc sản xuất với thu thập log và điều khiển từ xa**

[![.NET](https://github.com/thanhnvbk92/machine-management-system/workflows/CI%2FCD%20Pipeline/badge.svg)](https://github.com/thanhnvbk92/machine-management-system/actions)
[![Release](https://github.com/thanhnvbk92/machine-management-system/workflows/Release%20Pipeline/badge.svg)](https://github.com/thanhnvbk92/machine-management-system/releases)
[![Docker](https://img.shields.io/badge/Docker-Ready-blue?logo=docker)](https://github.com/thanhnvbk92/machine-management-system/pkgs/container/machine-management-system)

## 🚀 Tổng quan hệ thống

**✅ BACKEND API HOÀN THÀNH - READY FOR TESTING**
**✅ WPF DESKTOP CLIENT HOÀN THÀNH - READY FOR TESTING**
**✅ CI/CD PIPELINE HOÀN THÀNH - AUTOMATED DEPLOYMENT**

### Mô tả kiến trúc:
```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   CLIENT APP    │    │   BACKEND API    │    │  MANAGER WEB    │
│   (WPF Desktop) │◄──►│  (.NET 8 Core)   │◄──►│ (Blazor Server) │
│       ✅        │    │       ✅         │    │   (Coming)      │
│ - Log Collection│    │ - REST APIs      │    │ - Dashboard     │
│ - Command Exec  │    │ - Business Logic │    │ - Management    │
│ - Config Sync   │    │ - Data Access    │    │ - Reporting     │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                                │                        │
                                ▼                        ▼
                       ┌──────────────────┐    ┌─────────────────┐
                       │   MySQL DATABASE │    │  GITHUB ACTIONS │
                       │ machine_mgmt_db  │    │    CI/CD ✅     │
                       │       ✅         │    │ - Auto Build    │
                       └──────────────────┘    │ - Testing       │
                                              │ - Docker Deploy │
                                              │ - Security Scan │
                                              └─────────────────┘
```

### 3 thành phần chính:
1. **✅ Client App** (Windows Desktop - WPF):
   - Tự động đọc log files từ máy production
   - Gửi data lên server API với batch processing
   - Nhận lệnh và điều khiển ứng dụng khác
   - Material Design UI với real-time monitoring
   - Sử dụng: WPF, MVVM, Material Design, Dependency Injection

2. **✅ Backend API** (.NET 8 Web API):
   - Nhận log từ clients với RESTful endpoints
   - Lưu vào MySQL database với EF Core
   - Quản lý commands và client registry
   - Health checks, Swagger documentation, CORS
   - Sử dụng: ASP.NET Core, EF Core, Repository Pattern, Clean Architecture

3. **🔄 Manager Web** (Blazor Server - Coming Soon):
   - Dashboard xem clients và log data
   - Gửi lệnh điều khiển từ xa
   - Báo cáo và analytics
   - Sử dụng: Blazor Server, SignalR, Material Design

## 🛠️ Tech Stack

### Backend (.NET 8)
- **Framework**: ASP.NET Core Web API
- **Database**: MySQL với Entity Framework Core
- **Architecture**: Clean Architecture, Repository Pattern
- **Features**: Swagger, Health Checks, CORS, Structured Logging
- **ORM**: Entity Framework Core với Code-First approach

### Desktop Client (WPF)
- **Framework**: WPF với .NET 8
- **Pattern**: MVVM với CommunityToolkit.Mvvm
- **UI**: Material Design trong WPF
- **DI**: Microsoft.Extensions.DependencyInjection
- **HTTP**: HttpClient với System.Net.Http.Json
- **Logging**: Microsoft.Extensions.Logging

### Database
- **Engine**: MySQL 8.0+
- **Structure**: Hierarchical production management
- **Features**: Log partitioning, stored procedures, views
- **Support**: Multiple buyers (BMW, Audi, VW, Mercedes)

### DevOps & CI/CD
- **Platform**: GitHub Actions
- **Containers**: Docker với multi-stage builds
- **Registry**: GitHub Container Registry (GHCR)
- **Testing**: Automated unit và integration tests
- **Security**: CodeQL analysis, dependency scanning
- **Deployment**: Multi-environment với production releases

## 🚀 Quick Start

### Yêu cầu hệ thống

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **MySQL Server 8.0+** - [Download](https://dev.mysql.com/downloads/mysql/)
- **Visual Studio 2022** hoặc **VS Code** - [Download](https://visualstudio.microsoft.com/)
- **Git** - [Download](https://git-scm.com/)
- **Docker** (optional) - [Download](https://docs.docker.com/get-docker/)

### Cài đặt và chạy

#### Option 1: Development Setup

##### 1. Clone repository
```bash
git clone https://github.com/thanhnvbk92/machine-management-system.git
cd machine-management-system
```

##### 2. Cài đặt database
```powershell
# Tạo database và bảng
.\setup-database.ps1 -Username "root" -Password "your_password"

# Thêm dữ liệu mẫu
.\setup-database.ps1 -Action seed -Username "root" -Password "your_password"
```

##### 3. Cấu hình connection string
Chỉnh sửa `src/Backend/MachineManagement.API/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=machine_management_db;Uid=root;Pwd=your_password;Port=3306;CharSet=utf8mb4;"
  }
}
```

##### 4. Chạy Backend API
```powershell
# Build và chạy API server
.\run-backend.ps1

# Hoặc chạy trực tiếp
dotnet run --project src/Backend/MachineManagement.API
```

##### 5. Truy cập Swagger UI
Mở trình duyệt và truy cập: **https://localhost:5001**

##### 6. Chạy WPF Desktop Client
```powershell
cd src/ClientApp/MachineClient.WPF
dotnet restore
dotnet build
dotnet run
```

#### Option 2: Docker Deployment

##### 1. Pull và chạy từ GHCR
```bash
# Pull latest image
docker pull ghcr.io/thanhnvbk92/machine-management-system:latest

# Chạy với MySQL
docker-compose up -d
```

##### 2. Build local image
```bash
# Build custom image
docker build -t machine-management-system .

# Chạy với environment variables
docker run -p 5000:8080 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=machine_management_db;Uid=root;Pwd=yourpassword;" \
  machine-management-system
```

## 📊 Tính năng hiện tại

### Backend API
- ✅ **Machine Registration**: Đăng ký và quản lý máy móc
- ✅ **Log Collection**: Thu thập logs theo batch với optimization
- ✅ **Heartbeat Monitoring**: Giám sát trạng thái máy real-time
- ✅ **Command Management**: Gửi và theo dõi lệnh điều khiển
- ✅ **Health Checks**: Endpoint kiểm tra sức khỏe hệ thống
- ✅ **Swagger Documentation**: API documentation đầy đủ
- ✅ **CORS Support**: Cross-origin resource sharing
- ✅ **Structured Logging**: Logging với Serilog

### WPF Desktop Client
- ✅ **Material Design UI**: Giao diện đẹp với Material Design
- ✅ **Real-time Dashboard**: Hiển thị metrics và status live
- ✅ **Auto Log Collection**: Tự động đọc và parse log files
- ✅ **Configuration Management**: Quản lý settings với JSON
- ✅ **Connection Monitoring**: Theo dõi kết nối API server
- ✅ **Command Execution**: Nhận và thực thi lệnh từ xa
- ✅ **MVVM Pattern**: Kiến trúc MVVM với data binding
- ✅ **Dependency Injection**: DI container với services

### Database
- ✅ **Hierarchical Structure**: BUYERS→LINES→STATIONS→MACHINES
- ✅ **Log Partitioning**: Phân vùng logs theo tháng
- ✅ **Stored Procedures**: Procedures cho operations phức tạp
- ✅ **Views**: Views cho reporting và analytics
- ✅ **Sample Data**: Dữ liệu mẫu cho testing

### DevOps & CI/CD
- ✅ **GitHub Actions Workflows**: Automated CI/CD pipeline
- ✅ **Multi-Platform Builds**: Windows, Linux, macOS support
- ✅ **Automated Testing**: Unit tests, integration tests
- ✅ **Security Scanning**: CodeQL analysis, dependency checks
- ✅ **Docker Deployment**: Container builds và GHCR publishing
- ✅ **Release Automation**: Semantic versioning và GitHub releases
- ✅ **Environment Management**: Development, staging, production
- ✅ **Monitoring**: Build status badges và notifications

## 🔄 CI/CD Pipeline

### Workflow Features
```yaml
# .github/workflows/ci-cd.yml
🔄 Trigger: Push, PR, Schedule
📦 Jobs: Build → Test → Security → Docker → Deploy
🧪 Testing: Unit tests với coverage reporting
🔒 Security: CodeQL analysis, dependency scanning
🐳 Docker: Multi-stage builds với caching
📊 Reports: Test results, security findings
```

### Release Process
```yaml
# .github/workflows/release.yml
🏷️ Trigger: Version tags (v*.*.*)
📋 Steps: Changelog → Build → Test → Package → Release
📦 Artifacts: Binaries, Docker images, documentation
🚀 Deploy: Automated production deployment
```

### Monitoring
- **Build Status**: Realtime pipeline status
- **Test Coverage**: Code coverage reports
- **Security**: Vulnerability scanning
- **Performance**: Build time optimization
- **Notifications**: Slack, email alerts

## 📁 Cấu trúc project

```
machine-management-system/
├── src/
│   ├── Backend/                     # Backend API (.NET 8)
│   │   ├── MachineManagement.Core/  # Domain entities, interfaces
│   │   ├── MachineManagement.Infrastructure/ # Data access, repositories
│   │   └── MachineManagement.API/   # Web API controllers, middleware
│   ├── ClientApp/                   # Desktop WPF Client
│   │   └── MachineClient.WPF/       # WPF app với Material Design
│   └── ManagerApp/                  # Web Manager (Coming)
├── SRS_Documents/                   # System Requirements Specification
├── .github/
│   └── workflows/                   # GitHub Actions CI/CD
│       ├── ci-cd.yml               # Main CI/CD pipeline
│       └── release.yml             # Release automation
├── Dockerfile                      # Docker container definition
├── docker-compose.yml              # Multi-container setup
├── setup-database.ps1              # Database setup script
├── run-backend.ps1                 # Backend run script
└── README.md
```

## 🔄 Roadmap

### Phase 2: Manager Web Application (Next)
- [ ] Blazor Server web application
- [ ] Dashboard với real-time data
- [ ] Machine management interface
- [ ] Command center cho remote control
- [ ] Reporting và analytics
- [ ] User authentication và authorization

### Phase 3: Advanced Features
- [ ] Machine learning cho log analysis
- [ ] Predictive maintenance alerts
- [ ] Mobile app support
- [ ] Multi-tenant support
- [ ] Advanced reporting với charts
- [ ] Integration với third-party systems

### Phase 4: Production Excellence
- ✅ Docker containerization
- ✅ CI/CD pipeline hoàn chỉnh
- [ ] Kubernetes deployment
- [ ] Monitoring và alerting (Prometheus, Grafana)
- [ ] Load balancing (nginx, HAProxy)
- [ ] Security hardening (HTTPS, authentication)
- [ ] Performance optimization
- [ ] High availability setup

## 🤝 Contributing

### Development Workflow
1. Fork repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Make changes và test locally
4. Commit với conventional commits (`git commit -m 'feat: Add AmazingFeature'`)
5. Push to branch (`git push origin feature/AmazingFeature`)
6. Open Pull Request
7. CI/CD pipeline sẽ tự động chạy tests
8. Review và merge

### Testing
```bash
# Chạy unit tests
dotnet test

# Chạy integration tests
dotnet test --filter Category=Integration

# Check test coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Docker Development
```bash
# Build development image
docker build -t machine-management-dev .

# Run với hot reload
docker-compose -f docker-compose.dev.yml up
```

## 📝 License

Distributed under the MIT License. See `LICENSE` for more information.

## 📞 Contact

- **GitHub**: [@thanhnvbk92](https://github.com/thanhnvbk92)
- **Repository**: [machine-management-system](https://github.com/thanhnvbk92/machine-management-system)
- **Issues**: [GitHub Issues](https://github.com/thanhnvbk92/machine-management-system/issues)
- **CI/CD**: [GitHub Actions](https://github.com/thanhnvbk92/machine-management-system/actions)

---

> **💡 Tip**: Để test hệ thống nhanh chóng, hãy chạy Backend API trước, sau đó mở WPF Client và click "Start Services"!
> 
> **🐳 Docker Tip**: Sử dụng `docker-compose up` để chạy toàn bộ stack (API + Database) chỉ với một lệnh!
> 
> **🚀 CI/CD Tip**: Mỗi commit sẽ trigger automated pipeline - check tab Actions để theo dõi build status!