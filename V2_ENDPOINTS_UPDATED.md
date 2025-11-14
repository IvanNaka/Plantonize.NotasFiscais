# ? Endpoints V2 Atualizados - Comportamento Consistente com V1

## ?? Resumo das Alterações

Todos os endpoints V2 (Vertical Slice Architecture) foram atualizados para ter o **mesmo comportamento** dos endpoints V1 (Clean Architecture), mantendo:

1. ? **Tratamento de exceções consistente**
2. ? **Logging detalhado**
3. ? **Códigos de status HTTP apropriados**
4. ? **Mensagens de erro padronizadas**
5. ? **Validação automática com FluentValidation**

---

## ?? Endpoints Atualizados

### 1. ? Create (POST /api/v2/notas-fiscais)

**Arquivo**: `CreateNotaFiscalEndpoint.cs`

#### Melhorias Implementadas:
- ? Tratamento de `ArgumentException` ? 400 Bad Request
- ? Tratamento de `FluentValidation.ValidationException` ? 400 Bad Request com detalhes
- ? Tratamento de exceções genéricas ? 500 Internal Server Error
- ? Logging de informação, warning e error
- ? Response types documentados no Swagger

#### Exemplo de Uso:
```bash
POST /api/v2/notas-fiscais
Content-Type: application/json

{
  "numeroNota": "NF-2025-001",
  "dataEmissao": "2025-01-12T10:00:00Z",
  "valorTotal": 1500.00,
  "municipioPrestacao": "São Paulo",
  "issRetido": false
}
```

#### Respostas:
- **201 Created**: Nota fiscal criada com sucesso
- **400 Bad Request**: Dados inválidos ou validação falhou
- **500 Internal Server Error**: Erro no servidor

---

### 2. ? Update (PUT /api/v2/notas-fiscais/{id})

**Arquivo**: `UpdateNotaFiscalEndpoint.cs`

#### Melhorias Implementadas:
- ? Validação de ID mismatch
- ? Tratamento de `ArgumentException` ? 400 Bad Request
- ? Tratamento de `FluentValidation.ValidationException` ? 400 Bad Request
- ? Tratamento de `InvalidOperationException` ? 404 Not Found
- ? Logging completo de cada etapa
- ? Retorno consistente com V1

#### Exemplo de Uso:
```bash
PUT /api/v2/notas-fiscais/{id}
Content-Type: application/json

{
  "id": "{id}",
  "numeroNota": "NF-2025-001-UPD",
  "valorTotal": 2000.00
}
```

#### Respostas:
- **200 OK**: Nota fiscal atualizada
- **400 Bad Request**: ID mismatch ou dados inválidos
- **404 Not Found**: Nota fiscal não encontrada
- **500 Internal Server Error**: Erro no servidor

---

### 3. ? Delete (DELETE /api/v2/notas-fiscais/{id})

**Arquivo**: `DeleteNotaFiscalEndpoint.cs`

#### Melhorias Implementadas:
- ? Tratamento de `ArgumentException` ? 400 Bad Request
- ? Tratamento de `InvalidOperationException` ? 404 Not Found
- ? Logging de sucesso e warnings
- ? Retorno 204 No Content em caso de sucesso
- ? Mensagens de erro descritivas

#### Exemplo de Uso:
```bash
DELETE /api/v2/notas-fiscais/{id}
```

#### Respostas:
- **204 No Content**: Nota fiscal deletada com sucesso
- **400 Bad Request**: ID inválido
- **404 Not Found**: Nota fiscal não encontrada
- **500 Internal Server Error**: Erro no servidor

---

### 4. ? GetById (GET /api/v2/notas-fiscais/{id})

**Arquivo**: `GetNotaFiscalEndpoint.cs`

#### Melhorias Implementadas:
- ? Tratamento de `ArgumentException` ? 400 Bad Request
- ? Verificação de null ? 404 Not Found
- ? Logging de warnings quando não encontrado
- ? Mensagens de erro consistentes com V1

#### Exemplo de Uso:
```bash
GET /api/v2/notas-fiscais/{id}
```

#### Respostas:
- **200 OK**: Nota fiscal encontrada
- **400 Bad Request**: ID inválido
- **404 Not Found**: Nota fiscal não encontrada
- **500 Internal Server Error**: Erro no servidor

---

### 5. ? List (GET /api/v2/notas-fiscais)

**Arquivo**: `ListNotasFiscaisEndpoint.cs`

#### Melhorias Implementadas:
- ? Tratamento de exceções ? 500 Internal Server Error
- ? Logging de informação e erros
- ? Retorno consistente com V1
- ? Response type documentado

