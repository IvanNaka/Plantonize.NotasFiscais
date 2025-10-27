using MongoDB.Bson.Serialization.Attributes;

namespace Plantonize.NotasFiscais.Infrastructure.Configuration
{
    public class TomadorServicoConfiguration
    {
        [BsonElement("nome")]
        public string? Nome { get; set; }

        [BsonElement("cpfCnpj")]
        public string? CpfCnpj { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("tipoTomador")]
        public string? TipoTomador { get; set; }

        [BsonElement("endereco")]
        public string? Endereco { get; set; }

        [BsonElement("municipio")]
        public string? Municipio { get; set; }
    }
}
