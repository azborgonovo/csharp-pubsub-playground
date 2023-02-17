namespace Publisher.ConsoleApp;

public interface IMessagesPublisher
{
    Task<int> PublishMessagesAsync(Action<string> logAction, params string[] messageTexts);
}