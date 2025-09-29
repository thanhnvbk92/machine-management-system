#!/bin/bash

# Machine Management System - Development Setup Script
# Thiết lập development environment với Docker

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🚀 Machine Management System - Development Setup${NC}"
echo "=================================================="

# Function to print status
print_status() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    print_error "Docker is not installed. Please install Docker first."
    exit 1
fi

if ! command -v docker-compose &> /dev/null; then
    print_error "Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

print_status "Docker và Docker Compose detected"

# Create .env.local if it doesn't exist
if [ ! -f ".env.local" ]; then
    print_warning "Creating .env.local from template..."
    cp .env.example .env.local
    print_status ".env.local created. You may want to customize it."
else
    print_status ".env.local already exists"
fi

# Create necessary directories
print_status "Creating necessary directories..."
mkdir -p logs/{api,manager}
mkdir -p database/{init,production}
mkdir -p uploads
mkdir -p backups
mkdir -p nginx
mkdir -p monitoring/{grafana,prometheus}

# Create database init script
cat > database/init/01-create-tables.sql << 'EOF'
-- Machine Management System Database Initialization
-- This will be replaced by EF Core migrations when API project is ready

USE machine_management_db;

-- Placeholder table structure
-- TODO: Replace with actual EF Core migrations

CREATE TABLE IF NOT EXISTS system_info (
    id INT AUTO_INCREMENT PRIMARY KEY,
    version VARCHAR(50) NOT NULL,
    initialized_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    notes TEXT
);

INSERT INTO system_info (version, notes) VALUES 
('1.0.0-dev', 'Development database initialized via Docker setup script');

-- Grant permissions
GRANT ALL PRIVILEGES ON machine_management_db.* TO 'appuser'@'%';
FLUSH PRIVILEGES;
EOF

print_status "Database initialization script created"

# Create basic nginx configuration
cat > nginx/nginx.conf << 'EOF'
events {
    worker_connections 1024;
}

http {
    upstream backend {
        server backend-api:8080;
    }
    
    upstream manager {
        server manager-web:8080;
    }
    
    server {
        listen 80;
        server_name localhost;
        
        location /api/ {
            proxy_pass http://backend/;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }
        
        location / {
            proxy_pass http://manager/;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }
        
        location /health {
            return 200 "nginx healthy\n";
            add_header Content-Type text/plain;
        }
    }
}
EOF

print_status "Nginx configuration created"

# Function to start services
start_services() {
    local profile=$1
    print_status "Starting services with profile: $profile"
    
    case $profile in
        "database")
            docker-compose -f docker-compose.dev.yml up -d database
            ;;
        "full")
            docker-compose -f docker-compose.dev.yml --profile api --profile manager --profile cache up -d
            ;;
        "tools")
            docker-compose -f docker-compose.dev.yml --profile tools up -d
            ;;
        *)
            print_status "Available profiles:"
            echo "  database - Only database"
            echo "  tools    - Database + Adminer + Portainer"
            echo "  full     - All services (when implemented)"
            return 0
            ;;
    esac
}

# Parse command line arguments
PROFILE="help"

while [[ $# -gt 0 ]]; do
    case $1 in
        --profile|-p)
            PROFILE="$2"
            shift 2
            ;;
        --help|-h)
            echo "Usage: $0 [--profile|-p PROFILE]"
            echo "Profiles:"
            echo "  database - Start only database"
            echo "  tools    - Start database + management tools"
            echo "  full     - Start all services (when API/Manager are implemented)"
            exit 0
            ;;
        *)
            print_error "Unknown option $1"
            exit 1
            ;;
    esac
done

# Default to database profile for now
if [ "$PROFILE" = "help" ]; then
    PROFILE="database"
fi

# Start services
print_status "Setting up development environment..."
start_services $PROFILE

# Wait for database to be ready
if [ "$PROFILE" != "help" ]; then
    print_status "Waiting for database to be ready..."
    timeout=60
    while [ $timeout -gt 0 ]; do
        if docker-compose -f docker-compose.dev.yml exec -T database mysqladmin ping -h localhost -u root -padmin123 &>/dev/null; then
            break
        fi
        echo -n "."
        sleep 2
        ((timeout-=2))
    done
    
    if [ $timeout -le 0 ]; then
        print_error "Database failed to start within 60 seconds"
        exit 1
    fi
    
    print_status "Database is ready!"
fi

# Display service URLs
echo ""
echo -e "${BLUE}🔗 Service URLs:${NC}"
echo "================================"

if docker-compose -f docker-compose.dev.yml ps | grep -q "database.*Up"; then
    echo "📊 Database: localhost:3306"
fi

if docker-compose -f docker-compose.dev.yml ps | grep -q "adminer.*Up"; then
    echo "🛠️  Adminer (DB Admin): http://localhost:8080"
fi

if docker-compose -f docker-compose.dev.yml ps | grep -q "portainer.*Up"; then
    echo "🐳 Portainer (Container Management): http://localhost:9000"
fi

echo ""
print_status "Development environment setup completed!"

if [ "$PROFILE" = "database" ]; then
    print_warning "Only database is running. Use --profile tools to start management tools"
    print_warning "API and Manager services will be available when implemented"
fi

echo ""
echo -e "${BLUE}📋 Next Steps:${NC}"
echo "1. Connect to database: mysql -h localhost -u appuser -puserpass123 machine_management_db"
echo "2. Start developing: dotnet run --project src/ClientApp/MachineClient.WPF"
echo "3. Run tests: dotnet test (when test projects are added)"
echo "4. Stop services: docker-compose -f docker-compose.dev.yml down"