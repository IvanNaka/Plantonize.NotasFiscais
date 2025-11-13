using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Plantonize.NotasFiscais.ArchitectureTests;

/// <summary>
/// Tests to ensure Clean Architecture layer dependencies are respected
/// </summary>
public class LayerDependencyTests
{
    private const string DomainNamespace = "Plantonize.NotasFiscais.Domain";
    private const string ApplicationNamespace = "Plantonize.NotasFiscais.Application";
    private const string InfrastructureNamespace = "Plantonize.NotasFiscais.Infrastructure";
    private const string ApiNamespace = "Plantonize.NotasFiscais.API";

    [Fact]
    public void Domain_Should_Not_HaveDependencyOn_Application()
    {
        // Arrange
        var assembly = typeof(Domain.Entities.NotaFiscal).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace(DomainNamespace)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Domain layer should not depend on Application layer");
    }

    [Fact]
    public void Domain_Should_Not_HaveDependencyOn_Infrastructure()
    {
        // Arrange
        var assembly = typeof(Domain.Entities.NotaFiscal).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace(DomainNamespace)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Domain layer should not depend on Infrastructure layer");
    }

    [Fact]
    public void Domain_Should_Not_HaveDependencyOn_API()
    {
        // Arrange
        var assembly = typeof(Domain.Entities.NotaFiscal).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace(DomainNamespace)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Domain layer should not depend on API layer");
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOn_Infrastructure()
    {
        // Arrange
        var assembly = typeof(Application.Services.NotaFiscalService).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace(ApplicationNamespace)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Application layer should not depend on Infrastructure layer");
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOn_API()
    {
        // Arrange
        var assembly = typeof(Application.Services.NotaFiscalService).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace(ApplicationNamespace)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Application layer should not depend on API layer");
    }

    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOn_API()
    {
        // Arrange
        var assembly = typeof(Infrastructure.NotasFiscaisDBContext).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace(InfrastructureNamespace)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Infrastructure layer should not depend on API layer");
    }
}
