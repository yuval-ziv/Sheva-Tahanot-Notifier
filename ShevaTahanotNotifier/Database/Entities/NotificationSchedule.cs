using ShevaTahanotNotifier.Database.Entities.Enums;

namespace ShevaTahanotNotifier.Database.Entities;

public class NotificationSchedule : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
    public bool Enabled { get; set; }
    public Day Day { get; set; }
    public short Hour { get; set; }
    public short Minute { get; set; }

    public string ButtonText => $"{Day} - {Hour:00}:{Minute:00} ({(Enabled ? "Enabled" : "Disabled")})";
}