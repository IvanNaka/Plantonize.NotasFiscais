using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
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

// Add services to the container.
// Configure authentication - make it optional in development
if (builder.Environment.IsProduction())
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
            .EnableTokenAcquisitionToCallDownstreamApi()
                .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
                .AddInMemoryTokenCaches()
                .AddDownstreamApi("DownstreamApi", builder.Configuration.GetSection("DownstreamApi"))
                .AddInMemoryTokenCaches();
}
else
{
    // Development: Add authentication but don't require it
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // Allow HTTP in development
            options.SaveToken = true;
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    // Log authentication failures in development
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    // Skip default behavior to allow anonymous access in development
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        });
}

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

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Plantonize NotasFiscais API v1");
        options.RoutePrefix = "swagger";
    });
    
    // In development, don't redirect HTTP to HTTPS
    // This allows testing without certificate issues
}
else
{
    app.UseHttpsRedirection();
}

// Enable CORS
app.UseCors();

app.UseAuthentication();

// In development, make authorization optional
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        // Allow anonymous access in development
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            // Create a claims identity for development
            var identity = new System.Security.Claims.ClaimsIdentity("Development");
            context.User = new System.Security.Claims.ClaimsPrincipal(identity);
        }
        await next();
    });
}

app.UseAuthorization();

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
