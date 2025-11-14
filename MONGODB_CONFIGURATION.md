# ?? Configuração do MongoDB - Plantonize NotasFiscais API

## ?? Problema Identificado

Erro no AutoMapper ao tentar mapear `NFSEConfiguration` para `NotaFiscal`:

```
AutoMapper.AutoMapperMappingException: Error mapping types.
Requested value 'Emitido' was not found in StatusNFSEEnum
```

## ?? Causa do Problema

O enum `StatusNFSEEnum` não possui o valor `Emitido`, mas sim `Emitida`. O MongoDB está retornando dados com o status incorreto.

### Valores Corretos do Enum

```csharp
public enum StatusNFSEEnum
{
    Autorizado = 1,
    Rejeitado = 2,
    Cancelado = 3,
    Emitida = 4,    // ? Valor correto
    Enviada = 5,
    Paga = 6,
}
```

---

## ? Solução 1: Limpar Dados Existentes no MongoDB

### Opção A: Via MongoDB Compass (Interface Gráfica)

1. Abra **MongoDB Compass**
2. Conecte ao seu servidor MongoDB
3. Navegue até o database `PlantonizeNotasFiscaisDB`
4. Selecione a collection `NotasFiscais`
5. Clique em **DELETE** para remover todos os documentos com status inválido

### Opção B: Via MongoDB Shell

```javascript
// Conectar ao MongoDB
mongosh

// Usar o database
use PlantonizeNotasFiscaisDB

// Verificar documentos com status inválido
db.NotasFiscais.find({ Status: "Emitido" })

// Corrigir status de "Emitido" para "Emitida"
db.NotasFiscais.updateMany(
    { Status: "Emitido" },
    { $set: { Status: "Emitida" } }
)

// Ou deletar todos os documentos e recomeçar
db.NotasFiscais.deleteMany({})
```

### Opção C: Via C# - Script de Migração

Crie um arquivo `DatabaseMigration.cs` na pasta `Infrastructure/Services`:

```csharp
using MongoDB.Driver;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Enum;

namespace Plantonize.NotasFiscais.Infrastructure.Services;

public class DatabaseMigration
{
    private readonly IMongoDatabase _database;

    public DatabaseMigration(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task MigrateStatusEnumAsync()
    {
        var collection = _database.GetCollection<NotaFiscal>("NotasFiscais");

        // Buscar todos os documentos
        var documents = await collection.Find(_ => true).ToListAsync();

        foreach (var doc in documents)
        {
            // Atualizar status se necessário
            var filter = Builders<NotaFiscal>.Filter.Eq(x => x.Id, doc.Id);
            var update = Builders<NotaFiscal>.Update.Set(x => x.Status, StatusNFSEEnum.Emitida);
            
            await collection.UpdateOneAsync(filter, update);
        }
    }

    public async Task ClearAllDataAsync()
    {
        await _database.DropCollectionAsync("NotasFiscais");
        await _database.DropCollectionAsync("Faturas");
        await _database.DropCollectionAsync("ImpostosResumo");
        await _database.DropCollectionAsync("MunicipiosAliquota");
    }
}
```

---

## ? Solução 2: Melhorar a Configuração do MongoDB

### 1. Atualizar NFSEConfiguration

Adicione tratamento de erro para enums:

```csharp
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Plantonize.NotasFiscais.Domain.Enum;

namespace Plantonize.NotasFiscais.Infrastructure.Configuration;

public class NFSEConfiguration
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("numeroNota")]
    public string? NumeroNota { get; set; }

    [BsonElement("dataEmissao")]
    public DateTime DataEmissao { get; set; }

    [BsonElement("valorTotal")]
    public decimal ValorTotal { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)] // ? Importante: armazenar como string
    public StatusNFSEEnum Status { get; set; }

    // ... outras propriedades
}
```

### 2. Configurar Serialização de Enums no MongoDB

No `Program.cs`, adicione configuração global para enums:

```csharp
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Plantonize.NotasFiscais.Domain.Enum;

// Configurar convenções do MongoDB
var conventionPack = new ConventionPack
{
    new EnumRepresentationConvention(BsonType.String), // ? Enums como string
    new CamelCaseElementNameConvention()
};

ConventionRegistry.Register("EnumStringConvention", conventionPack, t => true);

// Registrar serializers personalizados para enums
BsonSerializer.RegisterSerializer(
    typeof(StatusNFSEEnum), 
    new EnumSerializer<StatusNFSEEnum>(BsonType.String)
);

BsonSerializer.RegisterSerializer(
    typeof(StatusFaturaEnum), 
    new EnumSerializer<StatusFaturaEnum>(BsonType.String)
);
```

---

## ? Solução 3: Remover AutoMapper do Repository

O AutoMapper está causando o problema. Remova-o do `NotaFiscalRepository`:

### Antes (Com Erro):

```csharp
public async Task<IEnumerable<NotaFiscal>> GetAllAsync()
{
    var configurations = await _collection.Find(_ => true).ToListAsync();
    return _mapper.Map<IEnumerable<NotaFiscal>>(configurations); // ? ERRO AQUI
}
```

### Depois (Correto):

```csharp
public async Task<IEnumerable<NotaFiscal>> GetAllAsync()
{
    // MongoDB já retorna NotaFiscal diretamente
    return await _collection.Find(_ => true).ToListAsync();
}
```

### Atualização Completa do Repository

