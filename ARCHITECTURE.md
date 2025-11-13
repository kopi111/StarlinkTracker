# System Architecture - Starlink Tracker Jamaica

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENT LAYER                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │   Web        │  │   Mobile     │  │   Desktop    │          │
│  │   Browser    │  │   Browser    │  │   API Client │          │
│  │              │  │              │  │              │          │
│  │  map.html    │  │  map.html    │  │  cURL/Postman│          │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘          │
│         │                  │                  │                   │
│         └──────────────────┴──────────────────┘                   │
│                            │                                      │
└────────────────────────────┼──────────────────────────────────────┘
                             │
                             │ HTTPS/HTTP
                             │
┌────────────────────────────┼──────────────────────────────────────┐
│                    WEB SERVER LAYER                               │
├────────────────────────────┼──────────────────────────────────────┤
│                            │                                      │
│  ┌────────────────────────────────────────────────────┐          │
│  │         ASP.NET Core 8.0 Web API                   │          │
│  │                                                      │          │
│  │  ┌────────────────────────────────────────────┐    │          │
│  │  │         Middleware Pipeline                 │    │          │
│  │  │  - CORS                                     │    │          │
│  │  │  - Static Files (wwwroot)                  │    │          │
│  │  │  - HTTPS Redirection                       │    │          │
│  │  │  - Authorization                           │    │          │
│  │  │  - Routing                                 │    │          │
│  │  └────────────────────────────────────────────┘    │          │
│  │                                                      │          │
│  │  ┌────────────────────────────────────────────┐    │          │
│  │  │          API Controllers                    │    │          │
│  │  │                                             │    │          │
│  │  │  ┌──────────────────────────────────┐      │    │          │
│  │  │  │ StarlinkDevicesController        │      │    │          │
│  │  │  │  - GET /api/StarlinkDevices     │      │    │          │
│  │  │  │  - POST /api/StarlinkDevices    │      │    │          │
│  │  │  │  - PUT /api/StarlinkDevices/{id}│      │    │          │
│  │  │  │  - DELETE /api/StarlinkDevices/{id}│   │    │          │
│  │  │  │  - GET /export/excel            │      │    │          │
│  │  │  │  - GET /export/geojson          │      │    │          │
│  │  │  └──────────────────────────────────┘      │    │          │
│  │  │                                             │    │          │
│  │  │  ┌──────────────────────────────────┐      │    │          │
│  │  │  │ DashboardController              │      │    │          │
│  │  │  │  - GET /api/Dashboard/stats     │      │    │          │
│  │  │  │  - GET /api/Dashboard/police... │      │    │          │
│  │  │  │  - GET /api/Dashboard/recent    │      │    │          │
│  │  │  │  - GET /api/Dashboard/alerts    │      │    │          │
│  │  │  └──────────────────────────────────┘      │    │          │
│  │  └────────────────────────────────────────────┘    │          │
│  └────────────────────────────────────────────────────┘          │
└───────────────────────────┬──────────────────────────────────────┘
                            │
                            │
┌───────────────────────────┼──────────────────────────────────────┐
│                   SERVICE LAYER                                   │
├───────────────────────────┼──────────────────────────────────────┤
│                           │                                       │
│  ┌────────────────────────┴────────────────────────┐             │
│  │                                                   │             │
│  │  ┌─────────────────────┐  ┌─────────────────┐   │             │
│  │  │ ExcelExportService  │  │ GeoJsonService  │   │             │
│  │  │                     │  │                 │   │             │
│  │  │ - ExportToExcel()   │  │ - GenerateGeo   │   │             │
│  │  │ - CreateSummary()   │  │   Json()        │   │             │
│  │  │ - CreateParish      │  │ - GenerateHeat  │   │             │
│  │  │   Breakdown()       │  │   MapData()     │   │             │
│  │  └─────────────────────┘  └─────────────────┘   │             │
│  │                                                   │             │
│  └───────────────────────────────────────────────────┘             │
└───────────────────────────┬──────────────────────────────────────┘
                            │
                            │
┌───────────────────────────┼──────────────────────────────────────┐
│                   DATA ACCESS LAYER                               │
├───────────────────────────┼──────────────────────────────────────┤
│                           │                                       │
│  ┌────────────────────────┴────────────────────────┐             │
│  │      Entity Framework Core 8.0                  │             │
│  │                                                  │             │
│  │  ┌─────────────────────────────────────┐        │             │
│  │  │     StarlinkDbContext               │        │             │
│  │  │                                     │        │             │
│  │  │  - DbSet<StarlinkDevice>           │        │             │
│  │  │  - OnModelCreating()               │        │             │
│  │  │  - GetSeedData()                   │        │             │
│  │  └─────────────────────────────────────┘        │             │
│  │                                                  │             │
│  │  ┌─────────────────────────────────────┐        │             │
│  │  │         Domain Models               │        │             │
│  │  │                                     │        │             │
│  │  │  - StarlinkDevice                  │        │             │
│  │  │  - DashboardStats                  │        │             │
│  │  │  - ParishSummary                   │        │             │
│  │  │  - JamaicaParishes (static)        │        │             │
│  │  └─────────────────────────────────────┘        │             │
│  └──────────────────────────────────────────────────┘             │
└───────────────────────────┬──────────────────────────────────────┘
                            │
                            │
