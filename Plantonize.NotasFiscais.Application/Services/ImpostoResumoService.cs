using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Application.Services
{
    public class ImpostoResumoService : IImpostoResumoService
    {
        private readonly IImpostoResumoRepository _repository;
        private readonly INotaFiscalRepository _notaFiscalRepository;
        private readonly IMunicipioAliquotaRepository _municipioAliquotaRepository;

        public ImpostoResumoService(
            IImpostoResumoRepository repository,
            INotaFiscalRepository notaFiscalRepository,
            IMunicipioAliquotaRepository municipioAliquotaRepository)
        {
            _repository = repository;
            _notaFiscalRepository = notaFiscalRepository;
            _municipioAliquotaRepository = municipioAliquotaRepository;
        }

        public async Task<ImpostoResumo?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ImpostoResumo>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<ImpostoResumo>> GetByMedicoIdAsync(Guid medicoId)
        {
            if (medicoId == Guid.Empty)
                throw new ArgumentException("Invalid Medico ID", nameof(medicoId));

            return await _repository.GetByMedicoIdAsync(medicoId);
        }

        public async Task<ImpostoResumo?> GetByMedicoMesAnoAsync(Guid medicoId, int mes, int ano)
        {
            if (medicoId == Guid.Empty)
                throw new ArgumentException("Invalid Medico ID", nameof(medicoId));

            ValidateMesAno(mes, ano);

            return await _repository.GetByMedicoMesAnoAsync(medicoId, mes, ano);
        }

        public async Task<ImpostoResumo> CreateAsync(ImpostoResumo impostoResumo)
        {
            if (impostoResumo == null)
                throw new ArgumentNullException(nameof(impostoResumo));

            ValidateImpostoResumo(impostoResumo);

            // Check if resumo already exists for this medico/mes/ano
            var existing = await _repository.GetByMedicoMesAnoAsync(
                impostoResumo.MedicoId, 
                impostoResumo.Mes, 
                impostoResumo.Ano);
            
            if (existing != null)
                throw new InvalidOperationException(
                    $"ImpostoResumo already exists for Medico {impostoResumo.MedicoId} in {impostoResumo.Mes}/{impostoResumo.Ano}");

            if (impostoResumo.Id == Guid.Empty)
                impostoResumo.Id = Guid.NewGuid();

            impostoResumo.DataCalculo = DateTime.UtcNow;

            return await _repository.CreateAsync(impostoResumo);
        }

        public async Task<ImpostoResumo> UpdateAsync(ImpostoResumo impostoResumo)
        {
            if (impostoResumo == null)
                throw new ArgumentNullException(nameof(impostoResumo));

            if (impostoResumo.Id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(impostoResumo.Id));

            var existing = await _repository.GetByIdAsync(impostoResumo.Id);
            if (existing == null)
                throw new InvalidOperationException($"ImpostoResumo with ID {impostoResumo.Id} not found");

            ValidateImpostoResumo(impostoResumo);

            impostoResumo.DataCalculo = DateTime.UtcNow;

            return await _repository.UpdateAsync(impostoResumo);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException($"ImpostoResumo with ID {id} not found");

            await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            if (id == Guid.Empty)
                return false;

            var impostoResumo = await _repository.GetByIdAsync(id);
            return impostoResumo != null;
        }

        public async Task<ImpostoResumo> CalculateResumoAsync(Guid medicoId, int mes, int ano)
        {
            if (medicoId == Guid.Empty)
                throw new ArgumentException("Invalid Medico ID", nameof(medicoId));

            ValidateMesAno(mes, ano);

            // Get all notas fiscais for the medico in the specified month/year
            var notasFiscais = await _notaFiscalRepository.GetByMedicoIdAsync(medicoId);
            var notasDoMes = notasFiscais.Where(nf => 
                nf.DataEmissao.Month == mes && 
                nf.DataEmissao.Year == ano).ToList();

            if (!notasDoMes.Any())
            {
                throw new InvalidOperationException(
                    $"No notas fiscais found for Medico {medicoId} in {mes}/{ano}");
            }

            // Get municipio aliquotas for calculations
            var municipiosPrestacao = notasDoMes
                .Where(nf => !string.IsNullOrWhiteSpace(nf.MunicipioPrestacao))
                .Select(nf => nf.MunicipioPrestacao)
                .Distinct();

            var aliquotas = new List<MunicipioAliquota>();
            foreach (var codigoMunicipio in municipiosPrestacao)
            {
                var aliquota = await _municipioAliquotaRepository.GetByCodigoMunicipioAsync(codigoMunicipio!);
                if (aliquota != null)
                    aliquotas.Add(aliquota);
            }

            // Calculate totals
            var totalReceitaBruta = notasDoMes.Sum(nf => nf.ValorTotal);
            
            decimal totalISS = 0;
            decimal totalIRPJ = 0;
            decimal totalCSLL = 0;
            decimal totalPIS = 0;
            decimal totalCOFINS = 0;
            decimal totalINSS = 0;

            foreach (var nota in notasDoMes)
            {
                var aliquota = aliquotas.FirstOrDefault(a => 
                    a.CodigoMunicipio == nota.MunicipioPrestacao);

                if (aliquota != null)
                {
                    totalISS += nota.ValorTotal * (aliquota.AliquotaISS / 100);
                    totalIRPJ += nota.ValorTotal * ((aliquota.AliquotaIRPJ ?? 0) / 100);
                    totalCSLL += nota.ValorTotal * ((aliquota.AliquotaCSLL ?? 0) / 100);
                    totalPIS += nota.ValorTotal * ((aliquota.AliquotaPIS ?? 0) / 100);
                    totalCOFINS += nota.ValorTotal * ((aliquota.AliquotaCOFINS ?? 0) / 100);
                    totalINSS += nota.ValorTotal * ((aliquota.AliquotaINSS ?? 0) / 100);
                }
            }

            var totalImpostos = totalISS + totalIRPJ + totalCSLL + totalPIS + totalCOFINS + totalINSS;
            var receitaLiquida = totalReceitaBruta - totalImpostos;

            var impostoResumo = new ImpostoResumo
            {
                Id = Guid.NewGuid(),
                MedicoId = medicoId,
                Mes = mes,
                Ano = ano,
                TotalReceitaBruta = totalReceitaBruta,
                TotalISS = totalISS,
                TotalIRPJ = totalIRPJ,
                TotalCSLL = totalCSLL,
                TotalPIS = totalPIS,
                TotalCOFINS = totalCOFINS,
                TotalINSS = totalINSS,
                TotalImpostos = totalImpostos,
                ReceitaLiquida = receitaLiquida,
                QuantidadeNotas = notasDoMes.Count,
                DataCalculo = DateTime.UtcNow
            };

            // Check if resumo already exists, update if it does, create if it doesn't
            var existingResumo = await _repository.GetByMedicoMesAnoAsync(medicoId, mes, ano);
            if (existingResumo != null)
            {
                impostoResumo.Id = existingResumo.Id;
                return await _repository.UpdateAsync(impostoResumo);
            }

            return await _repository.CreateAsync(impostoResumo);
        }

        private void ValidateImpostoResumo(ImpostoResumo impostoResumo)
        {
            if (impostoResumo.MedicoId == Guid.Empty)
                throw new ArgumentException("Medico ID is required", nameof(impostoResumo.MedicoId));

            ValidateMesAno(impostoResumo.Mes, impostoResumo.Ano);

            if (impostoResumo.TotalReceitaBruta < 0)
                throw new ArgumentException("Total receita bruta cannot be negative", nameof(impostoResumo.TotalReceitaBruta));

            if (impostoResumo.QuantidadeNotas < 0)
                throw new ArgumentException("Quantidade notas cannot be negative", nameof(impostoResumo.QuantidadeNotas));
        }

        private void ValidateMesAno(int mes, int ano)
        {
            if (mes < 1 || mes > 12)
                throw new ArgumentException("Mes must be between 1 and 12", nameof(mes));

            if (ano < 2000 || ano > DateTime.UtcNow.Year + 1)
                throw new ArgumentException($"Ano must be between 2000 and {DateTime.UtcNow.Year + 1}", nameof(ano));
        }
    }
}
