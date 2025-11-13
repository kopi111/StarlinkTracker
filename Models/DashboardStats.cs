namespace StarlinkTracker.Models;

public class DashboardStats
{
    public int TotalDevices { get; set; }
    public int ActiveDevices { get; set; }
    public int OfflineDevices { get; set; }
    public int MaintenanceNeeded { get; set; }
    public Dictionary<string, int> DevicesByParish { get; set; } = new();
    public Dictionary<string, int> DevicesByLocationType { get; set; } = new();
    public int PoliceStationCount { get; set; }
    public List<ParishSummary> ParishBreakdown { get; set; } = new();
}

public class ParishSummary
{
    public string Parish { get; set; } = string.Empty;
    public int TotalDevices { get; set; }
    public int ActiveDevices { get; set; }
    public int OfflineDevices { get; set; }
    public int PoliceStations { get; set; }
}
