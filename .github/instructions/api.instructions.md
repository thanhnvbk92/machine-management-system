```instructions
---
applyTo: "src/Backend/**/*.*"
---

# Hướng dẫn cho Backend API

- Controllers chỉ gọi **Service** qua DI, không chứa business logic.
- Business logic nằm trong **Service layer**.
- Data access qua **Repository pattern**.
- Mọi class Repository và Service phải có interface (IService, IRepository).
- API phải trả về **DTO** theo thiết kế, không trả trực tiếp entity DB.
- Validate dữ liệu đầu vào (DataAnnotation hoặc FluentValidation).
- Logging request/response và exception bằng **Serilog**.
- Khi thay đổi DB schema, Copilot phải gợi ý **EF Core migration**.
- Viết **unit test** cho Service, **integration test** cho API.

```