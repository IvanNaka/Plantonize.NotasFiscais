using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Infrastructure.Configuration
{
    public class ItemServicoConfiguration
    {
        [BsonElement("descricao")]
        public string Descricao { get; set; }

        [BsonElement("quantidade")]
        public int Quantidade { get; set; }

        [BsonElement("valorUnitario")]
        public decimal ValorUnitario { get; set; }

        [BsonElement("aliquotaIss")]
        public decimal AliquotaIss { get; set; }

        [BsonElement("valorTotal")]
        public decimal ValorTotal => Quantidade * ValorUnitario;
    }
}
