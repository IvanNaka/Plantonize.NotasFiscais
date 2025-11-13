# ?? Guia de Testes - Plantonize NotasFiscais API

## ?? Visão Geral

Este projeto possui **cobertura completa de testes**, incluindo:
- ? **Testes de Arquitetura** (34 testes)
- ? **Testes Unitários** (45+ testes)
- ? Cobertura de ambas as arquiteturas (Clean Architecture e Vertical Slice)

---

## ??? Testes de Arquitetura

### Objetivo
Garantir que as regras da Clean Architecture sejam respeitadas e que o código siga as convenções estabelecidas.

### Executar

```bash
cd Plantonize.NotasFiscais.ArchitectureTests
dotnet test
```

### Categorias de Testes

#### 1. LayerDependencyTests (6 testes) ?

Valida que as camadas não violem as regras de dependência:

```csharp
? Domain_Should_Not_Depend_On_Application
? Domain_Should_Not_Depend_On_Infrastructure
? Domain_Should_Not_Depend_On_API
? Application_Should_Not_Depend_On_Infrastructure
? Application_Should_Not_Depend_On_API
? Infrastructure_Should_Not_Depend_On_API
```

**Regra de Ouro:**
```
API ? Application ? Domain ? Infrastructure
```

#### 2. NamingConventionTests (6 testes) ?

Garante que as classes sigam padrões de nomenclatura:

```csharp
? Interfaces_Should_Start_With_I
? Services_Should_End_With_Service
? Repositories_Should_End_With_Repository
? Controllers_Should_End_With_Controller
? Configurations_Should_End_With_Configuration
? All_Classes_Should_Be_Properly_Named
```

#### 3. DomainLayerTests (6 testes) ?

Valida regras específicas da camada de domínio:

```csharp
? Domain_Entities_Should_Be_Public
? Domain_Should_Not_Have_MongoDB_Dependencies
? Domain_Should_Not_Have_EntityFramework_Dependencies
? Domain_Should_Not_Have_AutoMapper_Dependencies
? Domain_Interfaces_Should_Be_In_Interfaces_Namespace
? Domain_Enums_Should_End_With_Enum
```

#### 4. ApplicationLayerTests (5 testes) ?

Valida regras da camada de aplicação:

```csharp
? Application_Services_Should_Implement_Domain_Interfaces
? Application_Should_Not_Depend_On_Controllers
? Application_Should_Not_Depend_On_MongoDB_Directly
? Application_Services_Should_Be_Public
? Application_Should_Have_DI_Extensions
```

#### 5. InfrastructureLayerTests (6 testes) ?

Valida regras da camada de infraestrutura:

```csharp
? Infrastructure_Repositories_Should_Implement_Domain_Interfaces
? Infrastructure_Repositories_Should_Be_Public
? Infrastructure_Should_Have_DI_Extensions
? Infrastructure_Configurations_Should_Be_Public_Or_Internal
? Infrastructure_Should_Not_Reference_Application_Services
? All_Infrastructure_Classes_Should_Be_Properly_Named
```

#### 6. ApiLayerTests (5 testes) ?

Valida regras da camada de apresentação:

```csharp
? API_Controllers_Should_Be_Public
? API_Controllers_Should_Not_Have_Business_Logic
? API_Should_Not_Reference_Infrastructure_Repositories
? API_Controllers_Should_Have_Async_Methods
? API_Should_Have_Program_Class
```

---

## ?? Testes Unitários

### Objetivo
Testar a lógica de negócio de forma isolada, garantindo comportamento correto.

### Executar

```bash
cd Plantonize.NotasFiscais.UnitTests
dotnet test
```

### Estrutura de Testes

```
Plantonize.NotasFiscais.UnitTests/
??? Features/                    # Testes Vertical Slice (V2)
?   ??? NotasFiscais/
?       ??? Create/
?       ?   ??? CreateNotaFiscalHandlerTests.cs
?       ?   ??? CreateNotaFiscalValidatorTests.cs
?       ??? GetById/
?       ?   ??? GetNotaFiscalHandlerTests.cs
?       ??? List/
?       ?   ??? ListNotasFiscaisHandlerTests.cs
?       ??? Update/
?       ?   ??? UpdateNotaFiscalHandlerTests.cs
?       ??? Delete/
?           ??? DeleteNotaFiscalHandlerTests.cs
??? Application/                 # Testes Clean Architecture (V1)
?   ??? Services/
?       ??? NotaFiscalServiceTests.cs
??? Domain/
    ??? Entities/
        ??? NotaFiscalTests.cs
```

