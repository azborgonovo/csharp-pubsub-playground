using Microsoft.Extensions.DependencyInjection;
using Publisher.ConsoleApp;

var serviceCollection = new ServiceCollection();
serviceCollection.AddScoped<IMessagesPublisher, PubSubMessagesPublisher>();
var serviceProvider = serviceCollection.BuildServiceProvider();

Console.WriteLine("Type the message to publish and press Enter. Type 'stop' to stop.");

while (true)
{
    var message = Console.ReadLine();
    if (message == "exit")
    {
        break;
    }

    using var scope = serviceProvider.CreateScope();
    var publisher = scope.ServiceProvider.GetRequiredService<IMessagesPublisher>();
    await publisher.PublishMessagesAsync(Console.WriteLine, message);
}

Console.WriteLine("Stopping publisher. Press any key to exit.");
Console.ReadKey();