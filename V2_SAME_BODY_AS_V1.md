# ? V2 Endpoints Usando Mesmo Body do V1

## ?? Alteração Realizada

Os endpoints V2 (Vertical Slice) agora aceitam o **mesmo body JSON do V1**, usando a entidade `NotaFiscal` diretamente ao invés de Commands específicos.

---

## ?? Comparação

### ? Antes (Commands Diferentes)

**V1 - Clean Architecture:**
```http
POST /api/NotasFiscais
Content-Type: application/json

{
  "numeroNota": "NF-001",
  "dataEmissao": "2025-01-14T10:00:00Z",
  "valorTotal": 1500.00,
  "status": 1,
  "medico": { ... }
}
```

**V2 - Vertical Slice (ANTIGO):**
```http
POST /api/v2/notas-fiscais
Content-Type: application/json

{
  "numeroNota": "NF-001",      // ? Sem status
  "dataEmissao": "2025-01-14T10:00:00Z",
  "valorTotal": 1500.00,
  // ? Campos diferentes do V1
}
```

### ? Agora (Mesmo Body)

**V1 e V2 usam o MESMO JSON:**
```http
POST /api/NotasFiscais           # V1
POST /api/v2/notas-fiscais       # V2

Content-Type: application/json

{
  "id": "00000000-0000-0000-0000-000000000000",
  "numeroNota": "NF-2025-001",
  "dataEmissao": "2025-01-14T10:00:00Z",
  "valorTotal": 1500.00,
  "status": 4,
  "municipioPrestacao": "São Paulo",
  "issRetido": false,
  "medico": {
    "id": "00000000-0000-0000-0000-000000000000",
    "nome": "Dr. João Silva",
    "cpfCnpj": "123.456.789-00",
    "email": "joao@email.com",
    "municipio": "São Paulo",
    "inscricaoMunicipal": "12345",
    "medicoId": "5a754e33-f590-45cd-a8e5-707648cd49cf"
  },
  "tomador": {
    "nome": "Clínica ABC",
    "cpfCnpj": "12.345.678/0001-00",
    "email": "clinica@email.com",
    "tipoTomador": "PJ",
    "endereco": "Rua ABC, 123",
    "municipio": "São Paulo"
  },
  "servicos": [
    {
      "descricao": "Consulta médica",
      "quantidade": 1,
      "valorUnitario": 1500.00,
      "aliquotaIss": 5.0,
      "valorTotal": 1500.00
    }
  ],
  "enviadoEmail": false,
  "dataEnvioEmail": null
}
```

---

## ?? Implementação Técnica

### Create Endpoint (POST)

```csharp
[HttpPost]
public async Task<ActionResult<NotaFiscal>> Create([FromBody] NotaFiscal notaFiscal)
{
    // 1. Validar ModelState (JSON malformado)
    if (!ModelState.IsValid)
    {
        return BadRequest(errors);
    }

    // 2. Converter NotaFiscal para Command
    var command = new CreateNotaFiscalCommand(
        notaFiscal.NumeroNota,
        notaFiscal.DataEmissao,
        notaFiscal.ValorTotal,
        notaFiscal.MunicipioPrestacao,
        notaFiscal.IssRetido,
        notaFiscal.Medico,
        notaFiscal.Tomador,
        notaFiscal.Servicos
    );
    
    // 3. Enviar via MediatR
    var created = await _mediator.Send(command);
    
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}
```

### Update Endpoint (PUT)

