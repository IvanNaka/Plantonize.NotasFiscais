using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Application.Services
{
    public class NotaFiscalService : INotaFiscalService
    {
        private readonly INotaFiscalRepository _repository;

        public NotaFiscalService(INotaFiscalRepository repository)
        {
            _repository = repository;
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

            return await _repository.CreateAsync(notaFiscal);
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
    }
}
