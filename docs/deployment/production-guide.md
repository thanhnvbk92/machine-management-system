# Production Deployment Guide

## 🏭 Production Infrastructure

### Prerequisites

- **Docker**: 20.10+ với Docker Compose V2
- **Hardware**: 
  - CPU: 4+ cores
  - RAM: 8GB+ recommended
  - Storage: 50GB+ SSD
  - Network: Stable internet connection
- **OS**: Linux (Ubuntu 20.04+ recommended)
- **Domain**: SSL certificate cho production domain

### Server Setup

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER

# Install Docker Compose V2
sudo apt install docker-compose-plugin

# Create application directory
sudo mkdir -p /opt/machine-management
sudo chown $USER:$USER /opt/machine-management
cd /opt/machine-management
```

## 🔐 Security Configuration

### 1. Create Production Secrets

```bash
# Database secrets
openssl rand -base64 32 | docker secret create db_root_password -
openssl rand -base64 32 | docker secret create db_user_password -

# Redis secret
openssl rand -base64 32 | docker secret create redis_password -

# Grafana admin password
openssl rand -base64 16 | docker secret create grafana_admin_password -

# Application connection string
echo "Server=database;Database=machine_management_db;Uid=appuser;Pwd=$(openssl rand -base64 32);Port=3306;" | \
    docker secret create connection_string -
```

### 2. SSL Certificate Setup

```bash
# Using Let's Encrypt với Certbot
sudo apt install certbot
sudo certbot certonly --standalone -d your-domain.com

# Copy certificates to nginx directory
sudo cp /etc/letsencrypt/live/your-domain.com/fullchain.pem ./nginx/ssl/
sudo cp /etc/letsencrypt/live/your-domain.com/privkey.pem ./nginx/ssl/
```

### 3. Firewall Configuration

```bash
# Enable UFW
sudo ufw enable

# Allow SSH
sudo ufw allow ssh

# Allow HTTP/HTTPS
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Allow database access (internal only)
sudo ufw deny 3306/tcp
```

## 📦 Application Deployment

### 1. Clone Repository

```bash
git clone https://github.com/thanhnvbk92/machine-management-system.git
cd machine-management-system
git checkout main
```

### 2. Configure Production Environment

```bash
# Create production environment file
cat > .env.production << EOF
# Production Configuration
VERSION=v1.0.0
DB_NAME=machine_management_prod
BACKUP_SCHEDULE=0 2 * * *
TZ=UTC

# Nginx Configuration
DOMAIN=your-domain.com
SSL_CERT_PATH=./nginx/ssl/fullchain.pem
SSL_KEY_PATH=./nginx/ssl/privkey.pem

# Resource Limits
API_MEMORY_LIMIT=512m
MANAGER_MEMORY_LIMIT=512m
DB_MEMORY_LIMIT=2g

# Monitoring
GRAFANA_PORT=3000
PROMETHEUS_PORT=9090
EOF
```

### 3. Build Docker Images

```bash
# Build production images
docker build -f Dockerfile.backend -t machine-management/backend:v1.0.0 .
docker build -f Dockerfile.manager -t machine-management/manager:v1.0.0 .

# Tag as latest
docker tag machine-management/backend:v1.0.0 machine-management/backend:latest
docker tag machine-management/manager:v1.0.0 machine-management/manager:latest
```

### 4. Database Initialization

```bash
# Start database first
docker-compose -f docker-compose.prod.yml up -d database

# Wait for database to be ready
while ! docker-compose -f docker-compose.prod.yml exec -T database mysqladmin ping -h localhost --silent; do
    echo "Waiting for database..."
    sleep 5
done

# Run database migrations (when API project is ready)
# docker-compose -f docker-compose.prod.yml exec backend-api dotnet ef database update
```

### 5. Full Stack Deployment

```bash
# Deploy all services
docker-compose -f docker-compose.prod.yml up -d

# Enable monitoring
docker-compose -f docker-compose.prod.yml --profile monitoring up -d

# Enable backup
docker-compose -f docker-compose.prod.yml --profile backup up -d

# Check all services
docker-compose -f docker-compose.prod.yml ps
```

## 📊 Health Check & Monitoring

### 1. Service Health Verification

```bash
# Check all containers
docker-compose -f docker-compose.prod.yml ps

# Check specific service logs
docker-compose -f docker-compose.prod.yml logs backend-api
docker-compose -f docker-compose.prod.yml logs manager-web
docker-compose -f docker-compose.prod.yml logs database

# Test application endpoints
curl -f http://localhost/health
curl -f http://localhost/api/health
```

### 2. Monitoring Setup

**Grafana Dashboard**: http://your-domain.com:3000
- Username: admin
- Password: From `grafana_admin_password` secret
- Import dashboards từ `monitoring/grafana/dashboards/`

**Prometheus Metrics**: http://your-domain.com:9090
- Application metrics
- Infrastructure metrics
- Custom business metrics

### 3. Log Aggregation

```bash
# View aggregated logs
docker-compose -f docker-compose.prod.yml logs -f --tail=100

