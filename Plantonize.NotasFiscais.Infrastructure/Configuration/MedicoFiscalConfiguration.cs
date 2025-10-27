using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Plantonize.NotasFiscais.Infrastructure.Configuration
{
    public class MedicoFiscalConfiguration
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required Guid Id { get; set; }

        [BsonElement("nome")]
        public string? Nome { get; set; }

        [BsonElement("cpfCnpj")]
        public string? CpfCnpj { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("municipio")]
        public string? Municipio { get; set; }

        [BsonElement("inscricaoMunicipal")]
        public string? InscricaoMunicipal { get; set; }

        [BsonElement("medicoId")]
        [BsonRepresentation(BsonType.String)]
        public required Guid MedicoId { get; set; }
    }
}
