using MongoDB.Driver;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;

namespace Plantonize.NotasFiscais.Infrastructure.Repositories;

/// <summary>
/// Repository for MunicipioAliquota entity using MongoDB
/// </summary>
public class MunicipioAliquotaRepository : IMunicipioAliquotaRepository
{
    private readonly IMongoCollection<MunicipioAliquota> _collection;

    public MunicipioAliquotaRepository(NotasFiscaisDBContext context)
    {
        _collection = context.MunicipiosAliquota;
    }

    public async Task<MunicipioAliquota?> GetByIdAsync(Guid id)
    {
        return await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<MunicipioAliquota>> GetAllAsync()
    {
        return await _collection
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<MunicipioAliquota?> GetByCodigoMunicipioAsync(string codigoMunicipio)
    {
        return await _collection
            .Find(x => x.CodigoMunicipio == codigoMunicipio)
            .FirstOrDefaultAsync();
    }

    public async Task<MunicipioAliquota> CreateAsync(MunicipioAliquota municipioAliquota)
    {
        if (municipioAliquota.Id == Guid.Empty)
        {
            municipioAliquota.Id = Guid.NewGuid();
        }

        await _collection.InsertOneAsync(municipioAliquota);
        return municipioAliquota;
    }

    public async Task<MunicipioAliquota> UpdateAsync(MunicipioAliquota municipioAliquota)
    {
        await _collection.ReplaceOneAsync(
            x => x.Id == municipioAliquota.Id,
            municipioAliquota,
            new ReplaceOptions { IsUpsert = false }
        );
        return municipioAliquota;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
