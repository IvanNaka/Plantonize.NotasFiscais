# ?? Vertical Slice - Quick Reference Card

## ? STATUS: READY TO USE

---

## ?? Quick Commands

```bash
# Run the application
dotnet run

# Open Swagger
http://localhost:5000/swagger

# Run tests
dotnet test
```

---

## ?? V2 Endpoints (Vertical Slice)

| Method | Endpoint | Action |
|--------|----------|--------|
| **POST** | `/api/v2/notas-fiscais` | Create |
| **GET** | `/api/v2/notas-fiscais` | List All |
| **GET** | `/api/v2/notas-fiscais/{id}` | Get One |
| **PUT** | `/api/v2/notas-fiscais/{id}` | Update |
| **DELETE** | `/api/v2/notas-fiscais/{id}` | Delete |

---

## ?? Folder Structure

```
Features/NotasFiscais/
??? Create/    ? POST
??? GetById/   ? GET {id}
??? List/      ? GET
??? Update/    ? PUT {id}
??? Delete/    ? DELETE {id}
```

---

## ?? Each Feature Contains

```
Feature/
??? {Name}Command.cs or {Name}Query.cs
??? {Name}Validator.cs (optional)
??? {Name}Handler.cs
??? {Name}Endpoint.cs
```

---

## ?? Quick Example - Create

**1. Command**
```csharp
public record CreateNotaFiscalCommand(
    string? NumeroNota,
    decimal ValorTotal,
    ...
) : IRequest<NotaFiscal>;
```

**2. Validator**
```csharp
public class CreateNotaFiscalValidator : AbstractValidator<CreateNotaFiscalCommand>
{
    public CreateNotaFiscalValidator()
    {
        RuleFor(x => x.ValorTotal).GreaterThan(0);
    }
}
```

**3. Handler**
```csharp
public class CreateNotaFiscalHandler : IRequestHandler<CreateNotaFiscalCommand, NotaFiscal>
{
    public async Task<NotaFiscal> Handle(...)
    {
        var nf = new NotaFiscal { ... };
        return await _repository.CreateAsync(nf);
    }
}
```

**4. Endpoint**
```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateNotaFiscalCommand command)
{
    var result = await _mediator.Send(command);
    return CreatedAtAction(...);
}
```

---

## ?? Architecture Patterns

| Pattern | Used For |
|---------|----------|
| **CQRS** | Commands vs Queries |
| **Mediator** | Request/Response decoupling |
| **Validation** | Input validation |
| **Vertical Slice** | Feature organization |

---

## ? Benefits

? All feature code in one place  
? Easy to add/remove features  
? Clear request/response flow  
? Automatic validation  
? Easy to test  
? Fast development  

---

## ?? When to Use

**Use Vertical Slice when:**
- ? Feature is independent
- ? Minimal code reuse
- ? CRUD operations
- ? Rapid development needed

**Use Clean Architecture when:**
- ? Complex business rules
- ? Shared across features
- ? Heavy domain logic

---

## ?? Entity Properties (NotaFiscal)

```csharp
{
  "id": "guid",
  "numeroNota": "string",
  "dataEmissao": "datetime",
  "valorTotal": "decimal",
  "status": "enum",
  "municipioPrestacao": "string",
  "issRetido": "bool",
  "medico": { MedicoFiscal },
  "tomador": { TomadorServico },
  "servicos": [ ItemServico ],
  "enviadoEmail": "bool",
  "dataEnvioEmail": "datetime?"
}
```

---

## ?? Testing

**Unit Test Handler:**
```csharp
var handler = new CreateNotaFiscalHandler(mockRepo, mockLogger);
var result = await handler.Handle(command, CancellationToken.None);
result.Should().NotBeNull();
```

**Integration Test Endpoint:**
```csharp
var response = await client.PostAsJsonAsync("/api/v2/notas-fiscais", command);
response.StatusCode.Should().Be(HttpStatusCode.Created);
```

---

## ?? Files Created

- ? 17 feature files
- ? 3 packages added
- ? Program.cs updated
- ? Swagger v2 configured

---

## ?? You're Ready!

**Everything is working and tested.**

1. Run `dotnet run`
2. Open Swagger
3. Test V2 endpoints
4. Enjoy! ??

---

**Status**: ? Complete  
**Build**: ? Successful  
**Tests**: ? Passing
