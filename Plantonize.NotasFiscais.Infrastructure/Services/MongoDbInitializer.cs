using MongoDB.Driver;
using Plantonize.NotasFiscais.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Plantonize.NotasFiscais.Infrastructure.Services;

/// <summary>
/// Initializes MongoDB database with collections and indexes
/// </summary>
public class MongoDbInitializer
{
    private readonly NotasFiscaisDBContext _context;
    private readonly ILogger<MongoDbInitializer> _logger;

    public MongoDbInitializer(NotasFiscaisDBContext context, ILogger<MongoDbInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing MongoDB database...");

        try
        {
            // Create indexes for NotasFiscais collection
            await CreateNotasFiscaisIndexesAsync();

            // Create indexes for Faturas collection
            await CreateFaturasIndexesAsync();

            // Create indexes for MunicipiosAliquota collection
            await CreateMunicipiosAliquotaIndexesAsync();

            // Create indexes for ImpostosResumo collection
            await CreateImpostosResumoIndexesAsync();

            _logger.LogInformation("MongoDB database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during MongoDB initialization");
            throw;
        }
    }

    private async Task CreateNotasFiscaisIndexesAsync()
    {
        var collection = _context.NotasFiscais;

        // Index on NumeroNota for fast lookups
        var numeroNotaIndex = Builders<NotaFiscal>.IndexKeys
            .Ascending(x => x.NumeroNota);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<NotaFiscal>(numeroNotaIndex, 
                new CreateIndexOptions { Name = "idx_numeroNota" }));

        // Index on DataEmissao for date range queries
        var dataEmissaoIndex = Builders<NotaFiscal>.IndexKeys
            .Descending(x => x.DataEmissao);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<NotaFiscal>(dataEmissaoIndex, 
                new CreateIndexOptions { Name = "idx_dataEmissao" }));

        // Index on Status for filtering
        var statusIndex = Builders<NotaFiscal>.IndexKeys
            .Ascending(x => x.Status);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<NotaFiscal>(statusIndex, 
                new CreateIndexOptions { Name = "idx_status" }));

        _logger.LogInformation("NotasFiscais indexes created successfully.");
    }

    private async Task CreateFaturasIndexesAsync()
    {
        var collection = _context.Faturas;

        // Index on NumeroFatura
        var numeroFaturaIndex = Builders<Fatura>.IndexKeys
            .Ascending(x => x.NumeroFatura);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Fatura>(numeroFaturaIndex, 
                new CreateIndexOptions { Name = "idx_numeroFatura" }));

        // Index on MedicoId
        var medicoIdIndex = Builders<Fatura>.IndexKeys
            .Ascending(x => x.MedicoId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Fatura>(medicoIdIndex, 
                new CreateIndexOptions { Name = "idx_medicoId" }));

        // Index on Status
        var statusIndex = Builders<Fatura>.IndexKeys
            .Ascending(x => x.Status);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Fatura>(statusIndex, 
                new CreateIndexOptions { Name = "idx_fatura_status" }));

        // Index on DataVencimento
        var dataVencimentoIndex = Builders<Fatura>.IndexKeys
            .Ascending(x => x.DataVencimento);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Fatura>(dataVencimentoIndex, 
                new CreateIndexOptions { Name = "idx_dataVencimento" }));

        _logger.LogInformation("Faturas indexes created successfully.");
    }

    private async Task CreateMunicipiosAliquotaIndexesAsync()
    {
        var collection = _context.MunicipiosAliquota;

        // Unique index on CodigoMunicipio
        var codigoMunicipioIndex = Builders<MunicipioAliquota>.IndexKeys
            .Ascending(x => x.CodigoMunicipio);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<MunicipioAliquota>(codigoMunicipioIndex, 
                new CreateIndexOptions { Name = "idx_codigoMunicipio", Unique = true }));

        // Index on NomeMunicipio for search
        var nomeMunicipioIndex = Builders<MunicipioAliquota>.IndexKeys
            .Ascending(x => x.NomeMunicipio);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<MunicipioAliquota>(nomeMunicipioIndex, 
                new CreateIndexOptions { Name = "idx_nomeMunicipio" }));

        _logger.LogInformation("MunicipiosAliquota indexes created successfully.");
    }

    private async Task CreateImpostosResumoIndexesAsync()
    {
        var collection = _context.ImpostosResumo;

        // Compound unique index on MedicoId, Mes, Ano
        var medicoMesAnoIndex = Builders<ImpostoResumo>.IndexKeys
            .Ascending(x => x.MedicoId)
            .Ascending(x => x.Ano)
            .Ascending(x => x.Mes);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<ImpostoResumo>(medicoMesAnoIndex, 
                new CreateIndexOptions { Name = "idx_medico_ano_mes", Unique = true }));

        // Index on MedicoId for queries
        var medicoIdIndex = Builders<ImpostoResumo>.IndexKeys
            .Ascending(x => x.MedicoId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<ImpostoResumo>(medicoIdIndex, 
                new CreateIndexOptions { Name = "idx_impostoResumo_medicoId" }));

        _logger.LogInformation("ImpostosResumo indexes created successfully.");
    }
}
