# Test Projects - Machine Management System

## Structure

```
tests/
├── Unit/                           # Unit test projects
│   ├── Backend.Core.Tests/         # Core domain tests
│   ├── Backend.Infrastructure.Tests/ # Data access tests
│   ├── Backend.API.Tests/           # API controller tests (when created)
│   └── ClientApp.Tests/             # WPF client tests
├── Integration/                     # Integration test projects
│   ├── API.Integration.Tests/       # End-to-end API tests
│   ├── Database.Integration.Tests/  # Database integration tests
│   └── SignalR.Integration.Tests/   # Real-time communication tests
└── README.md                       # This file
```

## Test Strategy

### Unit Tests
- **Coverage Target**: 80%+
- **Framework**: xUnit với FluentAssertions
- **Mocking**: Moq hoặc NSubstitute
- **Test Categories**: 
  - Domain logic tests
  - Service layer tests
  - Repository pattern tests
  - ViewModel tests (WPF)

### Integration Tests
- **Database**: TestContainers với MySQL
- **API**: ASP.NET Core TestServer
- **End-to-End**: Playwright hoặc Selenium (cho WPF)
- **SignalR**: In-memory test server

### Performance Tests
- **Load Testing**: NBomber hoặc k6
- **Stress Testing**: API endpoints under load
- **Memory Testing**: Memory leak detection

## CI/CD Integration

Tests sẽ được chạy trong GitHub Actions:
- **Unit Tests**: Mỗi PR và push
- **Integration Tests**: Trước khi deploy
- **Performance Tests**: Weekly schedule

## TODO

1. **Create test projects**:
   ```bash
   dotnet new xunit -n Backend.Core.Tests
   dotnet new xunit -n Backend.Infrastructure.Tests
   dotnet new xunit -n API.Integration.Tests
   ```

2. **Add test packages**:
   - xUnit
   - FluentAssertions
   - Moq
   - Microsoft.EntityFrameworkCore.InMemory
   - TestContainers
   - Microsoft.AspNetCore.Mvc.Testing

3. **Setup test data builders**
4. **Configure test environments**
5. **Add code coverage reporting**

## Commands

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific category
dotnet test --filter Category=Unit

# Run integration tests (requires test database)
dotnet test --filter Category=Integration
```

## Notes

- Test projects sẽ được tạo khi API project được hoàn thành
- Integration tests cần Docker cho TestContainers
- Performance tests sẽ chạy trong separate pipeline
- Code coverage reports sẽ được upload lên GitHub Actions artifacts