using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Plantonize.NotasFiscais.Infrastructure.Configuration
{
    public class MunicipioAliquotaConfiguration
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required Guid Id { get; set; }

        [BsonElement("codigoMunicipio")]
        public string? CodigoMunicipio { get; set; }

        [BsonElement("nomeMunicipio")]
        public string? NomeMunicipio { get; set; }

        [BsonElement("uf")]
        public string? UF { get; set; }

        [BsonElement("aliquotaISS")]
        public decimal AliquotaISS { get; set; }

        [BsonElement("aliquotaIRPJ")]
        public decimal? AliquotaIRPJ { get; set; }

        [BsonElement("aliquotaCSLL")]
        public decimal? AliquotaCSLL { get; set; }

        [BsonElement("aliquotaPIS")]
        public decimal? AliquotaPIS { get; set; }

        [BsonElement("aliquotaCOFINS")]
        public decimal? AliquotaCOFINS { get; set; }

        [BsonElement("aliquotaINSS")]
        public decimal? AliquotaINSS { get; set; }

        [BsonElement("dataAtualizacao")]
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    }
}
