using ShevaTahanotNotifier.Database.Entities.Enums;

namespace ShevaTahanotNotifier.ExtensionMethods;

public static class DayExtensions
{
    public static List<DayOfWeek> ToDayOfWeek(this Day day)
    {
        return day switch
        {
            Day.Sunday => [DayOfWeek.Sunday],
            Day.Monday => [DayOfWeek.Monday],
            Day.Tuesday => [DayOfWeek.Tuesday],
            Day.Wednesday => [DayOfWeek.Wednesday],
            Day.Thursday => [DayOfWeek.Thursday],
            Day.Friday => [DayOfWeek.Friday],
            Day.Saturday => [DayOfWeek.Saturday],
            Day.Weekdays => [DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday],
            Day.Weekends => [DayOfWeek.Friday, DayOfWeek.Saturday],
            Day.Everyday => [DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday],
            _ => throw new ArgumentOutOfRangeException(nameof(day), day, null)
        };
    }
}