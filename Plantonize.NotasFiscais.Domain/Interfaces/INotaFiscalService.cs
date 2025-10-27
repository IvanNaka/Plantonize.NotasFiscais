using Plantonize.NotasFiscais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Domain.Interfaces
{
    public interface INotaFiscalService
    {
        Task<NotaFiscal?> GetByIdAsync(Guid id);
        Task<IEnumerable<NotaFiscal>> GetAllAsync();
        Task<IEnumerable<NotaFiscal>> GetByMedicoIdAsync(Guid medicoId);
        Task<NotaFiscal> CreateAsync(NotaFiscal notaFiscal);
        Task<NotaFiscal> UpdateAsync(NotaFiscal notaFiscal);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