```csharp
[HttpPut("{id:guid}")]
public async Task<ActionResult<NotaFiscal>> Update(Guid id, [FromBody] NotaFiscal notaFiscal)
{
    // 1. Validar ID mismatch
    if (id != notaFiscal.Id)
    {
        return BadRequest("ID mismatch");
    }

    // 2. Validar ModelState
    if (!ModelState.IsValid)
    {
        return BadRequest(errors);
    }

    // 3. Converter para Command
    var command = new UpdateNotaFiscalCommand(
        notaFiscal.Id,
        notaFiscal.NumeroNota,
        notaFiscal.DataEmissao,
        notaFiscal.ValorTotal,
        notaFiscal.MunicipioPrestacao,
        notaFiscal.IssRetido,
        notaFiscal.Status,
        notaFiscal.Medico,
        notaFiscal.Tomador,
        notaFiscal.Servicos,
        notaFiscal.EnviadoEmail,
        notaFiscal.DataEnvioEmail
    );
    
    var updated = await _mediator.Send(command);
    
    return Ok(updated);
}
```

---

## ?? Vantagens

### 1. **Compatibilidade Total**
- ? Clientes podem usar o mesmo JSON para V1 e V2
- ? Facilita migração de V1 para V2
- ? Reduz confusão sobre qual formato usar

### 2. **Validação Melhorada**
- ? Validação do ModelState (JSON malformado)
- ? Validação do FluentValidation (regras de negócio)
- ? Mensagens de erro claras

### 3. **Consistência**
- ? Mesma estrutura de dados
- ? Mesmos campos
- ? Mesmos tipos

### 4. **Mantém Benefícios do CQRS**
- ? Internamente ainda usa Commands
- ? Validação com FluentValidation
- ? Handlers isolados

---

## ?? Exemplos de Uso

### ? Exemplo Mínimo (Funciona em V1 e V2)

```json
{
  "numeroNota": "NF-001",
  "dataEmissao": "2025-01-14T10:00:00Z",
  "valorTotal": 1500.00,
  "municipioPrestacao": "São Paulo",
  "issRetido": false
}
```

