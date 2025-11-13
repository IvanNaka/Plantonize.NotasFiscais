# Architecture Tests Reference

## Overview

This project includes comprehensive architecture tests that validate the Clean Architecture principles are being followed. These tests run automatically and ensure that:

- ? Layer dependencies are correct
- ? Naming conventions are followed
- ? Domain layer is persistence-ignorant
- ? No business logic in controllers
- ? Proper separation of concerns

## Running the Tests

```bash
# Navigate to the test project
cd Plantonize.NotasFiscais.ArchitectureTests

# Run all architecture tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "FullyQualifiedName~LayerDependencyTests"
```

## Test Categories

### 1. Layer Dependency Tests (`LayerDependencyTests.cs`)

Validates the dependency rules between layers:

| Test | Rule | Why It Matters |
|------|------|----------------|
| `Domain_Should_Not_HaveDependencyOn_Application` | Domain ? Application ? | Domain must be independent |
| `Domain_Should_Not_HaveDependencyOn_Infrastructure` | Domain ? Infrastructure ? | Persistence ignorance |
| `Domain_Should_Not_HaveDependencyOn_API` | Domain ? API ? | Domain is the core |
| `Application_Should_Not_HaveDependencyOn_Infrastructure` | Application ? Infrastructure ? | Depends only on abstractions |
| `Application_Should_Not_HaveDependencyOn_API` | Application ? API ? | Business logic is independent |
| `Infrastructure_Should_Not_HaveDependencyOn_API` | Infrastructure ? API ? | Infrastructure serves, not controls |

**Expected Dependency Flow:**
```
API ? Application ? Domain ? Infrastructure
```

### 2. Naming Convention Tests (`NamingConventionTests.cs`)

Ensures consistent naming across the codebase:

| Test | Convention | Example |
|------|-----------|---------|
| `Domain_Interfaces_Should_StartWith_I` | Interface names start with `I` | `INotaFiscalService` |
| `Domain_Entities_Should_NotHave_Suffix` | No "Entity" suffix | `NotaFiscal` not `NotaFiscalEntity` |
| `Application_Services_Should_EndWith_Service` | Service classes end with `Service` | `NotaFiscalService` |
| `Infrastructure_Repositories_Should_EndWith_Repository` | Repositories end with `Repository` | `NotaFiscalRepository` |
| `API_Controllers_Should_EndWith_Controller` | Controllers end with `Controller` | `NotasFiscaisController` |
| `Infrastructure_Configurations_Should_EndWith_Configuration` | Configs end with `Configuration` | `NFSEConfiguration` |

### 3. Domain Layer Tests (`DomainLayerTests.cs`)

Validates domain purity and persistence ignorance:

| Test | Rule | Rationale |
|------|------|-----------|
| `Domain_Entities_Should_Be_Public` | All entities public | Can be used across layers |
| `Domain_Entities_Should_Not_HaveDependencyOn_MongoDB` | No MongoDB in Domain | Database-agnostic |
| `Domain_Entities_Should_Not_HaveDependencyOn_EntityFramework` | No EF Core in Domain | ORM-agnostic |
| `Domain_Should_Not_Have_Dependency_On_AutoMapper` | No AutoMapper in Domain | Mapping is infrastructure concern |
| `Domain_Interfaces_Should_Be_In_Interfaces_Namespace` | Interfaces in `/Interfaces` folder | Consistent organization |
| `Domain_Enums_Should_EndWith_Enum` | Enums end with `Enum` | Clear identification |

### 4. Application Layer Tests (`ApplicationLayerTests.cs`)

Validates application services follow best practices:

| Test | Rule | Purpose |
|------|------|---------|
| `Application_Services_Should_Implement_Domain_Interface` | Services implement Domain interfaces | Contract adherence |
| `Application_Services_Should_Not_Have_Dependency_On_Controllers` | No controller dependencies | Unidirectional flow |
| `Application_Should_Not_Have_Dependency_On_MongoDB` | No direct database access | Use repositories |
| `Application_Services_Should_Be_Public` | Public visibility | Accessible from API |
| `Application_Should_Have_Extensions_For_DI` | Extension methods exist | Clean DI configuration |

### 5. Infrastructure Layer Tests (`InfrastructureLayerTests.cs`)

Validates infrastructure implementations:

| Test | Rule | Purpose |
|------|------|---------|
| `Infrastructure_Repositories_Should_Implement_Domain_Interface` | Implements Domain contracts | Polymorphism |
| `Infrastructure_Repositories_Should_Be_Public` | Public visibility | DI registration |
| `Infrastructure_Should_Have_Extensions_For_DI` | Extension methods exist | Clean configuration |
| `Infrastructure_Configurations_Should_Be_Internal_Or_Public` | Proper visibility | Encapsulation |
| `Infrastructure_Should_Not_Reference_Application_Services` | No circular dependencies | Layered architecture |
| `Infrastructure_DbContext_Should_Be_In_Root_Namespace` | DbContext location | Convention |

### 6. API Layer Tests (`ApiLayerTests.cs`)

Validates API controllers follow REST best practices:

| Test | Rule | Purpose |
|------|------|---------|
| `API_Controllers_Should_Be_Public` | Public visibility | HTTP accessible |
| `API_Controllers_Should_Not_Have_Business_Logic` | No direct DB access | Use services |
| `API_Should_Not_Reference_Infrastructure_Repositories` | No repository injection | Use Application services |
| `API_Controllers_Should_Have_Async_Methods` | Async/await pattern | Scalability |
| `API_Program_Should_Exist` | Entry point exists | Application startup |

## Interpreting Test Results

