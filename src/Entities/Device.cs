namespace APBD2;

/// <summary>
/// Base class for all devices
/// </summary>
public abstract class Device
{
    public string Id { get; set; }
    public string Name { get; set; } 
    public bool IsTurnedOn { get; private set; }
    
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