### ? Exemplo Completo

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "numeroNota": "NF-2025-001",
  "dataEmissao": "2025-01-14T10:00:00Z",
  "valorTotal": 1500.00,
  "status": 4,
  "municipioPrestacao": "São Paulo",
  "issRetido": false,
  "medico": {
    "id": "00000000-0000-0000-0000-000000000000",
    "nome": "Dr. João Silva",
    "cpfCnpj": "123.456.789-00",
    "email": "joao@email.com",
    "telefone": "(11) 98765-4321",
    "endereco": "Rua Dr. Silva, 456",
    "municipio": "São Paulo",
    "especialidade": "Cardiologia",
    "inscricaoMunicipal": "12345",
    "medicoId": "5a754e33-f590-45cd-a8e5-707648cd49cf"
  },
  "tomador": {
    "nome": "Clínica ABC",
    "cpfCnpj": "12.345.678/0001-00",
    "email": "clinica@email.com",
    "telefone": "(11) 3456-7890",
    "tipoTomador": "PJ",
    "endereco": "Av. Paulista, 1000",
    "municipio": "São Paulo"
  },
  "servicos": [
    {
      "descricao": "Consulta cardiológica completa",
      "quantidade": 1,
      "valorUnitario": 1500.00,
      "aliquotaIss": 5.0,
      "valorTotal": 1500.00
    }
  ],
  "enviadoEmail": false,
  "dataEnvioEmail": null
}
```

---

## ?? Teste com cURL

### Create (POST)

```bash
curl -X 'POST' \
  'http://localhost:5113/api/v2/notas-fiscais' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "numeroNota": "NF-2025-001",
  "dataEmissao": "2025-01-14T10:00:00Z",
  "valorTotal": 1500.00,
  "status": 4,
  "municipioPrestacao": "São Paulo",
  "issRetido": false,
  "medico": {
    "nome": "Dr. João Silva",
    "cpfCnpj": "123.456.789-00",
    "email": "joao@email.com",
    "municipio": "São Paulo",
    "inscricaoMunicipal": "12345",
    "medicoId": "5a754e33-f590-45cd-a8e5-707648cd49cf"
  },
  "tomador": {
    "nome": "Clínica ABC",
    "cpfCnpj": "12.345.678/0001-00",
    "email": "clinica@email.com",
    "tipoTomador": "PJ",
    "endereco": "Rua ABC, 123",
    "municipio": "São Paulo"
  },
  "servicos": [
    {
      "descricao": "Consulta médica",
      "quantidade": 1,
      "valorUnitario": 1500.00,
      "aliquotaIss": 5.0,
      "valorTotal": 1500.00
    }
  ]
}'
```

### Update (PUT)

```bash
curl -X 'PUT' \
  'http://localhost:5113/api/v2/notas-fiscais/{id}' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "id": "{id}",
  "numeroNota": "NF-2025-001-UPD",
  "dataEmissao": "2025-01-14T10:00:00Z",
  "valorTotal": 2000.00,
  "status": 5,
  "municipioPrestacao": "São Paulo",
  "issRetido": false,
  "medico": {
    "nome": "Dr. João Silva",
    "cpfCnpj": "123.456.789-00",
    "email": "joao@email.com",
    "municipio": "São Paulo"
  },
  "enviadoEmail": true,
  "dataEnvioEmail": "2025-01-14T11:00:00Z"
}'
```

---

## ?? Campos Importantes

### Obrigatórios (Create)
- ? `numeroNota`
- ? `dataEmissao`
- ? `valorTotal`
- ? `issRetido`

### Opcionais
- `id` (gerado automaticamente se não fornecido)
- `status` (padrão: Emitida = 4)
- `municipioPrestacao`
- `medico`
- `tomador`
- `servicos`
- `enviadoEmail` (padrão: false)
- `dataEnvioEmail`

---

## ?? Status (StatusNFSEEnum)

| Valor | Nome | Descrição |
|-------|------|-----------|
| 1 | Autorizado | Autorizada pela prefeitura |
| 2 | Rejeitado | Rejeitada |
| 3 | Cancelado | Cancelada |
| 4 | Emitida | Emitida (padrão) |
| 5 | Enviada | Enviada ao cliente |
| 6 | Paga | Paga |

---

## ? Validações Aplicadas

### FluentValidation (Automático)

1. **ValorTotal**: Deve ser maior que 0
2. **DataEmissao**: Não pode ser futura
3. **NumeroNota**: Mínimo 1 caractere
4. **Servicos**: Se informado, deve ter pelo menos 1 item

### ModelState (JSON)

1. JSON deve ser válido
2. Tipos devem corresponder (string, number, boolean)
3. Datas no formato ISO 8601
4. GUIDs válidos

---

## ?? Fluxo Interno

```
1. Cliente envia JSON (NotaFiscal)
        ?
2. ASP.NET deserializa para NotaFiscal entity
        ?
3. ModelState valida sintaxe JSON
        ?
4. Endpoint converte NotaFiscal ? Command
        ?
5. MediatR envia Command para Handler
        ?
6. FluentValidation valida regras de negócio
        ?
7. Handler processa e salva no MongoDB
        ?
8. Retorna NotaFiscal criada/atualizada
```

---

## ?? Diferença V1 vs V2

| Aspecto | V1 (Clean) | V2 (Vertical Slice) |
|---------|-----------|---------------------|
| **Body JSON** | `NotaFiscal` | `NotaFiscal` ? IGUAL |
| **Validação** | Manual | FluentValidation (automática) |
| **Arquitetura** | Controller ? Service | Endpoint ? MediatR ? Handler |
| **Endpoint** | `/api/NotasFiscais` | `/api/v2/notas-fiscais` |

---

## ?? Benefícios Finais

1. ? **Compatibilidade**: Mesmo JSON funciona em V1 e V2
2. ? **Simplicidade**: Não precisa aprender estrutura diferente
3. ? **Validação**: ModelState + FluentValidation
4. ? **Migração Fácil**: Trocar endpoint URL é suficiente
5. ? **Consistência**: Uma estrutura de dados em todo o sistema

---

**Desenvolvido com ?? pela equipe Plantonize**

**Última atualização**: Janeiro 2025

**Status**: ? **V1 E V2 COMPATÍVEIS - MESMO BODY JSON**
