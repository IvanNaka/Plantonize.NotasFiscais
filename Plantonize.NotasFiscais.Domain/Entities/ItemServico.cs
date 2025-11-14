using System;

namespace Plantonize.NotasFiscais.Domain.Entities
{
    public class ItemServico
    {
        public string? Descricao { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal AliquotaIss { get; set; }
        
        private decimal? _valorTotal;
        public decimal ValorTotal 
        { 
            get => _valorTotal ?? (Quantidade * ValorUnitario);
            set => _valorTotal = value;
        }
    }
}