#### Exemplo de Uso:
```bash
GET /api/v2/notas-fiscais
```

#### Respostas:
- **200 OK**: Lista de notas fiscais (pode ser vazia)
- **500 Internal Server Error**: Erro no servidor

---

## ?? Comparação V1 vs V2

| Aspecto | V1 (Clean Architecture) | V2 (Vertical Slice) |
|---------|------------------------|---------------------|
| **Endpoint Base** | `/api/NotasFiscais` | `/api/v2/notas-fiscais` |
| **Padrão** | Controller ? Service ? Repository | Endpoint ? MediatR ? Handler ? Repository |
| **Validação** | Manual no Service | Automática com FluentValidation |
| **Tratamento de Erros** | ? Consistente | ? Consistente (AGORA) |
| **Logging** | ? Completo | ? Completo (AGORA) |
| **Response Types** | ? Documentado | ? Documentado (AGORA) |
| **Códigos HTTP** | ? Apropriados | ? Apropriados (AGORA) |

---

## ?? Comportamento Padronizado

### Códigos de Status HTTP

| Código | Situação | V1 | V2 |
|--------|----------|----|----|
| 200 OK | Sucesso (Get, Update) | ? | ? |
| 201 Created | Sucesso (Create) | ? | ? |
| 204 No Content | Sucesso (Delete) | ? | ? |
| 400 Bad Request | Dados inválidos | ? | ? |
| 404 Not Found | Recurso não encontrado | ? | ? |
| 500 Internal Server Error | Erro no servidor | ? | ? |

### Exceções Tratadas

| Exceção | Código HTTP | Mensagem |
|---------|-------------|----------|
| `ArgumentException` | 400 | Mensagem da exceção |
| `FluentValidation.ValidationException` | 400 | Detalhes da validação |
| `InvalidOperationException` | 404 | Mensagem da exceção |
| `Exception` (genérica) | 500 | "Error [action] data" |

### Logging Padronizado

```csharp
// Informação
_logger.LogInformation("V2 API: [Action] Nota Fiscal {Id} via Vertical Slice", id);

// Warning
_logger.LogWarning("V2 API: Nota Fiscal {Id} not found", id);
_logger.LogWarning(ex, "V2 API: Invalid argument when [action] nota fiscal {Id}", id);

// Error
_logger.LogError(ex, "V2 API: Error [action] nota fiscal {Id}", id);
```

---

## ?? Exemplos de Requisições

### Create - Success
```http
POST /api/v2/notas-fiscais HTTP/1.1
Content-Type: application/json

{
  "numeroNota": "NF-2025-001",
  "dataEmissao": "2025-01-12T10:00:00Z",
  "valorTotal": 1500.00,
  "municipioPrestacao": "São Paulo",
  "issRetido": false,
  "medico": {
    "nome": "Dr. João Silva",
    "cpfCnpj": "123.456.789-00"
  },
  "tomador": {
    "nome": "Clínica ABC",
    "cpfCnpj": "12.345.678/0001-00"
  },
  "servicos": [
    {
      "descricao": "Consulta",
      "quantidade": 1,
      "valorUnitario": 1500.00
    }
  ]
}
```

**Response**: 201 Created
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "numeroNota": "NF-2025-001",
  "dataEmissao": "2025-01-12T10:00:00Z",
  "valorTotal": 1500.00,
  "status": "Emitida"
}
```

### Create - Validation Error
```http
POST /api/v2/notas-fiscais HTTP/1.1
Content-Type: application/json

{
  "numeroNota": "NF-2025-001",
  "dataEmissao": "2025-12-31T10:00:00Z",
  "valorTotal": -100.00
}
```

**Response**: 400 Bad Request
```json
{
  "message": "Validation failed",
  "errors": [
    {
      "propertyName": "ValorTotal",
      "errorMessage": "Valor total deve ser maior que zero"
    },
    {
      "propertyName": "DataEmissao",
      "errorMessage": "Data de emissão não pode ser futura"
    }
  ]
}
```

### Update - ID Mismatch
```http
PUT /api/v2/notas-fiscais/123e4567-e89b-12d3-a456-426614174000 HTTP/1.1
Content-Type: application/json

