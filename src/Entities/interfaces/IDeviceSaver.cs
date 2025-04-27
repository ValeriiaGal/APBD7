namespace APBD2;

/// <summary>
/// Saves devices to file
/// </summary>
public interface IDeviceSaver
{
    void SaveDevices(List<object> devices);
}