using OfficeOpenXml;
using OfficeOpenXml.Style;
using StarlinkTracker.Models;
using System.Drawing;

namespace StarlinkTracker.Services;

public class ExcelExportService
{
    public byte[] ExportDevicesToExcel(List<StarlinkDevice> devices)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();

        // Main data sheet
        var worksheet = package.Workbook.Worksheets.Add("Starlink Devices");

        // Headers
        var headers = new[]
        {
            "Device ID", "Serial Number", "Installation Date", "Location Name",
            "Physical Address", "Parish", "Latitude", "Longitude",
            "Location Type", "Status", "Contact Person", "Contact Phone", "Notes"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 53, 69)); // Red header
            worksheet.Cells[1, i + 1].Style.Font.Color.SetColor(Color.White);
        }

        // Data
        for (int i = 0; i < devices.Count; i++)
        {
            var device = devices[i];
            int row = i + 2;

            worksheet.Cells[row, 1].Value = device.DeviceId;
            worksheet.Cells[row, 2].Value = device.SerialNumber;
            worksheet.Cells[row, 3].Value = device.InstallationDate.ToString("yyyy-MM-dd");
            worksheet.Cells[row, 4].Value = device.LocationName;
            worksheet.Cells[row, 5].Value = device.PhysicalAddress;
            worksheet.Cells[row, 6].Value = device.Parish;
            worksheet.Cells[row, 7].Value = device.Latitude;
            worksheet.Cells[row, 8].Value = device.Longitude;
            worksheet.Cells[row, 9].Value = device.LocationType.ToString();
            worksheet.Cells[row, 10].Value = device.Status.ToString();
            worksheet.Cells[row, 11].Value = device.ContactPerson;
            worksheet.Cells[row, 12].Value = device.ContactPhone;
            worksheet.Cells[row, 13].Value = device.Notes;

            // Color code by status
            var statusColor = device.Status switch
            {
                DeviceStatus.Active => Color.FromArgb(40, 167, 69),
                DeviceStatus.MaintenanceNeeded => Color.FromArgb(255, 193, 7),
                DeviceStatus.Offline => Color.FromArgb(220, 53, 69),
                DeviceStatus.Decommissioned => Color.FromArgb(108, 117, 125),
                _ => Color.White
            };

            worksheet.Cells[row, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[row, 10].Style.Fill.BackgroundColor.SetColor(statusColor);
            if (device.Status != DeviceStatus.MaintenanceNeeded)
            {
                worksheet.Cells[row, 10].Style.Font.Color.SetColor(Color.White);
            }
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        // Summary sheet
        CreateSummarySheet(package, devices);

        // Parish breakdown sheet
        CreateParishBreakdownSheet(package, devices);

        return package.GetAsByteArray();
    }

    private void CreateSummarySheet(ExcelPackage package, List<StarlinkDevice> devices)
    {
        var worksheet = package.Workbook.Worksheets.Add("Summary");

        worksheet.Cells["A1"].Value = "Starlink Deployment Summary - Jamaica";
        worksheet.Cells["A1"].Style.Font.Size = 16;
        worksheet.Cells["A1"].Style.Font.Bold = true;

        worksheet.Cells["A3"].Value = "Total Devices:";
        worksheet.Cells["B3"].Value = devices.Count;
        worksheet.Cells["B3"].Style.Font.Bold = true;

        worksheet.Cells["A4"].Value = "Active Devices:";
        worksheet.Cells["B4"].Value = devices.Count(d => d.Status == DeviceStatus.Active);
        worksheet.Cells["B4"].Style.Font.Color.SetColor(Color.Green);

        worksheet.Cells["A5"].Value = "Maintenance Needed:";
        worksheet.Cells["B5"].Value = devices.Count(d => d.Status == DeviceStatus.MaintenanceNeeded);
        worksheet.Cells["B5"].Style.Font.Color.SetColor(Color.Orange);

        worksheet.Cells["A6"].Value = "Offline Devices:";
        worksheet.Cells["B6"].Value = devices.Count(d => d.Status == DeviceStatus.Offline);
        worksheet.Cells["B6"].Style.Font.Color.SetColor(Color.Red);

        worksheet.Cells["A8"].Value = "Police Stations:";
        worksheet.Cells["B8"].Value = devices.Count(d => d.LocationType == LocationType.PoliceStation);
        worksheet.Cells["B8"].Style.Font.Bold = true;

        worksheet.Cells["A9"].Value = "Schools:";
        worksheet.Cells["B9"].Value = devices.Count(d => d.LocationType == LocationType.School);

        worksheet.Cells["A10"].Value = "Hospitals:";
        worksheet.Cells["B10"].Value = devices.Count(d => d.LocationType == LocationType.Hospital);

        worksheet.Cells["A11"].Value = "Other Locations:";
        worksheet.Cells["B11"].Value = devices.Count(d => d.LocationType == LocationType.Other);

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
    }

    private void CreateParishBreakdownSheet(ExcelPackage package, List<StarlinkDevice> devices)
    {
        var worksheet = package.Workbook.Worksheets.Add("Parish Breakdown");

        worksheet.Cells["A1"].Value = "Parish";
        worksheet.Cells["B1"].Value = "Total Devices";
        worksheet.Cells["C1"].Value = "Active";
        worksheet.Cells["D1"].Value = "Offline";
        worksheet.Cells["E1"].Value = "Police Stations";

        worksheet.Cells["A1:E1"].Style.Font.Bold = true;
        worksheet.Cells["A1:E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells["A1:E1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 53, 69));
        worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.White);

        var parishGroups = devices.GroupBy(d => d.Parish).OrderBy(g => g.Key);
        int row = 2;

        foreach (var group in parishGroups)
        {
            worksheet.Cells[row, 1].Value = group.Key;
            worksheet.Cells[row, 2].Value = group.Count();
            worksheet.Cells[row, 3].Value = group.Count(d => d.Status == DeviceStatus.Active);
            worksheet.Cells[row, 4].Value = group.Count(d => d.Status == DeviceStatus.Offline);
            worksheet.Cells[row, 5].Value = group.Count(d => d.LocationType == LocationType.PoliceStation);
            row++;
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
    }
}
