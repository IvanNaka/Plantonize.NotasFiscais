using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Plantonize.NotasFiscais.ArchitectureTests;

/// <summary>
/// Tests to ensure domain entities follow best practices
/// </summary>
public class DomainLayerTests
{
    [Fact]
    public void Domain_Entities_Should_Be_Public()
    {
        // Arrange
        var assembly = typeof(Domain.Entities.NotaFiscal).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Domain.Entities")
            .Should()
            .BePublic()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All domain entities should be public");
    }

    [Fact]
    public void Domain_Entities_Should_Not_HaveDependencyOn_MongoDB()
    {
        // Arrange
        var assembly = typeof(Domain.Entities.NotaFiscal).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Domain")
            .ShouldNot()
            .HaveDependencyOn("MongoDB.Driver")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Domain layer should not depend on MongoDB - persistence ignorance");
    }

    [Fact]
    public void Domain_Entities_Should_Not_HaveDependencyOn_EntityFramework()
    {
        // Arrange
        var assembly = typeof(Domain.Entities.NotaFiscal).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Domain")
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Domain layer should not depend on EF Core - persistence ignorance");
    }

    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_AutoMapper()
    {
        // Arrange
        var assembly = typeof(Domain.Entities.NotaFiscal).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Domain")
            .ShouldNot()
            .HaveDependencyOn("AutoMapper")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Domain layer should not depend on AutoMapper");
    }

    [Fact]
    public void Domain_Interfaces_Should_Be_In_Interfaces_Namespace()
    {
        // Arrange
        var assembly = typeof(Domain.Entities.NotaFiscal).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .AreInterfaces()
            .And()
            .ResideInNamespaceStartingWith("Plantonize.NotasFiscais.Domain")
            .Should()
            .ResideInNamespace("Plantonize.NotasFiscais.Domain.Interfaces")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All domain interfaces should be in the Interfaces namespace");
    }

    [Fact]
    public void Domain_Enums_Should_EndWith_Enum()
    {
        // Arrange
        var assembly = typeof(Domain.Entities.NotaFiscal).Assembly;

        // Act - Manual check since NetArchTest doesn't have AreEnums
        var enums = assembly.GetTypes()
            .Where(t => t.Namespace == "Plantonize.NotasFiscais.Domain.Enum" && t.IsEnum);

        // Assert
        foreach (var enumType in enums)
        {
            enumType.Name.Should().EndWith("Enum",
                $"Enum {enumType.Name} should end with 'Enum'");
        }
    }
}
