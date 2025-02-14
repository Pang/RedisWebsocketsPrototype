using RedisWebsocketsPrototype.API.Middleware;
using RedisWebsocketsPrototype.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://+:5000", "http://+:5001");

builder.Services.AddControllers();

// WebSocket manager registered as a singleton so data is persistent
builder.Services.AddSingleton<WebSocketConnectionManager>();

// Add Redis service
var redisService = new RedisPubSubService("redis:6379"); 
builder.Services.AddSingleton(redisService);

var app = builder.Build();

// Enable & Register WebSockets
app.UseWebSockets();
app.UseMiddleware<WebSocketHandler>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
