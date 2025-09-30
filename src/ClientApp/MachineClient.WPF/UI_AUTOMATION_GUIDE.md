# Hướng dẫn sử dụng UI Automation trong Machine Client WPF

## Tổng quan
Ứng dụng Machine Client WPF đã được tích hợp hệ thống UI Automation sử dụng **FlaUI**, cho phép:
- Tự động click button
- Đọc text từ các elements (TextBlock, Label, TextBox, ComboBox)
- Tương tác với ComboBox và Menu
- Theo dõi sự thay đổi của TextBlock elements
- Monitor Pin Count changes với event triggers

## Kiến trúc UI Automation

### 1. Services được triển khai:
- **IUIAutomationService** - Service cơ bản cho automation
- **IPinCountMonitoringService** - Service chuyên dụng cho pin count monitoring
- **IUIAutomationDemoService** - Service demo tất cả tính năng

### 2. AutomationIds đã được thêm vào:
```xml
<!-- Backup buttons -->
StartBackupButton
StopBackupButton

<!-- TextBlocks -->
MachineIdTextBlock
BackupStatusTextBlock
BackupProgressTextBlock

<!-- Demo buttons -->
DemoAllFeaturesButton
StartPinMonitoringButton
StopPinMonitoringButton
AutoClickBackupButton
ReadMachineIdButton
```

## Cách sử dụng

### 1. Demo tất cả tính năng:
- Click nút **"Demo All Features"** để test toàn bộ UI automation
- Xem kết quả trong Activity Log

### 2. Pin Count Monitoring:
- Click **"Start Pin Monitoring"** để bắt đầu theo dõi
- Service sẽ monitor changes trong BackupStatusTextBlock
- Click **"Stop Pin Monitoring"** để dừng

### 3. Auto Click Backup:
- Click **"Auto Click Backup"** để tự động click nút Start Backup
- Kiểm tra log để xem kết quả

### 4. Read Machine ID:
- Click **"Read Machine ID"** để đọc Machine ID từ UI element
- Text được đọc sẽ hiển thị trong log

## Cách mở rộng

### Thêm AutomationId cho elements mới:
```xml
<Button AutomationProperties.AutomationId="MyButtonId"
        AutomationProperties.Name="My Button Name"
        Content="My Button"/>
```

### Sử dụng UI Automation Service:
```csharp
// Click button
await _uiAutomationService.ClickButtonAsync("MyButtonId");

// Read text
var text = await _uiAutomationService.ReadTextAsync("MyTextBlockId");

// Monitor text changes
await _uiAutomationService.StartTextBlockMonitoringAsync("MyTextBlockId", 
    newText => Console.WriteLine($"Text changed to: {newText}"));
```

### Tạo custom monitoring:
```csharp
// Subscribe to pin count changes
_pinCountMonitoringService.PinCountChanged += (sender, e) => {
    Console.WriteLine($"Pin count changed from {e.PreviousValue} to {e.NewValue}");
};

// Start monitoring
await _pinCountMonitoringService.StartMonitoringAsync();
```

## Lưu ý quan trọng

1. **Element Identification**: FlaUI tìm elements theo AutomationId đầu tiên, sau đó fallback về Name
2. **Threading**: Tất cả UI operations phải chạy trên UI thread
3. **Error Handling**: Services có comprehensive error handling và logging
4. **Performance**: Text monitoring sử dụng timer với interval 500ms
5. **Disposal**: Nhớ dispose automation services khi thoát app

## Troubleshooting

### Element không tìm thấy:
- Kiểm tra AutomationId đã được set chưa
- Kiểm tra element có visible không
- Thử sử dụng Name thay vì AutomationId

### Timer issues:
- Kiểm tra timer đã được disposed chưa
- Verify element vẫn còn tồn tại

### Performance issues:
- Tăng monitoring interval nếu cần
- Optimize element finding logic
- Reduce số lượng concurrent monitors

## Demo Commands đã có sẵn

1. **TestUIAutomationCommand** - Demo comprehensive functionality
2. **StartPinCountMonitoringCommand** - Start pin monitoring
3. **StopPinCountMonitoringCommand** - Stop monitoring  
4. **AutoClickBackupButtonCommand** - Auto click backup button
5. **ReadMachineIdCommand** - Read machine ID from UI

## Log Messages

Service sẽ ghi log các thông tin sau:
- ✅ Success operations
- ⚠️ Warning messages  
- ❌ Error messages
- 🔄 Operation in progress
- 📍 Pin count changes

Tất cả log messages sẽ hiển thị trong Activity Log section của ứng dụng.