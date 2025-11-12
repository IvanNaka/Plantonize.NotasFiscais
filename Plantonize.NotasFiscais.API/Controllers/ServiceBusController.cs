using Microsoft.AspNetCore.Mvc;
using Plantonize.NotasFiscais.Domain.Interfaces;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceBusController : ControllerBase
    {
        private readonly IServiceBusService _serviceBusService;

        public ServiceBusController(IServiceBusService serviceBusService)
        {
            _serviceBusService = serviceBusService;
        }

        /// <summary>
        /// Send a test message to a queue
        /// </summary>
        [HttpPost("send/{queueName}")]
        public async Task<IActionResult> SendMessage(string queueName, [FromBody] object message)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                return BadRequest("Queue name is required");

            if (message == null)
                return BadRequest("Message is required");

            try
            {
                await _serviceBusService.SendMessageToQueueAsync(message, queueName);
                return Ok(new { success = true, message = $"Message sent to queue '{queueName}'" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Send a test message to a topic
        /// </summary>
        [HttpPost("send-topic/{topicName}")]
        public async Task<IActionResult> SendMessageToTopic(string topicName, [FromBody] object message)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                return BadRequest("Topic name is required");

            if (message == null)
                return BadRequest("Message is required");

            try
            {
                await _serviceBusService.SendMessageToTopicAsync(message, topicName);
                return Ok(new { success = true, message = $"Message sent to topic '{topicName}'" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Receive a message from a queue
        /// </summary>
        [HttpGet("receive/{queueName}")]
        public async Task<IActionResult> ReceiveMessage(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                return BadRequest("Queue name is required");

            try
            {
                var message = await _serviceBusService.ReceiveMessageAsync<object>(queueName);
                
                if (message == null)
                    return NotFound(new { success = false, message = "No messages in queue" });

                return Ok(new { success = true, data = message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}
