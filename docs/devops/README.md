# DevOps & CI/CD Documentation

## 🚀 Overview

Complete DevOps pipeline cho Machine Management System với GitHub Actions, Docker containerization, và automated deployment.

## 📋 Pipeline Structure

### 1. Main CI/CD Pipeline (`.github/workflows/main.yml`)

**Triggers:**
- Push to `main` và `develop` branches
- Pull requests to `main` và `develop`
- Manual workflow dispatch

**Jobs:**
- **build-backend**: Build Backend API (.NET 8) trên Ubuntu
- **build-wpf-client**: Build WPF Desktop Client trên Windows
- **unit-tests**: Chạy unit tests
- **code-quality**: Code analysis và security scan
- **database-migration**: EF Core migration validation
- **deploy-development**: Auto-deploy to dev environment
- **deploy-production**: Manual approval cho production

### 2. Feature Branch Pipeline (`.github/workflows/feature.yml`)

**Purpose:** Validation cho Pull Requests

**Checks:**
- Build verification
- Merge conflict detection
- Code formatting
- Basic security scan
- Documentation coverage

### 3. Release Pipeline (`.github/workflows/release.yml`)

**Triggers:**
- Git tags (`v*.*.*`)
- Manual workflow dispatch

**Artifacts:**
- WPF Client installer (MSI)
- Backend API Docker images
- Manager Web Docker images
- Release notes (auto-generated)

## 🐳 Docker Strategy

### Development Environment (`docker-compose.dev.yml`)

**Services:**
- **database**: MySQL 8.0
- **redis**: Caching và SignalR backplane
- **adminer**: Database management UI
- **portainer**: Container management UI

**Profiles:**
- `database`: Only database
- `tools`: Database + management tools
- `api`: Backend API (when implemented)
- `manager`: Manager Web App (when implemented)
- `cache`: Redis caching

### Production Environment (`docker-compose.prod.yml`)

**Features:**
- **nginx**: Load balancer với SSL termination
- **Multi-replica**: API và Manager services
- **Health checks**: All services
- **Resource limits**: Memory và CPU constraints
- **Secrets management**: External secrets
- **Monitoring**: Prometheus, Grafana, Loki
- **Backup**: Automated database backups

## ⚙️ Environment Configuration

### Development Setup

```bash
# Clone repository
git clone https://github.com/thanhnvbk92/machine-management-system.git
cd machine-management-system

# Setup development environment
chmod +x scripts/setup-dev.sh
./scripts/setup-dev.sh --profile tools

# Start development
dotnet run --project src/ClientApp/MachineClient.WPF
```

### Production Deployment

```bash
# Create production secrets
echo "admin_password_here" | docker secret create db_root_password -
echo "connection_string_here" | docker secret create connection_string -

# Deploy production stack
docker-compose -f docker-compose.prod.yml up -d

# Scale services
docker-compose -f docker-compose.prod.yml up -d --scale backend-api=3
```

## 🔒 Security Measures

### Development
- No hardcoded credentials
- Local-only database binding
- HTTPS redirect disabled cho development
- Debug information enabled

### Production
- **Docker secrets** cho sensitive data
- **Non-root containers** cho all services
- **Network isolation** với custom bridge
- **SSL termination** tại nginx
- **Resource limits** để prevent DoS
- **Health checks** cho all services

## 📊 Monitoring & Observability

### Metrics Collection
- **Prometheus**: Application và system metrics
- **Grafana**: Visualization và alerting
- **Loki**: Log aggregation
- **Custom metrics**: Business logic metrics

### Health Checks
- **Database**: Connection và query test
- **API**: `/health` endpoint
- **Manager**: Application health
- **nginx**: Load balancer health

### Alerting Rules
- Service down alerts
- High response time
- Database connection issues
- High memory/CPU usage
- Disk space warnings

## 🧪 Testing Strategy

### Automated Testing
- **Unit Tests**: Business logic validation
- **Integration Tests**: API endpoint testing
- **Database Tests**: EF Core và migrations
- **UI Tests**: WPF application testing

### Quality Gates
- **Code Coverage**: Minimum 80%
- **Security Scanning**: Dependency vulnerabilities
- **Performance Tests**: Response time thresholds
- **Code Analysis**: SonarQube integration

## 🚀 Deployment Phases

### Phase 1: Basic CI/CD ✅
- [x] GitHub Actions workflows
- [x] Docker containerization
- [x] Multi-platform builds
- [x] Basic testing pipeline

### Phase 2: Advanced Pipeline
- [ ] SonarQube integration
- [ ] Security scanning (Snyk/OWASP)
- [ ] Performance testing
- [ ] Database migration automation

### Phase 3: Production Deployment
- [ ] Kubernetes manifests
- [ ] Helm charts
- [ ] Blue-green deployment
- [ ] Canary releases

### Phase 4: Monitoring & Operations
- [ ] Complete monitoring stack
- [ ] Log aggregation
- [ ] Alerting rules
- [ ] Automated backup/restore

## 🔧 Troubleshooting

### Common Issues

1. **WPF Build Fails on Linux**
   - Solution: Uses `windows-latest` runner in GitHub Actions
   - Fix applied in `.github/workflows/*.yml`

2. **Missing Project References**
   - WPF project had invalid reference to non-existent `MachineClient.Core`
   - Fixed by commenting out invalid reference

3. **Database Connection Issues**
   - Check Docker container status
   - Verify connection string in `.env.local`
   - Ensure database initialization completed

### Debug Commands

```bash
# Check container status
docker-compose -f docker-compose.dev.yml ps

# View container logs
docker-compose -f docker-compose.dev.yml logs database

# Test database connection
docker-compose -f docker-compose.dev.yml exec database mysql -u appuser -p

# Restart services
docker-compose -f docker-compose.dev.yml restart
```

## 📝 Next Steps

1. **Complete API Project**: Create MachineManagement.API project
2. **Add Unit Tests**: Create test projects với xUnit
3. **Database Migrations**: Setup EF Core migrations
4. **Manager Web App**: Create Blazor Server application
5. **Production Deployment**: Setup production infrastructure
6. **Advanced Monitoring**: Complete observability stack

## 📞 Support

- **Repository**: https://github.com/thanhnvbk92/machine-management-system
- **Issues**: GitHub Issues for bug reports
- **Documentation**: `/docs` directory
- **CI/CD Status**: GitHub Actions tab