using System.ComponentModel.DataAnnotations;

namespace ShevaTahanotNotifier.Database.Entities.NotificationProviderConfiguration;

public class EmailNotificationProviderConfiguration : BaseNotificationProviderConfiguration
{
    // RFC 5322 limits the line length of headers to 998 characters
    [MaxLength(998)] public required string EmailAddress { get; set; }
}