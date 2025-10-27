using AutoMapper;
using MongoDB.Driver;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Plantonize.NotasFiscais.Infrastructure.Configuration;

namespace Plantonize.NotasFiscais.Infrastructure.Repositories
{
    public class FaturaRepository : IFaturaRepository
    {
        private readonly NotasFiscaisDBContext _context;
        private readonly IMapper _mapper;

        public FaturaRepository(NotasFiscaisDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Fatura?> GetByIdAsync(Guid id)
        {
            var config = await _context.Faturas
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
            
            return config != null ? _mapper.Map<Fatura>(config) : null;
        }

        public async Task<IEnumerable<Fatura>> GetAllAsync()
        {
            var configs = await _context.Faturas
                .Find(_ => true)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<Fatura>>(configs);
        }

        public async Task<IEnumerable<Fatura>> GetByMedicoIdAsync(Guid medicoId)
        {
            var configs = await _context.Faturas
                .Find(x => x.MedicoId == medicoId)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<Fatura>>(configs);
        }

        public async Task<Fatura> CreateAsync(Fatura fatura)
        {
            var config = _mapper.Map<FaturaConfiguration>(fatura);
            await _context.Faturas.InsertOneAsync(config);
            return _mapper.Map<Fatura>(config);
        }

        public async Task<Fatura> UpdateAsync(Fatura fatura)
        {
            var config = _mapper.Map<FaturaConfiguration>(fatura);
            await _context.Faturas.ReplaceOneAsync(
                x => x.Id == config.Id,
                config
            );
            return fatura;
        }

        public async Task DeleteAsync(Guid id)
        {
            await _context.Faturas.DeleteOneAsync(x => x.Id == id);
        }
    }
}
