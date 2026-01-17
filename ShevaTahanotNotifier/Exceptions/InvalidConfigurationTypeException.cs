namespace ShevaTahanotNotifier.Exceptions;

public class InvalidConfigurationTypeException : Exception
{
    public InvalidConfigurationTypeException(Type expectedType, Type? actualType) : base($"Expected {expectedType}, got {(actualType is null ? "null" : actualType)})")
    {
    }
}