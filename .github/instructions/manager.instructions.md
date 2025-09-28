```instructions
---
applyTo: "src/ManagerApp/**/*.*"
---

# Hướng dẫn cho ManagerApp (Web)

- Dùng **Blazor Server** hoặc **Razor Pages** (theo Detailed Design).
- Layout & UI tuân theo Material Design (MudBlazor nếu dùng Blazor).
- Tách logic UI và logic nghiệp vụ, sử dụng **Service layer**.
- Gọi API qua HttpClient hoặc gRPC nếu có.
- Logging & DI áp dụng như Backend.
- Validate input form bằng DataAnnotations hoặc FluentValidation.

```