using Plantonize.NotasFiscais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Domain.Interfaces
{
    public interface IFaturaService
    {
        Task<Fatura?> GetByIdAsync(Guid id);
        Task<IEnumerable<Fatura>> GetAllAsync();
        Task<IEnumerable<Fatura>> GetByMedicoIdAsync(Guid medicoId);
        Task<Fatura> CreateAsync(Fatura fatura);
        Task<Fatura> UpdateAsync(Fatura fatura);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<decimal> CalculateTotalValueAsync(Guid faturaId);
    }
}
