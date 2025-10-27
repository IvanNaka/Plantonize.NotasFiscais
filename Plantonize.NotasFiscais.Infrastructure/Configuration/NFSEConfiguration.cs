using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Plantonize.NotasFiscais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Infrastructure.Configuration
{
    public class NFSEConfiguration
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; }

        [BsonElement("numeroNota")]
        public string? NumeroNota { get; set; }

        [BsonElement("dataEmissao")]
        public DateTime DataEmissao { get; set; } = DateTime.UtcNow;

        [BsonElement("valorTotal")]
        public decimal ValorTotal { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Emitida";

        [BsonElement("municipioPrestacao")]
        public string? MunicipioPrestacao { get; set; }

        [BsonElement("issRetido")]
        public bool IssRetido { get; set; }

        [BsonElement("medico")]
        public MedicoFiscalConfiguration? Medico { get; set; }

        [BsonElement("tomador")]
        public required TomadorServicoConfiguration Tomador { get; set; }

        [BsonElement("servicos")]
        public List<ItemServicoConfiguration>? Servicos { get; set; }

        [BsonElement("enviadoEmail")]
        public bool EnviadoEmail { get; set; } = false;

        [BsonElement("dataEnvioEmail")]
        public DateTime? DataEnvioEmail { get; set; }
    }
}
