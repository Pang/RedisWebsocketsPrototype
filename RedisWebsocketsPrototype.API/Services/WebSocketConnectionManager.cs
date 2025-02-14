using System.Net.WebSockets;
using System.Collections.Concurrent;

namespace RedisWebsocketsPrototype.API.Services;

public class WebSocketConnectionManager
{
    private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    public void AddSocket(string id, WebSocket socket)
    {
        _sockets.TryAdd(id, socket);
    }

    public void RemoveSocket(string id)
    {
        _sockets.TryRemove(id, out _);
    }

    public WebSocket? GetSocket(string id)
    {
        _sockets.TryGetValue(id, out var socket);
        return socket;
    }

    public ConcurrentDictionary<string, WebSocket> GetAllSockets()
    {
        return _sockets;
    }
}
