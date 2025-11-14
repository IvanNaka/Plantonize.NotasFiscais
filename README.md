# Plantonize NotasFiscais API

![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)
![MongoDB](https://img.shields.io/badge/MongoDB-6.0+-green)
![License](https://img.shields.io/badge/license-MIT-blue)
![Tests](https://img.shields.io/badge/tests-passing-brightgreen)

API para gerenciamento de Notas Fiscais, Faturas e Cálculos de Impostos do sistema Plantonize.

Este projeto foi desenvolvido pelos seguintes alunos:

- **ANA PAULA MAGNABOSCO MILITÃO**
- **BRUNO DOS SANTOS**
- **GABRIELA VIEIRA RAMOS**
- **IVAN YUDI ODA NAKATANI**
- **SALION DE CONTO**
## ?? Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Arquitetura](#arquitetura)
  - [Clean Architecture (Camadas)](#clean-architecture-camadas)
  - [Vertical Slice Architecture](#vertical-slice-architecture)
  - [Comparação entre Arquiteturas](#comparação-entre-arquiteturas)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Tecnologias](#tecnologias)
- [Requisitos](#requisitos)
- [Instalação](#instalação)
- [Configuração](#configuração)
- [Execução](#execução)
- [Testes](#testes)
- [Endpoints da API](#endpoints-da-api)
- [Padrões e Boas Práticas](#padrões-e-boas-práticas)
- [Equipe de Desenvolvimento](#equipe-de-desenvolvimento)
- [Contribuição](#contribuição)

---

## ?? Sobre o Projeto

O **Plantonize NotasFiscais API** é uma aplicação .NET 8 desenvolvida para gerenciar o ciclo completo de notas fiscais, faturas e cálculos tributários. A aplicação utiliza MongoDB como banco de dados e Azure Service Bus para mensageria assíncrona.

Este projeto foi desenvolvido como parte de um trabalho acadêmico, implementando **duas arquiteturas distintas**: Clean Architecture (tradicional em camadas) e Vertical Slice Architecture (baseada em features), demonstrando as vantagens e casos de uso de cada abordagem.

### Principais Funcionalidades

- ? **Gestão de Notas Fiscais (NFSE)**
- ? **Gestão de Faturas**
- ? **Cálculo automático de Impostos (ISS, INSS, IR, CSLL, PIS, COFINS)**
- ? **Gestão de Alíquotas por Município**
- ? **Integração com Azure Service Bus**
- ? **Documentação automática com Swagger**
- ? **Health Checks**
- ? **Testes Unitários completos**
- ? **Testes de Arquitetura automatizados**

---

## ??? Arquitetura

Este projeto implementa **duas arquiteturas** em paralelo, permitindo comparação prática:

1. **Clean Architecture (V1)** - Arquitetura em camadas tradicional
2. **Vertical Slice Architecture (V2)** - Arquitetura baseada em features com CQRS

### Clean Architecture (Camadas)

A Clean Architecture organiza o código em camadas concêntricas, onde as camadas internas não conhecem as camadas externas.

```
???????????????????????????????????????????????
?            API (Presentation)               ?
?  - Controllers                              ?
?  - Program.cs                               ?
?  - Middlewares                              ?
???????????????????????????????????????????????
                   ?
???????????????????????????????????????????????
?         Application (Use Cases)             ?
?  - Services (Business Logic)                ?
?  - DTOs                                     ?
?  - Extensions (DI)                          ?
???????????????????????????????????????????????
                   ?
???????????????????????????????????????????????
?          Domain (Business Core)             ?
?  - Entities                                 ?
?  - Interfaces (Contracts)                   ?
?  - Enums                                    ?
?  - Business Rules                           ?
???????????????????????????????????????????????
                   ?
???????????????????????????????????????????????
?       Infrastructure (External)             ?
?  - Repositories (Data Access)               ?
?  - DbContext                                ?
?  - External Services (Service Bus)          ?
?  - Configurations                           ?
?  - Mappings                                 ?
???????????????????????????????????????????????
```

#### Camadas e Responsabilidades

##### 1?? **Domain (Núcleo de Negócio)**
- ?? **Responsabilidade**: Contém a lógica de negócio pura e as entidades do domínio
- ?? **Conteúdo**:
  - `Entities/`: Classes que representam os conceitos de negócio (NotaFiscal, Fatura, etc.)
  - `Interfaces/`: Contratos (interfaces) para serviços e repositórios
  - `Enum/`: Enumeradores de negócio
- ?? **Regra de Ouro**: NÃO pode depender de nenhuma outra camada
- ? **Vantagens**: 
  - Persistência agnóstica
  - Fácil de testar
  - Regras de negócio centralizadas

**Exemplo de Entidade:**
```csharp
namespace Plantonize.NotasFiscais.Domain.Entities;

public class NotaFiscal
{
    public Guid Id { get; set; }
    public string? NumeroNota { get; set; }
    public DateTime DataEmissao { get; set; }
    public decimal ValorTotal { get; set; }
    public StatusNFSEEnum Status { get; set; }
    // ... outras propriedades
}
```

##### 2?? **Application (Casos de Uso)**
- ?? **Responsabilidade**: Orquestra a lógica de negócio e coordena as operações
- ?? **Conteúdo**:
  - `Services/`: Implementação da lógica de aplicação
  - `DTOs/`: Objetos de transferência de dados
  - `Extensions/`: Configuração de injeção de dependência
- ?? **Dependências**: Pode depender apenas do **Domain**
- ? **Vantagens**:
  - Centraliza os casos de uso
  - Independente da infraestrutura
  - Fácil de testar

**Exemplo de Serviço:**
```csharp
namespace Plantonize.NotasFiscais.Application.Services;

public class NotaFiscalService : INotaFiscalService
{
    private readonly INotaFiscalRepository _repository;
    
    public NotaFiscalService(INotaFiscalRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<NotaFiscal> CreateAsync(NotaFiscal notaFiscal)
    {
        return await _repository.CreateAsync(notaFiscal);
    }
}
```

##### 3?? **Infrastructure (Infraestrutura)**
- ?? **Responsabilidade**: Implementa detalhes técnicos e acesso a recursos externos
- ?? **Conteúdo**:
  - `Repositories/`: Implementação de acesso a dados (MongoDB)
  - `Configuration/`: Configurações de entidades
  - `Services/`: Serviços externos (Service Bus, etc.)
  - `Extensions/`: Configuração de DI
  - `Mappings/`: AutoMapper profiles
- ?? **Dependências**: Depende do **Domain**
- ? **Vantagens**:
  - Isola detalhes técnicos
  - Facilita troca de tecnologias (ex: trocar MongoDB por SQL Server)

**Exemplo de Repositório:**
```csharp
namespace Plantonize.NotasFiscais.Infrastructure.Repositories;

public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly IMongoCollection<NotaFiscal> _collection;
    
    public NotaFiscalRepository(NotasFiscaisDBContext context)
    {
        _collection = context.NotasFiscais;
    }
    
    public async Task<NotaFiscal> CreateAsync(NotaFiscal entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }
}
```

##### 4?? **API (Apresentação)**
- ?? **Responsabilidade**: Expõe endpoints HTTP e gerencia requisições
- ?? **Conteúdo**:
  - `Controllers/`: Endpoints da API
  - `Program.cs`: Configuração da aplicação
  - `appsettings.json`: Configurações
- ?? **Dependências**: Depende de **Application** e **Infrastructure**
- ? **Vantagens**:
  - Fácil de substituir (ex: trocar por gRPC)
  - Testes de integração isolados

**Exemplo de Controller:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class NotasFiscaisController : ControllerBase
{
    private readonly INotaFiscalService _service;
    
    public NotasFiscaisController(INotaFiscalService service)
    {
        _service = service;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NotaFiscal notaFiscal)
    {
        var result = await _service.CreateAsync(notaFiscal);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
```

---

### ?? Regras de Dependência

```
API ? Application ? Domain ? Infrastructure
```

**Regras Rígidas:**
- ? Domain NÃO depende de ninguém
- ? Application depende APENAS de Domain
- ? Infrastructure depende APENAS de Domain
- ? API depende de Application e Infrastructure (apenas para DI)

**Essas regras são validadas automaticamente pelos testes de arquitetura!**

---

### Vertical Slice Architecture

A **Vertical Slice Architecture** é uma abordagem alternativa que organiza o código por funcionalidade (features) ao invés de camadas técnicas.

> ?? **Implementação**: Este projeto implementa Vertical Slices usando **MediatR** e **CQRS pattern**, disponível nos endpoints V2 da API.

#### ? Estrutura Implementada

```
Features/
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
    ?   ??? UpdateNotaFiscalCommand.cs
    ?   ??? UpdateNotaFiscalHandler.cs
    ?   ??? UpdateNotaFiscalValidator.cs
    ?   ??? UpdateNotaFiscalEndpoint.cs
    ??? Delete/
        ??? DeleteNotaFiscalCommand.cs
        ??? DeleteNotaFiscalHandler.cs
        ??? DeleteNotaFiscalEndpoint.cs
```

#### Implementação com MediatR

**Command:**
```csharp
public record CreateNotaFiscalCommand(
    string? NumeroNota,
    DateTime DataEmissao,
    decimal ValorTotal,
    string? MunicipioPrestacao,
    bool IssRetido,
    MedicoFiscal? Medico,
    TomadorServico? Tomador,
    List<ItemServico>? Servicos
) : IRequest<NotaFiscal>;
```

**Handler:**
```csharp
public class CreateNotaFiscalHandler : IRequestHandler<CreateNotaFiscalCommand, NotaFiscal>
{
    private readonly INotaFiscalRepository _repository;
    private readonly ILogger<CreateNotaFiscalHandler> _logger;
    
    public async Task<NotaFiscal> Handle(CreateNotaFiscalCommand request, CancellationToken ct)
    {
        var notaFiscal = new NotaFiscal
        {
            Id = Guid.NewGuid(),
            NumeroNota = request.NumeroNota,
            DataEmissao = request.DataEmissao,
            ValorTotal = request.ValorTotal,
            // ... outras propriedades
        };
        
        return await _repository.CreateAsync(notaFiscal);
    }
}
```

**Validator:**
```csharp
public class CreateNotaFiscalValidator : AbstractValidator<CreateNotaFiscalCommand>
{
    public CreateNotaFiscalValidator()
    {
        RuleFor(x => x.ValorTotal)
            .GreaterThan(0).WithMessage("Valor total deve ser maior que zero");
        
        RuleFor(x => x.DataEmissao)
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Data de emissão não pode ser futura");
    }
}
```

**Endpoint:**
```csharp
[ApiController]
[Route("api/v2/notas-fiscais")]
public class CreateNotaFiscalEndpoint : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotaFiscalCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
```

---

### ?? Comparação entre Arquiteturas

| Aspecto | Clean Architecture (V1) | Vertical Slice (V2) |
|---------|------------------------|---------------------|
| **Organização** | Por camadas técnicas | Por funcionalidades (features) |
| **Endpoints** | `/api/NotasFiscais` | `/api/v2/notas-fiscais` |
| **Padrão** | Layered Architecture | CQRS + MediatR |
| **Complexidade** | Média-Alta | Baixa-Média |
| **Reutilização** | Alta (serviços compartilhados) | Baixa (código duplicado intencional) |
| **Autonomia** | Baixa (mudanças afetam camadas) | Alta (mudanças isoladas) |
| **Curva de Aprendizado** | Íngreme | Suave |
| **Validação** | Manual no Controller/Service | Automática com FluentValidation |
| **Testabilidade** | Excelente | Excelente |
| **Melhor para** | Sistemas grandes e complexos | Features isoladas, CRUD, MVPs |

**Ambas as abordagens estão implementadas e funcionais neste projeto!**

---

## ?? Estrutura do Projeto

```
Plantonize.NotasFiscais/
?
??? Plantonize.NotasFiscais.API/              # Camada de Apresentação
?   ??? Controllers/                          # V1 - Clean Architecture
?   ?   ??? NotasFiscaisController.cs
?   ?   ??? FaturasController.cs
?   ?   ??? ImpostosResumoController.cs
?   ?   ??? MunicipiosAliquotaController.cs
?   ?   ??? ServiceBusController.cs
?   ??? Features/                             # V2 - Vertical Slice
?   ?   ??? NotasFiscais/
?   ?       ??? Create/
?   ?       ??? GetById/
?   ?       ??? List/
?   ?       ??? Update/
?   ?       ??? Delete/
?   ??? Properties/
?   ??? appsettings.json
?   ??? Program.cs
?
??? Plantonize.NotasFiscais.Application/      # Camada de Aplicação
?   ??? Services/                             # Lógica de negócio
?   ?   ??? NotaFiscalService.cs
?   ?   ??? FaturaService.cs
?   ?   ??? ImpostoResumoService.cs
?   ?   ??? MunicipioAliquotaService.cs
?   ??? DTOs/
?   ??? Extensions/
?
??? Plantonize.NotasFiscais.Domain/           # Camada de Domínio
?   ??? Entities/
?   ?   ??? NotaFiscal.cs
?   ?   ??? Fatura.cs
?   ?   ??? ImpostoResumo.cs
?   ?   ??? ItemServico.cs
?   ?   ??? MedicoFiscal.cs
?   ?   ??? MunicipioAliquota.cs
?   ?   ??? TomadorServico.cs
?   ??? Interfaces/
?   ??? Enum/
?
??? Plantonize.NotasFiscais.Infrastructure/   # Camada de Infraestrutura
?   ??? Repositories/
?   ??? Configuration/
?   ??? Services/
?   ??? Mappings/
?   ??? Extensions/
?   ??? NotasFiscaisDBContext.cs
?
??? Plantonize.NotasFiscais.ArchitectureTests/ # Testes de Arquitetura
?   ??? LayerDependencyTests.cs
?   ??? NamingConventionTests.cs
?   ??? DomainLayerTests.cs
?   ??? ApplicationLayerTests.cs
?   ??? InfrastructureLayerTests.cs
?   ??? ApiLayerTests.cs
?
??? Plantonize.NotasFiscais.UnitTests/        # Testes Unitários
    ??? Features/                             # Testes Vertical Slice
    ?   ??? NotasFiscais/
    ?       ??? Create/
    ?       ??? GetById/
    ?       ??? List/
    ?       ??? Update/
    ?       ??? Delete/
    ??? Application/                          # Testes Clean Architecture
    ?   ??? Services/
    ??? Domain/
        ??? Entities/
```

---

## ??? Tecnologias

### Backend
- **.NET 8** - Framework principal
- **C# 12.0** - Linguagem de programação
- **ASP.NET Core Web API** - API REST

### Banco de Dados
- **MongoDB 6.0+** - Banco de dados NoSQL
- **MongoDB.Driver 3.5.0** - Driver oficial

### Mensageria
- **Azure Service Bus 7.20.1** - Mensageria assíncrona

### Ferramentas e Bibliotecas
- **MediatR 12.4.1** - CQRS e Mediator Pattern
- **FluentValidation 11.10.0** - Validação de entrada
- **AutoMapper 12.0.1** - Mapeamento de objetos
- **Swagger/OpenAPI** - Documentação da API
- **NetArchTest.Rules 1.3.2** - Testes de arquitetura
- **xUnit 2.9.3** - Framework de testes
- **FluentAssertions 8.8.0** - Assertions para testes
- **Moq 4.20.72** - Mocking para testes

---

## ?? Requisitos

- **.NET 8 SDK** ou superior
- **MongoDB 6.0+** (local ou Atlas)
- **Azure Service Bus** (opcional, para mensageria)
- **Visual Studio 2022** ou **VS Code** (recomendado)
- **Docker** (opcional, para containerização)

---

## ?? Instalação

### 1. Clone o repositório

```bash
git clone https://github.com/IvanNaka/Plantonize.NotasFiscais.git
cd Plantonize.NotasFiscais
```

### 2. Restaure os pacotes NuGet

```bash
dotnet restore
```

### 3. Configure o MongoDB

Opção 1 - **MongoDB Local:**
```bash
# Inicie o MongoDB
mongod --dbpath /data/db
```

Opção 2 - **MongoDB Atlas (Cloud):**
- Crie uma conta em [MongoDB Atlas](https://www.mongodb.com/cloud/atlas)
- Crie um cluster gratuito
- Obtenha a connection string

---

## ?? Configuração

### 1. appsettings.json

Edite o arquivo `Plantonize.NotasFiscais.API/appsettings.json`:

```json
{
  "MongoDBSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "PlantonizeNotasFiscaisDB"
  },
  "ServiceBusSettings": {
    "ConnectionString": "Endpoint=sb://your-service-bus.servicebus.windows.net/;...",
    "QueueName": "notasfiscais-queue"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## ?? Execução

### Modo Development

```bash
cd Plantonize.NotasFiscais.API
dotnet run
```

A aplicação estará disponível em:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: http://localhost:5000/swagger

### Modo Production

```bash
dotnet run --configuration Release
```

### Com Docker

```bash
# Build da imagem
docker build -t plantonize-notasfiscais-api .

# Executar container
docker run -d -p 8080:80 --name plantonize-api plantonize-notasfiscais-api
```

---

## ?? Testes

### Testes de Arquitetura

Os testes de arquitetura garantem que as regras da Clean Architecture sejam respeitadas:

```bash
cd Plantonize.NotasFiscais.ArchitectureTests
dotnet test
```

**Testes implementados:**

1. **LayerDependencyTests** ? (6 testes)
   - Validam dependências entre camadas

2. **NamingConventionTests** ? (6 testes)
   - Validam convenções de nomenclatura

3. **DomainLayerTests** ? (6 testes)
   - Validam regras do domínio

4. **ApplicationLayerTests** ? (5 testes)
   - Validam camada de aplicação

5. **InfrastructureLayerTests** ? (6 testes)
   - Validam camada de infraestrutura

6. **ApiLayerTests** ? (5 testes)
   - Validam camada de API

**Total: 34 testes de arquitetura**

### Testes Unitários

Os testes unitários cobrem todas as funcionalidades implementadas:

```bash
cd Plantonize.NotasFiscais.UnitTests
dotnet test
```

**Testes implementados:**

#### Vertical Slice Architecture (V2)
- **CreateNotaFiscalHandlerTests** ? (6 testes)
- **CreateNotaFiscalValidatorTests** ? (8 testes)
- **GetNotaFiscalHandlerTests** ? (4 testes)
- **ListNotasFiscaisHandlerTests** ? (4 testes)
- **UpdateNotaFiscalHandlerTests** ? (5 testes)
- **DeleteNotaFiscalHandlerTests** ? (4 testes)

#### Clean Architecture (V1)
- **NotaFiscalServiceTests** ? (7 testes)
- **NotaFiscalTests** (Domain) ? (7 testes)

**Total: 45+ testes unitários**

### Executar todos os testes

```bash
dotnet test
```

### Executar testes com cobertura

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## ?? Endpoints da API

### API V1 - Clean Architecture

Base URL: `/api`

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/NotasFiscais` | Listar todas |
| GET | `/api/NotasFiscais/{id}` | Buscar por ID |
| POST | `/api/NotasFiscais` | Criar nova |
| PUT | `/api/NotasFiscais/{id}` | Atualizar |
| DELETE | `/api/NotasFiscais/{id}` | Deletar |

### API V2 - Vertical Slice

Base URL: `/api/v2`

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/v2/notas-fiscais` | Listar todas (CQRS) |
| GET | `/api/v2/notas-fiscais/{id}` | Buscar por ID (CQRS) |
| POST | `/api/v2/notas-fiscais` | Criar nova (CQRS + Validation) |
| PUT | `/api/v2/notas-fiscais/{id}` | Atualizar (CQRS + Validation) |
| DELETE | `/api/v2/notas-fiscais/{id}` | Deletar (CQRS) |

### Health Check

```http
GET /health
```

**Resposta:**
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "environment": "Development"
}
```

**Documentação completa disponível em**: `/swagger`

---

## ?? Padrões e Boas Práticas

### Princípios SOLID

- ? **S** - Single Responsibility: Cada classe tem uma única responsabilidade
- ? **O** - Open/Closed: Aberto para extensão, fechado para modificação
- ? **L** - Liskov Substitution: Uso de interfaces e abstrações
- ? **I** - Interface Segregation: Interfaces específicas e coesas
- ? **D** - Dependency Inversion: Dependência de abstrações, não implementações

### Design Patterns Utilizados

- **Repository Pattern**: Abstração do acesso a dados
- **Dependency Injection**: Inversão de controle
- **Service Layer Pattern**: Camada de serviços de aplicação
- **DTO Pattern**: Transferência de dados entre camadas
- **Options Pattern**: Configuração tipada
- **CQRS Pattern**: Separação de comandos e queries (V2)
- **Mediator Pattern**: MediatR para desacoplamento (V2)
- **Validator Pattern**: FluentValidation para validação (V2)

### Convenções de Código

- **Nomenclatura**: PascalCase para classes, camelCase para variáveis
- **Async/Await**: Todos os métodos I/O são assíncronos
- **Nullable Reference Types**: Habilitado para evitar null reference exceptions
- **ImplicitUsings**: Habilitado para reduzir boilerplate
- **Records**: Utilizados para Commands e Queries imutáveis

### Segurança

- ? HTTPS habilitado em produção
- ? CORS configurado (ajustar para produção)
- ? Validação de modelos automática
- ? Connection strings em configuração externa

---

## ?? Equipe de Desenvolvimento

Este projeto foi desenvolvido por:

- **Ana Paula Magnabosco Militão**
- **Bruno dos Santos**
- **Gabriela Vieira Ramos**
- **Ivan Yudi Oda Nakatani** - [GitHub](https://github.com/IvanNaka)
- **Salion de Conto**

---

## ?? Contribuição

Contribuições são bem-vindas! Siga os passos:

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

### Regras de Contribuição

- ? Siga as convenções de código existentes
- ? Adicione testes para novas funcionalidades
- ? Mantenha os testes de arquitetura passando
- ? Documente mudanças significativas
- ? Use commits semânticos (feat, fix, docs, refactor, test)

---

## ?? Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## ?? Suporte

Para suporte, envie um email para support@plantonize.com ou abra uma issue no GitHub.

---

## ?? Links Úteis

- [Documentação .NET 8](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [MongoDB Driver for .NET](https://www.mongodb.com/docs/drivers/csharp/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Vertical Slice Architecture](https://jimmybogard.com/vertical-slice-architecture/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [Azure Service Bus](https://learn.microsoft.com/en-us/azure/service-bus-messaging/)

---

## ?? Status do Projeto

? **Completo e Testado** - Versão 2.0.0

### Características Implementadas

- ? Clean Architecture (V1) - Completo
- ? Vertical Slice Architecture (V2) - Completo
- ? CRUD de Notas Fiscais (ambas arquiteturas)
- ? CRUD de Faturas
- ? CRUD de Impostos Resumo
- ? CRUD de Municípios Alíquota
- ? Testes de Arquitetura (34 testes)
- ? Testes Unitários (45+ testes)
- ? Documentação Swagger completa
- ? Health Checks
- ? Validação automática (FluentValidation)
- ? Logging estruturado

### Roadmap Futuro

- [ ] Implementar autenticação JWT
- [ ] Adicionar rate limiting
- [ ] Implementar cache com Redis
- [ ] Adicionar logs estruturados (Serilog)
- [ ] Implementar health checks avançados
- [ ] Adicionar métricas e observabilidade
- [ ] Testes de integração
- [ ] Deploy automatizado (CI/CD)

---

**Desenvolvido com ?? pela equipe Plantonize**

**Projeto Acadêmico - 2024/2025**
