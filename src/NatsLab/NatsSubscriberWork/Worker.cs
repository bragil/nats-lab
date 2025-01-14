using NATS.Client.Core;

namespace NatsSubscriberWork;

public class Worker(INatsClient nats, ILogger<Worker> logger) : BackgroundService
{
    private readonly INatsClient nats = nats;
    private readonly ILogger<Worker> logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var _ = Task.Run(async () =>
        {
            await foreach (var msg in nats.SubscribeAsync<string>("events.*").WithCancellation(stoppingToken))
            {
                logger.LogInformation($"[SUBSCRIBER] Recebido de {msg.Subject}: {msg.Data}");
            }
        });
    }
}
