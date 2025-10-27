using MongoDB.Driver;
using Plantonize.NotasFiscais.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Infrastructure.Services
{
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

            // Create indexes for NotasFiscais collection
            await CreateNotasFiscaisIndexesAsync();

            // Create indexes for Faturas collection
            await CreateFaturasIndexesAsync();

            // Create indexes for MunicipiosAliquota collection
            await CreateMunicipiosAliquotaIndexesAsync();

            // Create indexes for ImpostosResumo collection
            await CreateImpostosResumoIndexesAsync();

            _logger.LogInformation("MongoDB database initialization completed.");
        }

        private async Task CreateNotasFiscaisIndexesAsync()
        {
            var collection = _context.NotasFiscais;

            // Index on numeroNota for fast lookups
            var numeroNotaIndex = Builders<NFSEConfiguration>.IndexKeys
                .Ascending(x => x.NumeroNota);
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<NFSEConfiguration>(numeroNotaIndex, 
                    new CreateIndexOptions { Name = "idx_numeroNota" }));

            // Index on dataEmissao for date range queries
            var dataEmissaoIndex = Builders<NFSEConfiguration>.IndexKeys
                .Descending(x => x.DataEmissao);
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<NFSEConfiguration>(dataEmissaoIndex, 
                    new CreateIndexOptions { Name = "idx_dataEmissao" }));

            // Compound index on medico.medicoId and dataEmissao for filtered queries
            var medicoDataIndex = Builders<NFSEConfiguration>.IndexKeys
                .Ascending("medico.medicoId")
                .Descending(x => x.DataEmissao);
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<NFSEConfiguration>(medicoDataIndex, 
                    new CreateIndexOptions { Name = "idx_medico_dataEmissao" }));

            // Index on status for filtering
            var statusIndex = Builders<NFSEConfiguration>.IndexKeys
                .Ascending(x => x.Status);
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<NFSEConfiguration>(statusIndex, 
                    new CreateIndexOptions { Name = "idx_status" }));

            _logger.LogInformation("NotasFiscais indexes created successfully.");
        }

        private async Task CreateFaturasIndexesAsync()
        {
            var collection = _context.Faturas;

            // Index on numeroFatura
            var numeroFaturaIndex = Builders<FaturaConfiguration>.IndexKeys
                .Ascending(x => x.NumeroFatura);
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<FaturaConfiguration>(numeroFaturaIndex, 
                    new CreateIndexOptions { Name = "idx_numeroFatura" }));

            // Index on medicoId
            var medicoIdIndex = Builders<FaturaConfiguration>.IndexKeys
                .Ascending(x => x.MedicoId);
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<FaturaConfiguration>(medicoIdIndex, 
                    new CreateIndexOptions { Name = "idx_medicoId" }));

            // Index on status
            var statusIndex = Builders<FaturaConfiguration>.IndexKeys
                .Ascending(x => x.Status);
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<FaturaConfiguration>(statusIndex, 
                    new CreateIndexOptions { Name = "idx_fatura_status" }));

            // Index on dataVencimento
            var dataVencimentoIndex = Builders<FaturaConfiguration>.IndexKeys
                .Ascending(x => x.DataVencimento);
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<FaturaConfiguration>(dataVencimentoIndex, 
                    new CreateIndexOptions { Name = "idx_dataVencimento" }));

            _logger.LogInformation("Faturas indexes created successfully.");
        }

        private async Task CreateMunicipiosAliquotaIndexesAsync()
        {
            var collection = _context.MunicipiosAliquota;

            // Unique index on codigoMunicipio
            var codigoMunicipioIndex = Builders<MunicipioAliquotaConfiguration>.IndexKeys
                .Ascending(x => x.CodigoMunicipio);
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<MunicipioAliquotaConfiguration>(codigoMunicipioIndex, 
                    new CreateIndexOptions { Name = "idx_codigoMunicipio", Unique = true }));

            // Index on nomeMunicipio for search
            var nomeMunicipioIndex = Builders<MunicipioAliquotaConfiguration>.IndexKeys
                .Ascending(x => x.NomeMunicipio);
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<MunicipioAliquotaConfiguration>(nomeMunicipioIndex, 
                    new CreateIndexOptions { Name = "idx_nomeMunicipio" }));

            _logger.LogInformation("MunicipiosAliquota indexes created successfully.");
        }

        private async Task CreateImpostosResumoIndexesAsync()
        {
            var collection = _context.ImpostosResumo;

            // Compound unique index on medicoId, mes, ano
            var medicoMesAnoIndex = Builders<ImpostoResumoConfiguration>.IndexKeys
                .Ascending(x => x.MedicoId)
                .Ascending(x => x.Ano)
                .Ascending(x => x.Mes);
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<ImpostoResumoConfiguration>(medicoMesAnoIndex, 
                    new CreateIndexOptions { Name = "idx_medico_ano_mes", Unique = true }));

            // Index on medicoId for queries
            var medicoIdIndex = Builders<ImpostoResumoConfiguration>.IndexKeys
                .Ascending(x => x.MedicoId);
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<ImpostoResumoConfiguration>(medicoIdIndex, 
                    new CreateIndexOptions { Name = "idx_impostoResumo_medicoId" }));

            _logger.LogInformation("ImpostosResumo indexes created successfully.");
        }
    }
}
