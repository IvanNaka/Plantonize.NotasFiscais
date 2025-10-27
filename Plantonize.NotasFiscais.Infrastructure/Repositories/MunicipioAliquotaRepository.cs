using AutoMapper;
using MongoDB.Driver;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Plantonize.NotasFiscais.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Infrastructure.Repositories
{
    public class MunicipioAliquotaRepository : IMunicipioAliquotaRepository
    {
        private readonly NotasFiscaisDBContext _context;
        private readonly IMapper _mapper;

        public MunicipioAliquotaRepository(NotasFiscaisDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MunicipioAliquota?> GetByIdAsync(Guid id)
        {
            var config = await _context.MunicipiosAliquota
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
            
            return config != null ? _mapper.Map<MunicipioAliquota>(config) : null;
        }

        public async Task<IEnumerable<MunicipioAliquota>> GetAllAsync()
        {
            var configs = await _context.MunicipiosAliquota
                .Find(_ => true)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<MunicipioAliquota>>(configs);
        }

        public async Task<MunicipioAliquota?> GetByCodigoMunicipioAsync(string codigoMunicipio)
        {
            var config = await _context.MunicipiosAliquota
                .Find(x => x.CodigoMunicipio == codigoMunicipio)
                .FirstOrDefaultAsync();
            
            return config != null ? _mapper.Map<MunicipioAliquota>(config) : null;
        }

        public async Task<MunicipioAliquota> CreateAsync(MunicipioAliquota municipioAliquota)
        {
            var config = _mapper.Map<MunicipioAliquotaConfiguration>(municipioAliquota);
            await _context.MunicipiosAliquota.InsertOneAsync(config);
            return _mapper.Map<MunicipioAliquota>(config);
        }

        public async Task<MunicipioAliquota> UpdateAsync(MunicipioAliquota municipioAliquota)
        {
            var config = _mapper.Map<MunicipioAliquotaConfiguration>(municipioAliquota);
            await _context.MunicipiosAliquota.ReplaceOneAsync(
                x => x.Id == config.Id,
                config
            );
            return municipioAliquota;
        }

        public async Task DeleteAsync(Guid id)
        {
            await _context.MunicipiosAliquota.DeleteOneAsync(x => x.Id == id);
        }
    }
}
