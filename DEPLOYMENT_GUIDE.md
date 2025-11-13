# Deployment Guide - Starlink Tracker Jamaica

## Production Deployment Considerations

### 1. Database Migration

#### From SQLite to Production Database

For production use, consider migrating from SQLite to a more robust database:

**SQL Server**:
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

Update `Program.cs`:
```csharp
builder.Services.AddDbContext<StarlinkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

**PostgreSQL**:
```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

Update `Program.cs`:
```csharp
builder.Services.AddDbContext<StarlinkDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

**Connection String in `appsettings.json`**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=StarlinkTracker;User Id=sa;Password=YourPassword;"
  }
}
```

### 2. Security Hardening

#### Update CORS Policy
Edit `Program.cs` to restrict CORS to specific domains:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

#### Add Authentication
Install packages:
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

Update `Program.cs`:
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        // Configure JWT settings
    });

builder.Services.AddAuthorization();
```

Add to controllers:
```csharp
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StarlinkDevicesController : ControllerBase
```

#### Environment Variables
Store sensitive configuration in environment variables:

```bash
export ConnectionStrings__DefaultConnection="your-connection-string"
export JwtSettings__SecretKey="your-secret-key"
```

### 3. Docker Deployment

Create `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["StarlinkTracker.csproj", "./"]
RUN dotnet restore "StarlinkTracker.csproj"
COPY . .
RUN dotnet build "StarlinkTracker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StarlinkTracker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StarlinkTracker.dll"]
```

Create `docker-compose.yml`:
```yaml
version: '3.8'
services:
  web:
    build: .
    ports:
      - "8080:80"
      - "8443:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=StarlinkTracker;User=sa;Password=YourPassword123!
    depends_on:
      - db

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123!
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql

volumes:
  sqldata:
```

Build and run:
```bash
docker-compose up -d
```

### 4. Azure App Service Deployment

#### Using Azure CLI
```bash
# Login to Azure
az login

# Create resource group
az group create --name StarlinkTrackerRG --location eastus

# Create App Service plan
az appservice plan create --name StarlinkTrackerPlan \
  --resource-group StarlinkTrackerRG \
  --sku B1 --is-linux

# Create web app
az webapp create --name starlink-tracker-jamaica \
  --resource-group StarlinkTrackerRG \
  --plan StarlinkTrackerPlan \
  --runtime "DOTNET|8.0"

# Deploy
dotnet publish -c Release
cd bin/Release/net8.0/publish
zip -r publish.zip .
az webapp deployment source config-zip \
  --resource-group StarlinkTrackerRG \
  --name starlink-tracker-jamaica \
  --src publish.zip
```

#### Configure App Settings
```bash
az webapp config appsettings set \
  --resource-group StarlinkTrackerRG \
  --name starlink-tracker-jamaica \
  --settings ConnectionStrings__DefaultConnection="your-connection-string"
```

### 5. Linux Server Deployment (Ubuntu/Debian)

#### Install .NET Runtime
```bash
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-8.0
```

#### Setup Application
```bash
# Create app directory
sudo mkdir -p /var/www/starlink-tracker
cd /var/www/starlink-tracker

