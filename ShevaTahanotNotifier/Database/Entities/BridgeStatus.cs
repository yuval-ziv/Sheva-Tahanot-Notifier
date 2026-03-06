namespace ShevaTahanotNotifier.Database.Entities;

public class BridgeStatus : BaseEntity
{
    public bool IsOpen { get; set; }
    public bool IsManualRefresh { get; set; }
    public DateTimeOffset LastUpdated { get; set; }

    public string ToNotificationString()
    {
        return $"Bridge is currently {(IsOpen ? string.Empty : "not ")}open (last updated {LastUpdated.ToLocalTime():dd.MM.yyyy HH:mm:ss})";
    }
}