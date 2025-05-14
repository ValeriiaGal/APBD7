namespace Tests;

using System;
using System.Linq;
using APBD2;
using APBD2.Repositories;
using Microsoft.Extensions.Configuration;
using Xunit;

public class DeviceServiceTest
{
    private readonly IDeviceService _deviceService;

    public DeviceServiceTest()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString("UniversityDatabase")
                                ?? "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";

        var repository = new DeviceRepository(connectionString);
        _deviceService = new DeviceService(repository);
    }

    [Fact]
    public void AddDevice_ShouldCreateSmartwatch()
    {
        var device = new Smartwatch
        {
            Name = "UnitTestWatch",
            IsTurnedOn = true,
            Battery = 80
        };

        var result = _deviceService.CreateDevice(device);
        Assert.True(result, "Smartwatch should be created successfully.");
    }

    [Fact]
    public void GetAllDevices_ShouldReturnDevices()
    {
        var devices = _deviceService.GetAllDevices();
        Assert.NotNull(devices);
        Assert.True(devices.Any(), "Should return at least one device.");
    }

    [Fact]
    public void GetDeviceById_ShouldReturnCorrectDevice()
    {
        var allDevices = _deviceService.GetAllDevices();
        var firstDevice = allDevices.FirstOrDefault();

        Assert.NotNull(firstDevice);

        var device = _deviceService.GetDeviceById(firstDevice.Id);
        Assert.NotNull(device);
        Assert.Equal(firstDevice.Id, device.Id);
    }

    [Fact]
    public void UpdateDevice_ShouldUpdateData()
    {
        var allDevices = _deviceService.GetAllDevices();
        var device = allDevices.FirstOrDefault();

        if (device == null) return;

        device.Name = "UpdatedDeviceName";
        device.IsTurnedOn = !device.IsTurnedOn;

        // Simulate known RowVersion if not tracked
        if (device.RowVersion == null || device.RowVersion.Length == 0)
            device.RowVersion = new byte[8]; // dummy to test update

        var result = _deviceService.UpdateDevice(device);
        Assert.True(result, "Device should be updated successfully.");
    }

    [Fact]
    public void DeleteDevice_ShouldRemoveDevice()
    {
        var device = new PersonalComputer
        {
            Name = "TempPC",
            IsTurnedOn = false,
            OperatingSystem = "TestOS"
        };

        var createResult = _deviceService.CreateDevice(device);
        Assert.True(createResult, "Device should be created before deletion.");

        var allDevices = _deviceService.GetAllDevices();
        var lastDevice = allDevices.Last();

        var deleteResult = _deviceService.DeleteDevice(lastDevice.Id);
        Assert.True(deleteResult, "Device should be deleted.");
    }
}
