using Plantonize.NotasFiscais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Domain.Interfaces
{
    public interface IImpostoResumoService
    {
        Task<ImpostoResumo?> GetByIdAsync(Guid id);
        Task<IEnumerable<ImpostoResumo>> GetAllAsync();
        Task<IEnumerable<ImpostoResumo>> GetByMedicoIdAsync(Guid medicoId);
        Task<ImpostoResumo?> GetByMedicoMesAnoAsync(Guid medicoId, int mes, int ano);
        Task<ImpostoResumo> CreateAsync(ImpostoResumo impostoResumo);
        Task<ImpostoResumo> UpdateAsync(ImpostoResumo impostoResumo);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<ImpostoResumo> CalculateResumoAsync(Guid medicoId, int mes, int ano);
    }
}
