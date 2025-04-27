namespace APBD2;

public class DeviceMaker : IDeviceMaker
{
    public object CreateDevice(string line)
    {
        var parts = line.Split(',');
        if (parts.Length < 2) return null;

        var identifierParts = parts[0].Split('-');
        if (identifierParts.Length < 2) return null;

        string type = identifierParts[0];
        object device = null;

        try
        {
            switch (type)
            {
                case "SW":
                    if (parts.Length < 4) return null;
                    device = new Smartwatch { Name = parts[1], Battery = int.Parse(parts[3].TrimEnd('%')) };
                    break;
                case "P":
                    device = new PersonalComputer { Name = parts[1], OperatingSystem = parts.Length > 3 ? parts[3] : null };
                    break;
                case "ED":
                    if (parts.Length < 4) return null;
                    device = new EmbeddedDevice { Name = parts[1], IpAddress = parts[2], NetworkName = parts[3] };
                    break;
            }

            if (device is Device d && bool.TryParse(parts[2], out bool isOn) && isOn)
                d.TurnOn();
        }
        catch
        {
            return null;
        }

        return device;
    }
}