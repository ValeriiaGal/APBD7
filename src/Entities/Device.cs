
namespace APBD2;

public class Device
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsTurnedOn { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public virtual void TurnOn()
    {
        if (IsTurnedOn)
        {
            Console.WriteLine($"Device {Name} is already turned on.");
            return;
        }
        IsTurnedOn = true;
        Console.WriteLine($"Device {Name} has been turned on.");
    }

    public void TurnOff()
    {
        if (!IsTurnedOn)
        {
            Console.WriteLine($"Device {Name} is already turned off.");
            return;
        }
        IsTurnedOn = false;
        Console.WriteLine($"Device {Name} has been turned off.");
    }

    public override string ToString() =>
        $"{GetType().Name} [Id={Id}, Name={Name}, IsTurnedOn={IsTurnedOn}]";
}