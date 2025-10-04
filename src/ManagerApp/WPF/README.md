# Machine Manager WPF Application

## Tổng quan
Ứng dụng WPF Machine Manager được phát triển theo đúng các hướng dẫn instructions với:

- ✅ **MVVM chuẩn** với CommunityToolkit.Mvvm
- ✅ **MaterialDesignInXamlToolkit** cho UI modern 
- ✅ **Serilog** cho logging chuyên nghiệp
- ✅ **Dependency Injection** với Microsoft.Extensions.DependencyInjection
- ✅ **ObservableProperty** và **RelayCommand** attributes

## Tính năng chính

### 🌳 TreeView hiển thị máy theo line
- Hiển thị danh sách Production Lines dạng cây
- Cho phép select từng TreeView items hoặc tree parent
- Hiển thị số lượng máy trong từng line với biểu tượng trạng thái

### 📋 ListView hiển thị chi tiết máy
- Bên phải TreeView là ListView với Material Design
- Hiển thị tên, IP, trạng thái với styling đẹp
- Status chips với màu sắc theo trạng thái
- Tự động cập nhật khi select TreeView item

### 🎮 Panel Commands
- **Start Machine** - Khởi động máy (nút xanh lá)
- **Stop Machine** - Dừng máy (nút đỏ) 
- **Restart Machine** - Khởi động lại (nút cam)
- **Reset Machine** - Reset máy
- **Update Status** - Cập nhật trạng thái
- **Send Custom Command** - Gửi lệnh tùy chỉnh

### 📊 Activity Log
- Hiển thị lịch sử hoạt động realtime
- Logging với timestamp chi tiết
- Scroll tự động theo hoạt động mới

## Kiến trúc ứng dụng

### MVVM Pattern với CommunityToolkit.Mvvm
```csharp
[ObservableProperty]
private ObservableCollection<ProductionLine> _productionLines = new();

[RelayCommand(CanExecute = nameof(CanExecuteMachineCommand))]
private async Task StartMachineAsync() { ... }

partial void OnSelectedLineChanged(ProductionLine? value) { ... }
```

### Material Design UI
- **ColorZone** cho headers với elevation
- **Cards** cho layout sections
- **PackIcon** cho icons chuyên nghiệp
- **Chips** cho status indicators
- **RaisedButton** và **OutlinedButton**

### Serilog Logging
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/machine-manager-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

### Dependency Injection
```csharp
services.AddHttpClient<IMachineService, MachineService>();
services.AddSingleton<IMachineService, MachineService>();
services.AddTransient<MainViewModel>();
services.AddTransient<MainWindow>();
```

## Cách chạy ứng dụng

### 1. Từ Command Line
```powershell
cd "f:\Dev\Projects\Big-Project\machine-management-system\src\ManagerApp\WPF"
dotnet build
dotnet run
```

### 2. Hoặc chạy executable trực tiếp
```powershell
& ".\bin\Debug\net8.0-windows\MachineManagerApp.exe"
```

### 3. Sử dụng script tiện ích
```bash
run-wpf-app.bat
```

## Cấu trúc thư mục

```
WPF/
├── Models/                 # Data models với INotifyPropertyChanged
│   ├── Machine.cs         # Machine model với MachineStatus enum
│   ├── ProductionLine.cs  # ProductionLine với ObservableCollection
│   └── Command.cs         # Command và CommandResult models
├── Services/              # Business logic services
│   └── MachineService.cs  # API communication service
├── ViewModels/            # MVVM ViewModels
│   ├── BaseViewModel.cs   # Base với CommunityToolkit.Mvvm
│   └── MainViewModel.cs   # Main ViewModel với ObservableProperty
├── MainWindow.xaml        # Material Design UI layout
├── MainWindow.xaml.cs     # Code-behind tối thiểu
├── App.xaml              # Application resources với Material Design
├── App.xaml.cs           # DI configuration với Serilog
└── MachineManagerApp.csproj # Project với required packages
```

## Dependencies

### Required NuGet Packages
```xml
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
<PackageReference Include="MaterialDesignThemes" Version="4.9.0" />
<PackageReference Include="MaterialDesignColors" Version="2.1.4" />
<PackageReference Include="Serilog" Version="3.1.1" />
<PackageReference Include="Serilog.Extensions.Hosting" Version="8.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
```

## Tuân thủ Instructions

### ✅ MVVM chuẩn với CommunityToolkit.Mvvm
- `ObservableObject` cho ViewModel base class
- `RelayCommand` cho Command handling  
- `ObservableCollection` cho danh sách data binding

### ✅ Business Logic Separation
- **Không** viết business logic trong ViewModel
- Chỉ gọi qua service layer (`MachineService`)
- ViewModels chỉ handle UI logic và data binding

### ✅ Dependency Injection Container
- Microsoft.Extensions.DependencyInjection
- Service registration trong `App.xaml.cs`
- Constructor injection cho tất cả dependencies

### ✅ Serilog Logging
- Structured logging với Serilog
- File và Console sinks
- Rolling file policy theo ngày

### ✅ MaterialDesignInXamlToolkit
- Material Design theme và controls
- BundledTheme với Light theme
- Material Design icons và styling

### ✅ Code-behind tối thiểu
- Chỉ xử lý UI events (TreeView selection)
- Không chứa business logic
- Delegate tất cả logic cho ViewModel

## Mock Data cho Testing

Ứng dụng sử dụng mock data để demo:
- 3 Production Lines (Line A, B, C)
- Mỗi line có 4-5 machines
- Random status simulation
- Mock command execution

## Log Files

Logs được lưu tại: `logs/machine-manager-YYYY-MM-DD.txt`

## API Integration

Backend API URL: `http://localhost:5275/`
(Có thể thay đổi trong `App.xaml.cs`)

---

**Phát triển bởi**: Machine Management System Team  
**Tuân thủ**: WPF Instructions v1.0