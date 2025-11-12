namespace Plantonize.NotasFiscais.Infrastructure.Configuration
{
    public class ServiceBusSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string NotaFiscalQueueName { get; set; } = "integracao-nf";
        public string FaturaQueueName { get; set; } = "fatura-queue";
        public string NotaFiscalTopicName { get; set; } = "notafiscal-topic";
        public string FaturaTopicName { get; set; } = "fatura-topic";
    }
}
