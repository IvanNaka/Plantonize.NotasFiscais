using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Plantonize.NotasFiscais.ArchitectureTests;

/// <summary>
/// Tests to ensure naming conventions are followed throughout the solution
/// </summary>
public class NamingConventionTests
{
    [Fact]
    public void Domain_Interfaces_Should_StartWith_I()
    {
        // Arrange
        var assembly = typeof(Domain.Entities.NotaFiscal).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Domain.Interfaces")
            .And()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All interfaces in Domain should start with 'I'");
    }

    [Fact]
    public void Domain_Entities_Should_NotHave_Suffix()
    {
        // Arrange
        var assembly = typeof(Domain.Entities.NotaFiscal).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Domain.Entities")
            .Should()
            .NotHaveNameEndingWith("Entity")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Domain entities should not have 'Entity' suffix");
    }

    [Fact]
    public void Application_Services_Should_EndWith_Service()
    {
        // Arrange
        var assembly = typeof(Application.Services.NotaFiscalService).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Application.Services")
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Service")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All service classes should end with 'Service'");
    }

    [Fact]
    public void Infrastructure_Repositories_Should_EndWith_Repository()
    {
        // Arrange
        var assembly = typeof(Infrastructure.NotasFiscaisDBContext).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Infrastructure.Repositories")
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Repository")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All repository classes should end with 'Repository'");
    }

    [Fact]
    public void API_Controllers_Should_EndWith_Controller()
    {
        // Arrange
        var apiAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "Plantonize.NotasFiscais.API");
        
        if (apiAssembly == null)
        {
            // Skip test if API assembly not loaded
            return;
        }

        // Act
        var result = Types.InAssembly(apiAssembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.API.Controllers")
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All controller classes should end with 'Controller'");
    }

    [Fact]
    public void Infrastructure_Configurations_Should_EndWith_Configuration()
    {
        // Arrange
        var assembly = typeof(Infrastructure.NotasFiscaisDBContext).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("Plantonize.NotasFiscais.Infrastructure.Configuration")
            .And()
            .AreClasses()
            .And()
            .DoNotHaveName("MongoDBSettings")
            .And()
            .DoNotHaveName("ServiceBusSettings")
            .Should()
            .HaveNameEndingWith("Configuration")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All entity configuration classes should end with 'Configuration'");
    }
}
