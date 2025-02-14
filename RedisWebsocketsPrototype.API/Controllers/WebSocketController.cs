using Microsoft.AspNetCore.Mvc;
using RedisWebsocketsPrototype.API.Services;
using System.Net.WebSockets;
using System.Text;

namespace RedisWebsocketsPrototype.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebSocketController : ControllerBase
    {
        private readonly WebSocketConnectionManager _connectionManager;

        public WebSocketController(WebSocketConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromQuery] string message)
        {
            foreach (var socket in _connectionManager.GetAllSockets().Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var encodedMessage = Encoding.UTF8.GetBytes(message);
                    await socket.SendAsync(new ArraySegment<byte>(encodedMessage), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }

            return Ok("Message sent to all connected WebSockets");
        }
    }
}
