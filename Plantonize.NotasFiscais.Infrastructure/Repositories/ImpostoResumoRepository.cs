using MongoDB.Driver;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;

namespace Plantonize.NotasFiscais.Infrastructure.Repositories;

/// <summary>
/// Repository for ImpostoResumo entity using MongoDB
/// </summary>
public class ImpostoResumoRepository : IImpostoResumoRepository
{
    private readonly IMongoCollection<ImpostoResumo> _collection;

    public ImpostoResumoRepository(NotasFiscaisDBContext context)
    {
        _collection = context.ImpostosResumo;
    }

    public async Task<ImpostoResumo?> GetByIdAsync(Guid id)
    {
        return await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ImpostoResumo>> GetAllAsync()
    {
        return await _collection
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<IEnumerable<ImpostoResumo>> GetByMedicoIdAsync(Guid medicoId)
    {
        return await _collection
            .Find(x => x.MedicoId == medicoId)
            .ToListAsync();
    }

    public async Task<ImpostoResumo?> GetByMedicoMesAnoAsync(Guid medicoId, int mes, int ano)
    {
        return await _collection
            .Find(x => x.MedicoId == medicoId && x.Mes == mes && x.Ano == ano)
            .FirstOrDefaultAsync();
    }

    public async Task<ImpostoResumo> CreateAsync(ImpostoResumo impostoResumo)
    {
        if (impostoResumo.Id == Guid.Empty)
        {
            impostoResumo.Id = Guid.NewGuid();
        }

        await _collection.InsertOneAsync(impostoResumo);
        return impostoResumo;
    }

    public async Task<ImpostoResumo> UpdateAsync(ImpostoResumo impostoResumo)
    {
        await _collection.ReplaceOneAsync(
            x => x.Id == impostoResumo.Id,
            impostoResumo,
            new ReplaceOptions { IsUpsert = false }
        );
        return impostoResumo;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
