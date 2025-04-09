using System.Text.RegularExpressions;

namespace APBD2;

public class EmbeddedDevice : Device
{
    private string ipAddress;
    public string NetworkName { get; set; }

    public string IpAddress
    {
        get { return ipAddress; }
        set
        {
            if (!Regex.IsMatch(value, "^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$"))
                throw new InvalidArgumentException("Invalid IP format.");
            ipAddress = value;
        }
    }

    public void Connect()
    {
        if (!NetworkName.Contains("MD Ltd."))
            throw new ConnectionException("Invalid network name.");
    }

    public override void TurnOn()
    {
        Connect();
        base.TurnOn();
    }
    public override string ToString()
    {
        return base.ToString() + $", IP={IpAddress}, Network={NetworkName}";
    }
}