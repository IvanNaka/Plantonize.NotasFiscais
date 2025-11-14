using Plantonize.NotasFiscais.Application.Extensions;
using Plantonize.NotasFiscais.Infrastructure.Extensions;
using Plantonize.NotasFiscais.Infrastructure.Services;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Plantonize.NotasFiscais.Domain.Enum;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// CONFIGURAÇÃO MONGODB - Serialização de Enums e GUIDs
// ========================================

// Configurar convenções do MongoDB
var conventionPack = new ConventionPack
{
    new EnumRepresentationConvention(BsonType.String), // Enums como string
    new CamelCaseElementNameConvention(),
    new IgnoreExtraElementsConvention(true)
};

ConventionRegistry.Register("NotasFiscaisConventions", conventionPack, t => true);

// Registrar serializers personalizados para enums
BsonSerializer.RegisterSerializer(
    typeof(StatusNFSEEnum),
    new EnumSerializer<StatusNFSEEnum>(BsonType.String)
);

BsonSerializer.RegisterSerializer(
    typeof(StatusFaturaEnum),
    new EnumSerializer<StatusFaturaEnum>(BsonType.String)
);

// Registrar serializer para Guid com representação padrão
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// ========================================
// CONFIGURAÇÃO DE SERVIÇOS
// ========================================

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add Infrastructure (MongoDB, Repositories, AutoMapper)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Application Services
builder.Services.AddApplicationServices();

// ? Add MediatR for Vertical Slice Architecture
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// ? Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Plantonize NotasFiscais API",
        Version = "v1",
        Description = "API for managing Notas Fiscais, Faturas, and Tax Calculations (Clean Architecture)",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Plantonize",
            Email = "support@plantonize.com"
        }
    });

    // ? Add V2 API documentation for Vertical Slices
    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Plantonize NotasFiscais API - V2",
        Version = "v2",
        Description = "API using Vertical Slice Architecture with MediatR and CQRS patterns",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Plantonize",
            Email = "support@plantonize.com"
        }
    });

    // Enable Swagger annotations
    options.EnableAnnotations();

    // Enable XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// ========================================
// INICIALIZAÇÃO DO MONGODB
// ========================================

// Initialize MongoDB database (create collections and indexes)
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<MongoDbInitializer>();
    await initializer.InitializeAsync();
    
    // Optionally seed initial data (only in development)
    if (app.Environment.IsDevelopment())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}

// ========================================
// CONFIGURAÇÃO DO PIPELINE HTTP
// ========================================

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    // V1 - Clean Architecture
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Plantonize NotasFiscais API v1 (Clean Architecture)");
    
    // ? V2 - Vertical Slice Architecture
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "Plantonize NotasFiscais API v2 (Vertical Slice)");
    
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Plantonize NotasFiscais API Documentation";
    options.DefaultModelsExpandDepth(-1); // Hide schemas section
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Enable CORS
app.UseCors();

app.MapControllers();

// Add a health check endpoint
app.MapGet("/health", () => Results.Ok(new 
{ 
    status = "Healthy", 
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName,
    mongodb = "Connected with GUID Standard representation",
    architectures = new[] { "Clean Architecture (V1)", "Vertical Slice (V2)" }
}))
.WithName("HealthCheck")
.WithTags("Health");

// Add a root endpoint
app.MapGet("/", () => Results.Ok(new 
{ 
    message = "Plantonize NotasFiscais API", 
    version = "2.0.0",
    architectures = new 
    {
        v1 = new 
        {
            name = "Clean Architecture (Layered)",
            endpoint = "/api/NotasFiscais",
            description = "Traditional layered architecture with Services and Repositories"
        },
        v2 = new
        {
            name = "Vertical Slice Architecture (CQRS)",
            endpoint = "/api/v2/notas-fiscais",
            description = "Feature-based architecture with MediatR, CQRS, and FluentValidation"
        }
    },
    documentation = "/swagger",
    healthCheck = "/health"
}))
.WithName("Root")
.WithTags("Info");

app.Run();
