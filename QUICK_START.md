# Quick Start Guide - Starlink Tracker Jamaica

## 🚀 Getting Started in 3 Steps

### 1. Run the Application
```bash
cd StarlinkTracker
dotnet run
```

### 2. Access the System
Open your browser and navigate to:
- **Interactive Map**: http://localhost:5000
- **API Documentation**: http://localhost:5000/swagger

### 3. Explore the Features

#### View the Map
The main page shows an interactive map of Jamaica with:
- 6 pre-loaded police stations in Kingston Metropolitan Area
- Color-coded markers (Green=Active, Red=Offline, Yellow=Maintenance)
- Heat map showing device concentration zones in red
- Click any marker for detailed device information

#### Filter Devices
Use the dropdown filters to view:
- Specific parishes (e.g., "St. Andrew", "Kingston")
- Only police stations
- Active, offline, or maintenance devices

#### Export Data
- **Excel Export**: Download comprehensive spreadsheet with all device data
- **GeoJSON Export**: Export for use in GIS applications like QGIS or ArcGIS

## 📊 Sample Data Included

The system comes with 6 police stations already configured:

| Device ID | Location | Parish | Status |
|-----------|----------|--------|--------|
| SL-KGN-001 | Half Way Tree Police Station | St. Andrew | Active |
| SL-KGN-002 | Constant Spring Police Station | St. Andrew | Active |
| SL-KGN-003 | Central Police Station | Kingston | Active |
| SL-KGN-004 | Matilda's Corner Police Station | St. Andrew | Active |
| SL-KGN-005 | Hunts Bay Police Station | St. Andrew | Maintenance |
| SL-STK-001 | Spanish Town Police Station | St. Catherine | Active |

## 🔧 Adding New Devices

### Method 1: Using the API (via Swagger)
1. Navigate to http://localhost:5000/swagger
2. Expand `POST /api/StarlinkDevices`
3. Click "Try it out"
4. Enter device details in JSON format
5. Click "Execute"

**Example JSON**:
```json
{
  "deviceId": "SL-MBY-001",
  "serialNumber": "UTY2D34L11",
  "installationDate": "2025-03-15",
  "locationName": "Montego Bay Police Station",
  "physicalAddress": "Corner of Barnett & Market Streets",
  "parish": "St. James",
  "latitude": 18.4719,
  "longitude": -77.9183,
  "locationType": "PoliceStation",
  "status": "Active",
  "contactPerson": "Superintendent Williams",
  "contactPhone": "876-952-3333",
  "notes": "Main police station for Montego Bay"
}
```

### Method 2: Using cURL
```bash
curl -X POST "http://localhost:5000/api/StarlinkDevices" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": "SL-MBY-001",
    "serialNumber": "UTY2D34L11",
    "installationDate": "2025-03-15",
    "locationName": "Montego Bay Police Station",
    "physicalAddress": "Corner of Barnett & Market Streets",
    "parish": "St. James",
    "latitude": 18.4719,
    "longitude": -77.9183,
    "locationType": "PoliceStation",
    "status": "Active",
    "contactPerson": "Superintendent Williams",
    "contactPhone": "876-952-3333",
    "notes": "Main police station for Montego Bay"
  }'
```

### Method 3: Using PowerShell (Windows)
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/StarlinkDevices" `
  -Method Post `
  -ContentType "application/json" `
  -Body (@{
    deviceId = "SL-MBY-001"
    serialNumber = "UTY2D34L11"
    installationDate = "2025-03-15"
    locationName = "Montego Bay Police Station"
    physicalAddress = "Corner of Barnett & Market Streets"
    parish = "St. James"
    latitude = 18.4719
    longitude = -77.9183
    locationType = "PoliceStation"
    status = "Active"
    contactPerson = "Superintendent Williams"
    contactPhone = "876-952-3333"
    notes = "Main police station for Montego Bay"
  } | ConvertTo-Json)