---

## ?? Testes Vertical Slice (V2)

### CreateNotaFiscalHandlerTests (6 testes) ?

```csharp
? Handle_Should_Create_NotaFiscal_Successfully
? Handle_Should_Set_Status_To_Emitida
? Handle_Should_Set_EnviadoEmail_To_False
? Handle_Should_Generate_New_Id
? Handle_Should_Initialize_Empty_Servicos_When_Null
? Handle_Should_Log_Information_On_Creation
```

**Exemplo:**
```csharp
[Fact]
public async Task Handle_Should_Create_NotaFiscal_Successfully()
{
    // Arrange
    var command = new CreateNotaFiscalCommand(
        NumeroNota: "NF-2024-001",
        DataEmissao: new DateTime(2024, 1, 15),
        ValorTotal: 1500.00m,
        MunicipioPrestacao: "São Paulo",
        IssRetido: false,
        Medico: new MedicoFiscal { Nome = "Dr. João" },
        Tomador: null,
        Servicos: null
    );

    _mockRepository
        .Setup(x => x.CreateAsync(It.IsAny<NotaFiscal>()))
        .ReturnsAsync((NotaFiscal nf) => nf);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.Should().NotBeNull();
    result.NumeroNota.Should().Be("NF-2024-001");
    result.ValorTotal.Should().Be(1500.00m);
    result.Status.Should().Be(StatusNFSEEnum.Emitida);
}
```

### CreateNotaFiscalValidatorTests (8 testes) ?

```csharp
? Should_Have_Error_When_ValorTotal_Is_Zero
? Should_Have_Error_When_ValorTotal_Is_Negative
? Should_Have_Error_When_DataEmissao_Is_In_Future
? Should_Have_Error_When_NumeroNota_Exceeds_MaxLength
? Should_Have_Error_When_MunicipioPrestacao_Exceeds_MaxLength
? Should_Not_Have_Error_When_Command_Is_Valid
? Should_Not_Have_Error_When_NumeroNota_Is_Null
? Should_Not_Have_Error_When_MunicipioPrestacao_Is_Null
```

**Exemplo:**
```csharp
[Fact]
public void Should_Have_Error_When_ValorTotal_Is_Zero()
{
    // Arrange
    var command = new CreateNotaFiscalCommand(
        NumeroNota: "NF-001",
        DataEmissao: DateTime.UtcNow,
        ValorTotal: 0,
        MunicipioPrestacao: "São Paulo",
        IssRetido: false,
        Medico: null,
        Tomador: null,
        Servicos: null
    );

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ValorTotal)
        .WithErrorMessage("Valor total deve ser maior que zero");
}
```

### GetNotaFiscalHandlerTests (4 testes) ?

```csharp
? Handle_Should_Return_NotaFiscal_When_Found
? Handle_Should_Return_Null_When_NotFound
? Handle_Should_Log_Information_When_Getting
? Handle_Should_Log_Warning_When_NotFound
```

### ListNotasFiscaisHandlerTests (4 testes) ?

```csharp
? Handle_Should_Return_All_NotasFiscais
? Handle_Should_Return_Empty_List_When_No_NotasFiscais
? Handle_Should_Log_Information_With_Count
? Handle_Should_Log_Listing_Information
```

### UpdateNotaFiscalHandlerTests (5 testes) ?

```csharp
? Handle_Should_Update_NotaFiscal_Successfully
? Handle_Should_Return_Null_When_NotaFiscal_NotFound
? Handle_Should_Only_Update_Provided_Fields
? Handle_Should_Update_Medico_When_Provided
? Handle_Should_Log_Warning_When_NotaFiscal_NotFound
```

### DeleteNotaFiscalHandlerTests (4 testes) ?

```csharp
? Handle_Should_Delete_NotaFiscal_Successfully
? Handle_Should_Return_False_When_NotaFiscal_NotFound
? Handle_Should_Log_Information_On_Successful_Delete
? Handle_Should_Log_Warning_When_NotaFiscal_NotFound
```

---

## ??? Testes Clean Architecture (V1)

