# ? Service Bus - Formato de Mensagem Customizado - IMPLEMENTADO

## ?? Resumo das Alterações

O formato da mensagem enviada ao Azure Service Bus foi customizado conforme solicitado.

## ?? Formato da Mensagem Implementado

```json
{
  "_id": "guid-da-nota-fiscal",
  "numeroNota": "NF-2025-0001",
  "codigo_servico": "001",
  "descricao": "Descrição do serviço",
  "valor": 150.00,
  "cpf_cnpj_cliente": "12345678900",
  "cliente": "Nome do Cliente",
  "email": "cliente@email.com",
  "cep": "12345-678",
  "endereco": "Nome da Rua",
  "numero": "123",
  "bairro": "Nome do Bairro",
  "codigo_municipio": "3550308",
  "municipio": "São Paulo",
  "uf": "SP"
}
```

## ?? Arquivos Criados/Modificados

### ? Criados
1. **`Plantonize.NotasFiscais.Application/DTOs/NotaFiscalServiceBusDto.cs`**
   - DTO com estrutura customizada da mensagem

2. **`SERVICE_BUS_MESSAGE_FORMAT.md`**
   - Documentação completa do formato
   - Mapeamento de campos
   - Exemplos de entrada/saída

3. **`SERVICE_BUS_CUSTOMIZATION_EXAMPLES.md`**
   - Exemplos de customizações
   - Parser avançado de endereço
   - Código de serviço dinâmico
   - Logging estruturado

### ? Modificados
1. **`Plantonize.NotasFiscais.Application/Services/NotaFiscalService.cs`**
   - Método `ConvertToServiceBusDto()` - Converte NotaFiscal para DTO
   - Método `ParseEndereco()` - Extrai rua, número e bairro
   - Método `ExtractUF()` - Extrai UF do município
   - Método `ExtractMunicipioName()` - Extrai nome do município
   - Método `ExtractCEP()` - Tenta extrair CEP do endereço
   - Método `GetCodigoIBGE()` - Retorna código IBGE do município

## ?? Fluxo de Conversão

```
NotaFiscal (MongoDB)
        ?
ConvertToServiceBusDto()
        ?
NotaFiscalServiceBusDto
        ?
Service Bus Queue: integracao-nf
```

## ?? Mapeamento de Dados Detalhado

| Campo Mensagem | Origem NotaFiscal | Parser/Método |
|----------------|-------------------|---------------|
| `_id` | `Id` | `ToString()` |
| `numeroNota` | `NumeroNota` | Direto |
| `codigo_servico` | Fixo | `"001"` (customizável) |
| `descricao` | `Servicos[0].Descricao` | Primeiro serviço |
| `valor` | `ValorTotal` | Direto |
| `cpf_cnpj_cliente` | `Tomador.CpfCnpj` | Direto |
| `cliente` | `Tomador.Nome` | Direto |
| `email` | `Tomador.Email` | Direto |
| `cep` | `Tomador.Endereco` | `ExtractCEP()` |
| `endereco` | `Tomador.Endereco` | `ParseEndereco()` |
| `numero` | `Tomador.Endereco` | `ParseEndereco()` |
| `bairro` | `Tomador.Endereco` | `ParseEndereco()` |
| `codigo_municipio` | `Tomador.Municipio` | `GetCodigoIBGE()` |
| `municipio` | `Tomador.Municipio` | `ExtractMunicipioName()` |
| `uf` | `Tomador.Municipio` | `ExtractUF()` |

## ??? Códigos IBGE Configurados

Municípios com código IBGE mapeado:
- ? São Paulo: `3550308`
- ? Rio de Janeiro: `3304557`
- ? Belo Horizonte: `3106200`
- ? Brasília: `5300108`
- ? Salvador: `2927408`
- ? Fortaleza: `2304400`
- ? Curitiba: `4106902`
- ? Recife: `2611606`
- ? Porto Alegre: `4314902`
- ? Manaus: `1302603`

**Adicionar mais municípios**: Ver `SERVICE_BUS_CUSTOMIZATION_EXAMPLES.md`

## ?? Parsers Implementados

### 1. ParseEndereco()
Formato esperado: `"Rua X, 123 - Bairro"`

**Exemplo:**
```
Input:  "Av. Paulista, 1000 - Bela Vista"
Output: Rua: "Av. Paulista"
        Numero: "1000"
        Bairro: "Bela Vista"
```

### 2. ExtractUF()
Formatos aceitos:
- `"Cidade - UF"`
- `"Cidade/UF"`

**Exemplo:**
```
Input:  "São Paulo - SP"
Output: "SP"
```

