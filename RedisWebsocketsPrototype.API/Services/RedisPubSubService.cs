using StackExchange.Redis;

namespace RedisWebsocketsPrototype.API.Services;
public class RedisPubSubService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly ISubscriber _subscriber;

    public RedisPubSubService(string redisConnectionString)
    {
        _redis = ConnectionMultiplexer.Connect(redisConnectionString);
        _subscriber = _redis.GetSubscriber();
    }

    // Subscribe to a Redis channel
    public void Subscribe(string channel, Action<RedisChannel, RedisValue> handler)
    {
        _subscriber.Subscribe(channel, (ch, msg) => handler(ch, msg));
    }

    // Publish a message to a Redis channel
    public async Task PublishAsync(string channel, string message)
    {
        await _subscriber.PublishAsync(channel, message);
    }
}
