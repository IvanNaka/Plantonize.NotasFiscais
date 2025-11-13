# Vertical Slice Architecture - Implementation Guide

## Overview

This guide shows how to implement **Vertical Slice Architecture** alongside the existing Clean Architecture in this project. Vertical Slices organize code by feature rather than by technical layer.

## Why Vertical Slices?

- ? **Feature Cohesion**: All code for a feature is in one place
- ? **Independent Changes**: Changes to one feature don't affect others
- ? **Easy Onboarding**: New developers can understand one feature at a time
- ? **Parallel Development**: Teams can work on different features independently

## When to Use Vertical Slices vs Clean Architecture?

| Scenario | Recommended Approach |
|----------|---------------------|
| Core domain entities shared across features | Clean Architecture |
| Feature-specific logic with minimal reuse | Vertical Slice |
| Experimental features | Vertical Slice |
| MVP or prototypes | Vertical Slice |
| Complex business rules shared | Clean Architecture |

## Example: NotaFiscais Feature Slice

### Folder Structure

```
Plantonize.NotasFiscais.API/
??? Features/
    ??? NotasFiscais/
        ??? Create/
        ?   ??? CreateNotaFiscalCommand.cs
        ?   ??? CreateNotaFiscalHandler.cs
        ?   ??? CreateNotaFiscalValidator.cs
        ?   ??? CreateNotaFiscalEndpoint.cs
        ??? GetById/
        ?   ??? GetNotaFiscalQuery.cs
        ?   ??? GetNotaFiscalHandler.cs
        ?   ??? GetNotaFiscalEndpoint.cs
        ??? List/
        ?   ??? ListNotasFiscaisQuery.cs
        ?   ??? ListNotasFiscaisHandler.cs
        ?   ??? ListNotasFiscaisEndpoint.cs
        ??? Update/
            ??? UpdateNotaFiscalCommand.cs
            ??? UpdateNotaFiscalHandler.cs
            ??? UpdateNotaFiscalValidator.cs
            ??? UpdateNotaFiscalEndpoint.cs
```

### Implementation Steps

#### 1. Install MediatR (Recommended)

```bash
dotnet add package MediatR
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
```

#### 2. Create a Command/Query

**CreateNotaFiscalCommand.cs**
```csharp
using MediatR;
using Plantonize.NotasFiscais.Domain.Entities;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Create;

public record CreateNotaFiscalCommand(
    string NumeroNFSE,
    DateTime DataEmissao,
    decimal ValorServicos,
    string DescricaoServicos,
    Guid MedicoFiscalId
) : IRequest<NotaFiscal>;
```

#### 3. Create a Handler

**CreateNotaFiscalHandler.cs**
```csharp
using MediatR;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Plantonize.NotasFiscais.Domain.Enum;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Create;

public class CreateNotaFiscalHandler : IRequestHandler<CreateNotaFiscalCommand, NotaFiscal>
{
    private readonly INotaFiscalRepository _repository;
    private readonly ILogger<CreateNotaFiscalHandler> _logger;

    public CreateNotaFiscalHandler(
        INotaFiscalRepository repository,
        ILogger<CreateNotaFiscalHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<NotaFiscal> Handle(CreateNotaFiscalCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating Nota Fiscal {NumeroNFSE}", request.NumeroNFSE);

        var notaFiscal = new NotaFiscal
        {
            Id = Guid.NewGuid(),
            NumeroNFSE = request.NumeroNFSE,
            DataEmissao = request.DataEmissao,
            ValorServicos = request.ValorServicos,
            DescricaoServicos = request.DescricaoServicos,
            MedicoFiscalId = request.MedicoFiscalId,
            Status = StatusNFSEEnum.Emitida,
            DataCadastro = DateTime.UtcNow
        };

        var result = await _repository.AddAsync(notaFiscal);
        
        _logger.LogInformation("Nota Fiscal {Id} created successfully", result.Id);

        return result;
    }
}
```

#### 4. Create a Validator (Optional)

**CreateNotaFiscalValidator.cs**
```csharp
using FluentValidation;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Create;

public class CreateNotaFiscalValidator : AbstractValidator<CreateNotaFiscalCommand>
{
    public CreateNotaFiscalValidator()
    {
        RuleFor(x => x.NumeroNFSE)
            .NotEmpty().WithMessage("Número da NFSE é obrigatório")
            .MaximumLength(50).WithMessage("Número da NFSE deve ter no máximo 50 caracteres");

        RuleFor(x => x.ValorServicos)
            .GreaterThan(0).WithMessage("Valor dos serviços deve ser maior que zero");

        RuleFor(x => x.DescricaoServicos)
            .NotEmpty().WithMessage("Descrição dos serviços é obrigatória")
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");

        RuleFor(x => x.MedicoFiscalId)
            .NotEmpty().WithMessage("ID do médico é obrigatório");

        RuleFor(x => x.DataEmissao)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Data de emissão não pode ser futura");
    }
}
```