┌───────────────────────────┼──────────────────────────────────────┐
│                   DATABASE LAYER                                  │
├───────────────────────────┼──────────────────────────────────────┤
│                           │                                       │
│  ┌────────────────────────┴────────────────────────┐             │
│  │         SQLite Database (starlink.db)           │             │
│  │                                                  │             │
│  │  ┌──────────────────────────────────────┐       │             │
│  │  │      StarlinkDevices Table           │       │             │
│  │  │                                      │       │             │
│  │  │  Columns:                           │       │             │
│  │  │  - Id (PK, Auto)                    │       │             │
│  │  │  - DeviceId (Unique, Indexed)       │       │             │
│  │  │  - SerialNumber                     │       │             │
│  │  │  - InstallationDate                 │       │             │
│  │  │  - LocationName                     │       │             │
│  │  │  - PhysicalAddress                  │       │             │
│  │  │  - Parish (Indexed)                 │       │             │
│  │  │  - Latitude                         │       │             │
│  │  │  - Longitude                        │       │             │
│  │  │  - LocationType (Enum, Indexed)     │       │             │
│  │  │  - Status (Enum, Indexed)           │       │             │
│  │  │  - ContactPerson                    │       │             │
│  │  │  - ContactPhone                     │       │             │
│  │  │  - Notes                            │       │             │
│  │  │  - CreatedAt                        │       │             │
│  │  │  - LastUpdated                      │       │             │
│  │  │                                      │       │             │
│  │  │  Indexes:                           │       │             │
│  │  │  - IX_DeviceId                      │       │             │
│  │  │  - IX_Parish                        │       │             │
│  │  │  - IX_LocationType                  │       │             │
│  │  │  - IX_Status                        │       │             │
│  │  └──────────────────────────────────────┘       │             │
│  └──────────────────────────────────────────────────┘             │
└───────────────────────────────────────────────────────────────────┘
```

## Data Flow Diagrams

### 1. GET Device List Flow

```
User Browser
    │
    │ HTTP GET /api/StarlinkDevices?parish=Kingston
    ▼
StarlinkDevicesController
    │
    │ GetDevices(parish, locationType, status)
    ▼
Entity Framework Core
    │
    │ LINQ Query with Filters
    ▼
SQLite Database
    │
    │ SELECT * FROM StarlinkDevices WHERE Parish = 'Kingston'
    ▼
Entity Framework Core
    │
    │ Materialized List<StarlinkDevice>
    ▼
StarlinkDevicesController
    │
    │ JSON Serialization
    ▼
User Browser
    │
    │ Display on Map
    ▼
Leaflet.js renders markers
```

### 2. Excel Export Flow

```
User Browser
    │
    │ Click "Export to Excel"
    ▼
HTTP GET /api/StarlinkDevices/export/excel
    │
    ▼
StarlinkDevicesController
    │
    │ Retrieve filtered devices
    ▼
Entity Framework Core
    │
    │ Query Database
    ▼
ExcelExportService
    │
    ├─ Create Main Data Sheet
    ├─ Create Summary Sheet
    └─ Create Parish Breakdown Sheet
    │
    │ EPPlus generates .xlsx
    ▼
StarlinkDevicesController
    │
    │ File(bytes, content-type, filename)
    ▼
User Browser
    │
    │ Browser downloads file
    ▼
User opens Excel file
```

### 3. Create New Device Flow

```
User Browser
    │
    │ Fill form / JSON payload
    ▼
HTTP POST /api/StarlinkDevices
    │
    │ JSON Body
    ▼
StarlinkDevicesController
    │
    │ Model Binding & Validation
    ▼
Parish Validation
    │
    │ JamaicaParishes.IsValidParish()
    ▼
Entity Framework Core
    │
    │ _context.StarlinkDevices.Add(device)
    │ _context.SaveChangesAsync()
    ▼
SQLite Database
    │
    │ INSERT INTO StarlinkDevices
    │ Return Auto-generated Id
    ▼
StarlinkDevicesController
    │
    │ CreatedAtAction(201)
    ▼
User Browser
    │
    │ Success response
    ▼
Map auto-refreshes (60s)
    │
    │ New marker appears
    ▼
