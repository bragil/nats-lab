using NATS.Client.Core;
using NATS.Net;
using NatsReplyWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<INatsClient>(new NatsClient("127.0.0.1:4222"));
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
