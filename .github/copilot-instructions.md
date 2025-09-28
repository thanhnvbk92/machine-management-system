# Hướng dẫn chung cho dự án Machine Management System

## Mô tả dự án
- Đây là hệ thống quản lý máy móc gồm 3 thành phần: 
  1. **Database**
  2. **Backend API / ManagerApp (Web)**
  3. **ClientApp (Desktop WPF)**

- ClientApp sử dụng: **WPF, MVVM, Dependency Injection, Logging, CommunityToolkit.Mvvm, Material Design**.  
- Backend API sử dụng: **ASP.NET Core Web API, EF Core, Repository Pattern**.  
- ManagerApp sử dụng: **ASP.NET Core Blazor Server hoặc Razor Pages**.  
- DevOps: **GitHub Actions**, build/test/deploy/migrate DB.  
- Tất cả mã nguồn phải tuân thủ **Detailed Design Document** (thiết kế chi tiết).

## Nguyên tắc chung
- Luôn phản hồi bằng **tiếng Việt**.  
- Luôn tuân thủ kiến trúc **Clean Architecture**:
  - UI (View/ViewModel) tách biệt business logic
  - Service/Repository qua interface + DI
  - Logging ở mọi tầng
- Mỗi thay đổi phải có **unit test hoặc integration test** tương ứng.  
- Không bypass interface hoặc viết logic trực tiếp trong UI/Controller.  
- Đảm bảo code sinh ra dễ đọc, có comment, theo chuẩn .NET.

## DevOps / CI / GitHub Actions
- Workflow có các job:
  1. **Build** (client + backend + manager)
  2. **Test** (unit test, integration test)
  3. **Migrate-DB** (EF Core migrations)
  4. **Deploy** (Docker / container)
- Secrets (connection string, API keys) lưu trong GitHub Secrets.  
- Khi thêm API mới hoặc thay đổi DB, Copilot phải gợi ý migration và cập nhật test.  

## Công nghệ chính
- **Ngôn ngữ**: C# (.NET 8), SQL (MySQL / PostgreSQL / SQL Server tùy thiết kế).  
- **Database**: EF Core ORM + Migration.  
- **ClientApp**: WPF + MVVM + Material Design Toolkit.  
- **Backend**: ASP.NET Core Web API, Repository Pattern, DTO, AutoMapper.  
- **ManagerApp**: Blazor Server hoặc Razor Pages.  

## Cấu trúc thư mục
```
machine-management-system/
├── .github/
│   ├── workflows/           # GitHub Actions CI/CD
│   └── instructions/        # Development guidelines
├── src/
│   ├── ClientApp/          # WPF Desktop Application
│   │   ├── ViewModels/     # MVVM ViewModels
│   │   ├── Views/          # WPF Views/Windows
│   │   ├── Services/       # Business logic services
│   │   └── Models/         # Data models
│   ├── Backend/            # ASP.NET Core Web API
│   │   ├── Controllers/    # API Controllers
│   │   ├── Services/       # Business services
│   │   ├── Repositories/   # Data access layer
│   │   ├── Models/         # Entity models
│   │   └── DTOs/           # Data Transfer Objects
│   ├── ManagerApp/         # Blazor Server Web App
│   │   ├── Pages/          # Blazor pages
│   │   ├── Components/     # Reusable components
│   │   └── Services/       # Web app services
│   └── Database/           # SQL Scripts & Migrations
├── tests/                  # Unit & Integration tests
├── docs/                   # Technical documentation
└── SRS_Documents/          # Software Requirements
```

## Quy tắc phát triển
1. **Luôn tạo branch từ main** cho feature mới
2. **Pull Request** bắt buộc trước khi merge
3. **Code review** bởi ít nhất 1 developer khác
4. **Tests phải pass** trước khi merge
5. **Documentation** phải được cập nhật đồng thời

## Coding Standards
- **C#**: Tuân theo Microsoft C# Coding Conventions
- **Naming**: PascalCase cho public members, camelCase cho private
- **Comments**: XML documentation cho public APIs
- **Error Handling**: Sử dụng try-catch và logging đầy đủ
- **Async/Await**: Sử dụng async patterns cho I/O operations