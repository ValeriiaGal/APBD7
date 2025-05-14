
using APBD2.Repositories;
using Microsoft.Data.SqlClient;

namespace APBD2;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _repository;

    public DeviceService(IDeviceRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<Device> GetAllDevices()
    {
        return _repository.GetAllDevices();
    }

    public Device GetDeviceById(string id)
    {
        return _repository.GetDeviceById(id);
    }

    public bool CreateDevice(Device device)
    {
        return _repository.AddDeviceUsingProcedure(device);
    }

    public bool UpdateDevice(Device device)
    {
        return _repository.UpdateDevice(device);
    }

    public bool DeleteDevice(string id)
    {
        return _repository.DeleteDevice(id);
    }
}