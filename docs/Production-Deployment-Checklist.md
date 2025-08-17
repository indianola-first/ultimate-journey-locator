# Production Deployment Checklist

## Overview

This checklist ensures a successful and secure deployment of the LocationFinder application to production. Follow each section in order and verify all items before proceeding to the next step.

## 🏗️ Pre-Deployment Preparation

### Environment Setup

- [ ] **Production Server Configuration**
  - [ ] Server specifications meet requirements (2GB RAM, 2 CPU cores minimum)
  - [ ] Windows Server 2019/2022 with IIS 10+ installed
  - [ ] .NET 9 Runtime installed
  - [ ] SQL Server 2019+ installed and configured
  - [ ] SSL certificate obtained and installed
  - [ ] Domain name configured and DNS propagated

- [ ] **Database Preparation**
  - [ ] Production SQL Server instance created
  - [ ] Database user with appropriate permissions created
  - [ ] Connection string tested and verified
  - [ ] Backup strategy implemented
  - [ ] Database maintenance plan configured

- [ ] **Network Configuration**
  - [ ] Firewall rules configured for ports 80, 443, 1433 (SQL)
  - [ ] Load balancer configured (if applicable)
  - [ ] CDN configured for static assets
  - [ ] SSL/TLS certificates installed and configured

### Code Preparation

- [ ] **Source Code Review**
  - [ ] All code reviewed and approved
  - [ ] Security vulnerabilities addressed
  - [ ] Performance optimizations implemented
  - [ ] Error handling comprehensive
  - [ ] Logging configured appropriately

- [ ] **Configuration Files**
  - [ ] `appsettings.Production.json` configured
  - [ ] Connection strings updated for production
  - [ ] CORS settings updated with production domains
  - [ ] Logging levels set for production
  - [ ] Security headers configured

- [ ] **Dependencies**
  - [ ] All NuGet packages updated to latest stable versions
  - [ ] Angular dependencies updated
  - [ ] Security patches applied
  - [ ] License compliance verified

## 🚀 Build and Package

### Backend Build

- [ ] **Clean Build**
  ```bash
  dotnet clean
  dotnet restore
  dotnet build --configuration Release
  ```

- [ ] **Database Migration**
  ```bash
  dotnet ef database update --environment Production
  ```

- [ ] **Test Execution**
  ```bash
  dotnet test --configuration Release --no-build
  ```

- [ ] **Publish Application**
  ```bash
  dotnet publish --configuration Release --output ./publish
  ```

### Frontend Build

- [ ] **Angular Build**
  ```bash
  cd LocationFinder.Client
  npm install
  ng build --configuration production
  ```

- [ ] **Copy to API wwwroot**
  ```bash
  # Ensure Angular build output is copied to API wwwroot
  # This should be automated in the build process
  ```

- [ ] **Asset Optimization**
  - [ ] JavaScript minification verified
  - [ ] CSS minification verified
  - [ ] Image optimization completed
  - [ ] Gzip compression enabled

## 📦 Deployment Process

### File Deployment

- [ ] **Application Files**
  - [ ] All published files copied to production server
  - [ ] File permissions set correctly
  - [ ] Application pool configured
  - [ ] IIS site configured

- [ ] **Configuration Files**
  - [ ] `web.config` deployed and configured
  - [ ] `appsettings.Production.json` deployed
  - [ ] SSL certificate configured
  - [ ] URL rewriting rules configured

- [ ] **Static Assets**
  - [ ] Angular build files in wwwroot
  - [ ] Static file serving configured
  - [ ] Cache headers set appropriately
  - [ ] CDN integration verified

### Database Deployment

- [ ] **Migration Execution**
  ```bash
  dotnet ef database update --environment Production
  ```

- [ ] **Data Import**
  ```bash
  cd LocationFinder.DataImport
  dotnet run -- zipcodes --file Data/production-zipcodes.json
  dotnet run -- locations --file Data/production-locations.json
  ```

- [ ] **Data Validation**
  ```bash
  dotnet run -- validate
  ```

## 🔒 Security Configuration

### SSL/TLS Setup

- [ ] **Certificate Installation**
  - [ ] SSL certificate installed in IIS
  - [ ] Certificate binding configured
  - [ ] HTTPS redirection enabled
  - [ ] HSTS headers configured

