namespace APBD2;

public class FileDeviceLoader : IDeviceLoader
{
    private readonly string _filePath;
    private readonly IDeviceMaker _deviceMaker;

    public FileDeviceLoader(string filePath, IDeviceMaker deviceFactory)
    {
        _filePath = filePath;
        _deviceMaker = deviceFactory;
    }

    public List<object> LoadDevices()
    {
        if (!File.Exists(_filePath))
            throw new FileNotFoundException("File not found.");

        var devices = new List<object>();
        foreach (var line in File.ReadAllLines(_filePath))
        {
            var device = _deviceMaker.CreateDevice(line);
            if (device != null)
                devices.Add(device);
        }
        return devices;
    }
}