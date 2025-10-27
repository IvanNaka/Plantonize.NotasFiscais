using Plantonize.NotasFiscais.Domain.Enum;
using System;
using System.Collections.Generic;

namespace Plantonize.NotasFiscais.Domain.Entities
{
    public class Fatura
    {
        public Guid Id { get; set; }
        public string? NumeroFatura { get; set; }
        public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
        public DateTime? DataVencimento { get; set; }
        public decimal ValorTotal { get; set; }
        public StatusFaturaEnum Status { get; set; } = StatusFaturaEnum.Pendente;
        public Guid MedicoId { get; set; }
        public MedicoFiscal? Medico { get; set; }
        public List<NotaFiscal>? NotasFiscais { get; set; }
        public DateTime? DataPagamento { get; set; }
        public string? Observacoes { get; set; }
    }
}
