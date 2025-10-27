using System;

namespace Plantonize.NotasFiscais.Domain.Entities
{
    public class ImpostoResumo
    {
        public Guid Id { get; set; }
        public Guid MedicoId { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public decimal TotalReceitaBruta { get; set; }
        public decimal TotalISS { get; set; }
        public decimal TotalIRPJ { get; set; }
        public decimal TotalCSLL { get; set; }
        public decimal TotalPIS { get; set; }
        public decimal TotalCOFINS { get; set; }
        public decimal TotalINSS { get; set; }
        public decimal TotalImpostos { get; set; }
        public decimal ReceitaLiquida { get; set; }
        public int QuantidadeNotas { get; set; }
        public DateTime DataCalculo { get; set; } = DateTime.UtcNow;
    }
}
