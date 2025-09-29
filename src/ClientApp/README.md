# WPF Desktop Client App

## 🎯 Mục đích
Ứng dụng desktop WPF để:
- Thu thập log files từ các máy production
- Gửi dữ liệu lên Backend API server  
- Nhận và thực thi commands từ xa
- Giám sát trạng thái kết nối real-time

## 🏗️ Kiến trúc

### MVVM Pattern với Clean Architecture:
```
Views (XAML) ← DataBinding → ViewModels ← Services → API/FileSystem
    ↓                           ↓            ↓
Material Design            ObservableObject   Dependency Injection
```

### Thành phần chính:

#### 1. **Views & ViewModels**
- `MainWindow.xaml` + `MainViewModel.cs`: Dashboard chính
- Material Design UI với cards, progress bars, data grids
- Real-time status updates và log monitoring

#### 2. **Services Layer**
- `IApiService`: HTTP communication với Backend API
- `ILogCollectionService`: Đọc và parse log files
- `IConfigurationService`: Quản lý settings local

#### 3. **Models**
- `Machine`: Thông tin máy móc
- `LogData`: Cấu trúc dữ liệu log
- `Command`: Lệnh điều khiển từ xa
- `ClientConfiguration`: Cài đặt ứng dụng

## 🚀 Chạy ứng dụng

### Yêu cầu:
- .NET 8 Runtime
- Windows 10/11
- Backend API server đang chạy

### Cài đặt:
```bash
cd src/ClientApp/MachineClient.WPF
dotnet restore
dotnet build
dotnet run
```

### Cấu hình:
File config tự động tạo tại: `%LocalAppData%\MachineClient\config.json`

```json
{
  "machineId": "MACHINE_001",
  "stationName": "Station_A", 
  "lineName": "Line_1",
  "apiUrl": "https://localhost:5001",
  "logCollectionInterval": 30,
  "heartbeatInterval": 60,
  "autoStart": true,
  "logFolderPath": "C:\\MachineData\\Logs"
}
```

## 📊 Tính năng

### Dashboard chính:
- **Connection Status**: Hiển thị trạng thái kết nối API
- **Machine Info**: Thông tin máy, station, line
- **Log Statistics**: Số lượng logs đã gửi, queue size
- **Recent Logs**: Log entries gần đây với filtering
- **Commands**: Danh sách lệnh pending và executed

### Auto Services:
- **Heartbeat Timer**: Gửi signal sống định kỳ
- **Log Collection Timer**: Tự động đọc log files mới
- **Command Polling**: Kiểm tra lệnh từ server

### Log Processing:
- Hỗ trợ nhiều format: `.log`, `.txt`, `.csv`  
- Parse structured logs với regex pattern
- Batch upload để tối ưu bandwidth
- Queue management với retry logic

### Remote Commands:
- Nhận lệnh từ Management Web
- Execute và trả về kết quả
- Status tracking (Pending → Executing → Completed)

## 🔧 Customization

### Thêm Log Parser mới:
```csharp
// Trong LogCollectionService.cs
private LogData? ParseLogLine(string line, string filePath, int lineNumber, string machineId)
{
    // Thêm custom regex pattern cho format log mới
    var customPattern = new Regex(@"your-pattern-here");
    // ... implementation
}
```

### Thêm Command Handler:
```csharp
// Trong MainViewModel.cs  
[RelayCommand]
private async Task ExecuteCustomCommand(Command command)
{
    // Custom command execution logic
}
```

### Material Design Theme:
Colors và styles có thể customize trong `Styles/Styles.xaml`

## 🐛 Troubleshooting

### Lỗi thường gặp:

1. **"Cannot connect to API"**
   - Kiểm tra Backend API đang chạy
   - Verify URL trong config
   - Check firewall/antivirus

2. **"Log folder not found"**  
   - Tạo folder log path trong config
   - Kiểm tra permissions đọc/ghi

3. **"Service start failed"**
   - Check log files trong `%LocalAppData%\MachineClient\Logs\`
   - Verify machine registration

### Debug Mode:
```bash
dotnet run --configuration Debug
```

## 📝 Logs & Monitoring

Ứng dụng ghi logs vào:
- Console (khi debug)
- File: `%LocalAppData%\MachineClient\Logs\app-{date}.log`

Log levels: `Trace` → `Debug` → `Information` → `Warning` → `Error` → `Critical`