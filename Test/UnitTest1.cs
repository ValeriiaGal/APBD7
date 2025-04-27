namespace Tests;

using System.Collections.Generic;
using APBD2;
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
        
        _deviceService = new DeviceService(connectionString);
    }

    [Fact]
    public void AddDevice_ShouldCreateDevice()
    {
        var device = new Device
        {
            Name = "TestDevice",
            IsTurnedOn = true
        };

        var result = _deviceService.CreateDevice(device);

        Assert.True(result, "Device should be created successfully.");
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
        
        if (firstDevice != null)
        {
            var device = _deviceService.GetDeviceById(firstDevice.Id);
            Assert.NotNull(device);
            Assert.Equal(firstDevice.Id, device.Id);
        }
    }

    [Fact]
    public void UpdateDevice_ShouldUpdateData()
    {
        var allDevices = _deviceService.GetAllDevices();
        var device = allDevices.FirstOrDefault();

        if (device != null)
        {
            device.Name = "UpdatedName";
            device.IsTurnedOn = !device.IsTurnedOn;

            var result = _deviceService.UpdateDevice(device);

            Assert.True(result, "Device should be updated.");
        }
    }

    [Fact]
    public void DeleteDevice_ShouldRemoveDevice()
    {
        var newDevice = new Device
        {
            Name = "DeviceToDelete",
            IsTurnedOn = false
        };
        _deviceService.CreateDevice(newDevice);

        var allDevices = _deviceService.GetAllDevices();
        var deviceToDelete = allDevices.Last();

        var deleteResult = _deviceService.DeleteDevice(deviceToDelete.Id);

        Assert.True(deleteResult, "Device should be deleted.");
    }
}
