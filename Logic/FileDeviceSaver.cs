namespace APBD2;

public class FileDeviceSaver : IDeviceSaver
{
    private readonly string _filePath;

    public FileDeviceSaver(string filePath)
    {
        _filePath = filePath;
    }

    public void SaveDevices(List<object> devices)
    {
        var lines = new List<string>();
        foreach (var obj in devices)
        {
            if (obj is Device d)
                lines.Add(d.ToString());
        }
        File.WriteAllLines(_filePath, lines);
    }
}