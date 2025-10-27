using Plantonize.NotasFiscais.Application.Extensions;
using Plantonize.NotasFiscais.Infrastructure.Extensions;
using Plantonize.NotasFiscais.Infrastructure.Services;

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
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Plantonize NotasFiscais API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Plantonize NotasFiscais API Documentation";
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
    documentation = "/swagger"
}))
.WithName("Root");

app.Run();