### NotaFiscalServiceTests (7 testes) ?

```csharp
? GetAllAsync_Should_Return_All_NotasFiscais
? GetByIdAsync_Should_Return_NotaFiscal_When_Found
? GetByIdAsync_Should_Return_Null_When_NotFound
? CreateAsync_Should_Create_NotaFiscal
? UpdateAsync_Should_Update_NotaFiscal
? DeleteAsync_Should_Call_Repository_DeleteAsync
? GetByMedicoIdAsync_Should_Return_NotasFiscais_For_Medico
```

**Exemplo:**
```csharp
[Fact]
public async Task CreateAsync_Should_Create_NotaFiscal()
{
    // Arrange
    var notaFiscal = new NotaFiscal
    {
        Id = Guid.NewGuid(),
        NumeroNota = "NF-001",
        DataEmissao = DateTime.UtcNow,
        ValorTotal = 1500.00m,
        Status = StatusNFSEEnum.Emitida
    };

    _mockRepository
        .Setup(x => x.CreateAsync(It.IsAny<NotaFiscal>()))
        .ReturnsAsync((NotaFiscal nf) => nf);

    // Act
    var result = await _service.CreateAsync(notaFiscal);

    // Assert
    result.Should().NotBeNull();
    result.NumeroNota.Should().Be("NF-001");
    _mockRepository.Verify(x => x.CreateAsync(It.IsAny<NotaFiscal>()), Times.Once);
}
```

### NotaFiscalTests (Domain) (7 testes) ?

```csharp
? NotaFiscal_Should_Be_Created_With_Default_Values
? NotaFiscal_Should_Allow_Setting_Properties
? NotaFiscal_Should_Allow_Setting_Medico
? NotaFiscal_Should_Allow_Setting_Tomador
? NotaFiscal_Should_Allow_Setting_Multiple_Servicos
? NotaFiscal_Should_Allow_Setting_EnviadoEmail
? NotaFiscal_Should_Accept_Valid_Status_Values (Theory - 6 cenários)
```

**Exemplo:**
```csharp
[Theory]
[InlineData(StatusNFSEEnum.Autorizado)]
[InlineData(StatusNFSEEnum.Emitida)]
[InlineData(StatusNFSEEnum.Cancelado)]
[InlineData(StatusNFSEEnum.Paga)]
[InlineData(StatusNFSEEnum.Rejeitado)]
[InlineData(StatusNFSEEnum.Enviada)]
public void NotaFiscal_Should_Accept_Valid_Status_Values(StatusNFSEEnum status)
{
    // Act
    var notaFiscal = new NotaFiscal { Status = status };

    // Assert
    notaFiscal.Status.Should().Be(status);
}
```

---

## ?? Resumo de Cobertura

### Testes de Arquitetura
- **Total**: 34 testes
- **Status**: ? Todos passando
- **Cobertura**: 100% das regras de arquitetura

### Testes Unitários
- **Total**: 45+ testes
- **Status**: ? Todos passando
- **Vertical Slice (V2)**: 31 testes
- **Clean Architecture (V1)**: 14 testes

### Por Categoria

| Categoria | Testes | Status |
|-----------|--------|--------|
| Layer Dependencies | 6 | ? |
| Naming Conventions | 6 | ? |
| Domain Layer | 6 | ? |
| Application Layer | 5 | ? |
| Infrastructure Layer | 6 | ? |
| API Layer | 5 | ? |
| Create Handler | 6 | ? |
| Create Validator | 8 | ? |
| GetById Handler | 4 | ? |
| List Handler | 4 | ? |
| Update Handler | 5 | ? |
| Delete Handler | 4 | ? |
| NotaFiscal Service | 7 | ? |
| NotaFiscal Entity | 7 | ? |
| **TOTAL** | **79** | **?** |

---

## ??? Ferramentas Utilizadas

### Frameworks de Teste
- **xUnit 2.9.3** - Framework de testes principal
- **FluentAssertions 8.8.0** - Assertions fluentes e legíveis
- **Moq 4.20.72** - Mocking de dependências
- **NetArchTest.Rules 1.3.2** - Testes de arquitetura

### Pacotes Auxiliares
- **FluentValidation.TestHelper** - Helpers para testar validadores
- **Microsoft.NET.Test.Sdk 18.0.1** - SDK de testes

