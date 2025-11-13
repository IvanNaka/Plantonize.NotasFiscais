using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Plantonize.NotasFiscais.ArchitectureTests;

/// <summary>
/// Tests to ensure Infrastructure layer follows best practices
/// </summary>
public class InfrastructureLayerTests
{
    [Fact]
    public void Infrastructure_Repositories_Should_Implement_Domain_Interface()
    {
        // Arrange
        var assembly = typeof(Infrastructure.NotasFiscaisDBContext).Assembly;

        // Act - Custom check since NetArchTest doesn't support "any interface from namespace"
        var repositories = assembly.GetTypes()
            .Where(t => t.Namespace == "Plantonize.NotasFiscais.Infrastructure.Repositories" 
                     && t.Name.EndsWith("Repository")
                     && t.IsClass);

        // Assert
        foreach (var repository in repositories)
        {
            var implementsInterface = repository.GetInterfaces().Any(i => 
                i.Namespace == "Plantonize.NotasFiscais.Domain.Interfaces");
            
            implementsInterface.Should().BeTrue(
                $"Repository {repository.Name} should implement a domain interface");
        }
    }

    [Fact]
    public void Infrastructure_Repositories_Should_Be_Public()
    {
        // Arrange
        var assembly = typeof(Infrastructure.NotasFiscaisDBContext).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Infrastructure.Repositories")
            .Should()
            .BePublic()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All infrastructure repositories should be public");
    }

    [Fact]
    public void Infrastructure_Should_Have_Extensions_For_DI()
    {
        // Arrange
        var assembly = typeof(Infrastructure.NotasFiscaisDBContext).Assembly;

        // Act
        var extensionsExist = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Infrastructure.Extensions")
            .GetTypes()
            .Any();

        // Assert
        extensionsExist.Should().BeTrue(
            "Infrastructure layer should have extension methods for dependency injection");
    }

    [Fact]
    public void Infrastructure_Configurations_Should_Be_Internal_Or_Public()
    {
        // Arrange
        var assembly = typeof(Infrastructure.NotasFiscaisDBContext).Assembly;

        // Act
        var configurations = assembly.GetTypes()
            .Where(t => t.Namespace == "Plantonize.NotasFiscais.Infrastructure.Configuration"
                     && t.Name.EndsWith("Configuration")
                     && t.IsClass);

        // Assert
        configurations.Should().NotBeEmpty("Infrastructure should have entity configurations");
        
        foreach (var config in configurations)
        {
            var isPublicOrInternal = config.IsPublic || config.IsNotPublic;
            isPublicOrInternal.Should().BeTrue(
                $"Configuration {config.Name} should be public or internal");
        }
    }

    [Fact]
    public void Infrastructure_Should_Not_Reference_Application_Services()
    {
        // Arrange
        var assembly = typeof(Infrastructure.NotasFiscaisDBContext).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Infrastructure.Repositories")
            .ShouldNot()
            .HaveDependencyOn("Plantonize.NotasFiscais.Application.Services")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Infrastructure repositories should not depend on Application services");
    }

    [Fact]
    public void Infrastructure_DbContext_Should_Be_In_Root_Namespace()
    {
        // Arrange
        var assembly = typeof(Infrastructure.NotasFiscaisDBContext).Assembly;

        // Act
        var dbContextType = assembly.GetTypes()
            .FirstOrDefault(t => t.Name == "NotasFiscaisDBContext");

        // Assert
        dbContextType.Should().NotBeNull("DbContext should exist");
        dbContextType!.Namespace.Should().Be("Plantonize.NotasFiscais.Infrastructure",
            "DbContext should be in root Infrastructure namespace");
    }
}
