using System.ComponentModel.DataAnnotations;

namespace ShevaTahanotNotifier.Database.Entities;

public class Conversation : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
    public long ChatId { get; set; }
    [MaxLength(50)] public string? CurrentStep { get; set; }
    [MaxLength(50)] public string? NextStep { get; set; }
    public Dictionary<string, string>? ExtraData { get; set; }
}