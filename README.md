# Starlink Device Tracker - Jamaica

A comprehensive tracking system for monitoring Starlink device placements across Jamaica, with special focus on police stations and other key locations. Built with C# .NET 8, ASP.NET Core, Entity Framework Core, and interactive map visualizations.

## Features

### Core Functionality
- **Device Management**: Complete CRUD operations for Starlink devices
- **Interactive Map**: Real-time visualization using Leaflet.js with:
  - Red zone highlighting for device concentrations
  - Heat maps showing device density
  - Custom markers for different location types
  - Color-coded status indicators
  - Cluster grouping for better visibility
- **Excel Export**: Generate comprehensive spreadsheets with:
  - Main device data sheet
  - Summary statistics
  - Parish-by-parish breakdown
- **GeoJSON Export**: Export location data for use in GIS applications
- **Dashboard API**: Real-time statistics and analytics
- **Filtering**: Advanced filtering by parish, location type, and status

### Data Tracking
Each Starlink device tracks:
- Device ID/Serial Number
- Installation Date
- Location Name & Physical Address
- Parish (all 14 Jamaican parishes)
- GPS Coordinates (Latitude/Longitude)
- Location Type (Police Station, School, Hospital, etc.)
- Status (Active, Maintenance Needed, Offline, Decommissioned)
- Contact Information
- Notes/Issues

## Technology Stack

- **Backend**: ASP.NET Core 8.0 Web API
- **Database**: SQLite with Entity Framework Core
- **Excel Generation**: EPPlus
- **Frontend**: HTML5, CSS3, JavaScript
- **Mapping**: Leaflet.js, Leaflet.markercluster, Leaflet.heat
- **Icons**: Font Awesome
- **API Documentation**: Swagger/OpenAPI

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Any modern web browser

### Installation

1. **Clone or navigate to the project directory**:
   ```bash
   cd StarlinkTracker
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

4. **Access the application**:
   - Interactive Map: `https://localhost:5001` or `http://localhost:5000`
   - API Documentation: `https://localhost:5001/swagger`

The database will be automatically created with sample data on first run.

## Sample Data

The system comes pre-loaded with 6 police stations in the Kingston Metropolitan Area:

1. **Half Way Tree Police Station** - St. Andrew (Active)
2. **Constant Spring Police Station** - St. Andrew (Active)
3. **Central Police Station** - Kingston (Active)
4. **Matilda's Corner Police Station** - St. Andrew (Active)
5. **Hunts Bay Police Station** - St. Andrew (Maintenance Needed)
6. **Spanish Town Police Station** - St. Catherine (Active)

## API Endpoints

### Device Management

#### Get All Devices (with filtering)
```http
GET /api/StarlinkDevices?parish={parish}&locationType={type}&status={status}
```

**Query Parameters**:
- `parish`: Filter by Jamaican parish (e.g., "Kingston", "St. Andrew")
- `locationType`: Filter by type (PoliceStation, School, Hospital, Government, Business, Community, Other)
- `status`: Filter by status (Active, MaintenanceNeeded, Offline, Decommissioned)

**Example**:
```bash
GET /api/StarlinkDevices?parish=St. Andrew&locationType=PoliceStation&status=Active
```

#### Get Single Device
```http
GET /api/StarlinkDevices/{id}
```

#### Create New Device
```http
POST /api/StarlinkDevices
Content-Type: application/json

{
  "deviceId": "SL-KGN-007",
  "serialNumber": "UTY2D34K99",
  "installationDate": "2025-03-01",
  "locationName": "New Kingston Police Station",
  "physicalAddress": "20 Dominica Drive, Kingston 5",
  "parish": "Kingston",
  "latitude": 18.0059,
  "longitude": -76.7844,
  "locationType": "PoliceStation",
  "status": "Active",
  "contactPerson": "Inspector Green",
  "contactPhone": "876-926-5555",
  "notes": "Newly installed unit"
}
```

#### Update Device
```http
PUT /api/StarlinkDevices/{id}
Content-Type: application/json

{
  "id": 1,
  "deviceId": "SL-KGN-001",
  "status": "MaintenanceNeeded",
  "notes": "Antenna alignment required",
  ... (all other fields)
}
```

#### Delete Device
```http
DELETE /api/StarlinkDevices/{id}
```

### Export Endpoints

#### Export to Excel
```http
GET /api/StarlinkDevices/export/excel?parish={parish}&locationType={type}
```

Downloads a comprehensive Excel file with:
- Detailed device data
- Summary statistics
- Parish breakdown

#### Export to GeoJSON
```http
GET /api/StarlinkDevices/export/geojson?parish={parish}&locationType={type}&status={status}
```

Returns GeoJSON format data compatible with most GIS applications.

#### Get Heatmap Data
```http
GET /api/StarlinkDevices/heatmap
```

Returns coordinate arrays for heat map visualization.

#### Get Available Parishes
```http
GET /api/StarlinkDevices/parishes
```

Returns list of all Jamaican parishes.

### Dashboard Endpoints

#### Get Dashboard Statistics
```http
GET /api/Dashboard/stats
```

Returns:
```json
{
  "totalDevices": 6,
  "activeDevices": 5,
  "offlineDevices": 0,
  "maintenanceNeeded": 1,
  "policeStationCount": 6,
  "devicesByParish": {
    "St. Andrew": 4,
    "Kingston": 1,
    "St. Catherine": 1
  },
  "devicesByLocationType": {
    "PoliceStation": 6
  },
  "parishBreakdown": [
    {
      "parish": "St. Andrew",
      "totalDevices": 4,
      "activeDevices": 3,
      "offlineDevices": 0,
      "policeStations": 4
    }
  ]
}
```

