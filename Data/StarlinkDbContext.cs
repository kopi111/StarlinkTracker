using Microsoft.EntityFrameworkCore;
using StarlinkTracker.Models;

namespace StarlinkTracker.Data;

public class StarlinkDbContext : DbContext
{
    public StarlinkDbContext(DbContextOptions<StarlinkDbContext> options)
        : base(options)
    {
    }

    public DbSet<StarlinkDevice> StarlinkDevices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StarlinkDevice>(entity =>
        {
            entity.HasIndex(e => e.DeviceId).IsUnique();
            entity.HasIndex(e => e.Parish);
            entity.HasIndex(e => e.LocationType);
            entity.HasIndex(e => e.Status);

            // Seed data for Kingston Metropolitan Area police stations
            entity.HasData(GetSeedData());
        });
    }

    private static List<StarlinkDevice> GetSeedData()
    {
        return new List<StarlinkDevice>
        {
            new StarlinkDevice
            {
                Id = 1,
                DeviceId = "SL-KGN-001",
                SerialNumber = "UTY2D34E33",
                InstallationDate = new DateTime(2025, 1, 15),
                LocationName = "Half Way Tree Police Station",
                PhysicalAddress = "1 Hagley Park Road, Kingston 10",
                Parish = "St. Andrew",
                Latitude = 18.0122,
                Longitude = -76.7955,
                LocationType = LocationType.PoliceStation,
                Status = DeviceStatus.Active,
                ContactPerson = "Superintendent Brown",
                ContactPhone = "876-926-8184",
                Notes = "Main police station for Kingston Metropolitan Area",
                CreatedAt = DateTime.UtcNow
            },
            new StarlinkDevice
            {
                Id = 2,
                DeviceId = "SL-KGN-002",
                SerialNumber = "UTY2D34F44",
                InstallationDate = new DateTime(2025, 1, 20),
                LocationName = "Constant Spring Police Station",
                PhysicalAddress = "Constant Spring Road, Kingston 8",
                Parish = "St. Andrew",
                Latitude = 18.0245,
                Longitude = -76.7970,
                LocationType = LocationType.PoliceStation,
                Status = DeviceStatus.Active,
                ContactPerson = "Inspector Williams",
                ContactPhone = "876-924-1421",
                Notes = "Covers upper St. Andrew area",
                CreatedAt = DateTime.UtcNow
            },
            new StarlinkDevice
            {
                Id = 3,
                DeviceId = "SL-KGN-003",
                SerialNumber = "UTY2D34G55",
                InstallationDate = new DateTime(2025, 2, 1),
                LocationName = "Central Police Station",
                PhysicalAddress = "Duke Street, Kingston",
                Parish = "Kingston",
                Latitude = 17.9686,
                Longitude = -76.7940,
                LocationType = LocationType.PoliceStation,
                Status = DeviceStatus.Active,
                ContactPerson = "Superintendent Davis",
                ContactPhone = "876-922-0223",
                Notes = "Downtown Kingston central station",
                CreatedAt = DateTime.UtcNow
            },
            new StarlinkDevice
            {
                Id = 4,
                DeviceId = "SL-KGN-004",
                SerialNumber = "UTY2D34H66",
                InstallationDate = new DateTime(2025, 2, 5),
                LocationName = "Matilda's Corner Police Station",
                PhysicalAddress = "Old Hope Road, Kingston 6",
                Parish = "St. Andrew",
                Latitude = 18.0158,
                Longitude = -76.7847,
                LocationType = LocationType.PoliceStation,
                Status = DeviceStatus.Active,
                ContactPerson = "Inspector Thompson",
                ContactPhone = "876-927-1131",
                Notes = "University area coverage",
                CreatedAt = DateTime.UtcNow
            },
            new StarlinkDevice
            {
                Id = 5,
                DeviceId = "SL-KGN-005",
                SerialNumber = "UTY2D34I77",
                InstallationDate = new DateTime(2025, 2, 10),
                LocationName = "Hunts Bay Police Station",
                PhysicalAddress = "Spanish Town Road, Kingston 11",
                Parish = "St. Andrew",
                Latitude = 17.9775,
                Longitude = -76.8264,
                LocationType = LocationType.PoliceStation,
                Status = DeviceStatus.MaintenanceNeeded,
                ContactPerson = "Sergeant Campbell",
                ContactPhone = "876-923-8745",
                Notes = "Antenna requires adjustment due to recent storm",
                CreatedAt = DateTime.UtcNow
            },
            new StarlinkDevice
            {
                Id = 6,
                DeviceId = "SL-STK-001",
                SerialNumber = "UTY2D34J88",
                InstallationDate = new DateTime(2025, 2, 15),
                LocationName = "Spanish Town Police Station",
                PhysicalAddress = "Brunswick Avenue, Spanish Town",
                Parish = "St. Catherine",
                Latitude = 17.9913,
                Longitude = -76.9569,
                LocationType = LocationType.PoliceStation,
                Status = DeviceStatus.Active,
                ContactPerson = "Inspector Robinson",
                ContactPhone = "876-984-2305",
                Notes = "Primary station for St. Catherine",
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}
