# YÊU CẦU PHI CHỨC NĂNG - SCALED VERSION WITH WEB ADMIN

> **Đơn giản hóa cho hệ thống <1000 clients + Web Admin Portal**

## 1. HIỆU SUẤT (PERFORMANCE) - SCALED

### 1.1 Response Time Requirements

**NFR-PERF-001: API Response Time**
- **Target**: < 1 second cho 95% API calls
- **Critical**: < 500ms cho client registration
- **Acceptable**: < 2 seconds under peak load
- **Test Method**: Load testing với 500 concurrent clients

**NFR-PERF-002: Client Application Performance**
- **File Processing**: Process 1MB log file trong < 10 seconds
- **Memory Usage**: < 100MB RAM per client instance
- **CPU Usage**: < 5% during normal operation
- **Startup Time**: Client service start trong < 30 seconds

**NFR-PERF-003: Web Applications Response**
- **Manager App Page Load**: < 3 seconds cho dashboard
- **Admin Portal Load**: < 2 seconds cho admin pages
- **Search Results**: < 5 seconds cho log search
- **Command Processing**: < 2 seconds từ click đến confirmation
- **CRUD Operations**: < 1 second cho database operations

### 1.2 Throughput Requirements (Scaled Down)

**NFR-PERF-004: Concurrent Clients**
- **Target**: 500 concurrent clients
- **Maximum**: 1000 concurrent clients
- **Degradation**: < 50% performance loss at max capacity
- **Growth**: Plan for 20% yearly increase

**NFR-PERF-005: Data Processing Rate**
- **Log Ingestion**: 1,000 log entries/minute
- **Command Distribution**: 100 commands/minute
- **Batch Processing**: 50 clients can send data simultaneously

---

## 2. ĐỘ TIN CẬY (RELIABILITY) - SIMPLIFIED

### 2.1 Availability Requirements

**NFR-REL-001: System Uptime**
- **Target**: 99% uptime (7.2 hours downtime/month)
- **Planned Maintenance**: < 4 hours/month during off-hours
- **Recovery Time**: < 30 minutes từ system failure
- **Monitoring**: Basic health checks

**NFR-REL-002: Component Reliability**
- **API Server**: 99% availability
- **Client Apps**: Restart automatically on failure
- **Web App**: 99% availability
- **Database**: Single instance với daily backup

### 2.2 Data Integrity (Basic)

**NFR-REL-003: Data Protection**
- **Log Data**: No loss of log entries
- **Commands**: Reliable delivery tracking
- **Configuration**: Version control for config changes
- **Backup**: Daily automated backups

---

## 3. BẢO MẬT (SECURITY) - ENHANCED FOR ADMIN

### 3.1 Authentication & Authorization

**NFR-SEC-001: Web Admin Security**
- **Authentication**: JWT-based authentication với refresh tokens
- **Authorization**: Role-based access control (Super Admin, Admin, Operator, Viewer)
- **Session Management**: 8-hour active session, auto-logout after 30min inactivity
- **Password Policy**: Minimum 12 characters, complexity requirements
- **MFA Support**: Optional two-factor authentication
- **Account Lockout**: 3 failed attempts → 30 minutes lockout

**NFR-SEC-002: Manager App Security (Basic)**
- **Authentication**: Simple username/password cho manager app
- **Session Management**: 8-hour session timeout
- **Password Policy**: Minimum 8 characters
- **Account Lockout**: 5 failed attempts → 15 minutes lockout

**NFR-SEC-003: API Security**
- **Client Security**: Machine-based identification (no passwords)
- **Admin API Security**: JWT token required
- **Rate Limiting**: Prevent API abuse (100 req/min per client)
- **Input Validation**: Sanitize all inputs
- **API Logging**: Log all API access và admin actions

### 3.2 Data Protection

**NFR-SEC-004: Data Security**
- **In Transit**: HTTPS required cho admin portal
- **Admin Authentication**: Encrypted credentials storage
- **Database Access**: Admin actions logged và audited
- **Sensitive Data**: User passwords hashed với bcrypt
- **Session Security**: Secure cookie settings

**NFR-SEC-005: Access Control**
- **Role Permissions**: Granular permissions per role
- **Data Isolation**: Users can only access authorized data
- **Audit Trail**: All admin actions logged with timestamps
- **Database Security**: Connection string encryption

---

## 4. KHẢ NĂNG SỬ DỤNG (USABILITY) - SIMPLIFIED

### 4.1 User Experience

**NFR-USA-001: Interface Design**
- **Simplicity**: Clean, minimal interface
- **Consistency**: Consistent UI patterns
- **Browser Support**: Chrome, Firefox, Edge latest versions
- **Mobile**: Basic mobile responsiveness

**NFR-USA-002: Learning Curve**
- **Training Time**: < 1 day for basic operations
- **Help System**: Basic help text và tooltips
- **Documentation**: User manual PDF
- **Support**: Email support

### 4.2 Language Support

**NFR-USA-003: Localization**
- **Primary**: Vietnamese interface
- **Secondary**: English (if needed)
- **Time Format**: Vietnam timezone (GMT+7)
- **Date Format**: DD/MM/YYYY