# Copy published files
sudo cp -r /path/to/published/files/* .

# Set permissions
sudo chown -R www-data:www-data /var/www/starlink-tracker
```

#### Create systemd Service
Create `/etc/systemd/system/starlink-tracker.service`:
```ini
[Unit]
Description=Starlink Tracker API
After=network.target

[Service]
WorkingDirectory=/var/www/starlink-tracker
ExecStart=/usr/bin/dotnet /var/www/starlink-tracker/StarlinkTracker.dll
Restart=always
RestartSec=10
SyslogIdentifier=starlink-tracker
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

Enable and start:
```bash
sudo systemctl enable starlink-tracker
sudo systemctl start starlink-tracker
sudo systemctl status starlink-tracker
```

#### Setup Nginx Reverse Proxy
Install Nginx:
```bash
sudo apt-get install nginx
```

Create `/etc/nginx/sites-available/starlink-tracker`:
```nginx
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Enable site:
```bash
sudo ln -s /etc/nginx/sites-available/starlink-tracker /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

#### Setup SSL with Let's Encrypt
```bash
sudo apt-get install certbot python3-certbot-nginx
sudo certbot --nginx -d your-domain.com
```

### 6. Windows Server (IIS) Deployment

#### Install Prerequisites
1. Install .NET 8.0 Hosting Bundle
2. Install IIS with ASP.NET Core Module

#### Publish Application
```powershell
dotnet publish -c Release -o C:\inetpub\starlink-tracker
```

#### Create IIS Site
1. Open IIS Manager
2. Right-click "Sites" → "Add Website"
3. Set:
   - Site name: StarlinkTracker
   - Physical path: C:\inetpub\starlink-tracker
   - Binding: Port 80 (or 443 for HTTPS)
4. Set Application Pool to "No Managed Code"

#### Configure web.config
Automatically created during publish, verify it contains:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
    </handlers>
    <aspNetCore processPath="dotnet"
                arguments=".\StarlinkTracker.dll"
                stdoutLogEnabled="false"
                stdoutLogFile=".\logs\stdout"
                hostingModel="inprocess" />
  </system.webServer>
</configuration>
```

### 7. Performance Optimization

#### Enable Response Compression
Add to `Program.cs`:
```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// After app.Build()
app.UseResponseCompression();
```

#### Add Response Caching
```csharp
builder.Services.AddResponseCaching();

app.UseResponseCaching();
```

Add to controllers:
```csharp
[ResponseCache(Duration = 60)] // Cache for 60 seconds
[HttpGet]
public async Task<ActionResult<IEnumerable<StarlinkDevice>>> GetDevices()
```

#### Connection Pooling
For SQL Server, add to connection string:
```
;Max Pool Size=100;Min Pool Size=5;
```

### 8. Monitoring and Logging

#### Application Insights (Azure)
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

Update `Program.cs`:
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

#### Serilog for File Logging
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
```

Update `Program.cs`:
```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/starlink-tracker-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

### 9. Backup Strategy

#### Database Backup Script (SQL Server)
```sql
BACKUP DATABASE StarlinkTracker
TO DISK = 'C:\Backups\StarlinkTracker_Full.bak'
WITH FORMAT, NAME = 'Full Backup';
```

#### Automated Daily Backup (Linux with cron)
```bash
# Create backup script
cat > /opt/backup-starlink.sh << 'EOF'
#!/bin/bash
DATE=$(date +%Y%m%d)
pg_dump starlink_db > /backups/starlink_$DATE.sql
find /backups -name "starlink_*.sql" -mtime +7 -delete
EOF

chmod +x /opt/backup-starlink.sh

# Add to crontab (daily at 2 AM)
echo "0 2 * * * /opt/backup-starlink.sh" | sudo crontab -
```

### 10. Health Checks

Add health check endpoint to `Program.cs`:
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<StarlinkDbContext>();

app.MapHealthChecks("/health");
```

### 11. API Rate Limiting

Install package:
```bash
dotnet add package AspNetCoreRateLimit
```

Configure in `Program.cs`:
```csharp
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

app.UseIpRateLimiting();
```

Add to `appsettings.json`:
```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 30
      }
    ]
  }
}
```

### 12. Environment-Specific Configuration

Create multiple appsettings files:
- `appsettings.json` (base)
- `appsettings.Development.json`
- `appsettings.Production.json`

Example `appsettings.Production.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Production connection string"
  },
  "AllowedHosts": "*.yourdomain.com"
}
```

### 13. SSL/TLS Configuration

For production, always use HTTPS. Update `Program.cs`:
```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}
```

### 14. Post-Deployment Checklist

- [ ] Database connection verified
- [ ] CORS policy configured for production domain
- [ ] HTTPS/SSL certificate installed
- [ ] Authentication/Authorization enabled
- [ ] Rate limiting configured
- [ ] Logging and monitoring active
- [ ] Health check endpoint responding
- [ ] Backup strategy implemented
- [ ] Error pages customized
- [ ] API documentation accessible
- [ ] Performance testing completed
- [ ] Security scan performed
- [ ] Load testing conducted

### 15. Maintenance Windows

Plan for regular maintenance:
- **Weekly**: Review logs for errors
- **Monthly**: Database optimization, backup verification
- **Quarterly**: Security updates, dependency updates
- **Annually**: Full system audit, DR testing

### Support Contacts

- .NET Issues: https://github.com/dotnet/aspnetcore/issues
- EPPlus License: https://epplussoftware.com/
- Azure Support: https://azure.microsoft.com/support/

---

## Quick Deploy Commands

### Development
```bash
dotnet run
```

### Production Build
```bash
dotnet publish -c Release -o ./publish
```

### Docker
```bash
docker build -t starlink-tracker .
docker run -d -p 8080:80 starlink-tracker
```

### Azure
```bash
az webapp up --name starlink-tracker-jamaica --resource-group StarlinkTrackerRG
```

---

**For production deployments, always test in a staging environment first!**