```

## 📍 Finding GPS Coordinates

### Online Tools
1. **Google Maps**: Right-click location → "What's here?" → Copy coordinates
2. **OpenStreetMap**: https://www.openstreetmap.org → Search location → Right-click → "Show address"
3. **GPS Coordinates**: https://www.gps-coordinates.net/

### Coordinates for Major Jamaica Police Stations

| Location | Parish | Latitude | Longitude |
|----------|--------|----------|-----------|
| Half Way Tree | St. Andrew | 18.0122 | -76.7955 |
| Central (Downtown) | Kingston | 17.9686 | -76.7940 |
| Montego Bay | St. James | 18.4719 | -77.9183 |
| May Pen | Clarendon | 17.9652 | -77.2433 |
| Ocho Rios | St. Ann | 18.4078 | -77.1032 |
| Mandeville | Manchester | 18.0418 | -77.5042 |
| Port Antonio | Portland | 18.1787 | -76.4519 |
| Savanna-la-Mar | Westmoreland | 18.2185 | -78.1329 |
| Black River | St. Elizabeth | 18.0257 | -77.8515 |
| Falmouth | Trelawny | 18.4919 | -77.6565 |

## 🗺️ Valid Parish Names

**IMPORTANT**: Parish names must match exactly (case-sensitive):

1. Kingston
2. St. Andrew
3. St. Thomas
4. Portland
5. St. Mary
6. St. Ann
7. Trelawny
8. St. James
9. Hanover
10. Westmoreland
11. St. Elizabeth
12. Manchester
13. Clarendon
14. St. Catherine

## 📈 Dashboard Statistics

Access real-time statistics:
```bash
curl http://localhost:5000/api/Dashboard/stats
```

Returns:
- Total devices
- Active devices count
- Offline devices count
- Devices needing maintenance
- Police station count
- Breakdown by parish
- Breakdown by location type

## 🔄 Updating Device Status

When a device needs maintenance or goes offline:

```bash
# Get the device first
curl http://localhost:5000/api/StarlinkDevices/1

# Update with PUT (include ALL fields)
curl -X PUT "http://localhost:5000/api/StarlinkDevices/1" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "deviceId": "SL-KGN-001",
    "serialNumber": "UTY2D34E33",
    "installationDate": "2025-01-15",
    "locationName": "Half Way Tree Police Station",
    "physicalAddress": "1 Hagley Park Road, Kingston 10",
    "parish": "St. Andrew",
    "latitude": 18.0122,
    "longitude": -76.7955,
    "locationType": "PoliceStation",
    "status": "MaintenanceNeeded",
    "contactPerson": "Superintendent Brown",
    "contactPhone": "876-926-8184",
    "notes": "Antenna requires realignment after storm"
  }'
```

## 🎯 Common Tasks

### View All Police Stations
```bash
curl "http://localhost:5000/api/StarlinkDevices?locationType=PoliceStation"
```

### View Devices in Specific Parish
```bash
curl "http://localhost:5000/api/StarlinkDevices?parish=St. Andrew"
```

### View Offline Devices
```bash
curl "http://localhost:5000/api/Dashboard/alerts"
```

### Export All Data to Excel
Open in browser:
```
http://localhost:5000/api/StarlinkDevices/export/excel
```

### Export Police Stations to GeoJSON
Open in browser:
```
http://localhost:5000/api/StarlinkDevices/export/geojson?locationType=PoliceStation
```

## 🎨 Map Features Explained

### Color Coding
- **🟢 Green Markers**: Device is operational and active
- **🟡 Yellow Markers**: Device needs maintenance or servicing
- **🔴 Red Markers**: Device is offline or not responding
- **⚫ Gray Markers**: Device has been decommissioned

### Icons
- **🛡️ Shield Icon**: Police Station (priority locations)
- **📍 Pin Icon**: Other location types

### Heat Map (Red Zones)
- Shows concentration of **active** devices only
- Darker red = higher concentration
- Helps identify coverage density
- Automatically updates when filtering

### Marker Clusters
- Red circles with numbers show grouped devices
- Click to zoom in and separate markers
- Prevents map clutter in dense areas

## 🚨 Troubleshooting

### Port Already in Use
If you get an error about port 5000/5001:
```bash
# Use a different port
dotnet run --urls "http://localhost:5500;https://localhost:5501"
```

### Database Issues
Delete and recreate:
```bash
rm starlink.db
dotnet run
```

### Map Not Loading
- Check internet connection (needs CDN access for Leaflet/Font Awesome)
- Check browser console for errors (F12)
- Try a different browser

### API Returns Empty Results
The database might not have been created:
```bash
# Force database creation
dotnet run
# Wait for "Application started" message, then Ctrl+C
dotnet run
```

## 📱 Next Steps

1. **Add More Devices**: Start with police stations in other parishes
2. **Expand Coverage**: Add schools, hospitals, government buildings
3. **Monitor Status**: Regular checks using the dashboard
4. **Export Reports**: Generate Excel reports for management
5. **Track Trends**: Use the dashboard to identify problem areas

## 💡 Tips

- Use consistent Device ID naming (e.g., SL-{Parish}-{Number})
- Always include contact information for police stations
- Update status promptly when issues arise
- Use the notes field to track maintenance history
- Export data regularly for backup
- Filter by parish to focus on specific regions

## 🆘 Need Help?

- **API Documentation**: http://localhost:5000/swagger
- **Full README**: See README.md for complete documentation
- **Sample Requests**: Check Swagger UI for interactive examples

## 📊 Performance Tips

- The map auto-refreshes every 60 seconds
- Use filters to reduce data load for large datasets
- Export Excel for offline analysis
- Use GeoJSON export for integration with other GIS tools
- Database is lightweight SQLite - suitable for thousands of devices

---

**Happy Tracking! 🛰️ 🇯🇲**
