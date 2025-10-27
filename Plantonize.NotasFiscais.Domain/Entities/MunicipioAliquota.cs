using System;

namespace Plantonize.NotasFiscais.Domain.Entities
{
    public class MunicipioAliquota
    {
        public Guid Id { get; set; }
        public string? CodigoMunicipio { get; set; }
        public string? NomeMunicipio { get; set; }
        public string? UF { get; set; }
        public decimal AliquotaISS { get; set; }
        public decimal? AliquotaIRPJ { get; set; }
        public decimal? AliquotaCSLL { get; set; }
        public decimal? AliquotaPIS { get; set; }
        public decimal? AliquotaCOFINS { get; set; }
        public decimal? AliquotaINSS { get; set; }
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    }
}
