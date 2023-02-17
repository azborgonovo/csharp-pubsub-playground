using Consumer.WebApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ConsumerOptions>(builder.Configuration.GetSection(ConsumerOptions.ConfigSectionKey));
builder.Services.AddHostedService<MessagesConsumerService>();

var app = builder.Build();

app.Run();