### ? Success
```
Test run for C:\...\Plantonize.NotasFiscais.ArchitectureTests.dll (.NETCoreApp,Version=v8.0)
Microsoft (R) Test Execution Command Line Tool Version 17.8.0 (x64)
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    24, Skipped:     0, Total:    24
```

All architectural rules are being followed! ?

### ? Failure Example
```
Failed Domain_Should_Not_HaveDependencyOn_Infrastructure
Expected result.IsSuccessful to be true because Domain layer should not depend on Infrastructure layer, but found False.
```

**What to do:**
1. Review the failing test description
2. Check which class is violating the rule
3. Refactor to follow Clean Architecture principles
4. Re-run tests

## Common Violations and Solutions

### ? Domain depending on Infrastructure
```csharp
// ? WRONG - Domain entity depends on MongoDB
using MongoDB.Bson.Serialization.Attributes;

public class NotaFiscal
{
    [BsonId]
    public Guid Id { get; set; }
}
```

```csharp
// ? CORRECT - Clean domain entity
public class NotaFiscal
{
    public Guid Id { get; set; }
}

// Infrastructure handles mapping
public class NFSEConfiguration
{
    public void Configure()
    {
        BsonClassMap.RegisterClassMap<NotaFiscal>(cm =>
        {
            cm.MapIdProperty(x => x.Id);
        });
    }
}
```

### ? Controllers injecting Repositories
```csharp
// ? WRONG - Controller bypasses Application layer
public class NotasFiscaisController : ControllerBase
{
    private readonly INotaFiscalRepository _repository;
    
    public NotasFiscaisController(INotaFiscalRepository repository)
    {
        _repository = repository;
    }
}
```

```csharp
// ? CORRECT - Controller uses Application service
public class NotasFiscaisController : ControllerBase
{
    private readonly INotaFiscalService _service;
    
    public NotasFiscaisController(INotaFiscalService service)
    {
        _service = service;
    }
}
```

### ? Application depending on Infrastructure
```csharp
// ? WRONG - Service depends on concrete implementation
using Plantonize.NotasFiscais.Infrastructure.Repositories;

public class NotaFiscalService
{
    private readonly NotaFiscalRepository _repository;
}
```

```csharp
// ? CORRECT - Service depends on abstraction
using Plantonize.NotasFiscais.Domain.Interfaces;

public class NotaFiscalService
{
    private readonly INotaFiscalRepository _repository;
}
```

## Integration with CI/CD

### GitHub Actions Example

```yaml
name: Architecture Tests

on: [push, pull_request]

jobs:
  architecture-tests:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Run Architecture Tests
      run: dotnet test Plantonize.NotasFiscais.ArchitectureTests/Plantonize.NotasFiscais.ArchitectureTests.csproj --no-restore --verbosity normal
```

### Azure DevOps Example

```yaml
trigger:
- master
- develop

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: DotNetCoreCLI@2
  displayName: 'Restore packages'
  inputs:
    command: 'restore'

- task: DotNetCoreCLI@2
  displayName: 'Run Architecture Tests'
  inputs:
    command: 'test'
    projects: '**/Plantonize.NotasFiscais.ArchitectureTests.csproj'
    arguments: '--no-restore --verbosity normal'
```

## Best Practices

1. ? **Run tests before every commit**
   ```bash
   dotnet test Plantonize.NotasFiscais.ArchitectureTests
   ```

2. ? **Add to pre-commit hooks**
   ```bash
   # .git/hooks/pre-commit
   #!/bin/sh
   dotnet test Plantonize.NotasFiscais.ArchitectureTests --no-build
   ```

3. ? **Integrate into CI/CD pipeline**
   - Block PRs if architecture tests fail
   - Generate reports

4. ? **Review failing tests immediately**
   - Don't ignore or skip failing tests
   - Fix the architecture violation

5. ? **Add custom tests for your rules**
   - Extend the test suites
   - Add project-specific conventions

## Adding Custom Architecture Tests

Example: Ensure all DTOs are in a specific namespace

```csharp
[Fact]
public void DTOs_Should_Be_In_DTOs_Namespace()
{
    // Arrange
    var assembly = typeof(Application.Services.NotaFiscalService).Assembly;

    // Act
    var result = Types.InAssembly(assembly)
        .That()
        .HaveNameEndingWith("Dto")
        .Should()
        .ResideInNamespace("Plantonize.NotasFiscais.Application.DTOs")
        .GetResult();

    // Assert
    result.IsSuccessful.Should().BeTrue(
        "All DTOs should be in the DTOs namespace");
}
```

## Troubleshooting

### Tests not discovering assemblies

**Problem**: Some tests skip because assemblies aren't loaded

**Solution**: Add project references to test project:
```xml
<ItemGroup>
  <ProjectReference Include="..\Plantonize.NotasFiscais.API\Plantonize.NotasFiscais.API.csproj" />
  <ProjectReference Include="..\Plantonize.NotasFiscais.Application\Plantonize.NotasFiscais.Application.csproj" />
  <ProjectReference Include="..\Plantonize.NotasFiscais.Domain\Plantonize.NotasFiscais.Domain.csproj" />
  <ProjectReference Include="..\Plantonize.NotasFiscais.Infrastructure\Plantonize.NotasFiscais.Infrastructure.csproj" />
</ItemGroup>
```

### False positives

**Problem**: Test fails but code seems correct

**Solution**: Check namespace and naming carefully. NetArchTest is case-sensitive.

## Summary

These architecture tests are your **safety net** ensuring:

- ? Clean Architecture principles are maintained
- ? Code remains testable and maintainable
- ? New team members follow conventions
- ? Technical debt is prevented
- ? Refactoring doesn't break architecture

**Run them often. Trust them always.** ???

---

For more information, see:
- [NetArchTest Documentation](https://github.com/BenMorris/NetArchTest)
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
