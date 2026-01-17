namespace ShevaTahanotNotifier.Database.Entities;

public class NotificationSchedule : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
    public bool Enabled { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public short Hour { get; set; }
    public short Minute { get; set; }
}