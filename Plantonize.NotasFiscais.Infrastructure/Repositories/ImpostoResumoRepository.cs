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
    public class ImpostoResumoRepository : IImpostoResumoRepository
    {
        private readonly NotasFiscaisDBContext _context;
        private readonly IMapper _mapper;

        public ImpostoResumoRepository(NotasFiscaisDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ImpostoResumo?> GetByIdAsync(Guid id)
        {
            var config = await _context.ImpostosResumo
                .Find(x => x.Id == id.ToString())
                .FirstOrDefaultAsync();
            
            return config != null ? _mapper.Map<ImpostoResumo>(config) : null;
        }

        public async Task<IEnumerable<ImpostoResumo>> GetAllAsync()
        {
            var configs = await _context.ImpostosResumo
                .Find(_ => true)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<ImpostoResumo>>(configs);
        }

        public async Task<IEnumerable<ImpostoResumo>> GetByMedicoIdAsync(Guid medicoId)
        {
            var configs = await _context.ImpostosResumo
                .Find(x => x.MedicoId == medicoId.ToString())
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<ImpostoResumo>>(configs);
        }

        public async Task<ImpostoResumo?> GetByMedicoMesAnoAsync(Guid medicoId, int mes, int ano)
        {
            var config = await _context.ImpostosResumo
                .Find(x => x.MedicoId == medicoId.ToString() && x.Mes == mes && x.Ano == ano)
                .FirstOrDefaultAsync();
            
            return config != null ? _mapper.Map<ImpostoResumo>(config) : null;
        }

        public async Task<ImpostoResumo> CreateAsync(ImpostoResumo impostoResumo)
        {
            var config = _mapper.Map<ImpostoResumoConfiguration>(impostoResumo);
            await _context.ImpostosResumo.InsertOneAsync(config);
            return _mapper.Map<ImpostoResumo>(config);
        }

        public async Task<ImpostoResumo> UpdateAsync(ImpostoResumo impostoResumo)
        {
            var config = _mapper.Map<ImpostoResumoConfiguration>(impostoResumo);
            await _context.ImpostosResumo.ReplaceOneAsync(
                x => x.Id == config.Id,
                config
            );
            return impostoResumo;
        }

        public async Task DeleteAsync(Guid id)
        {
            await _context.ImpostosResumo.DeleteOneAsync(x => x.Id == id.ToString());
        }
    }
}