- [ ] **Security Headers**
  ```xml
  <httpProtocol>
    <customHeaders>
      <add name="X-Content-Type-Options" value="nosniff" />
      <add name="X-Frame-Options" value="SAMEORIGIN" />
      <add name="X-XSS-Protection" value="1; mode=block" />
      <add name="Referrer-Policy" value="strict-origin-when-cross-origin" />
      <add name="Content-Security-Policy" value="frame-ancestors 'self' https://yourdomain.com;" />
    </customHeaders>
  </httpProtocol>
  ```

### CORS Configuration

- [ ] **Production Domains**
  ```csharp
  policy.WithOrigins(
      "https://yourdomain.com",
      "https://www.yourdomain.com"
  )
  ```

- [ ] **CORS Testing**
  - [ ] Test from WordPress site
  - [ ] Test from different subdomains
  - [ ] Verify preflight requests work

### Database Security

- [ ] **Connection Security**
  - [ ] SQL Server configured for SSL
  - [ ] Database user has minimal required permissions
  - [ ] Connection string encrypted
  - [ ] Network access restricted

## 🧪 Testing and Verification

### Health Checks

- [ ] **Basic Health Check**
  ```bash
  curl https://yourdomain.com/api/health
  ```

- [ ] **Detailed Health Check**
  ```bash
  curl https://yourdomain.com/api/health/detailed
  ```

- [ ] **Database Health Check**
  ```bash
  curl https://yourdomain.com/api/health/database
  ```

- [ ] **System Health Check**
  ```bash
  curl https://yourdomain.com/api/health/system
  ```

### Functional Testing

- [ ] **API Endpoints**
  - [ ] Search endpoint working
  - [ ] Error handling working
  - [ ] Response times acceptable
  - [ ] CORS working correctly

- [ ] **Frontend Application**
  - [ ] Angular app loads correctly
  - [ ] Search functionality working
  - [ ] Responsive design working
  - [ ] Error states handled properly

- [ ] **WordPress Integration**
  - [ ] Iframe loads correctly
  - [ ] Communication between WordPress and app working
  - [ ] Styling consistent with WordPress theme
  - [ ] Mobile responsiveness verified

### Performance Testing

- [ ] **Load Testing**
  - [ ] Application handles expected load
  - [ ] Database queries optimized
  - [ ] Response times under 2 seconds
  - [ ] Memory usage stable

- [ ] **Stress Testing**
  - [ ] Application handles peak load
  - [ ] Graceful degradation under stress
  - [ ] Error recovery working
  - [ ] Resource cleanup working

## 📊 Monitoring and Logging

### Application Monitoring

- [ ] **Logging Configuration**
  - [ ] File logging enabled
  - [ ] Log rotation configured
  - [ ] Log levels appropriate for production
  - [ ] Error logging comprehensive

- [ ] **Performance Monitoring**
  - [ ] Application performance counters configured
  - [ ] Database performance monitoring enabled
  - [ ] Response time monitoring configured
  - [ ] Error rate monitoring enabled

- [ ] **Health Monitoring**
  - [ ] Health check endpoints monitored
  - [ ] Database connectivity monitored
  - [ ] System resources monitored
  - [ ] Alerting configured

### Error Tracking

- [ ] **Exception Handling**
  - [ ] Global exception handler configured
  - [ ] Errors logged with context
  - [ ] Error notifications configured
  - [ ] Error recovery procedures documented

## 🔄 Backup and Recovery

### Database Backup

- [ ] **Backup Strategy**
  - [ ] Full backup scheduled daily
  - [ ] Transaction log backup every 15 minutes
  - [ ] Backup retention policy configured
  - [ ] Backup verification automated

- [ ] **Recovery Testing**
  - [ ] Database restore tested
  - [ ] Point-in-time recovery tested
  - [ ] Recovery procedures documented
  - [ ] Recovery time objectives defined

### Application Backup

- [ ] **File Backup**
  - [ ] Application files backed up
  - [ ] Configuration files backed up
  - [ ] Static assets backed up
  - [ ] Backup verification automated

## 📈 Performance Optimization

### Caching

- [ ] **Response Caching**
  - [ ] API response caching configured
  - [ ] Static file caching configured
  - [ ] Database query caching enabled
  - [ ] Cache invalidation strategy defined

