namespace ShevaTahanotNotifier.Exceptions;

public class InvalidCallbackChatId : InvalidCallbackData
{
    public InvalidCallbackChatId(long callbackChatId, long callbackDataChatId) : base($"There was a mismatch between callback chat id and callback data chat id " +
                                                                                      $"[callback chat id: {callbackChatId},  callback data chat id: {callbackDataChatId}]")
    {
    }
}