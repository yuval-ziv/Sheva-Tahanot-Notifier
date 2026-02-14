namespace ShevaTahanotNotifier.Exceptions;

public class InvalidCallbackData : Exception
{
    public InvalidCallbackData(string format, string error) : base($"Expected callback data with format {format} but {error}")
    {
    }

    protected InvalidCallbackData(string? message) : base(message)
    {
    }
}