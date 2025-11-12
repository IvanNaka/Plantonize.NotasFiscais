using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Plantonize.NotasFiscais.Infrastructure.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Infrastructure.Services
{
    public class ServiceBusService : IServiceBusService, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSettings _settings;
        private readonly ILogger<ServiceBusService> _logger;

        public ServiceBusService(
            IOptions<ServiceBusSettings> settings,
            ILogger<ServiceBusService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            if (string.IsNullOrWhiteSpace(_settings.ConnectionString))
            {
                throw new InvalidOperationException("Service Bus connection string is not configured");
            }

            _client = new ServiceBusClient(_settings.ConnectionString);
        }

        public async Task SendMessageAsync<T>(T message, string queueOrTopicName) where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (string.IsNullOrWhiteSpace(queueOrTopicName))
                throw new ArgumentException("Queue or topic name cannot be empty", nameof(queueOrTopicName));

            try
            {
                await using var sender = _client.CreateSender(queueOrTopicName);
                
                var messageBody = JsonSerializer.Serialize(message);
                var serviceBusMessage = new ServiceBusMessage(messageBody)
                {
                    ContentType = "application/json",
                    Subject = typeof(T).Name,
                    MessageId = Guid.NewGuid().ToString()
                };

                await sender.SendMessageAsync(serviceBusMessage);
                
                _logger.LogInformation(
                    "Message sent to {QueueOrTopic}. Type: {MessageType}, MessageId: {MessageId}",
                    queueOrTopicName, typeof(T).Name, serviceBusMessage.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error sending message to {QueueOrTopic}. Type: {MessageType}",
                    queueOrTopicName, typeof(T).Name);
                throw;
            }
        }

        public async Task SendMessageToQueueAsync<T>(T message, string queueName) where T : class
        {
            await SendMessageAsync(message, queueName);
        }

        public async Task SendMessageToTopicAsync<T>(T message, string topicName) where T : class
        {
            await SendMessageAsync(message, topicName);
        }

        public async Task<T?> ReceiveMessageAsync<T>(string queueName) where T : class
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be empty", nameof(queueName));

            try
            {
                await using var receiver = _client.CreateReceiver(queueName);
                
                var message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));
                
                if (message == null)
                {
                    _logger.LogInformation("No message available in queue {QueueName}", queueName);
                    return null;
                }

                var messageBody = message.Body.ToString();
                var deserializedMessage = JsonSerializer.Deserialize<T>(messageBody);

                // Complete the message so it's removed from the queue
                await receiver.CompleteMessageAsync(message);

                _logger.LogInformation(
                    "Message received from {QueueName}. Type: {MessageType}, MessageId: {MessageId}",
                    queueName, typeof(T).Name, message.MessageId);

                return deserializedMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error receiving message from {QueueName}. Type: {MessageType}",
                    queueName, typeof(T).Name);
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_client != null)
            {
                await _client.DisposeAsync();
            }
        }
    }
}