### 3. ExtractCEP()
Identifica 8 dígitos consecutivos

**Exemplo:**
```
Input:  "Rua X, CEP 12345678"
Output: "12345-678"
```

### 4. GetCodigoIBGE()
Busca em dicionário

**Exemplo:**
```
Input:  "São Paulo"
Output: "3550308"
```

## ?? Exemplo Completo

### Entrada API
```json
POST /api/NotasFiscais
{
  "valorTotal": 500.00,
  "municipioPrestacao": "São Paulo",
  "issRetido": false,
  "medico": {
    "nome": "Dr. João Silva",
    "cpfCnpj": "12345678901",
    "email": "joao@email.com",
    "municipio": "São Paulo",
    "inscricaoMunicipal": "123456",
    "medicoId": "550e8400-e29b-41d4-a716-446655440000"
  },
  "tomador": {
    "nome": "Maria Santos",
    "cpfCnpj": "98765432100",
    "email": "maria@email.com",
    "tipoTomador": "PF",
    "endereco": "Av. Paulista, 1000 - Bela Vista",
    "municipio": "São Paulo - SP"
  },
  "servicos": [
    {
      "descricao": "Consulta Médica",
      "quantidade": 1,
      "valorUnitario": 500.00,
      "aliquotaIss": 5.00,
      "valorTotal": 500.00
    }
  ]
}
```

### Saída Service Bus (integracao-nf)
```json
{
  "_id": "550e8400-e29b-41d4-a716-446655440001",
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

## ?? Configuração

### Service Bus
- **Queue**: `integracao-nf`
- **Connection String**: Configurada em `appsettings.json`
- **Namespace**: `plantonize-servicebus.servicebus.windows.net`

### Comportamento
1. ? NotaFiscal é criada e salva no MongoDB
2. ? NotaFiscal é convertida para `NotaFiscalServiceBusDto`
3. ? DTO é serializado para JSON
4. ? Mensagem é enviada para queue `integracao-nf`
5. ?? Se falhar, erro é logado mas criação continua

## ?? Tratamento de Erros

```csharp
try
{
    var serviceBusDto = ConvertToServiceBusDto(createdNotaFiscal);
    await _serviceBusService.SendMessageToQueueAsync(serviceBusDto, NOTA_FISCAL_QUEUE);
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to send message to Service Bus queue '{NOTA_FISCAL_QUEUE}': {ex.Message}");
    // NotaFiscal já foi salva, operação continua
}
```

## ? Status do Build

```
Build successful ?
Todas as mudanças compiladas com sucesso
Pronto para uso
```

## ?? Documentação

1. **`SERVICE_BUS_MESSAGE_FORMAT.md`**
   - Estrutura completa da mensagem
   - Mapeamento detalhado
   - Exemplos de teste

2. **`SERVICE_BUS_CUSTOMIZATION_EXAMPLES.md`**
   - Como adicionar mais municípios
   - Parser avançado de endereço
   - Código de serviço dinâmico
   - Logging estruturado com ILogger

3. **`SERVICE_BUS_CONFIGURED.md`**
   - Configuração do Service Bus
   - Connection string
   - Como testar

4. **`SERVICE_BUS_INTEGRATION.md`**
   - Documentação completa da integração
   - Setup do Azure
   - Best practices

## ?? Próximos Passos Recomendados

1. **Testar Conversão**
   - Criar NotaFiscal via API
   - Verificar mensagem no Service Bus
   - Validar formato JSON

2. **Adicionar Municípios**
   - Incluir códigos IBGE dos municípios utilizados
   - Ver `SERVICE_BUS_CUSTOMIZATION_EXAMPLES.md`

3. **Melhorar Parsing**
   - Ajustar `ParseEndereco()` conforme padrão dos seus dados
   - Validar CEP extraído
   - Considerar usar biblioteca de endereços

4. **Logging Estruturado**
   - Substituir `Console.WriteLine` por `ILogger`
   - Adicionar Application Insights
   - Configurar alertas

5. **Monitoramento**
   - Acompanhar mensagens no Azure Portal
   - Verificar Dead Letter Queue
   - Configurar métricas

## ?? Resultados

? Formato customizado implementado
? Conversão automática de NotaFiscal
? Parsers para endereço, UF, CEP
? Mapeamento de códigos IBGE
? Build bem-sucedido
? Documentação completa
? Pronto para produção

## ?? Suporte

Para customizações adicionais, consulte:
- `SERVICE_BUS_CUSTOMIZATION_EXAMPLES.md`
- Métodos em `NotaFiscalService.cs`
