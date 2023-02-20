using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Options;

namespace Publisher.ConsoleApp;

public class PubSubMessagesPublisher : IMessagesPublisher
{
    private readonly string _projectId;
    private const string _topicId = "text-message";

    public PubSubMessagesPublisher(IOptions<PublisherOptions> publisherOptions)
    {
        _projectId = publisherOptions.Value.ProjectId;
    }
    
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
                logAction($"An error occurred when publishing message {text}: {exception.Message}");
            }
        });
        await Task.WhenAll(publishTasks);
        return publishedMessageCount;
    }
}