using NATS.Client.Core;
using NATS.Net;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<INatsClient>(new NatsClient("127.0.0.1:4222"));

var app = builder.Build();

app.MapGet("/users/{id}", async (int id, INatsClient nats) =>
{
    var replyOpts = new NatsSubOpts { Timeout = TimeSpan.FromSeconds(10) };
    var reply = await nats.RequestAsync<int, User>("users.get", id, replyOpts: replyOpts);

    if (reply.Data is null || reply.Data.Id == 0)
    {
        Console.WriteLine("[REQUEST] Nenhum conteudo.");
        return Results.NoContent();
    }
    Console.WriteLine($"[REQUEST] Respondido: {reply.Data}");
    return Results.Ok(reply.Data);
});


app.Run();

public record User(int Id, string Name, string Email);