using Plantonize.NotasFiscais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Domain.Interfaces
{
    public interface IMunicipioAliquotaService
    {
        Task<MunicipioAliquota?> GetByIdAsync(Guid id);
        Task<IEnumerable<MunicipioAliquota>> GetAllAsync();
        Task<MunicipioAliquota?> GetByCodigoMunicipioAsync(string codigoMunicipio);
        Task<MunicipioAliquota> CreateAsync(MunicipioAliquota municipioAliquota);
        Task<MunicipioAliquota> UpdateAsync(MunicipioAliquota municipioAliquota);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByCodigoAsync(string codigoMunicipio);
    }
}
