namespace APBD2;

/// <summary>
/// Loads devices from a file
/// </summary>
public interface IDeviceLoader
{
    List<object> LoadDevices();
}