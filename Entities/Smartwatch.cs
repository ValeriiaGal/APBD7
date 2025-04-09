namespace APBD2;

public class Smartwatch : Device, IPowerNotifier
{
    private int _battery;
    public int Battery
    {
        get { return _battery; }
        set
        {
            if (value < 0 || value > 100)
                throw new InvalidArgumentException("Battery percentage must be between 0 and 100.");
            _battery = value;
            if (_battery < 20)
                BatteryLow();
        }
    }
    
    public void BatteryLow() { Console.WriteLine("Warning: Low battery!"); }
    
    public override void TurnOn()
    {
        if (_battery < 11)
            throw new EmptyBatteryException(" Battery is too low to turn on.");
        base.TurnOn();
        _battery -= 10;
    }
    public override string ToString()
    {
        return base.ToString() + $", Battery={_battery}%";
    }
}