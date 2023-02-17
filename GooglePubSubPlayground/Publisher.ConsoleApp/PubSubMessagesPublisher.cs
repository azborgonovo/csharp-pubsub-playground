using Google.Cloud.PubSub.V1;

namespace Publisher.ConsoleApp;

public class PubSubMessagesPublisher : IMessagesPublisher
{
    private readonly string _projectId = "TODO";
    private readonly string _topicId = "TODO";
    
    public async Task<int> PublishMessagesAsync(Action<string> logAction, params string[] messageTexts)
    {
        var topicName = TopicName.FromProjectTopic(_projectId, _topicId);
        var publisher = await PublisherClient.CreateAsync(topicName);

        int publishedMessageCount = 0;
        var publishTasks = messageTexts.Select(async text =>
        {
            try
            {
                var message = await publisher.PublishAsync(text);
                logAction($"Published message {message}");
                Interlocked.Increment(ref publishedMessageCount);
            }
            catch (Exception exception)
            {
                logAction($"An error ocurred when publishing message {text}: {exception.Message}");
            }
        });
        await Task.WhenAll(publishTasks);
        return publishedMessageCount;
    }
}