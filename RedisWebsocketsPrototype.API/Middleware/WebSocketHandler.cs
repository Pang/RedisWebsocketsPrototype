using RedisWebsocketsPrototype.API.Services;
using System.Net.WebSockets;
using System.Text;

namespace RedisWebsocketsPrototype.API.Middleware;

public class WebSocketHandler
{
    private readonly RequestDelegate _next;
    private readonly WebSocketConnectionManager _connectionManager;

    public WebSocketHandler(RequestDelegate next, WebSocketConnectionManager connectionManager)
    {
        _next = next;
        _connectionManager = connectionManager;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string socketId = Guid.NewGuid().ToString();

            _connectionManager.AddSocket(socketId, webSocket);

            await HandleWebSocketAsync(socketId, webSocket);
        }
        else
        {
            await _next(context);
        }
    }

    private async Task HandleWebSocketAsync(string socketId, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];

        try
        {
            WebSocketReceiveResult result;
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                Console.WriteLine($"Received from {socketId}: {receivedMessage}");

                var responseMessage = $"Echo: {receivedMessage}";
                var encodedResponse = Encoding.UTF8.GetBytes(responseMessage);
                await webSocket.SendAsync(new ArraySegment<byte>(encodedResponse), WebSocketMessageType.Text, true, CancellationToken.None);

            } while (!result.CloseStatus.HasValue);
        }
        finally
        {
            _connectionManager.RemoveSocket(socketId);
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
        }
    }
}
