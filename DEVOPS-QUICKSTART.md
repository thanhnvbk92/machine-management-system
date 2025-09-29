# 🚀 Machine Management System - DevOps Quick Start

## Quick Setup cho Development

### 1. Clone và Setup
```bash
git clone https://github.com/thanhnvbk92/machine-management-system.git
cd machine-management-system

# Setup development environment
chmod +x scripts/setup-dev.sh
./scripts/setup-dev.sh --profile tools
```

### 2. Development Workflow
```bash
# Start developing
dotnet run --project src/ClientApp/MachineClient.WPF

# Run backend (when API project is created)
# dotnet run --project src/Backend/MachineManagement.API

# Run tests (when test projects exist)
# dotnet test
```

### 3. Docker Development
```bash
# Only database
docker-compose -f docker-compose.dev.yml up -d database

# With management tools
docker-compose -f docker-compose.dev.yml --profile tools up -d

# Full stack (when ready)
# docker-compose -f docker-compose.dev.yml --profile api --profile manager up -d
```

## 🚀 CI/CD Pipeline Status

### ✅ Ready Components:
- **GitHub Actions Workflows**: 3 complete workflows
- **Docker Infrastructure**: Development + Production ready
- **Backend Build**: Cross-platform (.NET 8)
- **WPF Client Build**: Windows-specific (working)
- **Database Setup**: MySQL với Docker
- **Monitoring Stack**: Prometheus + Grafana + Loki
- **Deployment Automation**: Production-ready

### 🔄 In Progress:
- API Project creation
- Unit test implementation
- Integration test setup
- Manager Web App (Blazor)

### 📊 Service URLs (Development):
- **Database**: localhost:3306
- **Adminer**: http://localhost:8080
- **Portainer**: http://localhost:9000
- **API**: http://localhost:5000 (when ready)
- **Manager**: http://localhost:5001 (when ready)

### 📊 Service URLs (Production):
- **Main App**: https://your-domain.com
- **API**: https://your-domain.com/api
- **Grafana**: https://your-domain.com:3000
- **Prometheus**: https://your-domain.com:9090

## 🧪 Testing

### Current Testing Strategy:
```bash
# Build tests
dotnet build src/Backend/

# WPF tests (on Windows)
dotnet build src/ClientApp/MachineClient.WPF/

# When test projects are ready:
# dotnet test --collect:"XPlat Code Coverage"
```

## 🐳 Docker Commands

### Development:
```bash
# Start database only
docker-compose -f docker-compose.dev.yml up -d database

# Start with tools
docker-compose -f docker-compose.dev.yml --profile tools up -d

# View logs
docker-compose -f docker-compose.dev.yml logs -f

# Stop services
docker-compose -f docker-compose.dev.yml down
```

### Production:
```bash
# Deploy full stack
docker-compose -f docker-compose.prod.yml up -d

# Enable monitoring
docker-compose -f docker-compose.prod.yml --profile monitoring up -d

# Scale API services
docker-compose -f docker-compose.prod.yml up -d --scale backend-api=3

# View status
docker-compose -f docker-compose.prod.yml ps
```

## 📋 Next Development Steps

### 1. Complete API Project
```bash
# Create API project
dotnet new webapi -n MachineManagement.API -o src/Backend/MachineManagement.API

# Add project references
# Update Dockerfile.backend
# Update docker-compose files
```

### 2. Add Test Projects
```bash
# Create test projects
dotnet new xunit -n Backend.Tests -o tests/Unit/Backend.Tests
dotnet new xunit -n API.Integration.Tests -o tests/Integration/API.Integration.Tests

# Add test packages
# Update CI/CD pipeline to run tests
```

### 3. Create Manager Web App
```bash
# Create Blazor Server project
dotnet new blazorserver -n MachineManagement.ManagerApp -o src/ManagerApp/

# Update Dockerfile.manager
# Add to docker-compose files
```

## 🔧 Troubleshooting

### Common Issues:

1. **WPF Build Fails**:
   - Use Windows runner for WPF builds
   - Already configured in GitHub Actions

2. **Docker Permission Issues**:
   ```bash
   sudo usermod -aG docker $USER
   # Logout và login lại
   ```

3. **Database Connection Issues**:
   ```bash
   # Check container status
   docker-compose -f docker-compose.dev.yml ps
   
   # Check logs
   docker-compose -f docker-compose.dev.yml logs database
   
   # Test connection
   docker-compose -f docker-compose.dev.yml exec database mysql -u appuser -p
   ```

4. **Missing Secrets (Production)**:
   ```bash
   # Create required secrets
   echo "password123" | docker secret create db_root_password -
   # See docs/deployment/production-guide.md for complete setup
   ```

## 📞 Support

- **Issues**: https://github.com/thanhnvbk92/machine-management-system/issues
- **Documentation**: `/docs` directory
- **CI/CD Status**: https://github.com/thanhnvbk92/machine-management-system/actions

---

**🎉 DevOps infrastructure is ready! Start developing with confidence!**