using Plantonize.NotasFiscais.Application.DTOs;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Application.Services
{
    public class NotaFiscalService : INotaFiscalService
    {
        private readonly INotaFiscalRepository _repository;
        private readonly IServiceBusService _serviceBusService;
        private const string NOTA_FISCAL_QUEUE = "integracao-nf";

        public NotaFiscalService(
            INotaFiscalRepository repository,
            IServiceBusService serviceBusService)
        {
            _repository = repository;
            _serviceBusService = serviceBusService;
        }

        public async Task<NotaFiscal?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<NotaFiscal>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<NotaFiscal>> GetByMedicoIdAsync(Guid medicoId)
        {
            if (medicoId == Guid.Empty)
                throw new ArgumentException("Invalid Medico ID", nameof(medicoId));

            return await _repository.GetByMedicoIdAsync(medicoId);
        }

        public async Task<NotaFiscal> CreateAsync(NotaFiscal notaFiscal)
        {
            if (notaFiscal == null)
                throw new ArgumentNullException(nameof(notaFiscal));

            ValidateNotaFiscal(notaFiscal);

            if (notaFiscal.Id == Guid.Empty)
                notaFiscal.Id = Guid.NewGuid();

            notaFiscal.DataEmissao = DateTime.UtcNow;

            var createdNotaFiscal = await _repository.CreateAsync(notaFiscal);

            // Send message to Service Bus after successful creation
            try
            {
                var serviceBusDto = ConvertToServiceBusDto(createdNotaFiscal);
                await _serviceBusService.SendMessageToQueueAsync(serviceBusDto, NOTA_FISCAL_QUEUE);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the operation
                Console.WriteLine($"Failed to send message to Service Bus queue '{NOTA_FISCAL_QUEUE}': {ex.Message}");
            }

            return createdNotaFiscal;
        }

        public async Task<NotaFiscal> UpdateAsync(NotaFiscal notaFiscal)
        {
            if (notaFiscal == null)
                throw new ArgumentNullException(nameof(notaFiscal));

            if (notaFiscal.Id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(notaFiscal.Id));

            var existing = await _repository.GetByIdAsync(notaFiscal.Id);
            if (existing == null)
                throw new InvalidOperationException($"NotaFiscal with ID {notaFiscal.Id} not found");

            ValidateNotaFiscal(notaFiscal);

            return await _repository.UpdateAsync(notaFiscal);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException($"NotaFiscal with ID {id} not found");

            await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            if (id == Guid.Empty)
                return false;

            var notaFiscal = await _repository.GetByIdAsync(id);
            return notaFiscal != null;
        }

        private void ValidateNotaFiscal(NotaFiscal notaFiscal)
        {
            if (notaFiscal.ValorTotal <= 0)
                throw new ArgumentException("Valor total must be greater than zero", nameof(notaFiscal.ValorTotal));

            if (notaFiscal.Medico == null)
                throw new ArgumentException("Medico is required", nameof(notaFiscal.Medico));

            if (notaFiscal.Tomador == null)
                throw new ArgumentException("Tomador is required", nameof(notaFiscal.Tomador));

            if (notaFiscal.Servicos == null || notaFiscal.Servicos.Count == 0)
                throw new ArgumentException("At least one service is required", nameof(notaFiscal.Servicos));
        }

        private NotaFiscalServiceBusDto ConvertToServiceBusDto(NotaFiscal notaFiscal)
        {
            // Pega o primeiro serviço (assumindo que existe pelo menos um devido à validação)
            var primeiroServico = notaFiscal.Servicos?.FirstOrDefault();

            // Extrai informações do endereço do tomador
            var endereco = notaFiscal.Tomador?.Endereco ?? string.Empty;
            var enderecoPartes = ParseEndereco(endereco);

            // Extrai UF do município (assumindo formato "Cidade - UF" ou similar)
            var municipioCompleto = notaFiscal.Tomador?.Municipio ?? string.Empty;
            var uf = ExtractUF(municipioCompleto);
            var municipio = ExtractMunicipioName(municipioCompleto);

            return new NotaFiscalServiceBusDto
            {
                _id = notaFiscal.Id.ToString(),
                numeroNota = notaFiscal.NumeroNota ?? string.Empty,
                codigo_servico = "001", // Código padrão, pode ser customizado
                descricao = primeiroServico?.Descricao ?? string.Empty,
                valor = notaFiscal.ValorTotal,
                cpf_cnpj_cliente = notaFiscal.Tomador?.CpfCnpj ?? string.Empty,
                cliente = notaFiscal.Tomador?.Nome ?? string.Empty,
                email = notaFiscal.Tomador?.Email ?? string.Empty,
                cep = ExtractCEP(endereco),
                endereco = enderecoPartes.Rua,
                numero = enderecoPartes.Numero,
                bairro = enderecoPartes.Bairro,
                codigo_municipio = GetCodigoIBGE(municipio),
                municipio = municipio,
                uf = uf
            };
        }

        private (string Rua, string Numero, string Bairro) ParseEndereco(string endereco)
        {
            // Formato esperado: "Rua X, 123 - Bairro" ou similar
            // Esta é uma implementação simplificada, ajuste conforme necessário
            
            if (string.IsNullOrWhiteSpace(endereco))
                return (string.Empty, string.Empty, string.Empty);

            var partes = endereco.Split(',', '-');
            
            var rua = partes.Length > 0 ? partes[0].Trim() : string.Empty;
            var numero = partes.Length > 1 ? partes[1].Trim().Split(' ')[0] : string.Empty;
            var bairro = partes.Length > 2 ? partes[2].Trim() : string.Empty;

            return (rua, numero, bairro);
        }

        private string ExtractUF(string municipioCompleto)
        {
            // Extrai UF do formato "Cidade - UF" ou "Cidade/UF"
            if (string.IsNullOrWhiteSpace(municipioCompleto))
                return string.Empty;

            if (municipioCompleto.Contains(" - "))
            {
                var partes = municipioCompleto.Split(" - ");
                return partes.Length > 1 ? partes[1].Trim() : string.Empty;
            }

            if (municipioCompleto.Contains("/"))
            {
                var partes = municipioCompleto.Split('/');
                return partes.Length > 1 ? partes[1].Trim() : string.Empty;
            }

            // Se não encontrou separador, retorna vazio
            return string.Empty;
        }

        private string ExtractMunicipioName(string municipioCompleto)
        {
            // Extrai nome do município do formato "Cidade - UF" ou "Cidade/UF"
            if (string.IsNullOrWhiteSpace(municipioCompleto))
                return string.Empty;

            if (municipioCompleto.Contains(" - "))
            {
                return municipioCompleto.Split(" - ")[0].Trim();
            }

            if (municipioCompleto.Contains("/"))
            {
                return municipioCompleto.Split('/')[0].Trim();
            }

            return municipioCompleto.Trim();
        }

        private string ExtractCEP(string endereco)
        {
            // Tenta extrair CEP do endereço (formato XXXXX-XXX ou XXXXXXXX)
            // Esta é uma implementação simplificada
            if (string.IsNullOrWhiteSpace(endereco))
                return string.Empty;

            var palavras = endereco.Split(' ', ',', '-');
            foreach (var palavra in palavras)
            {
                var numerosApenas = new string(palavra.Where(char.IsDigit).ToArray());
                if (numerosApenas.Length == 8)
                {
                    return numerosApenas.Insert(5, "-");
                }
            }

            return string.Empty;
        }

        private string GetCodigoIBGE(string municipio)
        {
            // Mapeamento de códigos IBGE - adicione mais conforme necessário
            var codigosIBGE = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "São Paulo", "3550308" },
                { "Rio de Janeiro", "3304557" },
                { "Belo Horizonte", "3106200" },
                { "Brasília", "5300108" },
                { "Salvador", "2927408" },
                { "Fortaleza", "2304400" },
                { "Curitiba", "4106902" },
                { "Recife", "2611606" },
                { "Porto Alegre", "4314902" },
                { "Manaus", "1302603" }
            };

            return codigosIBGE.TryGetValue(municipio, out var codigo) ? codigo : string.Empty;
        }
    }
}
