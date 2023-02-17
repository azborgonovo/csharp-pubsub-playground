using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publisher.ConsoleApp;

Console.WriteLine("Configuring publisher...");

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .Build();

var serviceCollection = new ServiceCollection();
serviceCollection.AddScoped<IMessagesPublisher, PubSubMessagesPublisher>();
serviceCollection.Configure<PublisherOptions>(config.GetSection(PublisherOptions.ConfigSectionKey));
var serviceProvider = serviceCollection.BuildServiceProvider();

Console.WriteLine("Publisher ready!");
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