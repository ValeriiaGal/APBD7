namespace APBD2;

/// <summary>
/// Manages devices and provides operations to modify them
/// </summary>
public class DeviceManager
{
    private readonly List<Device> _devices;
    private const int MaxDevices = 15;

    public DeviceManager()
    {
        _devices = new List<Device>
        {
            new Smartwatch { Id = 1, Name = "Galaxy Fit", Battery = 75 },
            new Smartwatch { Id = 2, Name = "Apple Watch", Battery = 90 },
            new Smartwatch { Id = 3, Name = "Xiaomi Band", Battery = 65 },

            new PersonalComputer { Id = 4, Name = "Dell XPS", OperatingSystem = "Windows 11" },
            new PersonalComputer { Id = 5, Name = "MacBook Pro", OperatingSystem = "macOS Ventura" },
            new PersonalComputer { Id = 6, Name = "Lenovo ThinkPad", OperatingSystem = "Ubuntu 22.04" },

            new EmbeddedDevice { Id = 7, Name = "Sensor A1", IpAddress = "192.168.1.1", NetworkName = "MD Ltd. Network" },
            new EmbeddedDevice { Id = 8, Name = "Camera X3", IpAddress = "192.168.0.10", NetworkName = "MD Ltd. Network" },
            new EmbeddedDevice { Id = 9, Name = "Alarm Unit", IpAddress = "10.0.0.5", NetworkName = "MD Ltd. Alarm" },

            new Smartwatch { Id = 10, Name = "Huawei Watch", Battery = 80 },
            new PersonalComputer { Id = 11, Name = "Asus ROG", OperatingSystem = "Windows 10" },
            new EmbeddedDevice { Id = 12, Name = "Temperature Sensor", IpAddress = "172.16.0.12", NetworkName = "MD Ltd. Climate" },
            new Smartwatch { Id = 13, Name = "Fitbit Sense", Battery = 55 },
            new PersonalComputer { Id = 14, Name = "Acer Aspire", OperatingSystem = "Linux Mint" },
            new EmbeddedDevice { Id = 15, Name = "Gateway Device", IpAddress = "192.168.100.1", NetworkName = "MD Ltd. Hub" },
        };
    }

    public void AddDevice(Device device)
    {
        if (_devices.Count >= MaxDevices)
        {
            Console.WriteLine("Storage limit reached.");
            return;
        }

        device.Id = _devices.Count + 1;
        _devices.Add(device);
    }

    public Device GetDeviceById(int id)
    {
        var device = _devices.Find(d => d.Id == id);
        if (device == null)
        {
            Console.WriteLine($"Device with Id {id} not found.");
            throw new Exception("Device not found");
        }

        return device;
    }

    public void RemoveDevice(int id)
    {
        var deviceToRemove = _devices.Find(d => d.Id == id);
        if (deviceToRemove != null)
        {
            _devices.Remove(deviceToRemove);
        }
        else
        {
            Console.WriteLine($"Device with Id {id} not found.");
        }
    }

public void EditDeviceData(int id, string propertyName, object newValue)
{
    var device = _devices.Find(d => d.Id == id);
    if (device != null)
    {
        switch (propertyName)
        {
            case "Name":
                device.Name = newValue.ToString();
                break;

            case "Battery":
                if (device is Smartwatch sw && int.TryParse(newValue.ToString(), out int battery))
                {
                    sw.Battery = battery;
                }
                else
                {
                    throw new InvalidOperationException("Battery can only be updated for Smartwatch devices.");
                }
                break;

            case "OperatingSystem":
                if (device is PersonalComputer pc)
                {
                    pc.OperatingSystem = newValue.ToString();
                }
                else
                {
                    throw new InvalidOperationException("OperatingSystem can only be updated for PersonalComputer devices.");
                }
                break;

            case "IpAddress":
                if (device is EmbeddedDevice ed)
                {
                    ed.IpAddress = newValue.ToString();
                }
                else
                {
                    throw new InvalidOperationException("IpAddress can only be updated for EmbeddedDevice devices.");
                }
                break;

            case "NetworkName":
                if (device is EmbeddedDevice ed2)
                {
                    ed2.NetworkName = newValue.ToString();
                }
                else
                {
                    throw new InvalidOperationException("NetworkName can only be updated for EmbeddedDevice devices.");
                }
                break;

            case "IsTurnedOn":
                bool newState = Convert.ToBoolean(newValue);
                if (newState) device.TurnOn();
                else device.TurnOff();
                break;

            default:
                throw new InvalidOperationException($"Unknown property: {propertyName}");
        }
    }
    else
    {
        throw new Exception($"Device with Id {id} not found.");
    }
}


    public void ShowDevices()
    {
        if (_devices.Count == 0)
        {
            Console.WriteLine("No devices to display.");
        }
        else
        {
            foreach (var device in _devices)
            {
                Console.WriteLine(device);
            }
        }
    }

    public List<Device> GetAllDevices()
    {
        return new List<Device>(_devices);
    }

    public void TurnOnDevice(int id)
    {
        var device = _devices.Find(d => d.Id == id);
        if (device != null)
        {
            try
            {
                device.TurnOn();
                Console.WriteLine($"Device {device.Name} is now turned on.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error turning on device: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Device with Id {id} not found.");
        }
    }

    public void TurnOffDevice(int id)
    {
        var device = _devices.Find(d => d.Id == id);
        if (device != null)
        {
            device.TurnOff();
            Console.WriteLine($"Device {device.Name} is now turned off.");
        }
        else
        {
            Console.WriteLine($"Device with Id {id} not found.");
        }
    }
}
