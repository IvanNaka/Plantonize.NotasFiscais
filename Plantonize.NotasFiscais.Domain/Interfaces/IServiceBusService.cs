using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Domain.Interfaces
{
    public interface IServiceBusService
    {
        Task SendMessageAsync<T>(T message, string queueOrTopicName) where T : class;
        Task SendMessageToQueueAsync<T>(T message, string queueName) where T : class;
        Task SendMessageToTopicAsync<T>(T message, string topicName) where T : class;
        Task<T?> ReceiveMessageAsync<T>(string queueName) where T : class;
    }
}