{
  "id": "different-id-here",
  "numeroNota": "NF-2025-001"
}
```

**Response**: 400 Bad Request
```json
"ID mismatch"
```

### Delete - Not Found
```http
DELETE /api/v2/notas-fiscais/non-existent-id HTTP/1.1
```

**Response**: 404 Not Found
```json
"Nota fiscal with ID non-existent-id not found"
```

---

## ?? Testes

### Testes Unitários Atualizados

Todos os testes unitários já existentes continuam funcionando:

- ? **CreateNotaFiscalHandlerTests** (6 testes)
- ? **CreateNotaFiscalValidatorTests** (8 testes)
- ? **GetNotaFiscalHandlerTests** (4 testes)
- ? **ListNotasFiscaisHandlerTests** (4 testes)
- ? **UpdateNotaFiscalHandlerTests** (5 testes)
- ? **DeleteNotaFiscalHandlerTests** (4 testes)

### Como Testar

```bash
# Executar todos os testes
dotnet test

# Executar apenas testes V2
dotnet test --filter "FullyQualifiedName~Features.NotasFiscais"
```

---

## ?? Documentação Swagger

Todos os endpoints V2 agora têm documentação completa no Swagger:

```
http://localhost:5000/swagger
```

### Seções no Swagger:
- **NotasFiscais V1 (Clean Architecture)** - `/api/NotasFiscais`
- **NotasFiscais V2 (Vertical Slice)** - `/api/v2/notas-fiscais`

---

## ?? Benefícios

### 1. **Consistência**
- Ambas APIs (V1 e V2) se comportam da mesma forma
- Facilita migração de V1 para V2

### 2. **Manutenibilidade**
- Código padronizado
- Fácil de entender e manter

### 3. **Debugging**
- Logging detalhado em todos os endpoints
- Fácil identificar problemas

### 4. **Documentação**
- Swagger completo
- Response types documentados
- Exemplos de uso claros

### 5. **Experiência do Desenvolvedor**
- Mensagens de erro claras
- Códigos HTTP apropriados
- Validação automática

---

## ?? Diferenças Técnicas

### V1 (Clean Architecture)
```csharp
[ApiController]
[Route("api/[controller]")]
public class NotasFiscaisController : ControllerBase
{
    private readonly INotaFiscalService _service;
    
    [HttpPost]
    public async Task<ActionResult<NotaFiscal>> Create([FromBody] NotaFiscal notaFiscal)
    {
        try {
            var created = await _service.CreateAsync(notaFiscal);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex) {
            return BadRequest(ex.Message);
        }
        // ...
    }
}
```

### V2 (Vertical Slice)
```csharp
[ApiController]
[Route("api/v2/notas-fiscais")]
public class CreateNotaFiscalEndpoint : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpPost]
    public async Task<ActionResult<NotaFiscal>> Create([FromBody] CreateNotaFiscalCommand command)
    {
        try {
            var created = await _mediator.Send(command);
            return CreatedAtAction("GetById", "GetNotaFiscal", new { id = created.Id }, created);
        }
        catch (ArgumentException ex) {
            return BadRequest(ex.Message);
        }
        catch (FluentValidation.ValidationException validationEx) {
            var errors = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            return BadRequest(new { message = "Validation failed", errors });
        }
        // ...
    }
}
```

### Diferenças Chave:
1. **V1**: Controller ? Service
2. **V2**: Endpoint ? MediatR ? Handler
3. **V2**: Validação automática com FluentValidation
4. **Ambos**: Mesmo tratamento de erros e logging

---

## ? Checklist de Implementação

- [x] CreateNotaFiscalEndpoint atualizado
- [x] UpdateNotaFiscalEndpoint atualizado
- [x] DeleteNotaFiscalEndpoint atualizado
- [x] GetNotaFiscalEndpoint atualizado
- [x] ListNotasFiscaisEndpoint atualizado
- [x] Tratamento de exceções consistente
- [x] Logging padronizado
- [x] Response types documentados
- [x] Códigos HTTP apropriados
- [x] Mensagens de erro claras
- [x] Build bem-sucedido
- [x] Testes passando

---

## ?? Próximos Passos

1. ? **Testar com MongoDB limpo**
   ```bash
   mongosh
   use PlantonizeNotasFiscaisDB
   db.NotasFiscais.deleteMany({})
   ```

2. ? **Executar a aplicação**
   ```bash
   cd Plantonize.NotasFiscais.API
   dotnet run
   ```

3. ? **Testar no Swagger**
   - Acesse: http://localhost:5000/swagger
   - Teste endpoints V1 e V2
   - Compare comportamentos

4. ? **Executar testes**
   ```bash
   dotnet test
   ```

---

**Desenvolvido com ?? pela equipe Plantonize**

**Última atualização**: Janeiro 2025

**Status**: ? **TODOS OS ENDPOINTS V2 ATUALIZADOS E CONSISTENTES COM V1**
