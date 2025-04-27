namespace APBD2;

/// <summary>
/// Shows a low battery warning
/// </summary>
public interface IPowerNotifier
{
    void BatteryLow();
}