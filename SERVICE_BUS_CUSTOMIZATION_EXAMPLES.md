# Customizações do Formato de Mensagem

## ?? Exemplos de Customização

### 1. Adicionar Mais Municípios (Códigos IBGE)

```csharp
// Em NotaFiscalService.cs, método GetCodigoIBGE()

private string GetCodigoIBGE(string municipio)
{
    var codigosIBGE = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // Capitais
        { "São Paulo", "3550308" },
        { "Rio de Janeiro", "3304557" },
        { "Belo Horizonte", "3106200" },
        { "Brasília", "5300108" },
        { "Salvador", "2927408" },
        { "Fortaleza", "2304400" },
        { "Curitiba", "4106902" },
        { "Recife", "2611606" },
        { "Porto Alegre", "4314902" },
        { "Manaus", "1302603" },
        
        // SP Interior
        { "Campinas", "3509502" },
        { "Santos", "3548500" },
        { "São José dos Campos", "3549904" },
        { "Ribeirão Preto", "3543402" },
        { "Sorocaba", "3552205" },
        
        // RJ Interior
        { "Niterói", "3303302" },
        { "Campos dos Goytacazes", "3301009" },
        { "Nova Iguaçu", "3303500" },
        
        // MG Interior
        { "Uberlândia", "3170206" },
        { "Contagem", "3118601" },
        { "Juiz de Fora", "3136702" },
        
        // Adicione mais conforme necessário...
    };

    return codigosIBGE.TryGetValue(municipio, out var codigo) ? codigo : string.Empty;
}
```

### 2. Código de Serviço Dinâmico

```csharp
// Opção A: Baseado na descrição do serviço
private string GetCodigoServico(ItemServico servico)
{
    if (servico?.Descricao == null)
        return "001";

    var descricao = servico.Descricao.ToLower();
    
    if (descricao.Contains("consulta"))
        return "001";
    if (descricao.Contains("cirurgia"))
        return "002";
    if (descricao.Contains("exame"))
        return "003";
    if (descricao.Contains("procedimento"))
        return "004";
        
    return "999"; // Outros
}

// Opção B: Baseado em valor
private string GetCodigoServico(ItemServico servico)
{
    if (servico?.ValorTotal == null)
        return "001";

    if (servico.ValorTotal <= 100)
        return "001"; // Serviço básico
    if (servico.ValorTotal <= 500)
        return "002"; // Serviço intermediário
    
    return "003"; // Serviço premium
}

// Usar no ConvertToServiceBusDto:
codigo_servico = GetCodigoServico(primeiroServico),
```

### 3. Enviar Todos os Serviços (Array)

Se precisar enviar múltiplos serviços, crie um DTO diferente:

```csharp
// NotaFiscalServiceBusDto.cs
public class NotaFiscalServiceBusDto
{
    public string _id { get; set; } = string.Empty;
    public string numeroNota { get; set; } = string.Empty;
    public List<ServicoDto> servicos { get; set; } = new();
    public decimal valor { get; set; }
    public string cpf_cnpj_cliente { get; set; } = string.Empty;
    public string cliente { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string cep { get; set; } = string.Empty;
    public string endereco { get; set; } = string.Empty;
    public string numero { get; set; } = string.Empty;
    public string bairro { get; set; } = string.Empty;
    public string codigo_municipio { get; set; } = string.Empty;
    public string municipio { get; set; } = string.Empty;
    public string uf { get; set; } = string.Empty;
}

public class ServicoDto
{
    public string codigo_servico { get; set; } = string.Empty;
    public string descricao { get; set; } = string.Empty;
    public decimal valor { get; set; }
}

// No ConvertToServiceBusDto:
servicos = notaFiscal.Servicos?.Select(s => new ServicoDto
{
    codigo_servico = GetCodigoServico(s),
    descricao = s.Descricao ?? string.Empty,
    valor = s.ValorTotal
}).ToList() ?? new(),
```

### 4. Parser de Endereço Avançado

```csharp
private (string Rua, string Numero, string Bairro, string CEP) ParseEnderecoAvancado(string endereco)
{
    if (string.IsNullOrWhiteSpace(endereco))
        return (string.Empty, string.Empty, string.Empty, string.Empty);

    // Remove espaços extras
    endereco = System.Text.RegularExpressions.Regex.Replace(endereco, @"\s+", " ").Trim();

    // Padrões comuns:
    // "Rua X, 123 - Bairro - CEP 12345-678"
    // "Av Y, 456, Bairro, 12345-678"
    // "Praça Z, nº 789, Centro"

    string rua = string.Empty;
    string numero = string.Empty;
    string bairro = string.Empty;
    string cep = string.Empty;

    // Extrai CEP (XXXXX-XXX ou XXXXXXXX)
    var cepMatch = System.Text.RegularExpressions.Regex.Match(endereco, @"(\d{5}-?\d{3})");
    if (cepMatch.Success)
    {
        cep = cepMatch.Value;
        endereco = endereco.Replace(cepMatch.Value, "").Replace("CEP", "").Trim();
    }

    // Divide por vírgula ou hífen
    var partes = endereco.Split(new[] { ',', '-' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(p => p.Trim())
                          .ToArray();

    if (partes.Length >= 1)
        rua = partes[0];

    if (partes.Length >= 2)
    {
        // Tenta extrair número
        var numeroMatch = System.Text.RegularExpressions.Regex.Match(partes[1], @"\d+");
        numero = numeroMatch.Success ? numeroMatch.Value : partes[1];
    }

    if (partes.Length >= 3)
        bairro = partes[2];

    return (rua, numero, bairro, cep);
}
```

