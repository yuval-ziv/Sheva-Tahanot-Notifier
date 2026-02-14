using ShevaTahanotNotifier.Database.Entities.Enums;

namespace ShevaTahanotNotifier.ExtensionMethods;

public static class DayExtensions
{
    public static string ToCronDayOfWeek(this Day day)
    {
        return day switch
        {
            Day.Sunday or Day.Monday or Day.Tuesday or Day.Wednesday or Day.Thursday or Day.Friday or Day.Saturday => ((int)day).ToString(),
            Day.Weekdays => "0-4",
            Day.Weekends => "5,6",
            Day.Everyday => "*",
            _ => throw new ArgumentOutOfRangeException(nameof(day), day, null),
        };
    }
}