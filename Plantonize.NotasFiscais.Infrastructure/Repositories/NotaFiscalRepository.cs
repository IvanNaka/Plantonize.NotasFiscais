using MongoDB.Driver;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;

namespace Plantonize.NotasFiscais.Infrastructure.Repositories;

/// <summary>
/// Repository for NotaFiscal entity using MongoDB
/// </summary>
public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly IMongoCollection<NotaFiscal> _collection;

    public NotaFiscalRepository(NotasFiscaisDBContext context)
    {
        _collection = context.NotasFiscais;
    }

    public async Task<NotaFiscal?> GetByIdAsync(Guid id)
    {
        return await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<NotaFiscal>> GetAllAsync()
    {
        return await _collection
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<IEnumerable<NotaFiscal>> GetByMedicoIdAsync(Guid medicoId)
    {
        // Filtrar por medico se existir um campo MedicoId
        // Como não temos MedicoId direto, vamos retornar todas
        // Você pode ajustar este filtro conforme necessário
        return await _collection
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<NotaFiscal> CreateAsync(NotaFiscal notaFiscal)
    {
        // Garantir que o ID seja gerado se não existir
        if (notaFiscal.Id == Guid.Empty)
        {
            notaFiscal.Id = Guid.NewGuid();
        }

        await _collection.InsertOneAsync(notaFiscal);
        return notaFiscal;
    }

    public async Task<NotaFiscal> UpdateAsync(NotaFiscal notaFiscal)
    {
        await _collection.ReplaceOneAsync(
            x => x.Id == notaFiscal.Id,
            notaFiscal,
            new ReplaceOptions { IsUpsert = false }
        );
        return notaFiscal;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
