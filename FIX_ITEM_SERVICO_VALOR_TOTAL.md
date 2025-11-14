# ?? Fix: ItemServico ValorTotal Property

## ? Problema Encontrado

O campo `ValorTotal` em `ItemServico` era uma **propriedade calculada (read-only)**:

```csharp
// ANTES (causava erro 500)
public decimal ValorTotal => Quantidade * ValorUnitario;
```

Isso impedia que o JSON desserializasse corretamente, causando **erro 500** ao criar nota fiscal.

## ? Solução Aplicada

Agora `ValorTotal` aceita valores do JSON mas também calcula automaticamente se não fornecido:

```csharp
// DEPOIS (funciona)
private decimal? _valorTotal;
public decimal ValorTotal 
{ 
    get => _valorTotal ?? (Quantidade * ValorUnitario);
    set => _valorTotal = value;
}
```

## ?? Comportamento

1. **Se JSON enviar `valorTotal`**: Usa o valor enviado
2. **Se JSON NÃO enviar**: Calcula automaticamente (Quantidade × ValorUnitario)

## ?? Exemplo de Uso

### JSON com valorTotal explícito
```json
{
  "descricao": "Consulta",
  "quantidade": 1,
  "valorUnitario": 122,
  "valorTotal": 122,      // ? Será usado este valor
  "aliquotaIss": 5.0
}
```

### JSON sem valorTotal (cálculo automático)
```json
{
  "descricao": "Consulta",
  "quantidade": 2,
  "valorUnitario": 50,
  // valorTotal não enviado
  "aliquotaIss": 5.0
}
// ? valorTotal = 2 × 50 = 100
```

## ?? Teste Agora

Reinicie a aplicação e teste novamente com o mesmo JSON:

```json
{
  "numeroNota": "123456789",
  "dataEmissao": "2025-10-31T02:23:00.000Z",
  "valorTotal": 122,
  "status": 1,
  "municipioPrestacao": "asdasd",
  "issRetido": false,
  "medico": {
    "id": "5a754e33-f590-45cd-a8e5-707648cd49cf",
    "nome": "Ivan",
    "cpfCnpj": null,
    "email": "ivan@email.com",
    "municipio": null,
    "inscricaoMunicipal": null,
    "medicoId": "5a754e33-f590-45cd-a8e5-707648cd49cf"
  },
  "tomador": {
    "nome": "asdasd",
    "cpfCnpj": "12312312312",
    "email": "asdsasddsadsa@emai",
    "tipoTomador": "asddsa",
    "endereco": "adsad",
    "municipio": "asdasd"
  },
  "servicos": [
    {
      "descricao": "123",
      "quantidade": 1,
      "valorUnitario": 1,
      "valorTotal": 1,
      "aliquotaIss": 1
    }
  ],
  "enviadoEmail": false
}
```

**Status esperado**: ? 201 Created

---

**Correção aplicada**: Janeiro 2025
