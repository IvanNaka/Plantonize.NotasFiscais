using MongoDB.Driver;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Infrastructure.Configuration;
using System;
using System.Security.Authentication;

namespace Plantonize.NotasFiscais.Infrastructure;

/// <summary>
/// MongoDB Database Context for Plantonize NotasFiscais
/// </summary>
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

    // ✅ Use Domain Entities directly instead of Configuration classes
    public IMongoCollection<NotaFiscal> NotasFiscais =>
        _database.GetCollection<NotaFiscal>(_settings.NotasFiscaisCollectionName ?? "NotasFiscais");

    public IMongoCollection<Fatura> Faturas =>
        _database.GetCollection<Fatura>(_settings.FaturasCollectionName ?? "Faturas");

    public IMongoCollection<MunicipioAliquota> MunicipiosAliquota =>
        _database.GetCollection<MunicipioAliquota>(_settings.MunicipiosAliquotaCollectionName ?? "MunicipiosAliquota");

    public IMongoCollection<ImpostoResumo> ImpostosResumo =>
        _database.GetCollection<ImpostoResumo>(_settings.ImpostosResumoCollectionName ?? "ImpostosResumo");
}