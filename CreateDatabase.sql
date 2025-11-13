-- =============================================
-- Starlink Tracker Jamaica - Database Creation Script
-- Database: StarlinkTrackerJamaica
-- Server: 69.164.241.104,1433
-- =============================================

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StarlinkTrackerJamaica')
BEGIN
    CREATE DATABASE StarlinkTrackerJamaica;
    PRINT 'Database StarlinkTrackerJamaica created successfully.';
END
ELSE
BEGIN
    PRINT 'Database StarlinkTrackerJamaica already exists.';
END
GO

USE StarlinkTrackerJamaica;
GO

-- Create StarlinkDevices table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StarlinkDevices]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StarlinkDevices] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [DeviceId] NVARCHAR(100) NOT NULL,
        [SerialNumber] NVARCHAR(100) NULL,
        [InstallationDate] DATETIME2 NOT NULL,
        [LocationName] NVARCHAR(200) NOT NULL,
        [PhysicalAddress] NVARCHAR(300) NOT NULL,
        [Parish] NVARCHAR(50) NOT NULL,
        [Latitude] FLOAT NOT NULL,
        [Longitude] FLOAT NOT NULL,
        [LocationType] INT NOT NULL,
        [Status] INT NOT NULL,
        [ContactPerson] NVARCHAR(100) NULL,
        [ContactPhone] NVARCHAR(20) NULL,
        [Notes] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [LastUpdated] DATETIME2 NULL,

        CONSTRAINT [PK_StarlinkDevices] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    PRINT 'Table StarlinkDevices created successfully.';
END
ELSE
BEGIN
    PRINT 'Table StarlinkDevices already exists.';
END
GO

-- Create indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_StarlinkDevices_DeviceId' AND object_id = OBJECT_ID('StarlinkDevices'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_StarlinkDevices_DeviceId]
        ON [dbo].[StarlinkDevices]([DeviceId] ASC);
    PRINT 'Index IX_StarlinkDevices_DeviceId created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_StarlinkDevices_Parish' AND object_id = OBJECT_ID('StarlinkDevices'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_StarlinkDevices_Parish]
        ON [dbo].[StarlinkDevices]([Parish] ASC);
    PRINT 'Index IX_StarlinkDevices_Parish created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_StarlinkDevices_LocationType' AND object_id = OBJECT_ID('StarlinkDevices'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_StarlinkDevices_LocationType]
        ON [dbo].[StarlinkDevices]([LocationType] ASC);
    PRINT 'Index IX_StarlinkDevices_LocationType created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_StarlinkDevices_Status' AND object_id = OBJECT_ID('StarlinkDevices'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_StarlinkDevices_Status]
        ON [dbo].[StarlinkDevices]([Status] ASC);
    PRINT 'Index IX_StarlinkDevices_Status created.';
END
GO

-- Insert sample data (6 police stations in Kingston Metropolitan Area)
-- LocationType Enum: 0=PoliceStation, 1=School, 2=Hospital, 3=Government, 4=Business, 5=Community, 6=Other
-- Status Enum: 0=Active, 1=MaintenanceNeeded, 2=Offline, 3=Decommissioned

IF NOT EXISTS (SELECT 1 FROM StarlinkDevices WHERE DeviceId = 'SL-KGN-001')
BEGIN
    SET IDENTITY_INSERT [dbo].[StarlinkDevices] ON;

    INSERT INTO [dbo].[StarlinkDevices]
        ([Id], [DeviceId], [SerialNumber], [InstallationDate], [LocationName], [PhysicalAddress],
         [Parish], [Latitude], [Longitude], [LocationType], [Status],
         [ContactPerson], [ContactPhone], [Notes], [CreatedAt])
    VALUES
        (1, 'SL-KGN-001', 'UTY2D34E33', '2025-01-15', 'Half Way Tree Police Station',
         '1 Hagley Park Road, Kingston 10', 'St. Andrew', 18.0122, -76.7955,
         0, 0, 'Superintendent Brown', '876-926-8184',
         'Main police station for Kingston Metropolitan Area', GETUTCDATE()),

        (2, 'SL-KGN-002', 'UTY2D34F44', '2025-01-20', 'Constant Spring Police Station',
         'Constant Spring Road, Kingston 8', 'St. Andrew', 18.0245, -76.7970,
         0, 0, 'Inspector Williams', '876-924-1421',
         'Covers upper St. Andrew area', GETUTCDATE()),

        (3, 'SL-KGN-003', 'UTY2D34G55', '2025-02-01', 'Central Police Station',
         'Duke Street, Kingston', 'Kingston', 17.9686, -76.7940,
         0, 0, 'Superintendent Davis', '876-922-0223',
         'Downtown Kingston central station', GETUTCDATE()),

        (4, 'SL-KGN-004', 'UTY2D34H66', '2025-02-05', 'Matilda''s Corner Police Station',
         'Old Hope Road, Kingston 6', 'St. Andrew', 18.0158, -76.7847,
         0, 0, 'Inspector Thompson', '876-927-1131',
         'University area coverage', GETUTCDATE()),

        (5, 'SL-KGN-005', 'UTY2D34I77', '2025-02-10', 'Hunts Bay Police Station',
         'Spanish Town Road, Kingston 11', 'St. Andrew', 17.9775, -76.8264,
         0, 1, 'Sergeant Campbell', '876-923-8745',
         'Antenna requires adjustment due to recent storm', GETUTCDATE()),

        (6, 'SL-STK-001', 'UTY2D34J88', '2025-02-15', 'Spanish Town Police Station',
         'Brunswick Avenue, Spanish Town', 'St. Catherine', 17.9913, -76.9569,
         0, 0, 'Inspector Robinson', '876-984-2305',
         'Primary station for St. Catherine', GETUTCDATE());

    SET IDENTITY_INSERT [dbo].[StarlinkDevices] OFF;

    PRINT '6 sample police stations inserted successfully.';
END
ELSE
BEGIN
    PRINT 'Sample data already exists.';
END
GO

-- Verify data
SELECT COUNT(*) AS TotalDevices FROM StarlinkDevices;
SELECT * FROM StarlinkDevices ORDER BY Parish, LocationName;
GO

PRINT '';
PRINT '=================================================';
PRINT 'Database setup completed successfully!';
PRINT 'Database: StarlinkTrackerJamaica';
PRINT 'Total Devices: 6 police stations';
PRINT '=================================================';
GO
