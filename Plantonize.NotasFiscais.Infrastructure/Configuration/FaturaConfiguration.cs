using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Plantonize.NotasFiscais.Infrastructure.Configuration
{
    public class FaturaConfiguration
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required Guid Id { get; set; }

        [BsonElement("numeroFatura")]
        public string? NumeroFatura { get; set; }

        [BsonElement("dataEmissao")]
        public DateTime DataEmissao { get; set; } = DateTime.UtcNow;

        [BsonElement("dataVencimento")]
        public DateTime? DataVencimento { get; set; }

        [BsonElement("valorTotal")]
        public decimal ValorTotal { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Pendente";

        [BsonElement("medicoId")]
        [BsonRepresentation(BsonType.String)]
        public Guid MedicoId { get; set; }

        [BsonElement("medico")]
        public MedicoFiscalConfiguration? Medico { get; set; }

        [BsonElement("notasFiscais")]
        public List<NFSEConfiguration>? NotasFiscais { get; set; }

        [BsonElement("dataPagamento")]
        public DateTime? DataPagamento { get; set; }

        [BsonElement("observacoes")]
        public string? Observacoes { get; set; }
    }
}