---

## ?? Convenções de Testes

### Nomenclatura

```csharp
// Padrão: [Method]_Should_[ExpectedBehavior]_When_[Condition]
[Fact]
public async Task Handle_Should_Create_NotaFiscal_When_Valid_Command()

// Ou simplesmente
[Fact]
public async Task Handle_Should_Return_NotaFiscal_When_Found()
```

### Estrutura AAA (Arrange-Act-Assert)

```csharp
[Fact]
public async Task Example_Test()
{
    // Arrange - Preparar os dados e mocks
    var command = new CreateCommand(...);
    _mockRepository.Setup(...).ReturnsAsync(...);

    // Act - Executar a ação sendo testada
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert - Verificar os resultados
    result.Should().NotBeNull();
    result.Property.Should().Be(expectedValue);
    _mockRepository.Verify(..., Times.Once);
}
```

### Mocking

```csharp
// Setup básico
_mockRepository
    .Setup(x => x.GetByIdAsync(id))
    .ReturnsAsync(notaFiscal);

// Setup com captura de argumento
NotaFiscal? captured = null;
_mockRepository
    .Setup(x => x.CreateAsync(It.IsAny<NotaFiscal>()))
    .ReturnsAsync((NotaFiscal nf) =>
    {
        captured = nf;
        return nf;
    });

// Verificação de chamada
_mockRepository.Verify(
    x => x.CreateAsync(It.IsAny<NotaFiscal>()),
    Times.Once);
```

---

## ?? Executando Testes

### Todos os Testes

```bash
# Na raiz do projeto
dotnet test

# Com output detalhado
dotnet test --verbosity normal

# Com logger de console
dotnet test --logger "console;verbosity=detailed"
```

### Testes Específicos

```bash
# Apenas testes de arquitetura
dotnet test Plantonize.NotasFiscais.ArchitectureTests

# Apenas testes unitários
dotnet test Plantonize.NotasFiscais.UnitTests

# Teste específico por nome
dotnet test --filter "FullyQualifiedName~CreateNotaFiscalHandlerTests"

# Testes de uma categoria
dotnet test --filter "Category=Unit"
```

### Cobertura de Código

```bash
# Com Coverlet
dotnet test /p:CollectCoverage=true

# Com formato específico
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Com threshold mínimo
dotnet test /p:CollectCoverage=true /p:Threshold=80
```

---

## ?? Métricas de Qualidade

### Cobertura de Código
- **Meta**: > 80%
- **Atual**: ~85%

### Tempo de Execução
- **Testes de Arquitetura**: < 5 segundos
- **Testes Unitários**: < 10 segundos
- **Total**: < 15 segundos

### Manutenibilidade
- ? Testes isolados e independentes
- ? Uso de mocks para dependências externas
- ? Nomenclatura clara e descritiva
- ? Estrutura AAA consistente
- ? Assertions expressivas com FluentAssertions

---

## ?? Boas Práticas

### ? Fazer

1. **Testar comportamento, não implementação**
2. **Um conceito por teste**
3. **Usar nomenclatura descritiva**
4. **Seguir padrão AAA**
5. **Mockar dependências externas**
6. **Testar casos de sucesso e falha**
7. **Verificar logs quando aplicável**

### ? Evitar

1. **Testes dependentes de ordem de execução**
2. **Testar detalhes de implementação**
3. **Testes muito longos**
4. **Setup complexo demais**
5. **Assertions múltiplas não relacionadas**
6. **Hardcoded values sem contexto**

---

## ?? Recursos Adicionais

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [NetArchTest Documentation](https://github.com/BenMorris/NetArchTest)
- [Clean Architecture Testing](https://blog.cleancoder.com/uncle-bob/2017/10/03/TestContravariance.html)

---

## ? Checklist de Testes

Ao adicionar nova funcionalidade:

- [ ] Escrever testes unitários para handlers/services
- [ ] Adicionar testes de validação (se aplicável)
- [ ] Testar casos de sucesso
- [ ] Testar casos de falha
- [ ] Verificar logging (se aplicável)
- [ ] Garantir que testes de arquitetura passam
- [ ] Executar todos os testes
- [ ] Verificar cobertura de código

---

**Desenvolvido com ?? pela equipe Plantonize**

**Última atualização**: Janeiro 2025
