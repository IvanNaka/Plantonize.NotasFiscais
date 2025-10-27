using System;

namespace Plantonize.NotasFiscais.Domain.Entities
{
    public class ItemServico
    {
        public string? Descricao { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal AliquotaIss { get; set; }
        public decimal ValorTotal => Quantidade * ValorUnitario;
    }
}
