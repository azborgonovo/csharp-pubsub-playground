using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Options;

namespace Consumer.WebApp;

public class MessagesConsumerService : IHostedService
{
    private readonly string _projectId;
    private const string _subscriptionId = "text-message-sub";

    private readonly IServiceProvider _serviceProvider;
    private SubscriberClient _subscriber;
    private Task _startTask;

    public MessagesConsumerService(IServiceProvider serviceProvider, IOptions<ConsumerOptions> consumerOptions)
    {
        _serviceProvider = serviceProvider;
        _projectId = consumerOptions.Value.ProjectId;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var subscriptionName = SubscriptionName.FromProjectSubscription(_projectId, _subscriptionId);
        _subscriber = await SubscriberClient.CreateAsync(subscriptionName);
        _startTask = _subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) => ExecuteAsync(message, true, cancel));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _subscriber.StopAsync(cancellationToken);
        // Lets make sure that the start task finished successfully after the call to stop.
        await _startTask;
    }
    
    private Task<SubscriberClient.Reply> ExecuteAsync(PubsubMessage message, bool acknowledge, CancellationToken _)
    {
        using var scope = _serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<MessagesConsumerService>>();
        
        var text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
        logger.LogInformation("Message {MessageMessageId}: {Text}", message.MessageId, text);

        return Task.FromResult(acknowledge ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack);
    }
}