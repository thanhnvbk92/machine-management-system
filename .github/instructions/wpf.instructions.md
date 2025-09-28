```instructions
---
applyTo: "src/ClientApp/**/*.*"
---

# Hướng dẫn cho ClientApp (WPF)

- Sử dụng **MVVM chuẩn** với CommunityToolkit.Mvvm:
  - `ObservableObject` cho ViewModel
  - `RelayCommand` cho Command
  - `ObservableCollection` cho danh sách
- **Không** viết business logic trong ViewModel, chỉ gọi qua service.
- Sử dụng **DI container** (ví dụ: Microsoft.Extensions.DependencyInjection).
- Logging bằng **Serilog** hoặc logger được cấu hình chung.
- UI phải sử dụng **MaterialDesignInXamlToolkit** cho theme và control.
- Code-behind chỉ dùng cho xử lý UI đặc thù (animation, event UI), không chứa nghiệp vụ.

```