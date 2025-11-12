namespace Plantonize.NotasFiscais.Application.DTOs
{
    public class NotaFiscalServiceBusDto
    {
        public string _id { get; set; } = string.Empty;
        public string numeroNota { get; set; } = string.Empty;
        public string codigo_servico { get; set; } = string.Empty;
        public string descricao { get; set; } = string.Empty;
        public decimal valor { get; set; }
        public string cpf_cnpj_cliente { get; set; } = string.Empty;
        public string cliente { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string cep { get; set; } = string.Empty;
        public string endereco { get; set; } = string.Empty;
        public string numero { get; set; } = string.Empty;
        public string bairro { get; set; } = string.Empty;
        public string codigo_municipio { get; set; } = string.Empty;
        public string municipio { get; set; } = string.Empty;
        public string uf { get; set; } = string.Empty;
    }
}