```csharp
using MongoDB.Driver;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;

namespace Plantonize.NotasFiscais.Infrastructure.Repositories;

public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly IMongoCollection<NotaFiscal> _collection;

    public NotaFiscalRepository(NotasFiscaisDBContext context)
    {
        _collection = context.NotasFiscais;
    }

    public async Task<NotaFiscal?> GetByIdAsync(Guid id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<NotaFiscal>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<NotaFiscal>> GetByMedicoIdAsync(Guid medicoId)
    {
        // Aqui você precisa ter um campo MedicoId na entidade
        // ou fazer um filtro diferente
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<NotaFiscal> CreateAsync(NotaFiscal notaFiscal)
    {
        await _collection.InsertOneAsync(notaFiscal);
        return notaFiscal;
    }

    public async Task<NotaFiscal> UpdateAsync(NotaFiscal notaFiscal)
    {
        await _collection.ReplaceOneAsync(
            x => x.Id == notaFiscal.Id,
            notaFiscal
        );
        return notaFiscal;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
```

---

## ? Solução 4: Atualizar Program.cs

```csharp
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Plantonize.NotasFiscais.Domain.Enum;
using Plantonize.NotasFiscais.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// CONFIGURAÇÃO MONGODB
// ========================================

// 1. Configurar convenções do MongoDB
var conventionPack = new ConventionPack
{
    new EnumRepresentationConvention(BsonType.String),
    new CamelCaseElementNameConvention(),
    new IgnoreExtraElementsConvention(true)
};

ConventionRegistry.Register("NotasFiscaisConventions", conventionPack, t => true);

// 2. Registrar serializers para enums
BsonSerializer.RegisterSerializer(
    typeof(StatusNFSEEnum),
    new EnumSerializer<StatusNFSEEnum>(BsonType.String)
);

BsonSerializer.RegisterSerializer(
    typeof(StatusFaturaEnum),
    new EnumSerializer<StatusFaturaEnum>(BsonType.String)
);

// ... resto do código
```

---

## ?? Checklist de Correção

- [ ] **Passo 1**: Limpar dados existentes no MongoDB
  ```bash
  mongosh
  use PlantonizeNotasFiscaisDB
  db.NotasFiscais.deleteMany({})
  ```

- [ ] **Passo 2**: Atualizar `Program.cs` com configuração de enums

- [ ] **Passo 3**: Remover AutoMapper do `NotaFiscalRepository`

- [ ] **Passo 4**: Verificar que `NotasFiscaisDBContext` usa `NotaFiscal` diretamente:
  ```csharp
  public IMongoCollection<NotaFiscal> NotasFiscais => _database.GetCollection<NotaFiscal>("NotasFiscais");
  ```

- [ ] **Passo 5**: Testar criação de nova nota fiscal

- [ ] **Passo 6**: Testar listagem de notas fiscais

---

## ?? Teste de Verificação

Após aplicar as correções, teste com:

```bash
# 1. Limpar o banco
mongosh
use PlantonizeNotasFiscaisDB
db.NotasFiscais.deleteMany({})
exit

# 2. Executar a aplicação
cd Plantonize.NotasFiscais.API
dotnet run

# 3. Testar no Swagger
# - Criar uma nota fiscal (POST /api/v2/notas-fiscais)
# - Listar notas fiscais (GET /api/v2/notas-fiscais)
```

### Payload de Teste

```json
{
  "numeroNota": "NF-2025-001",
  "dataEmissao": "2025-01-12T10:00:00Z",
  "valorTotal": 1500.00,
  "municipioPrestacao": "São Paulo",
  "issRetido": false,
  "medico": {
    "nome": "Dr. João Silva",
    "cpfCnpj": "123.456.789-00",
    "email": "joao@example.com",
    "municipio": "São Paulo"
  },
  "tomador": {
    "nome": "Clínica ABC",
    "cpfCnpj": "12.345.678/0001-00",
    "email": "contato@clinica.com",
    "municipio": "São Paulo"
  },
  "servicos": [
    {
      "descricao": "Consulta médica",
      "quantidade": 1,
      "valorUnitario": 1500.00,
      "aliquotaIss": 5.0
    }
  ]
}
```

---

## ?? Resultado Esperado

Após as correções:

1. ? Enums são serializados como strings no MongoDB
2. ? Não há mais erro de AutoMapper
3. ? Status é salvo corretamente como "Emitida"
4. ? Listagem de notas fiscais funciona perfeitamente

---

## ?? Comandos Úteis do MongoDB

```javascript
// Conectar
mongosh

// Usar database
use PlantonizeNotasFiscaisDB

// Ver todas as collections
show collections

// Ver documentos
db.NotasFiscais.find().pretty()

// Contar documentos
db.NotasFiscais.countDocuments()

// Deletar todos
db.NotasFiscais.deleteMany({})

// Deletar por status
db.NotasFiscais.deleteMany({ Status: "Emitido" })

// Atualizar status
db.NotasFiscais.updateMany(
    { Status: "Emitido" },
    { $set: { Status: "Emitida" } }
)

// Ver estrutura de um documento
db.NotasFiscais.findOne()
```

---

## ?? Documentação de Referência

- [MongoDB .NET Driver - Enum Serialization](https://mongodb.github.io/mongo-csharp-driver/2.14/reference/bson/serialization/)
- [MongoDB Conventions](https://mongodb.github.io/mongo-csharp-driver/2.14/reference/bson/mapping/#conventions)
- [BSON Serialization](https://mongodb.github.io/mongo-csharp-driver/2.14/reference/bson/serialization/)

---

**Desenvolvido com ?? pela equipe Plantonize**

**Última atualização**: Janeiro 2025
