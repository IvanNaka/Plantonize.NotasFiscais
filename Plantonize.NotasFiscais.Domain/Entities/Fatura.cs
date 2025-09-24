using Plantonize.NotasFiscais.Domain.Enum;

namespace Plantonize.NotasFiscais.Domain.Entities
{
    public class Fatura
    {
        public Fatura() { }
        public Guid Id { get; set; }
        public Guid MedicoId { get; set; }
        public TipoDestinatarioEnum TipoDestinatario { get; set; }
        public StatusFaturaEnum StatusFatura { get; set; }
        public Decimal ValorTotal { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime? DataEnvio { get; set; }
        public DateTime? DataPagamento { get; set; }
        public NFSE NotaNFSE { get; set; }

    }
}
