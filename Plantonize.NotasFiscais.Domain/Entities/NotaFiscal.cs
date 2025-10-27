using Plantonize.NotasFiscais.Domain.Enum;
using System;
using System.Collections.Generic;

namespace Plantonize.NotasFiscais.Domain.Entities
{
    public class NotaFiscal
    {
        public Guid Id { get; set; }
        public string? NumeroNota { get; set; }
        public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
        public decimal ValorTotal { get; set; }
        public StatusNFSEEnum Status { get; set; } = StatusNFSEEnum.Autorizado;
        public string? MunicipioPrestacao { get; set; }
        public bool IssRetido { get; set; }
        public MedicoFiscal? Medico { get; set; }
        public TomadorServico? Tomador { get; set; }
        public List<ItemServico>? Servicos { get; set; }
        public bool EnviadoEmail { get; set; } = false;
        public DateTime? DataEnvioEmail { get; set; }
    }
}
