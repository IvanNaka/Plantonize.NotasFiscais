using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.Infrastructure.Services
{
    public class DatabaseSeeder
    {
        private readonly IMunicipioAliquotaRepository _municipioAliquotaRepository;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            IMunicipioAliquotaRepository municipioAliquotaRepository,
            ILogger<DatabaseSeeder> logger)
        {
            _municipioAliquotaRepository = municipioAliquotaRepository;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Starting database seeding...");

            await SeedMunicipiosAliquotaAsync();

            _logger.LogInformation("Database seeding completed.");
        }

        private async Task SeedMunicipiosAliquotaAsync()
        {
            var existingMunicipios = await _municipioAliquotaRepository.GetAllAsync();
            if (existingMunicipios.Any())
            {
                _logger.LogInformation("MunicipiosAliquota already seeded. Skipping...");
                return;
            }

            _logger.LogInformation("Seeding MunicipiosAliquota...");

            var municipios = new List<MunicipioAliquota>
            {
                // São Paulo - Capital
                new MunicipioAliquota
                {
                    Id = Guid.NewGuid(),
                    CodigoMunicipio = "3550308",
                    NomeMunicipio = "São Paulo",
                    UF = "SP",
                    AliquotaISS = 5.0m,
                    AliquotaIRPJ = 4.8m,
                    AliquotaCSLL = 2.88m,
                    AliquotaPIS = 0.65m,
                    AliquotaCOFINS = 3.0m,
                    AliquotaINSS = 11.0m,
                    DataAtualizacao = DateTime.UtcNow
                },
                // Rio de Janeiro - Capital
                new MunicipioAliquota
                {
                    Id = Guid.NewGuid(),
                    CodigoMunicipio = "3304557",
                    NomeMunicipio = "Rio de Janeiro",
                    UF = "RJ",
                    AliquotaISS = 5.0m,
                    AliquotaIRPJ = 4.8m,
                    AliquotaCSLL = 2.88m,
                    AliquotaPIS = 0.65m,
                    AliquotaCOFINS = 3.0m,
                    AliquotaINSS = 11.0m,
                    DataAtualizacao = DateTime.UtcNow
                },
                // Belo Horizonte
                new MunicipioAliquota
                {
                    Id = Guid.NewGuid(),
                    CodigoMunicipio = "3106200",
                    NomeMunicipio = "Belo Horizonte",
                    UF = "MG",
                    AliquotaISS = 5.0m,
                    AliquotaIRPJ = 4.8m,
                    AliquotaCSLL = 2.88m,
                    AliquotaPIS = 0.65m,
                    AliquotaCOFINS = 3.0m,
                    AliquotaINSS = 11.0m,
                    DataAtualizacao = DateTime.UtcNow
                },
                // Brasília
                new MunicipioAliquota
                {
                    Id = Guid.NewGuid(),
                    CodigoMunicipio = "5300108",
                    NomeMunicipio = "Brasília",
                    UF = "DF",
                    AliquotaISS = 5.0m,
                    AliquotaIRPJ = 4.8m,
                    AliquotaCSLL = 2.88m,
                    AliquotaPIS = 0.65m,
                    AliquotaCOFINS = 3.0m,
                    AliquotaINSS = 11.0m,
                    DataAtualizacao = DateTime.UtcNow
                },
                // Porto Alegre
                new MunicipioAliquota
                {
                    Id = Guid.NewGuid(),
                    CodigoMunicipio = "4314902",
                    NomeMunicipio = "Porto Alegre",
                    UF = "RS",
                    AliquotaISS = 5.0m,
                    AliquotaIRPJ = 4.8m,
                    AliquotaCSLL = 2.88m,
                    AliquotaPIS = 0.65m,
                    AliquotaCOFINS = 3.0m,
                    AliquotaINSS = 11.0m,
                    DataAtualizacao = DateTime.UtcNow
                }
            };

            foreach (var municipio in municipios)
            {
                await _municipioAliquotaRepository.CreateAsync(municipio);
                _logger.LogInformation($"Seeded município: {municipio.NomeMunicipio} ({municipio.UF})");
            }

            _logger.LogInformation($"Seeded {municipios.Count} municípios successfully.");
        }
    }
}