# Export logs for analysis
docker-compose -f docker-compose.prod.yml logs > application-logs-$(date +%Y%m%d).log
```

## 🔄 Backup & Recovery

### 1. Automated Database Backup

Database backups chạy daily lúc 2 AM (configurable trong `.env.production`):

```bash
# Manual backup
docker-compose -f docker-compose.prod.yml exec backup /backup.sh

# View backup files
ls -la backups/

# Restore from backup
docker-compose -f docker-compose.prod.yml exec database \
    mysql -u root -p < backups/backup-YYYYMMDD.sql
```

### 2. Application Data Backup

```bash
# Backup Docker volumes
docker run --rm -v machine-management_mysql_data:/data \
    -v $(pwd)/backups:/backup alpine tar czf /backup/mysql-data-$(date +%Y%m%d).tar.gz /data

# Backup configuration
tar czf config-backup-$(date +%Y%m%d).tar.gz \
    .env.production docker-compose.prod.yml nginx/ monitoring/
```

### 3. Disaster Recovery Plan

1. **Database Recovery**:
   - Stop all services
   - Restore database from latest backup
   - Run migration validation
   - Restart services

2. **Full System Recovery**:
   - Restore configuration files
   - Rebuild Docker images
   - Restore database data
   - Restart monitoring stack

## ⚡ Scaling & Performance

### 1. Horizontal Scaling

```bash
# Scale API services
docker-compose -f docker-compose.prod.yml up -d --scale backend-api=5

# Scale Manager services
docker-compose -f docker-compose.prod.yml up -d --scale manager-web=3

# View scaled services
docker-compose -f docker-compose.prod.yml ps
```

### 2. Performance Tuning

**Database Optimization**:
```sql
-- Enable query cache
SET GLOBAL query_cache_type = ON;
SET GLOBAL query_cache_size = 268435456; -- 256MB

-- Optimize innodb
SET GLOBAL innodb_buffer_pool_size = 1073741824; -- 1GB
```

**nginx Load Balancing**:
```nginx
upstream backend {
    least_conn;
    server backend-api:8080 weight=1 max_fails=3 fail_timeout=30s;
    # Additional servers auto-discovered
}
```

### 3. Resource Monitoring

```bash
# Monitor resource usage
docker stats

# Check disk usage
df -h
du -sh /opt/machine-management/

# Monitor network
netstat -tlnp | grep :80
netstat -tlnp | grep :443
```

## 🚨 Troubleshooting

### Common Production Issues

1. **Service Won't Start**
   ```bash
   # Check logs
   docker-compose -f docker-compose.prod.yml logs service-name
   
   # Check resource usage
   docker stats
   
   # Restart specific service
   docker-compose -f docker-compose.prod.yml restart service-name
   ```

2. **Database Connection Issues**
   ```bash
   # Test database connectivity
   docker-compose -f docker-compose.prod.yml exec database \
       mysql -u root -p -e "SELECT 1"
   
   # Check database processes
   docker-compose -f docker-compose.prod.yml exec database \
       mysql -u root -p -e "SHOW PROCESSLIST"
   ```

3. **High Resource Usage**
   ```bash
   # Analyze memory usage
   docker-compose -f docker-compose.prod.yml exec backend-api \
       cat /proc/meminfo
   
   # Check application metrics
   curl http://localhost:9090/metrics
   ```

### Emergency Procedures

1. **Rollback Deployment**
   ```bash
   # Tag current version
   docker tag machine-management/backend:latest machine-management/backend:rollback
   
   # Deploy previous version
   docker-compose -f docker-compose.prod.yml pull
   docker-compose -f docker-compose.prod.yml up -d
   ```

2. **Service Restart**
   ```bash
   # Graceful restart
   docker-compose -f docker-compose.prod.yml restart
   
   # Force restart
   docker-compose -f docker-compose.prod.yml down
   docker-compose -f docker-compose.prod.yml up -d
   ```

## 🔄 Maintenance

### Regular Maintenance Tasks

1. **Weekly Tasks**
   - Check service health
   - Review monitoring alerts
   - Update security patches
   - Clean old Docker images

2. **Monthly Tasks**
   - Database optimization
   - Log rotation
   - Backup verification
   - Performance review

3. **Quarterly Tasks**
   - Dependency updates
   - Security audit
   - Disaster recovery testing
   - Capacity planning

### Update Procedures

```bash
# Update application
git pull origin main
docker-compose -f docker-compose.prod.yml build
docker-compose -f docker-compose.prod.yml up -d

# Update base images
docker-compose -f docker-compose.prod.yml pull
docker-compose -f docker-compose.prod.yml up -d
```

## 📞 Production Support

### Monitoring Alerts
- Service down notifications
- High resource usage warnings
- Database connection alerts
- SSL certificate expiration

### Contact Information
- **Operations Team**: ops@company.com
- **Development Team**: dev@company.com
- **Emergency Contact**: +84-xxx-xxx-xxxx

### Support Tools
- **Grafana**: Real-time monitoring
- **Portainer**: Container management
- **Adminer**: Database administration
- **GitHub Actions**: Deployment pipeline