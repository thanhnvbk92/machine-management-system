```instructions
---
applyTo: ".github/workflows/**/*.yml"
---

# Hướng dẫn cho GitHub Actions

- Workflow phải gồm:
  1. **Build** (dotnet build cho tất cả project)
  2. **Test** (dotnet test)
  3. **Migrate-DB** (dotnet ef database update)
  4. **Deploy** (docker build & push hoặc publish)
- Dùng action chuẩn: `actions/checkout`, `actions/setup-dotnet`, `docker/login-action`.
- Secrets lưu trong **GitHub Secrets** (`DB_CONNECTION_STRING`, `API_KEY`…).
- Nếu deploy thất bại, cần hỗ trợ rollback migration.
- Các job phải chạy song song khi có thể, trừ `deploy` (chạy sau khi test pass).

```