### 5. Adicionar Campos Extras

```csharp
public class NotaFiscalServiceBusDto
{
    // Campos existentes...
    
    // Novos campos
    public DateTime dataEmissao { get; set; }
    public string statusNota { get; set; } = string.Empty;
    public string nomeEmissor { get; set; } = string.Empty;
    public string cpfCnpjEmissor { get; set; } = string.Empty;
    public string inscricaoMunicipal { get; set; } = string.Empty;
    public decimal aliquotaIss { get; set; }
    public decimal valorIss { get; set; }
}

// No ConvertToServiceBusDto:
dataEmissao = notaFiscal.DataEmissao,
statusNota = notaFiscal.Status.ToString(),
nomeEmissor = notaFiscal.Medico?.Nome ?? string.Empty,
cpfCnpjEmissor = notaFiscal.Medico?.CpfCnpj ?? string.Empty,
inscricaoMunicipal = notaFiscal.Medico?.InscricaoMunicipal ?? string.Empty,
aliquotaIss = primeiroServico?.AliquotaIss ?? 0,
valorIss = CalcularISS(notaFiscal.ValorTotal, primeiroServico?.AliquotaIss ?? 0),
```

### 6. Tratamento de Erros Melhorado

```csharp
public async Task<NotaFiscal> CreateAsync(NotaFiscal notaFiscal)
{
    // ... código existente ...

    var createdNotaFiscal = await _repository.CreateAsync(notaFiscal);

    // Send message to Service Bus after successful creation
    try
    {
        var serviceBusDto = ConvertToServiceBusDto(createdNotaFiscal);
        
        // Validar DTO antes de enviar
        if (string.IsNullOrWhiteSpace(serviceBusDto.numeroNota))
            throw new InvalidOperationException("Número da nota não pode ser vazio");
            
        if (serviceBusDto.valor <= 0)
            throw new InvalidOperationException("Valor deve ser maior que zero");

        await _serviceBusService.SendMessageToQueueAsync(serviceBusDto, NOTA_FISCAL_QUEUE);
        
        Console.WriteLine($"Mensagem enviada com sucesso para '{NOTA_FISCAL_QUEUE}'. ID: {serviceBusDto._id}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Validação falhou ao enviar para Service Bus: {ex.Message}");
        // Considere registrar em log estruturado ou re-lançar exceção
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao enviar para Service Bus queue '{NOTA_FISCAL_QUEUE}': {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        // Considere implementar retry policy ou dead letter queue
    }

    return createdNotaFiscal;
}
```

### 7. Usar ILogger ao Invés de Console

```csharp
// Adicionar ILogger no construtor
private readonly ILogger<NotaFiscalService> _logger;

public NotaFiscalService(
    INotaFiscalRepository repository,
    IServiceBusService serviceBusService,
    ILogger<NotaFiscalService> logger)
{
    _repository = repository;
    _serviceBusService = serviceBusService;
    _logger = logger;
}

// No CreateAsync:
catch (Exception ex)
{
    _logger.LogError(ex, 
        "Failed to send message to Service Bus queue '{QueueName}' for NotaFiscal {NotaFiscalId}", 
        NOTA_FISCAL_QUEUE, 
        createdNotaFiscal.Id);
}
```

## ?? Recomendações

1. **Use Logging Estruturado**: Substitua `Console.WriteLine` por `ILogger`
2. **Valide Dados**: Valide o DTO antes de enviar
3. **CEP**: Considere validar formato do CEP
4. **Códigos IBGE**: Mantenha dicionário atualizado
5. **Retry Policy**: Implemente retry automático
6. **Monitoramento**: Configure alertas no Azure
7. **Testes**: Adicione testes unitários para conversão

## ?? Template de Teste

```json
// POST https://localhost:7164/api/NotasFiscais
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
    "medicoId": "123e4567-e89b-12d3-a456-426614174000"
  },
  "tomador": {
    "nome": "Maria Santos",
    "cpfCnpj": "98765432100",
    "email": "maria@email.com",
    "tipoTomador": "PF",
    "endereco": "Av. Paulista, 1000 - Bela Vista - CEP 01310-100",
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

## ?? Próximos Passos

1. Implementar as customizações necessárias
2. Testar com dados reais
3. Ajustar parsers conforme padrões dos seus dados
4. Adicionar logging estruturado
5. Configurar monitoramento
6. Criar testes automatizados
