using MongoDB.Driver;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;

namespace Plantonize.NotasFiscais.Infrastructure.Repositories;

/// <summary>
/// Repository for Fatura entity using MongoDB
/// </summary>
public class FaturaRepository : IFaturaRepository
{
    private readonly IMongoCollection<Fatura> _collection;

    public FaturaRepository(NotasFiscaisDBContext context)
    {
        _collection = context.Faturas;
    }

    public async Task<Fatura?> GetByIdAsync(Guid id)
    {
        return await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Fatura>> GetAllAsync()
    {
        return await _collection
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<IEnumerable<Fatura>> GetByMedicoIdAsync(Guid medicoId)
    {
        return await _collection
            .Find(x => x.MedicoId == medicoId)
            .ToListAsync();
    }

    public async Task<Fatura> CreateAsync(Fatura fatura)
    {
        if (fatura.Id == Guid.Empty)
        {
            fatura.Id = Guid.NewGuid();
        }

        await _collection.InsertOneAsync(fatura);
        return fatura;
    }

    public async Task<Fatura> UpdateAsync(Fatura fatura)
    {
        await _collection.ReplaceOneAsync(
            x => x.Id == fatura.Id,
            fatura,
            new ReplaceOptions { IsUpsert = false }
        );
        return fatura;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