Device visible on map
```

### 4. Map Visualization Flow

```
map.html loads
    │
    ├─ Load Leaflet.js from CDN
    ├─ Load Leaflet.markercluster from CDN
    ├─ Load Leaflet.heat from CDN
    └─ Load Font Awesome from CDN
    │
    ▼
Initialize Map
    │
    │ Center on Jamaica [18.1096, -77.2975]
    │ Zoom level 9
    ▼
Fetch Parishes
    │
    │ GET /api/StarlinkDevices/parishes
    ▼
Populate Filter Dropdowns
    │
    ▼
Fetch Devices
    │
    │ GET /api/StarlinkDevices
    ▼
Process Device Data
    │
    ├─ Create Markers (color-coded)
    ├─ Create Marker Clusters (red bubbles)
    ├─ Create Heat Map (red zones)
    └─ Bind Popups
    │
    ▼
Render on Map
    │
    ▼
Fetch Statistics
    │
    │ GET /api/Dashboard/stats
    ▼
Update Dashboard Cards
    │
    ▼
Setup Event Listeners
    │
    ├─ Filter change → reload devices
    ├─ Export buttons → trigger downloads
    └─ Auto-refresh timer (60s)
    │
    ▼
Map ready for interaction
```

## Component Interaction Diagram

```
┌─────────────────┐
│   wwwroot/      │
│   map.html      │──────┐
└─────────────────┘      │
                         │ HTTP Requests
┌─────────────────┐      │
│  Swagger UI     │──────┤
└─────────────────┘      │
                         │
┌─────────────────┐      │
│  External API   │──────┤
│  Clients        │      │
└─────────────────┘      │
                         │
                         ▼
              ┌─────────────────────┐
              │                     │
              │  Controllers        │
              │                     │
              │  ┌───────────────┐  │
              │  │ Starlink      │  │
              │  │ Devices       │  │
              │  └───────┬───────┘  │
              │          │          │
              │  ┌───────▼───────┐  │
              │  │ Dashboard     │  │
              │  └───────┬───────┘  │
              │          │          │
              └──────────┼──────────┘
                         │
         ┌───────────────┼───────────────┐
         │               │               │
         ▼               ▼               ▼
┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│   Excel     │  │   GeoJson   │  │    EF Core  │
│   Export    │  │   Service   │  │   Context   │
│   Service   │  │             │  │             │
└─────────────┘  └─────────────┘  └──────┬──────┘
                                          │
                                          │
                                          ▼
                                  ┌──────────────┐
                                  │   SQLite     │
                                  │   Database   │
                                  └──────────────┘
```

## Technology Stack Layers

```
┌─────────────────────────────────────────────────┐
│              PRESENTATION LAYER                  │
│  - HTML5 (map.html)                             │
│  - CSS3 (Inline styles)                         │
│  - JavaScript (Vanilla JS)                      │
│  - Leaflet.js + Plugins                         │
│  - Font Awesome Icons                           │
└─────────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────────┐
│              APPLICATION LAYER                   │
│  - ASP.NET Core 8.0 Web API                     │
│  - C# .NET 8                                    │
│  - Controllers (REST endpoints)                 │
│  - Middleware Pipeline                          │
│  - Dependency Injection                         │
│  - Swagger/OpenAPI                              │
└─────────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────────┐
│              BUSINESS LOGIC LAYER                │
│  - Services (Excel, GeoJson)                    │
│  - Models (Domain Entities)                     │
│  - Validation Logic                             │
│  - Business Rules                               │
└─────────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────────┐
│              DATA ACCESS LAYER                   │
│  - Entity Framework Core 8.0                    │
│  - DbContext                                    │
│  - LINQ Queries                                 │
│  - Change Tracking                              │
│  - Migrations                                   │
└─────────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────────┐
│              DATABASE LAYER                      │
│  - SQLite (Development)                         │
│  - SQL Server (Production Ready)                │
│  - PostgreSQL (Production Ready)                │
└─────────────────────────────────────────────────┘
```

## Deployment Architectures

### Development Architecture

```
┌────────────────────────────────────┐
│     Developer Workstation          │
│                                    │
│  ┌──────────────────────────────┐  │
│  │  dotnet run                  │  │
│  │  Port 5000/5001             │  │
│  └──────────────────────────────┘  │
│                                    │
│  ┌──────────────────────────────┐  │
│  │  starlink.db (SQLite)        │  │
│  └──────────────────────────────┘  │
│                                    │
│  ┌──────────────────────────────┐  │
│  │  Browser → localhost:5000    │  │
│  └──────────────────────────────┘  │
└────────────────────────────────────┘
```

### Production Architecture (Azure)

```
                    Internet
                       │
                       ▼
              ┌────────────────┐
              │  Azure Front   │
              │  Door / CDN    │
              └────────┬───────┘
                       │
                       ▼
              ┌────────────────┐
              │  Azure App     │
              │  Service       │
              │  (Web API)     │
              └────────┬───────┘
                       │
                       ▼
              ┌────────────────┐
              │  Azure SQL     │
              │  Database      │
              └────────────────┘
                       │
                       ▼
              ┌────────────────┐
              │  Azure Blob    │
              │  Storage       │
              │  (Excel files) │
              └────────────────┘
