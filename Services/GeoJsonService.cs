using System.Text.Json;
using StarlinkTracker.Models;

namespace StarlinkTracker.Services;

public class GeoJsonService
{
    public string GenerateGeoJson(List<StarlinkDevice> devices)
    {
        var features = devices.Select(device => new
        {
            type = "Feature",
            geometry = new
            {
                type = "Point",
                coordinates = new[] { device.Longitude, device.Latitude }
            },
            properties = new
            {
                id = device.Id,
                deviceId = device.DeviceId,
                serialNumber = device.SerialNumber,
                locationName = device.LocationName,
                physicalAddress = device.PhysicalAddress,
                parish = device.Parish,
                locationType = device.LocationType.ToString(),
                status = device.Status.ToString(),
                contactPerson = device.ContactPerson,
                contactPhone = device.ContactPhone,
                installationDate = device.InstallationDate.ToString("yyyy-MM-dd"),
                notes = device.Notes,
                // Color coding for map visualization
                markerColor = GetMarkerColor(device.Status),
                markerSize = device.LocationType == LocationType.PoliceStation ? "large" : "medium",
                // Icon based on location type
                icon = GetLocationIcon(device.LocationType)
            }
        }).ToList();

        var geoJson = new
        {
            type = "FeatureCollection",
            features = features
        };

        return JsonSerializer.Serialize(geoJson, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    public string GenerateHeatMapData(List<StarlinkDevice> devices)
    {
        var heatmapPoints = devices
            .Where(d => d.Status == DeviceStatus.Active)
            .Select(device => new[] { device.Latitude, device.Longitude, 1.0 })
            .ToList();

        return JsonSerializer.Serialize(heatmapPoints);
    }

    private static string GetMarkerColor(DeviceStatus status)
    {
        return status switch
        {
            DeviceStatus.Active => "#28a745",          // Green
            DeviceStatus.MaintenanceNeeded => "#ffc107", // Yellow
            DeviceStatus.Offline => "#dc3545",          // Red
            DeviceStatus.Decommissioned => "#6c757d",   // Gray
            _ => "#007bff"                               // Blue (default)
        };
    }

    private static string GetLocationIcon(LocationType locationType)
    {
        return locationType switch
        {
            LocationType.PoliceStation => "shield",
            LocationType.School => "graduation-cap",
            LocationType.Hospital => "hospital",
            LocationType.Government => "landmark",
            LocationType.Business => "building",
            LocationType.Community => "home",
            _ => "map-pin"
        };
    }
}
