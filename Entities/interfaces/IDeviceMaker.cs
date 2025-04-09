namespace APBD2;

/// <summary>
/// Creates Device instances
/// </summary>
public interface IDeviceMaker
{
    object CreateDevice(string line);
}