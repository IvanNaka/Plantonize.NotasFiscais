using System;

namespace Plantonize.NotasFiscais.Domain.Entities
{
    public class TomadorServico
    {
        public string? Nome { get; set; }
        public string? CpfCnpj { get; set; }
        public string? Email { get; set; }
        public string? TipoTomador { get; set; }
        public string? Endereco { get; set; }
        public string? Municipio { get; set; }
    }
}