---

## 5. TƯƠNG THÍCH (COMPATIBILITY) - BASIC

### 5.1 Platform Requirements

**NFR-COMP-001: Client Requirements**
- **Operating System**: Windows 10/11 (64-bit)
- **Framework**: .NET 6.0 Runtime
- **Memory**: Minimum 2GB RAM
- **Storage**: 100MB free space

**NFR-COMP-002: Server Requirements**
- **Web Server**: IIS 10+ hoặc Docker container
- **Database**: SQL Server 2019+ hoặc PostgreSQL 13+
- **OS**: Windows Server 2019+ hoặc Linux
- **Memory**: 8GB RAM minimum

**NFR-COMP-003: Web Applications**
- **Manager App Browsers**: Chrome 90+, Firefox 88+, Edge 90+
- **Admin Portal Browsers**: Chrome 95+, Firefox 95+, Edge 95+ (modern browsers)
- **Resolution**: 1366x768 minimum, optimized for 1920x1080
- **Internet**: Broadband connection required
- **Mobile Support**: Basic responsive design cho admin portal

---

## 6. KHẢ NĂNG BẢO TRÌ (MAINTAINABILITY) - BASIC

### 6.1 System Maintenance

**NFR-MAIN-001: Deployment**
- **Client Deployment**: MSI installer package
- **Server Deployment**: Simple copy deployment hoặc Docker
- **Updates**: Manual update process
- **Configuration**: File-based configuration

**NFR-MAIN-002: Monitoring**
- **Basic Logging**: Application event logs
- **Health Checks**: Simple ping endpoints
- **Error Tracking**: Log file monitoring
- **Performance**: Basic performance counters

### 6.2 Support Requirements

**NFR-MAIN-003: Documentation**
- **User Manual**: Step-by-step guides
- **Admin Guide**: Installation và configuration
- **API Documentation**: Swagger documentation
- **Troubleshooting**: Common issues và solutions

---

## 7. SCALABILITY - LIMITED

### 7.1 Growth Planning

**NFR-SCALE-001: Capacity Planning**
- **Current**: Support 500 clients comfortably
- **Maximum**: 1000 clients (with performance degradation)
- **Database Growth**: Plan for 5GB/year data growth
- **Future Scaling**: Document scale-up procedures

---

## 8. PERFORMANCE BENCHMARKS (SCALED)

| Metric | Target | Maximum Acceptable | 
|--------|--------|--------------------|
| Concurrent Clients | 500 | 1000 |
| API Response | < 1s | < 2s |
| Admin Portal Response | < 2s | < 3s |
| Log Processing | 1K/min | 2K/min |
| Web Page Load | < 3s | < 5s |
| System Uptime | 99% | 98% |
| Client Memory | < 100MB | < 200MB |
| Database Size | < 10GB | < 20GB |

---

## 9. TRADE-OFFS & SIMPLIFICATIONS

### What We're NOT Implementing:
- ❌ High Availability (clustering, failover)
- ❌ Advanced security (encryption, certificates)
- ❌ Real-time monitoring dashboards
- ❌ Advanced reporting và analytics
- ❌ Multi-tenant architecture
- ❌ Comprehensive audit trails
- ❌ Advanced user management
- ❌ Mobile applications

### What We Focus On:
- ✅ Core functionality reliability
- ✅ Web Admin portal với authentication
- ✅ Role-based access control
- ✅ Database management interface
- ✅ Simple deployment và maintenance
- ✅ Basic monitoring và logging
- ✅ Adequate performance for <1000 clients
- ✅ User-friendly interfaces
- ✅ Cost-effective solution

---

## 10. ACCEPTANCE CRITERIA

### System Performance:
- [ ] Handle 500 clients sending data every minute
- [ ] API responds within 1 second under normal load
- [ ] Manager app loads within 3 seconds
- [ ] Admin portal loads within 2 seconds
- [ ] Client application uses <100MB memory

### Security & Access:
- [ ] Admin authentication working với JWT
- [ ] Role-based access implemented correctly
- [ ] All admin actions are logged
- [ ] Password policies enforced
- [ ] Session timeout working

### Reliability:
- [ ] System runs for 30 days without manual restart
- [ ] No data loss during normal operations
- [ ] Admin portal maintains user sessions
- [ ] Database operations are reliable
- [ ] Automatic recovery from common failures
- [ ] Daily backups complete successfully

### Usability:
- [ ] New user can navigate interface within 30 minutes
- [ ] All features work in Chrome, Firefox, Edge
- [ ] Mobile interface is usable on tablets
- [ ] Help documentation covers all features

### Maintainability:
- [ ] Installation takes <2 hours for experienced admin
- [ ] Configuration changes don't require code changes
- [ ] Log files provide sufficient troubleshooting info
- [ ] Update process documented và tested

---

**Ghi chú**: Đây là phiên bản NFR đơn giản hóa, tập trung vào practical requirements cho hệ thống vừa và nhỏ. Có thể nâng cấp sau khi system stable và có user feedback.