```

### Production Architecture (Linux Server)

```
                    Internet
                       │
                       ▼
              ┌────────────────┐
              │  Cloudflare    │
              │  DNS + CDN     │
              └────────┬───────┘
                       │
                       ▼
              ┌────────────────┐
              │  Nginx         │
              │  Reverse Proxy │
              │  SSL/TLS       │
              └────────┬───────┘
                       │
                       ▼
              ┌────────────────┐
              │  ASP.NET Core  │
              │  Web API       │
              │  (systemd)     │
              └────────┬───────┘
                       │
                       ▼
              ┌────────────────┐
              │  PostgreSQL    │
              │  Database      │
              └────────────────┘
```

## Security Architecture

```
┌─────────────────────────────────────────────────┐
│              SECURITY LAYERS                     │
├─────────────────────────────────────────────────┤
│                                                  │
│  Layer 1: Transport Security                    │
│  ┌────────────────────────────────────────┐     │
│  │  - HTTPS/TLS Encryption                │     │
│  │  - SSL Certificate                     │     │
│  │  - HSTS Headers                        │     │
│  └────────────────────────────────────────┘     │
│                                                  │
│  Layer 2: Authentication & Authorization        │
│  ┌────────────────────────────────────────┐     │
│  │  - JWT Bearer Tokens (optional)        │     │
│  │  - Role-based Access Control           │     │
│  │  - API Key Authentication              │     │
│  └────────────────────────────────────────┘     │
│                                                  │
│  Layer 3: Input Validation                      │
│  ┌────────────────────────────────────────┐     │
│  │  - Model Validation Attributes         │     │
│  │  - Parish Name Validation              │     │
│  │  - Data Type Checking                  │     │
│  │  - Required Field Validation           │     │
│  └────────────────────────────────────────┘     │
│                                                  │
│  Layer 4: Data Access Security                  │
│  ┌────────────────────────────────────────┐     │
│  │  - Parameterized Queries (EF Core)     │     │
│  │  - SQL Injection Prevention            │     │
│  │  - Connection String Encryption        │     │
│  └────────────────────────────────────────┘     │
│                                                  │
│  Layer 5: Application Security                  │
│  ┌────────────────────────────────────────┐     │
│  │  - CORS Policy                         │     │
│  │  - Rate Limiting                       │     │
│  │  - Request Size Limits                 │     │
│  │  - Exception Handling                  │     │
│  └────────────────────────────────────────┘     │
└─────────────────────────────────────────────────┘
```

## Performance Optimization Points

```
1. Database Level
   - Indexed columns (DeviceId, Parish, LocationType, Status)
   - Efficient queries via EF Core LINQ
   - Connection pooling

2. Application Level
   - Async/await throughout
   - Response caching (configurable)
   - Response compression
   - Minimal middleware pipeline

3. Frontend Level
   - CDN for libraries
   - Marker clustering (reduces DOM elements)
   - Lazy loading of map tiles
   - Debounced filter changes

4. Network Level
   - GZIP compression
   - Static file caching
   - HTTP/2 support
   - Browser caching headers
```

## Scalability Patterns

```
Vertical Scaling:
- Increase CPU/RAM on single server
- Suitable for up to 10,000 concurrent users

Horizontal Scaling:
┌─────────────┐
│  Load       │
│  Balancer   │
└──────┬──────┘
       │
   ┌───┴───┬────────┬────────┐
   ▼       ▼        ▼        ▼
┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐
│ API  │ │ API  │ │ API  │ │ API  │
│ Node │ │ Node │ │ Node │ │ Node │
└───┬──┘ └───┬──┘ └───┬──┘ └───┬──┘
    └────────┴────────┴────────┘
              │
              ▼
    ┌──────────────────┐
    │  Shared Database │
    └──────────────────┘
```

## Monitoring Architecture

```
Application
    │
    ├─ Logs → Serilog → File System / Cloud
    │
    ├─ Metrics → Application Insights / Prometheus
    │
    ├─ Health Checks → /health endpoint
    │
    └─ Exceptions → Error Tracking Service
```

---

**This architecture supports:**
- Rapid development and testing
- Easy deployment to multiple platforms
- Horizontal and vertical scaling
- Security best practices
- Performance optimization
- Monitoring and observability
