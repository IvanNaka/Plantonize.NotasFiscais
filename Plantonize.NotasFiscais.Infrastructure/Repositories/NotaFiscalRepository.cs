using AutoMapper;
using MongoDB.Driver;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Plantonize.NotasFiscais.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Infrastructure.Repositories
{
    public class NotaFiscalRepository : INotaFiscalRepository
    {
        private readonly NotasFiscaisDBContext _context;
        private readonly IMapper _mapper;

        public NotaFiscalRepository(NotasFiscaisDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<NotaFiscal?> GetByIdAsync(Guid id)
        {
            var config = await _context.NotasFiscais
                .Find(x => x.Id == id.ToString())
                .FirstOrDefaultAsync();
            
            return config != null ? _mapper.Map<NotaFiscal>(config) : null;
        }

        public async Task<IEnumerable<NotaFiscal>> GetAllAsync()
        {
            var configs = await _context.NotasFiscais
                .Find(_ => true)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<NotaFiscal>>(configs);
        }

        public async Task<IEnumerable<NotaFiscal>> GetByMedicoIdAsync(Guid medicoId)
        {
            var configs = await _context.NotasFiscais
                .Find(x => x.Medico != null && x.Medico.MedicoId == medicoId)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<NotaFiscal>>(configs);
        }

        public async Task<NotaFiscal> CreateAsync(NotaFiscal notaFiscal)
        {
            var config = _mapper.Map<NFSEConfiguration>(notaFiscal);
            await _context.NotasFiscais.InsertOneAsync(config);
            return _mapper.Map<NotaFiscal>(config);
        }

        public async Task<NotaFiscal> UpdateAsync(NotaFiscal notaFiscal)
        {
            var config = _mapper.Map<NFSEConfiguration>(notaFiscal);
            await _context.NotasFiscais.ReplaceOneAsync(
                x => x.Id == config.Id,
                config
            );
            return notaFiscal;
        }

        public async Task DeleteAsync(Guid id)
        {
            await _context.NotasFiscais.DeleteOneAsync(x => x.Id == id.ToString());
        }
    }
}
