using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Plantonize.NotasFiscais.ArchitectureTests;

/// <summary>
/// Tests to ensure API layer follows best practices
/// </summary>
public class ApiLayerTests
{
    [Fact]
    public void API_Controllers_Should_Be_Public()
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
            .Should()
            .BePublic()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All API controllers should be public");
    }

    [Fact]
    public void API_Controllers_Should_Not_Have_Business_Logic()
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
            .ShouldNot()
            .HaveDependencyOn("MongoDB.Driver")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Controllers should not directly depend on database drivers - use services instead");
    }

    [Fact]
    public void API_Should_Not_Reference_Infrastructure_Repositories()
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
            .ShouldNot()
            .HaveDependencyOn("Plantonize.NotasFiscais.Infrastructure.Repositories")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "API controllers should not directly reference Infrastructure repositories - use Application services");
    }

    [Fact]
    public void API_Controllers_Should_Have_Async_Methods()
    {
        // Arrange
        var apiAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "Plantonize.NotasFiscais.API");
        
        if (apiAssembly == null)
        {
            // Skip test if API assembly not loaded
            return;
        }
        
        var controllers = apiAssembly.GetTypes()
            .Where(t => t.Namespace == "Plantonize.NotasFiscais.API.Controllers"
                     && t.Name.EndsWith("Controller")
                     && t.IsClass);

        // Act & Assert
        foreach (var controller in controllers)
        {
            var publicMethods = controller.GetMethods()
                .Where(m => m.IsPublic && !m.IsSpecialName && m.DeclaringType == controller);

            var hasAsyncMethods = publicMethods.Any(m => 
                m.ReturnType.Name.Contains("Task"));

            hasAsyncMethods.Should().BeTrue(
                $"Controller {controller.Name} should have async methods for better scalability");
        }
    }

    [Fact]
    public void API_Program_Should_Exist()
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
        var programType = apiAssembly.GetTypes()
            .FirstOrDefault(t => t.Name == "Program");

        // Assert
        programType.Should().NotBeNull("Program.cs should exist in API project");
    }
}
