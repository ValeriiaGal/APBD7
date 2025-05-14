using APBD2;
using System.Collections.Generic;

namespace APBD2.Repositories;

public interface IDeviceRepository
{
    IEnumerable<Device> GetAllDevices();
    Device GetDeviceById(string id);
    bool AddDeviceUsingProcedure(Device device);
    bool UpdateDevice(Device device);
    bool DeleteDevice(string id);
}