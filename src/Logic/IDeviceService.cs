namespace APBD2;

public interface IDeviceService
{
    IEnumerable<Device> GetAllDevices();
    Device GetDeviceById(string id);
    bool CreateDevice(Device device);
    bool UpdateDevice(Device device);
    bool DeleteDevice(string id);
}
