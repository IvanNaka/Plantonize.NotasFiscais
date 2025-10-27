using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Application.Services
{
    public class MunicipioAliquotaService : IMunicipioAliquotaService
    {
        private readonly IMunicipioAliquotaRepository _repository;

        public MunicipioAliquotaService(IMunicipioAliquotaRepository repository)
        {
            _repository = repository;
        }

        public async Task<MunicipioAliquota?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<MunicipioAliquota>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<MunicipioAliquota?> GetByCodigoMunicipioAsync(string codigoMunicipio)
        {
            if (string.IsNullOrWhiteSpace(codigoMunicipio))
                throw new ArgumentException("Codigo municipio is required", nameof(codigoMunicipio));

            return await _repository.GetByCodigoMunicipioAsync(codigoMunicipio);
        }

        public async Task<MunicipioAliquota> CreateAsync(MunicipioAliquota municipioAliquota)
        {
            if (municipioAliquota == null)
                throw new ArgumentNullException(nameof(municipioAliquota));

            ValidateMunicipioAliquota(municipioAliquota);

            // Check if codigo municipio already exists
            if (!string.IsNullOrWhiteSpace(municipioAliquota.CodigoMunicipio))
            {
                var existing = await _repository.GetByCodigoMunicipioAsync(municipioAliquota.CodigoMunicipio);
                if (existing != null)
                    throw new InvalidOperationException($"Municipio with codigo {municipioAliquota.CodigoMunicipio} already exists");
            }

            if (municipioAliquota.Id == Guid.Empty)
                municipioAliquota.Id = Guid.NewGuid();

            municipioAliquota.DataAtualizacao = DateTime.UtcNow;

            return await _repository.CreateAsync(municipioAliquota);
        }

        public async Task<MunicipioAliquota> UpdateAsync(MunicipioAliquota municipioAliquota)
        {
            if (municipioAliquota == null)
                throw new ArgumentNullException(nameof(municipioAliquota));

            if (municipioAliquota.Id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(municipioAliquota.Id));

            var existing = await _repository.GetByIdAsync(municipioAliquota.Id);
            if (existing == null)
                throw new InvalidOperationException($"MunicipioAliquota with ID {municipioAliquota.Id} not found");

            ValidateMunicipioAliquota(municipioAliquota);

            // Check if codigo municipio is being changed and if new codigo already exists
            if (!string.IsNullOrWhiteSpace(municipioAliquota.CodigoMunicipio) && 
                municipioAliquota.CodigoMunicipio != existing.CodigoMunicipio)
            {
                var duplicate = await _repository.GetByCodigoMunicipioAsync(municipioAliquota.CodigoMunicipio);
                if (duplicate != null)
                    throw new InvalidOperationException($"Municipio with codigo {municipioAliquota.CodigoMunicipio} already exists");
            }

            municipioAliquota.DataAtualizacao = DateTime.UtcNow;

            return await _repository.UpdateAsync(municipioAliquota);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException($"MunicipioAliquota with ID {id} not found");

            await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            if (id == Guid.Empty)
                return false;

            var municipio = await _repository.GetByIdAsync(id);
            return municipio != null;
        }

        public async Task<bool> ExistsByCodigoAsync(string codigoMunicipio)
        {
            if (string.IsNullOrWhiteSpace(codigoMunicipio))
                return false;

            var municipio = await _repository.GetByCodigoMunicipioAsync(codigoMunicipio);
            return municipio != null;
        }

        private void ValidateMunicipioAliquota(MunicipioAliquota municipioAliquota)
        {
            if (string.IsNullOrWhiteSpace(municipioAliquota.CodigoMunicipio))
                throw new ArgumentException("Codigo municipio is required", nameof(municipioAliquota.CodigoMunicipio));

            if (string.IsNullOrWhiteSpace(municipioAliquota.NomeMunicipio))
                throw new ArgumentException("Nome municipio is required", nameof(municipioAliquota.NomeMunicipio));

            if (string.IsNullOrWhiteSpace(municipioAliquota.UF) || municipioAliquota.UF.Length != 2)
                throw new ArgumentException("Valid UF is required (2 characters)", nameof(municipioAliquota.UF));

            if (municipioAliquota.AliquotaISS < 0 || municipioAliquota.AliquotaISS > 100)
                throw new ArgumentException("AliquotaISS must be between 0 and 100", nameof(municipioAliquota.AliquotaISS));

            if (municipioAliquota.AliquotaIRPJ.HasValue && (municipioAliquota.AliquotaIRPJ < 0 || municipioAliquota.AliquotaIRPJ > 100))
                throw new ArgumentException("AliquotaIRPJ must be between 0 and 100", nameof(municipioAliquota.AliquotaIRPJ));

            if (municipioAliquota.AliquotaCSLL.HasValue && (municipioAliquota.AliquotaCSLL < 0 || municipioAliquota.AliquotaCSLL > 100))
                throw new ArgumentException("AliquotaCSLL must be between 0 and 100", nameof(municipioAliquota.AliquotaCSLL));

            if (municipioAliquota.AliquotaPIS.HasValue && (municipioAliquota.AliquotaPIS < 0 || municipioAliquota.AliquotaPIS > 100))
                throw new ArgumentException("AliquotaPIS must be between 0 and 100", nameof(municipioAliquota.AliquotaPIS));

            if (municipioAliquota.AliquotaCOFINS.HasValue && (municipioAliquota.AliquotaCOFINS < 0 || municipioAliquota.AliquotaCOFINS > 100))
                throw new ArgumentException("AliquotaCOFINS must be between 0 and 100", nameof(municipioAliquota.AliquotaCOFINS));

            if (municipioAliquota.AliquotaINSS.HasValue && (municipioAliquota.AliquotaINSS < 0 || municipioAliquota.AliquotaINSS > 100))
                throw new ArgumentException("AliquotaINSS must be between 0 and 100", nameof(municipioAliquota.AliquotaINSS));
        }
    }
}