#### 5. Create an Endpoint

**CreateNotaFiscalEndpoint.cs**
```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Create;

[ApiController]
[Route("api/v2/notas-fiscais")]
[Tags("NotasFiscais V2 (Vertical Slice)")]
public class CreateNotaFiscalEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateNotaFiscalEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new Nota Fiscal (Vertical Slice implementation)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NotaFiscal), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateNotaFiscalCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetNotaFiscalEndpoint.GetById), 
            new { id = result.Id }, result);
    }
}
```

#### 6. Configure MediatR in Program.cs

```csharp
// Add this after builder.Services.AddApplicationServices();
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
```

## Complete Example Files

Create these files to see Vertical Slices in action:

### GetById Feature

**GetNotaFiscalQuery.cs**
```csharp
using MediatR;
using Plantonize.NotasFiscais.Domain.Entities;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.GetById;

public record GetNotaFiscalQuery(Guid Id) : IRequest<NotaFiscal?>;
```

**GetNotaFiscalHandler.cs**
```csharp
using MediatR;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.GetById;

public class GetNotaFiscalHandler : IRequestHandler<GetNotaFiscalQuery, NotaFiscal?>
{
    private readonly INotaFiscalRepository _repository;

    public GetNotaFiscalHandler(INotaFiscalRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotaFiscal?> Handle(GetNotaFiscalQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
```

**GetNotaFiscalEndpoint.cs**
```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.GetById;

[ApiController]
[Route("api/v2/notas-fiscais")]
[Tags("NotasFiscais V2 (Vertical Slice)")]
public class GetNotaFiscalEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public GetNotaFiscalEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a Nota Fiscal by ID (Vertical Slice implementation)
    /// </summary>
    [HttpGet("{id:guid}", Name = nameof(GetById))]
    [ProducesResponseType(typeof(NotaFiscal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetNotaFiscalQuery(id));
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }
}
```

## Testing Vertical Slices

### Unit Test Example

```csharp
using FluentAssertions;
using Moq;
using Plantonize.NotasFiscais.API.Features.NotasFiscais.Create;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Xunit;

namespace Plantonize.NotasFiscais.Tests.Features;

public class CreateNotaFiscalHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_NotaFiscal()
    {
        // Arrange
        var mockRepository = new Mock<INotaFiscalRepository>();
        var mockLogger = new Mock<ILogger<CreateNotaFiscalHandler>>();
        
        var command = new CreateNotaFiscalCommand(
            "12345",
            DateTime.Now,
            1000m,
            "Consulta médica",
            Guid.NewGuid()
        );

        mockRepository
            .Setup(x => x.AddAsync(It.IsAny<NotaFiscal>()))
            .ReturnsAsync((NotaFiscal nf) => nf);

        var handler = new CreateNotaFiscalHandler(mockRepository.Object, mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.NumeroNFSE.Should().Be("12345");
        result.ValorServicos.Should().Be(1000m);
        mockRepository.Verify(x => x.AddAsync(It.IsAny<NotaFiscal>()), Times.Once);
    }
}
```

## Benefits of This Hybrid Approach

1. **Clean Architecture** for shared domain logic and entities
2. **Vertical Slices** for feature-specific implementation
3. **Best of Both Worlds**: Stability + Flexibility
4. **Easy Migration**: Can migrate features gradually from Clean Architecture to Vertical Slices

## Migration Strategy

To migrate existing features to Vertical Slices:

1. Create new `/Features` folder in API project
2. Implement new features using Vertical Slice pattern
3. Gradually migrate existing features as needed
4. Keep domain entities and interfaces in Domain layer
5. Remove Application layer services for migrated features

## Additional Resources

- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [Vertical Slice Architecture by Jimmy Bogard](https://jimmybogard.com/vertical-slice-architecture/)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)

---

**Note**: The Vertical Slice examples in this document are for illustration purposes. To actually implement them, you would need to:
1. Install the required NuGet packages
2. Create the Features folder structure
3. Implement the handlers and endpoints
4. Register MediatR in Program.cs
