namespace Tests;
using System.Collections.Generic;
using APBD2;
using Xunit;

public class DeviceManagerTest
{
    private class StubLoader : IDeviceLoader
    {
        public List<object> LoadDevices() => new List<object>();
    }

    private DeviceManager CreateManager()
    {
        return new DeviceManager();
    }

    [Fact]
    public void addDevice()
    {
        var manager = CreateManager();
        var device = new Smartwatch { Id = 1, Name = "Test Smartwatch", Battery = 50 };

        manager.AddDevice(device);
        manager.ShowDevices(); 
    }

    [Fact]
    public void removeDevice()
    {
        var manager = CreateManager();
        var device = new Smartwatch { Id = 1, Name = "Test Smartwatch", Battery = 50 };
        manager.AddDevice(device);

        manager.RemoveDevice(1);
        manager.ShowDevices(); 
    }

    [Fact]
    public void turnOnDevice()
    {
        var manager = CreateManager();
        var device = new Smartwatch { Id = 1, Name = "Test Smartwatch", Battery = 50 };
        manager.AddDevice(device);

        manager.TurnOnDevice(1);
        var deviceAfterTurnOn = manager.GetDeviceById(1);
        Assert.True(deviceAfterTurnOn.IsTurnedOn, "Device should be turned on.");
    }

    [Fact]
    public void editedDevice()
    {
        var manager = CreateManager();
        var device = new Smartwatch { Id = 1, Name = "Test Smartwatch", Battery = 50 };
        manager.AddDevice(device);

        manager.EditDeviceData(1, "Battery", 80);

        var updatedDevice = (Smartwatch)manager.GetDeviceById(1);
        Assert.Equal(80, updatedDevice.Battery);
    }

    [Fact]
    public void turnOffDevice()
    {
        var manager = CreateManager();
        var device = new Smartwatch { Id = 1, Name = "Test Smartwatch", Battery = 50 };
        manager.AddDevice(device);
        manager.TurnOnDevice(1);

        manager.TurnOffDevice(1);

        var deviceAfterTurnOff = manager.GetDeviceById(1);
        Assert.False(deviceAfterTurnOff.IsTurnedOn, "Device should be turned off.");
    }
}