using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Plantonize.NotasFiscais.ArchitectureTests;

/// <summary>
/// Tests to ensure Application layer follows best practices
/// </summary>
public class ApplicationLayerTests
{
    [Fact]
    public void Application_Services_Should_Implement_Domain_Interface()
    {
        // Arrange
        var assembly = typeof(Application.Services.NotaFiscalService).Assembly;

        // Get all services
        var services = assembly.GetTypes()
            .Where(t => t.Namespace == "Plantonize.NotasFiscais.Application.Services" 
                     && t.Name.EndsWith("Service")
                     && t.IsClass
                     && !t.IsAbstract);

        // Assert
        foreach (var service in services)
        {
            var implementsInterface = service.GetInterfaces().Any(i => 
                i.Namespace == "Plantonize.NotasFiscais.Domain.Interfaces");
            
            implementsInterface.Should().BeTrue(
                $"Service {service.Name} should implement a domain interface");
        }
    }

    [Fact]
    public void Application_Services_Should_Not_Have_Dependency_On_Controllers()
    {
        // Arrange
        var assembly = typeof(Application.Services.NotaFiscalService).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Application")
            .ShouldNot()
            .HaveDependencyOn("Plantonize.NotasFiscais.API.Controllers")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Application services should not depend on API controllers");
    }

    [Fact]
    public void Application_Should_Not_Have_Dependency_On_MongoDB()
    {
        // Arrange
        var assembly = typeof(Application.Services.NotaFiscalService).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Application")
            .ShouldNot()
            .HaveDependencyOn("MongoDB.Driver")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Application layer should not depend directly on MongoDB");
    }

    [Fact]
    public void Application_Services_Should_Be_Public()
    {
        // Arrange
        var assembly = typeof(Application.Services.NotaFiscalService).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Application.Services")
            .Should()
            .BePublic()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All application services should be public");
    }

    [Fact]
    public void Application_Should_Have_Extensions_For_DI()
    {
        // Arrange
        var assembly = typeof(Application.Services.NotaFiscalService).Assembly;

        // Act
        var extensionsExist = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Application.Extensions")
            .GetTypes()
            .Any();

        // Assert
        extensionsExist.Should().BeTrue(
            "Application layer should have extension methods for dependency injection");
    }
}
