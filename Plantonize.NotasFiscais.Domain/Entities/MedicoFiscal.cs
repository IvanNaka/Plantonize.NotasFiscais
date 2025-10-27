using System;

namespace Plantonize.NotasFiscais.Domain.Entities
{
    public class MedicoFiscal
    {
        public Guid Id { get; set; }
        public string? Nome { get; set; }
        public string? CpfCnpj { get; set; }
        public string? Email { get; set; }
        public string? Municipio { get; set; }
        public string? InscricaoMunicipal { get; set; }
        public Guid MedicoId { get; set; }
    }
}
