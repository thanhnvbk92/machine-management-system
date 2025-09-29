#!/bin/bash

# Database Backup Script for Production
# Used by Docker container in production environment

set -e

# Configuration
BACKUP_DIR="/backups"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="machine_management_backup_${TIMESTAMP}.sql"
RETENTION_DAYS=${BACKUP_RETENTION_DAYS:-30}

# Database credentials from Docker secrets
DB_HOST="database"
DB_PORT="3306"
DB_NAME="machine_management_db"
DB_ROOT_PASSWORD=$(cat /run/secrets/db_root_password 2>/dev/null || echo "admin123")

# Logging function
log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1"
}

# Create backup directory if it doesn't exist
mkdir -p "$BACKUP_DIR"

log "Starting database backup..."

# Perform database backup
mysqldump \
    --host="$DB_HOST" \
    --port="$DB_PORT" \
    --user="root" \
    --password="$DB_ROOT_PASSWORD" \
    --single-transaction \
    --routines \
    --triggers \
    --all-databases > "$BACKUP_DIR/$BACKUP_FILE"

# Check if backup was successful
if [ $? -eq 0 ]; then
    log "Database backup completed successfully: $BACKUP_FILE"
    
    # Compress backup
    gzip "$BACKUP_DIR/$BACKUP_FILE"
    log "Backup compressed: ${BACKUP_FILE}.gz"
    
    # Set proper permissions
    chmod 600 "$BACKUP_DIR/${BACKUP_FILE}.gz"
    
    # Remove old backups
    log "Cleaning up old backups (older than $RETENTION_DAYS days)..."
    find "$BACKUP_DIR" -name "machine_management_backup_*.sql.gz" -type f -mtime +$RETENTION_DAYS -delete
    
    # Log backup info
    BACKUP_SIZE=$(du -h "$BACKUP_DIR/${BACKUP_FILE}.gz" | cut -f1)
    log "Backup size: $BACKUP_SIZE"
    log "Available backups:"
    ls -la "$BACKUP_DIR"/machine_management_backup_*.sql.gz | tail -5
    
else
    log "ERROR: Database backup failed!"
    exit 1
fi

log "Backup script completed successfully"