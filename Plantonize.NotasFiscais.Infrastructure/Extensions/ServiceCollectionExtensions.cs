
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Plantonize.NotasFiscais.Infrastructure.Configuration;
using Plantonize.NotasFiscais.Infrastructure.Repositories;
using Plantonize.NotasFiscais.Infrastructure.Services;

namespace Plantonize.NotasFiscais.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register MongoDB Settings using Options pattern
            services.Configure<MongoDBSettings>(
                configuration.GetSection("MongoDBSettings"));

            // Register MongoDBSettings as singleton for direct injection
            services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);

            // Register DbContext
            services.AddSingleton<NotasFiscaisDBContext>();

            // Register Database Services
            services.AddSingleton<MongoDbInitializer>();
            services.AddScoped<DatabaseSeeder>();

            // Register AutoMapper
            services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

            // Register Repositories
            services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
            services.AddScoped<IFaturaRepository, FaturaRepository>();
            services.AddScoped<IMunicipioAliquotaRepository, MunicipioAliquotaRepository>();
            services.AddScoped<IImpostoResumoRepository, ImpostoResumoRepository>();

            // Additional configuration for TLS cipher suites
            configuration.Bind("WEBSITE_TLS_CIPHER_SUITES", new List<string>()
            {
                "TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384",
                "TLS_ECDHE_RSA_WITH_AES_128_GLM_SHA256"
            });

            return services;
        }
    }
}