- [ ] **CDN Configuration**
  - [ ] Static assets served via CDN
  - [ ] CDN caching rules configured
  - [ ] Cache warming strategy implemented
  - [ ] CDN performance monitored

### Database Optimization

- [ ] **Indexing**
  - [ ] Database indexes created
  - [ ] Query performance optimized
  - [ ] Index maintenance scheduled
  - [ ] Performance monitoring enabled

- [ ] **Connection Pooling**
  - [ ] Connection pooling configured
  - [ ] Pool size optimized
  - [ ] Connection timeout configured
  - [ ] Pool monitoring enabled

## 🔧 Maintenance Procedures

### Regular Maintenance

- [ ] **Database Maintenance**
  - [ ] Index rebuild scheduled
  - [ ] Statistics update scheduled
  - [ ] Database integrity checks scheduled
  - [ ] Maintenance window defined

- [ ] **Application Maintenance**
  - [ ] Log cleanup scheduled
  - [ ] Temporary file cleanup scheduled
  - [ ] Application restart procedures documented
  - [ ] Maintenance notifications configured

### Update Procedures

- [ ] **Deployment Process**
  - [ ] Zero-downtime deployment configured
  - [ ] Rollback procedures documented
  - [ ] Update testing procedures defined
  - [ ] Change management process established

## 📋 Post-Deployment Verification

### Final Testing

- [ ] **End-to-End Testing**
  - [ ] Complete user workflow tested
  - [ ] All features working correctly
  - [ ] Performance meets requirements
  - [ ] Security requirements met

- [ ] **User Acceptance Testing**
  - [ ] Stakeholder approval received
  - [ ] User feedback collected
  - [ ] Issues documented and prioritized
  - [ ] Go-live decision made

### Documentation

- [ ] **Operational Documentation**
  - [ ] Runbook created
  - [ ] Troubleshooting guide created
  - [ ] Contact information documented
  - [ ] Escalation procedures defined

- [ ] **User Documentation**
  - [ ] User guide created
  - [ ] FAQ created
  - [ ] Support contact information provided
  - [ ] Training materials prepared

## 🚨 Go-Live Checklist

### Pre-Go-Live

- [ ] **Final Verification**
  - [ ] All health checks passing
  - [ ] Performance metrics acceptable
  - [ ] Security scan completed
  - [ ] Backup procedures tested

- [ ] **Team Readiness**
  - [ ] Support team briefed
  - [ ] Monitoring alerts configured
  - [ ] Escalation procedures tested
  - [ ] Emergency contacts confirmed

### Go-Live

- [ ] **Deployment**
  - [ ] Application deployed to production
  - [ ] DNS updated (if applicable)
  - [ ] Load balancer configured
  - [ ] SSL certificate active

- [ ] **Verification**
  - [ ] Application accessible
  - [ ] All features working
  - [ ] Performance acceptable
  - [ ] Monitoring active

### Post-Go-Live

- [ ] **Monitoring**
  - [ ] Monitor application performance
  - [ ] Monitor error rates
  - [ ] Monitor user feedback
  - [ ] Monitor system resources

- [ ] **Support**
  - [ ] Support team available
  - [ ] Issues tracked and resolved
  - [ ] Performance optimized
  - [ ] User training completed

## 📞 Emergency Procedures

### Incident Response

- [ ] **Emergency Contacts**
  - [ ] System administrator contact
  - [ ] Database administrator contact
  - [ ] Network administrator contact
  - [ ] Vendor support contacts

- [ ] **Escalation Procedures**
  - [ ] Issue classification defined
  - [ ] Escalation levels defined
  - [ ] Response times defined
  - [ ] Communication procedures defined

### Rollback Procedures

- [ ] **Application Rollback**
  - [ ] Previous version available
  - [ ] Rollback procedures tested
  - [ ] Data migration procedures defined
  - [ ] Rollback decision criteria defined

---

## ✅ Deployment Sign-Off

**Deployment Completed By:** _________________  
**Date:** _________________  
**Time:** _________________  

**Verified By:** _________________  
**Date:** _________________  
**Time:** _________________  

**Approved By:** _________________  
**Date:** _________________  
**Time:** _________________  

---

**Last Updated**: January 2024  
**Version**: 1.0.0