#### Get Police Stations Only
```http
GET /api/Dashboard/police-stations
```

Returns all devices with location type "PoliceStation".

#### Get Recent Installations
```http
GET /api/Dashboard/recent?days=30
```

Returns devices installed within the specified number of days.

#### Get Alerts
```http
GET /api/Dashboard/alerts
```

Returns devices that are offline or need maintenance.

## Jamaica Parish List

The system supports all 14 Jamaican parishes:

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

## Interactive Map Features

### Visualization Elements
- **Color-coded markers** based on device status:
  - 🟢 Green: Active
  - 🟡 Yellow: Maintenance Needed
  - 🔴 Red: Offline
  - ⚫ Gray: Decommissioned

- **Special police station markers**: Shield icon for easy identification

- **Heat map overlay**: Shows concentration of active devices in red zones

- **Marker clustering**: Automatic grouping of nearby devices with red cluster bubbles

### Filtering Options
- Filter by Parish
- Filter by Location Type
- Filter by Device Status
- Real-time map updates

### Interactive Popups
Click any marker to see:
- Device ID and Serial Number
- Location details
- Contact information
- Current status
- Installation notes

## Excel Export Format

The Excel export includes three sheets:

### 1. Starlink Devices (Main Data)
Columns:
- Device ID
- Serial Number
- Installation Date
- Location Name
- Physical Address
- Parish
- Latitude
- Longitude
- Location Type
- Status (color-coded)
- Contact Person
- Contact Phone
- Notes

### 2. Summary
- Total device count
- Status breakdown
- Location type breakdown

### 3. Parish Breakdown
- Devices per parish
- Status counts per parish
- Police station counts per parish

## Project Structure

```
StarlinkTracker/
├── Controllers/
│   ├── StarlinkDevicesController.cs  # Main device API
│   └── DashboardController.cs         # Statistics & analytics
├── Data/
│   └── StarlinkDbContext.cs           # Database context with seed data
├── Models/
│   ├── StarlinkDevice.cs              # Main device model
│   ├── JamaicaParishes.cs             # Parish validation
│   └── DashboardStats.cs              # Statistics models
├── Services/
│   ├── ExcelExportService.cs          # Excel generation
│   └── GeoJsonService.cs              # GeoJSON export
├── wwwroot/
│   └── map.html                       # Interactive map UI
├── Program.cs                         # Application configuration
└── starlink.db                        # SQLite database (auto-created)
```

## Database Schema

### StarlinkDevices Table
- `Id` (Primary Key)
- `DeviceId` (Unique, Indexed)
- `SerialNumber`
- `InstallationDate`
- `LocationName`
- `PhysicalAddress`
- `Parish` (Indexed)
- `Latitude`
- `Longitude`
- `LocationType` (Enum, Indexed)
- `Status` (Enum, Indexed)
- `ContactPerson`
- `ContactPhone`
- `Notes`
- `CreatedAt`
- `LastUpdated`

## Customization

### Adding New Location Types
Edit `Models/StarlinkDevice.cs`:
```csharp
public enum LocationType
{
    PoliceStation,
    School,
    Hospital,
    Government,
    Business,
    Community,
    FireStation,    // Add new type
    CoastGuard,     // Add new type
    Other
}
```

### Adding More Seed Data
Edit `Data/StarlinkDbContext.cs` and add devices to the `GetSeedData()` method.

### Changing Map Center/Zoom
Edit `wwwroot/map.html`:
```javascript
const map = L.map('map').setView([18.1096, -77.2975], 9);
// [latitude, longitude], zoom level
```

## Security Considerations

- The application uses CORS for API access
- In production, restrict CORS to specific domains
- Consider adding authentication/authorization
- Use HTTPS in production
- Implement rate limiting for API endpoints

## Future Enhancements

Potential additions:
- User authentication and role-based access
- Email alerts for offline devices
- Mobile app integration
- Real-time device monitoring via webhooks
- Automated maintenance scheduling
- Historical data tracking and analytics
- Multi-language support
- PDF report generation
- Integration with Starlink's management API

## Troubleshooting

### Database Issues
If the database becomes corrupted, delete `starlink.db` and restart the application. It will be recreated with seed data.

### Port Conflicts
If ports 5000/5001 are in use, modify `Properties/launchSettings.json` to use different ports.

### Excel Export Fails
Ensure EPPlus package is properly installed:
```bash
dotnet add package EPPlus --version 7.0.0
```

### Map Not Loading
Check browser console for JavaScript errors. Ensure internet connection for CDN resources (Leaflet, Font Awesome).

## License

EPPlus is used under the PolyForm Noncommercial License. For commercial use, a commercial license is required from EPPlus Software.

## Support

For issues or questions:
1. Check the API documentation at `/swagger`
2. Review the browser console for client-side errors
3. Check application logs for server-side errors

## Contributing

When adding new devices:
1. Use the POST API endpoint or
2. Use the interactive map to verify coordinates
3. Ensure parish names match exactly from the valid list
4. Include contact information for all police stations

## Credits

- Built with ASP.NET Core
- Maps powered by Leaflet.js and OpenStreetMap
- Icons by Font Awesome
- Excel generation by EPPlus
