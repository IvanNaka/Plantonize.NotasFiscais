using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Plantonize.NotasFiscais.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Application.Services
{
    public class FaturaService : IFaturaService
    {
        private readonly IFaturaRepository _repository;

        public FaturaService(IFaturaRepository repository)
        {
            _repository = repository;
        }

        public async Task<Fatura?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Fatura>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<Fatura>> GetByMedicoIdAsync(Guid medicoId)
        {
            if (medicoId == Guid.Empty)
                throw new ArgumentException("Invalid Medico ID", nameof(medicoId));

            return await _repository.GetByMedicoIdAsync(medicoId);
        }

        public async Task<Fatura> CreateAsync(Fatura fatura)
        {
            if (fatura == null)
                throw new ArgumentNullException(nameof(fatura));

            ValidateFatura(fatura);

            if (fatura.Id == Guid.Empty)
                fatura.Id = Guid.NewGuid();

            fatura.DataEmissao = DateTime.UtcNow;
            
            if (fatura.Status == default)
                fatura.Status = StatusFaturaEnum.Pendente;

            return await _repository.CreateAsync(fatura);
        }

        public async Task<Fatura> UpdateAsync(Fatura fatura)
        {
            if (fatura == null)
                throw new ArgumentNullException(nameof(fatura));

            if (fatura.Id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(fatura.Id));

            var existing = await _repository.GetByIdAsync(fatura.Id);
            if (existing == null)
                throw new InvalidOperationException($"Fatura with ID {fatura.Id} not found");

            ValidateFatura(fatura);

            return await _repository.UpdateAsync(fatura);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException($"Fatura with ID {id} not found");

            await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            if (id == Guid.Empty)
                return false;

            var fatura = await _repository.GetByIdAsync(id);
            return fatura != null;
        }

        public async Task<decimal> CalculateTotalValueAsync(Guid faturaId)
        {
            var fatura = await _repository.GetByIdAsync(faturaId);
            
            if (fatura == null)
                throw new InvalidOperationException($"Fatura with ID {faturaId} not found");

            if (fatura.NotasFiscais == null || !fatura.NotasFiscais.Any())
                return 0;

            return fatura.NotasFiscais.Sum(nf => nf.ValorTotal);
        }

        private void ValidateFatura(Fatura fatura)
        {
            if (fatura.MedicoId == Guid.Empty)
                throw new ArgumentException("Medico ID is required", nameof(fatura.MedicoId));

            if (fatura.ValorTotal < 0)
                throw new ArgumentException("Valor total cannot be negative", nameof(fatura.ValorTotal));

            if (fatura.DataVencimento.HasValue && fatura.DataVencimento < fatura.DataEmissao)
                throw new ArgumentException("Data vencimento cannot be before data emissao", nameof(fatura.DataVencimento));

            if (fatura.Status == StatusFaturaEnum.Paga && !fatura.DataPagamento.HasValue)
                throw new ArgumentException("Data pagamento is required for paid faturas", nameof(fatura.DataPagamento));
        }
    }
}
