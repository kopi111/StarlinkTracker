using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarlinkTracker.Data;
using StarlinkTracker.Models;

namespace StarlinkTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly StarlinkDbContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(StarlinkDbContext context, ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Dashboard/stats
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStats>> GetStats()
    {
        var devices = await _context.StarlinkDevices.ToListAsync();

        var stats = new DashboardStats
        {
            TotalDevices = devices.Count,
            ActiveDevices = devices.Count(d => d.Status == DeviceStatus.Active),
            OfflineDevices = devices.Count(d => d.Status == DeviceStatus.Offline),
            MaintenanceNeeded = devices.Count(d => d.Status == DeviceStatus.MaintenanceNeeded),
            PoliceStationCount = devices.Count(d => d.LocationType == LocationType.PoliceStation)
        };

        // Devices by parish
        stats.DevicesByParish = devices
            .GroupBy(d => d.Parish)
            .ToDictionary(g => g.Key, g => g.Count());

        // Devices by location type
        stats.DevicesByLocationType = devices
            .GroupBy(d => d.LocationType.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        // Parish breakdown
        stats.ParishBreakdown = devices
            .GroupBy(d => d.Parish)
            .Select(g => new ParishSummary
            {
                Parish = g.Key,
                TotalDevices = g.Count(),
                ActiveDevices = g.Count(d => d.Status == DeviceStatus.Active),
                OfflineDevices = g.Count(d => d.Status == DeviceStatus.Offline),
                PoliceStations = g.Count(d => d.LocationType == LocationType.PoliceStation)
            })
            .OrderByDescending(p => p.TotalDevices)
            .ToList();

        return Ok(stats);
    }

    // GET: api/Dashboard/police-stations
    [HttpGet("police-stations")]
    public async Task<ActionResult<IEnumerable<StarlinkDevice>>> GetPoliceStations()
    {
        var policeStations = await _context.StarlinkDevices
            .Where(d => d.LocationType == LocationType.PoliceStation)
            .OrderBy(d => d.Parish)
            .ThenBy(d => d.LocationName)
            .ToListAsync();

        return Ok(policeStations);
    }

    // GET: api/Dashboard/recent
    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<StarlinkDevice>>> GetRecentInstallations(
        [FromQuery] int days = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);

        var recentDevices = await _context.StarlinkDevices
            .Where(d => d.InstallationDate >= cutoffDate)
            .OrderByDescending(d => d.InstallationDate)
            .ToListAsync();

        return Ok(recentDevices);
    }

    // GET: api/Dashboard/alerts
    [HttpGet("alerts")]
    public async Task<ActionResult<object>> GetAlerts()
    {
        var offlineDevices = await _context.StarlinkDevices
            .Where(d => d.Status == DeviceStatus.Offline)
            .ToListAsync();

        var maintenanceDevices = await _context.StarlinkDevices
            .Where(d => d.Status == DeviceStatus.MaintenanceNeeded)
            .ToListAsync();

        return Ok(new
        {
            offline = offlineDevices,
            maintenanceNeeded = maintenanceDevices,
            totalAlerts = offlineDevices.Count + maintenanceDevices.Count
        });
    }
}
