using Plantonize.NotasFiscais.Application.Extensions;
using Plantonize.NotasFiscais.Infrastructure.Extensions;
using Plantonize.NotasFiscais.Infrastructure.Services;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

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
        Description = "API for managing Notas Fiscais, Faturas, and Tax Calculations",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Plantonize",
            Email = "support@plantonize.com"
        }
    });

    // ? Add V2 API documentation for Vertical Slices
    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Plantonize NotasFiscais API - V2 (Vertical Slice)",
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

// Configure the HTTP request pipeline.
// Enable Swagger in all environments (Development and Production)
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
    environment = app.Environment.EnvironmentName
}))
.WithName("HealthCheck");

// Add a root endpoint
app.MapGet("/", () => Results.Ok(new 
{ 
    message = "Plantonize NotasFiscais API", 
    version = "2.0.0",
    architectures = new 
    {
        v1 = "Clean Architecture (Layered)",
        v2 = "Vertical Slice Architecture (CQRS)"
    },
    documentation = "/swagger"
}))
.WithName("Root");

app.Run();
