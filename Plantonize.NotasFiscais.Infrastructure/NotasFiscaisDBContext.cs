using MongoDB.Driver;
using Plantonize.NotasFiscais.Infrastructure.Configuration;
using System;
using System.Security.Authentication;

namespace Plantonize.NotasFiscais.Infrastructure
{
    public class NotasFiscaisDBContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDBSettings _settings;

        public NotasFiscaisDBContext(MongoDBSettings settings)
        {
            _settings = settings;

            // Configure MongoDB client with explicit SSL/TLS settings for Azure compatibility
            var mongoClientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);

            // Configure SSL/TLS settings
            mongoClientSettings.SslSettings = new SslSettings
            {
                EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                CheckCertificateRevocation = false  // Azure App Service compatibility
            };

            // Increase timeouts for better reliability in cloud environments
            mongoClientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
            mongoClientSettings.ConnectTimeout = TimeSpan.FromSeconds(30);
            mongoClientSettings.SocketTimeout = TimeSpan.FromSeconds(60);

            // Add retry configuration
            mongoClientSettings.RetryWrites = true;
            mongoClientSettings.RetryReads = true;

            var client = new MongoClient(mongoClientSettings);
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