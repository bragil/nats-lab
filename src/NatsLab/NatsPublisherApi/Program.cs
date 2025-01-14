using NATS.Client.Core;
using NATS.Net;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<INatsClient>(new NatsClient("127.0.0.1:4222"));

var app = builder.Build();

app.MapGet("/publish/{evnt}", async (string evnt, INatsClient nats) =>
{
	try
	{
		var msg = $"{evnt}: {evnt}@email.com";
        await nats.PublishAsync($"events.{evnt}", msg);
		Console.WriteLine($"[PUBLISHER] Evento {evnt}, publicado: {msg}");

        return Results.Ok(evnt);
    }
	catch (Exception ex)
	{
		return Results.Problem(ex.Message, null, StatusCodes.Status500InternalServerError);
	}
});

app.Run();