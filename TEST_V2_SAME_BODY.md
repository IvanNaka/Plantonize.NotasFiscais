# ?? Teste Rápido - V2 com Body do V1

## ? Exemplo Funcional

Use este JSON **exatamente como está** no Swagger ou Postman:

### POST /api/v2/notas-fiscais

```json
{
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
  ],
  "enviadoEmail": false
}
```

## ?? Checklist

Antes de testar, certifique-se:

1. ? MongoDB está rodando
2. ? Database está limpo (ou aceita novos dados)
3. ? Aplicação está rodando (`dotnet run`)
4. ? Swagger aberto em `http://localhost:5113/swagger`

## ?? Resultado Esperado

**Status**: 201 Created

**Response Body**:
```json
{
  "id": "gerado-automaticamente",
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

## ?? Verificar no MongoDB

```bash
mongosh
use PlantonizeNotasFiscaisDB
db.NotasFiscais.find().pretty()
```

Deve retornar a nota fiscal criada com todos os campos.

---

**Se funcionou**: ?? V2 agora aceita o mesmo body do V1!
