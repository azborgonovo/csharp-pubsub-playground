using Consumer.WebApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<MessagesConsumerService>();

var app = builder.Build();

app.Run();