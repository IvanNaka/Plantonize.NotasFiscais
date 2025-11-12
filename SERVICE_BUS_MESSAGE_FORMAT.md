# Service Bus - Formato de Mensagem Customizado

## ?? Estrutura da Mensagem

Quando uma **NotaFiscal** é criada, a mensagem enviada ao Service Bus tem o seguinte formato:

```json
{
  "_id": "identificador único para MongoDB",
  "numeroNota": "número da nota fiscal",
  "codigo_servico": "código do serviço",
  "descricao": "descrição do serviço",
  "valor": 150.00,
  "cpf_cnpj_cliente": "CPF/CNPJ do cliente",
  "cliente": "Nome do cliente",
  "email": "email@cliente.com",
  "cep": "CEP",
  "endereco": "Nome da rua",
  "numero": "Número",
  "bairro": "Bairro",
  "codigo_municipio": "Código IBGE",
  "municipio": "Nome da cidade",
  "uf": "Estado"
}
```

## ?? Mapeamento de Dados

### Origem dos Dados

| Campo | Origem | Observações |
|-------|--------|-------------|
| `_id` | `NotaFiscal.Id` | Convertido para string |
| `numeroNota` | `NotaFiscal.NumeroNota` | - |
| `codigo_servico` | Fixo: "001" | Pode ser customizado |
| `descricao` | `NotaFiscal.Servicos[0].Descricao` | Primeiro serviço da lista |
| `valor` | `NotaFiscal.ValorTotal` | Valor total da nota |
| `cpf_cnpj_cliente` | `NotaFiscal.Tomador.CpfCnpj` | - |
| `cliente` | `NotaFiscal.Tomador.Nome` | - |
| `email` | `NotaFiscal.Tomador.Email` | - |
| `cep` | Extraído de `NotaFiscal.Tomador.Endereco` | Parser automático |
| `endereco` | Extraído de `NotaFiscal.Tomador.Endereco` | Nome da rua |
| `numero` | Extraído de `NotaFiscal.Tomador.Endereco` | Número do endereço |
| `bairro` | Extraído de `NotaFiscal.Tomador.Endereco` | Bairro |
| `codigo_municipio` | Lookup IBGE | Baseado no nome do município |
| `municipio` | `NotaFiscal.Tomador.Municipio` | Apenas nome, sem UF |
| `uf` | Extraído de `NotaFiscal.Tomador.Municipio` | Formato: "Cidade - UF" |

## ?? Parsers Implementados

### 1. Parser de Endereço
Formato esperado: `"Rua X, 123 - Bairro"`

```csharp
ParseEndereco("Av. Paulista, 1000 - Bela Vista")
// Retorna: (Rua: "Av. Paulista", Numero: "1000", Bairro: "Bela Vista")
```

### 2. Extração de UF
Formatos aceitos:
- `"São Paulo - SP"`
- `"São Paulo/SP"`

```csharp
ExtractUF("São Paulo - SP")  // Retorna: "SP"
ExtractUF("São Paulo/SP")    // Retorna: "SP"
```

### 3. Extração de CEP
Identifica CEP no endereço (8 dígitos)

```csharp
ExtractCEP("Rua X, 123, CEP 12345678")  // Retorna: "12345-678"
```

### 4. Código IBGE
Mapeamento de municípios para códigos IBGE

Municípios suportados:
- São Paulo: `3550308`
- Rio de Janeiro: `3304557`
- Belo Horizonte: `3106200`
- Brasília: `5300108`
- Salvador: `2927408`
- Fortaleza: `2304400`
- Curitiba: `4106902`
- Recife: `2611606`
- Porto Alegre: `4314902`
- Manaus: `1302603`

## ?? Exemplo Completo

### Entrada (NotaFiscal)
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "numeroNota": "NF-2025-0001",
  "valorTotal": 500.00,
  "medico": {
    "nome": "Dr. João Silva",
    "cpfCnpj": "12345678901",
    "email": "joao@email.com"
  },
  "tomador": {
    "nome": "Maria Santos",
    "cpfCnpj": "98765432100",
    "email": "maria@email.com",
    "endereco": "Av. Paulista, 1000 - Bela Vista",
    "municipio": "São Paulo - SP"
  },
  "servicos": [
    {
      "descricao": "Consulta Médica",
      "valorTotal": 500.00
    }
  ]
}
```

### Saída (Mensagem Service Bus)
```json
{
  "_id": "123e4567-e89b-12d3-a456-426614174000",
  "numeroNota": "NF-2025-0001",
  "codigo_servico": "001",
  "descricao": "Consulta Médica",
  "valor": 500.00,
  "cpf_cnpj_cliente": "98765432100",
  "cliente": "Maria Santos",
  "email": "maria@email.com",
  "cep": "",
  "endereco": "Av. Paulista",
  "numero": "1000",
  "bairro": "Bela Vista",
  "codigo_municipio": "3550308",
  "municipio": "São Paulo",
  "uf": "SP"
}
```

## ?? Customizações Possíveis

### 1. Adicionar Códigos IBGE
Edite o método `GetCodigoIBGE()` em `NotaFiscalService.cs`:

```csharp
private string GetCodigoIBGE(string municipio)
{
    var codigosIBGE = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "São Paulo", "3550308" },
        { "Seu Município", "CODIGO_IBGE" }
        // Adicione mais municípios aqui
    };
    // ...
}
```

### 2. Customizar Código de Serviço
Modifique a linha em `ConvertToServiceBusDto()`:

```csharp
codigo_servico = "001", // Altere aqui ou use lógica dinâmica
```

### 3. Melhorar Parser de Endereço
Ajuste o método `ParseEndereco()` para lidar com diferentes formatos de endereço.

## ?? Observações Importantes

1. **Primeiro Serviço**: Apenas o primeiro serviço da lista é enviado na mensagem
2. **Campos Vazios**: Se dados não estiverem disponíveis, campos ficam vazios (string.Empty)
3. **Código IBGE**: Se município não estiver no dicionário, retorna vazio
4. **Parser de Endereço**: Assume formato específico, pode precisar ajustes

## ?? Validação

A mensagem é enviada após:
1. ? NotaFiscal validada
2. ? NotaFiscal salva no MongoDB
3. ? Conversão para DTO bem-sucedida

Se falhar o envio:
- ? Erro é registrado no console
- ? NotaFiscal permanece salva
- ? Operação continua normalmente

## ?? Arquivos Relacionados

- **DTO**: `Plantonize.NotasFiscais.Application/DTOs/NotaFiscalServiceBusDto.cs`
- **Service**: `Plantonize.NotasFiscais.Application/Services/NotaFiscalService.cs`
- **Queue**: `integracao-nf`

## ?? Teste

```bash
# 1. Criar NotaFiscal
POST https://localhost:7164/api/NotasFiscais
Content-Type: application/json

# 2. Verificar mensagem no Service Bus
GET https://localhost:7164/api/ServiceBus/receive/integracao-nf

# 3. Verificar no Azure Portal
- Acesse Service Bus > integracao-nf > Service Bus Explorer
```

## ?? Links Úteis
- `SERVICE_BUS_CONFIGURED.md` - Configuração do Service Bus
- `SERVICE_BUS_INTEGRATION.md` - Documentação completa
- `SERVICE_BUS_QUICK_START.md` - Guia rápido
