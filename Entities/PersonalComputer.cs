namespace APBD2;

public class PersonalComputer : Device
{
    public string OperatingSystem { get; set; }
    public override void TurnOn()
    {
        if (string.IsNullOrEmpty(OperatingSystem))
            throw new EmptySystemException("No OS installed.");
        base.TurnOn();
    }
    public override string ToString()
    {
        return base.ToString() + $", OS={(string.IsNullOrEmpty(OperatingSystem) ? "Not installed" : OperatingSystem)}";
    }
}