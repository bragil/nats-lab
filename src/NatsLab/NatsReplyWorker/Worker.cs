using NATS.Client.Core;

namespace NatsReplyWorker;

public class Worker(INatsClient nats, ILogger<Worker> logger) : BackgroundService
{
    private readonly INatsClient nats = nats;
    private readonly ILogger logger = logger;
    private readonly Dictionary<int, User> db = new()
    {
        { 1, new User(1, "João", "joao@email.com") },
        { 2, new User(2, "José", "jose@email.com") },
        { 3, new User(3, "Antonio", "antonio@email.com") },
        { 4, new User(4, "Joaquim", "joaquim@email.com") },
        { 5, new User(5, "Arnaldo", "arnaldo@email.com") },
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var _ = Task.Run(async () =>
        {
            await foreach (var msg in nats.SubscribeAsync<int>("users.get").WithCancellation(stoppingToken))
            {
                logger.LogInformation($"[REPLY] Recebido de {msg.Subject}: {msg.Data}");

                if (!db.ContainsKey(msg.Data))
                {
                    await msg.ReplyAsync(new User(0, string.Empty, string.Empty)); // Não encontrado
                }
                else
                {
                    var user = db[msg.Data];
                    await msg.ReplyAsync(user);
                }
            }
        });
    }
}

public record User(int Id, string Name, string Email);