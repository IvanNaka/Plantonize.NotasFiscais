using MongoDB.Driver;
using Plantonize.NotasFiscais.Infrastructure.Configuration;

namespace Plantonize.NotasFiscais.Infrastructure
{
    public class NotasFiscaisDBContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDBSettings _settings;

        public NotasFiscaisDBContext(MongoDBSettings settings)
        {
            _settings = settings;
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<NFSEConfiguration> NotasFiscais => 
            _database.GetCollection<NFSEConfiguration>(_settings.NotasFiscaisCollectionName);

        public IMongoCollection<FaturaConfiguration> Faturas => 
            _database.GetCollection<FaturaConfiguration>(_settings.FaturasCollectionName);

        public IMongoCollection<MunicipioAliquotaConfiguration> MunicipiosAliquota => 
            _database.GetCollection<MunicipioAliquotaConfiguration>(_settings.MunicipiosAliquotaCollectionName);

        public IMongoCollection<ImpostoResumoConfiguration> ImpostosResumo => 
            _database.GetCollection<ImpostoResumoConfiguration>(_settings.ImpostosResumoCollectionName);
    }
}

