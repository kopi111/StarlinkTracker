using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarlinkTracker.Data;
using StarlinkTracker.Models;
using StarlinkTracker.Services;

namespace StarlinkTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StarlinkDevicesController : ControllerBase
{
    private readonly StarlinkDbContext _context;
    private readonly ExcelExportService _excelService;
    private readonly GeoJsonService _geoJsonService;
    private readonly ILogger<StarlinkDevicesController> _logger;

    public StarlinkDevicesController(
        StarlinkDbContext context,
        ExcelExportService excelService,
        GeoJsonService geoJsonService,
        ILogger<StarlinkDevicesController> logger)
    {
        _context = context;
        _excelService = excelService;
        _geoJsonService = geoJsonService;
        _logger = logger;
    }

    // GET: api/StarlinkDevices
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StarlinkDevice>>> GetDevices(
        [FromQuery] string? parish = null,
        [FromQuery] LocationType? locationType = null,
        [FromQuery] DeviceStatus? status = null)
    {
        var query = _context.StarlinkDevices.AsQueryable();

        if (!string.IsNullOrEmpty(parish))
            query = query.Where(d => d.Parish == parish);

        if (locationType.HasValue)
            query = query.Where(d => d.LocationType == locationType.Value);

        if (status.HasValue)
            query = query.Where(d => d.Status == status.Value);

        return await query.OrderBy(d => d.Parish).ThenBy(d => d.LocationName).ToListAsync();
    }

    // GET: api/StarlinkDevices/5
    [HttpGet("{id}")]
    public async Task<ActionResult<StarlinkDevice>> GetDevice(int id)
    {
        var device = await _context.StarlinkDevices.FindAsync(id);

        if (device == null)
            return NotFound();

        return device;
    }

    // POST: api/StarlinkDevices
    [HttpPost]
    public async Task<ActionResult<StarlinkDevice>> CreateDevice(StarlinkDevice device)
    {
        if (!JamaicaParishes.IsValidParish(device.Parish))
            return BadRequest($"Invalid parish. Must be one of: {string.Join(", ", JamaicaParishes.AllParishes)}");

        device.CreatedAt = DateTime.UtcNow;

        _context.StarlinkDevices.Add(device);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDevice), new { id = device.Id }, device);
    }

    // PUT: api/StarlinkDevices/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDevice(int id, StarlinkDevice device)
    {
        if (id != device.Id)
            return BadRequest();

        if (!JamaicaParishes.IsValidParish(device.Parish))
            return BadRequest($"Invalid parish. Must be one of: {string.Join(", ", JamaicaParishes.AllParishes)}");

        device.LastUpdated = DateTime.UtcNow;
        _context.Entry(device).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await DeviceExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/StarlinkDevices/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDevice(int id)
    {
        var device = await _context.StarlinkDevices.FindAsync(id);
        if (device == null)
            return NotFound();

        _context.StarlinkDevices.Remove(device);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/StarlinkDevices/export/excel
    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportToExcel(
        [FromQuery] string? parish = null,
        [FromQuery] LocationType? locationType = null)
    {
        var query = _context.StarlinkDevices.AsQueryable();

        if (!string.IsNullOrEmpty(parish))
            query = query.Where(d => d.Parish == parish);

        if (locationType.HasValue)
            query = query.Where(d => d.LocationType == locationType.Value);

        var devices = await query.ToListAsync();
        var excelData = _excelService.ExportDevicesToExcel(devices);

        var fileName = $"Starlink_Devices_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    // GET: api/StarlinkDevices/export/geojson
    [HttpGet("export/geojson")]
    public async Task<IActionResult> ExportToGeoJson(
        [FromQuery] string? parish = null,
        [FromQuery] LocationType? locationType = null,
        [FromQuery] DeviceStatus? status = null)
    {
        var query = _context.StarlinkDevices.AsQueryable();

        if (!string.IsNullOrEmpty(parish))
            query = query.Where(d => d.Parish == parish);

        if (locationType.HasValue)
            query = query.Where(d => d.LocationType == locationType.Value);

        if (status.HasValue)
            query = query.Where(d => d.Status == status.Value);

        var devices = await query.ToListAsync();
        var geoJson = _geoJsonService.GenerateGeoJson(devices);

        return Content(geoJson, "application/geo+json");
    }

    // GET: api/StarlinkDevices/heatmap
    [HttpGet("heatmap")]
    public async Task<IActionResult> GetHeatmapData()
    {
        var devices = await _context.StarlinkDevices.ToListAsync();
        var heatmapData = _geoJsonService.GenerateHeatMapData(devices);

        return Content(heatmapData, "application/json");
    }

    // GET: api/StarlinkDevices/parishes
    [HttpGet("parishes")]
    public ActionResult<IEnumerable<string>> GetParishes()
    {
        return Ok(JamaicaParishes.AllParishes);
    }

    private async Task<bool> DeviceExists(int id)
    {
        return await _context.StarlinkDevices.AnyAsync(e => e.Id == id);
    }
}
