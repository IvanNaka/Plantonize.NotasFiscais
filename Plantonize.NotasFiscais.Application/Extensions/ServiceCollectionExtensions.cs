using Microsoft.Extensions.DependencyInjection;
using Plantonize.NotasFiscais.Application.Services;
using Plantonize.NotasFiscais.Domain.Interfaces;

namespace Plantonize.NotasFiscais.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Application Services
            services.AddScoped<INotaFiscalService, NotaFiscalService>();
            services.AddScoped<IFaturaService, FaturaService>();
            services.AddScoped<IMunicipioAliquotaService, MunicipioAliquotaService>();
            services.AddScoped<IImpostoResumoService, ImpostoResumoService>();

            return services;
        }
    }
}
