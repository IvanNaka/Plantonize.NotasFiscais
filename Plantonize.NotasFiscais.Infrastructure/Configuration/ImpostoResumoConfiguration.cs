using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Plantonize.NotasFiscais.Infrastructure.Configuration
{
    public class ImpostoResumoConfiguration
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; }

        [BsonElement("medicoId")]
        public required string MedicoId { get; set; }

        [BsonElement("mes")]
        public int Mes { get; set; }

        [BsonElement("ano")]
        public int Ano { get; set; }

        [BsonElement("totalReceitaBruta")]
        public decimal TotalReceitaBruta { get; set; }

        [BsonElement("totalISS")]
        public decimal TotalISS { get; set; }

        [BsonElement("totalIRPJ")]
        public decimal TotalIRPJ { get; set; }

        [BsonElement("totalCSLL")]
        public decimal TotalCSLL { get; set; }

        [BsonElement("totalPIS")]
        public decimal TotalPIS { get; set; }

        [BsonElement("totalCOFINS")]
        public decimal TotalCOFINS { get; set; }

        [BsonElement("totalINSS")]
        public decimal TotalINSS { get; set; }

        [BsonElement("totalImpostos")]
        public decimal TotalImpostos { get; set; }

        [BsonElement("receitaLiquida")]
        public decimal ReceitaLiquida { get; set; }

        [BsonElement("quantidadeNotas")]
        public int QuantidadeNotas { get; set; }

        [BsonElement("dataCalculo")]
        public DateTime DataCalculo { get; set; } = DateTime.UtcNow;
    }
}
