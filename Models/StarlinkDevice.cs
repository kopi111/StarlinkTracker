using System.ComponentModel.DataAnnotations;

namespace StarlinkTracker.Models;

public class StarlinkDevice
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string DeviceId { get; set; } = string.Empty;

    [StringLength(100)]
    public string? SerialNumber { get; set; }

    [Required]
    public DateTime InstallationDate { get; set; }

    [Required]
    [StringLength(200)]
    public string LocationName { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string PhysicalAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Parish { get; set; } = string.Empty;

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    [Required]
    public LocationType LocationType { get; set; }

    [Required]
    public DeviceStatus Status { get; set; }

    [StringLength(100)]
    public string? ContactPerson { get; set; }

    [StringLength(20)]
    public string? ContactPhone { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastUpdated { get; set; }
}

public enum LocationType
{
    PoliceStation,
    School,
    Hospital,
    Government,
    Business,
    Community,
    Other
}

public enum DeviceStatus
{
    Active,
    MaintenanceNeeded,
    Offline,
    Decommissioned
}
