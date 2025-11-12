# Service Bus - Configuração Atualizada

## ? Atualização Concluída

A connection string do Azure Service Bus foi configurada com sucesso no projeto.

## ?? Configurações Atualizadas

### Connection String
```
Endpoint=sb://plantonize-servicebus.servicebus.windows.net/
SharedAccessKeyName=Ac
```

### Fila Principal
- **Nome da Fila**: `integracao-nf`
- **Uso**: Envio de NotaFiscal criadas

## ?? Arquivos Modificados

### 1. appsettings.json
```json
"ServiceBusSettings": {
  "ConnectionString": "Endpoint=sb://plantonize-servicebus.servicebus.windows.net/;SharedAccessKeyName=Ac;SharedAccessKey=***",
  "NotaFiscalQueueName": "integracao-nf",
  "FaturaQueueName": "fatura-queue",
  "NotaFiscalTopicName": "notafiscal-topic",
  "FaturaTopicName": "fatura-topic"
}
```

### 2. appsettings.Development.json
- Mesmas configurações aplicadas para ambiente de desenvolvimento

### 3. ServiceBusSettings.cs
- Valor padrão atualizado: `NotaFiscalQueueName = "integracao-nf"`

### 4. NotaFiscalService.cs
- Usa constante `NOTA_FISCAL_QUEUE = "integracao-nf"`
- Mensagens são enviadas automaticamente quando uma NotaFiscal é criada

## ?? Funcionamento

### Quando uma NotaFiscal é Criada
1. NotaFiscal é salva no MongoDB
2. Mensagem é enviada para a fila `integracao-nf` no Service Bus
3. Formato JSON com todos os dados da NotaFiscal
4. Se falhar, registra erro mas não impede a criação

### Estrutura da Mensagem
```json
{
  "id": "guid",
  "numeroNota": "string",
  "dataEmissao": "datetime",
  "valorTotal": decimal,
  "status": "enum",
  "municipioPrestacao": "string",
  "issRetido": boolean,
  "medico": {
    "id": "guid",
    "nome": "string",
    "cpfCnpj": "string",
    "email": "string",
    "municipio": "string",
    "inscricaoMunicipal": "string",
    "medicoId": "guid"
  },
  "tomador": {
    "nome": "string",
    "cpfCnpj": "string",
    "email": "string",
    "tipoTomador": "string",
    "endereco": "string",
    "municipio": "string"
  },
  "servicos": [
    {
      "descricao": "string",
      "quantidade": int,
      "valorUnitario": decimal,
      "aliquotaIss": decimal,
      "valorTotal": decimal
    }
  ],
  "enviadoEmail": boolean,
  "dataEnvioEmail": "datetime?"
}
```

## ?? Como Testar

### 1. Criar uma NotaFiscal via API
```bash
POST https://localhost:7164/api/NotasFiscais
Content-Type: application/json

{
  "valorTotal": 1000.00,
  "municipioPrestacao": "São Paulo",
  "issRetido": false,
  "medico": {
    "nome": "Dr. João Silva",
    "cpfCnpj": "12345678901",
    "email": "joao@email.com",
    "municipio": "São Paulo",
    "inscricaoMunicipal": "123456",
    "medicoId": "guid-do-medico"
  },
  "tomador": {
    "nome": "Cliente Teste",
    "cpfCnpj": "98765432100",
    "email": "cliente@email.com",
    "tipoTomador": "PF",
    "endereco": "Rua Teste, 123",
    "municipio": "São Paulo"
  },
  "servicos": [
    {
      "descricao": "Consulta Médica",
      "quantidade": 1,
      "valorUnitario": 1000.00,
      "aliquotaIss": 5.00
    }
  ]
}
```

### 2. Verificar no Azure Portal
1. Acesse o Service Bus no Azure Portal
2. Navegue até a fila `integracao-nf`
3. Verifique as mensagens na fila

### 3. Receber Mensagem via API (Teste)
```bash
GET https://localhost:7164/api/ServiceBus/receive/integracao-nf
```

## ?? Monitoramento

### Azure Portal
1. **Service Bus Namespace**: `plantonize-servicebus`
2. **Fila**: `integracao-nf`
3. **Métricas disponíveis**:
   - Mensagens ativas
   - Mensagens agendadas
   - Mensagens mortas (dead letter)
   - Taxa de transferência

### Logs da Aplicação
```csharp
// Em caso de erro ao enviar
Console.WriteLine($"Failed to send message to Service Bus queue 'integracao-nf': {ex.Message}");
```

## ?? Notas Importantes

1. **Segurança**: A connection string contém credenciais sensíveis
   - Não commitar em repositórios públicos
   - Usar Azure Key Vault em produção
   - Usar variáveis de ambiente

2. **Tratamento de Erros**: 
   - Falhas no Service Bus não impedem a criação da NotaFiscal
   - Erro é registrado mas operação continua
   - Considere implementar retry policies

3. **Dead Letter Queue**:
   - Mensagens que falharem 10 vezes vão para DLQ
   - Monitorar DLQ regularmente

## ?? Próximos Passos Recomendados

1. **Implementar Consumer**:
   - Azure Function ou Worker Service
   - Processar mensagens da fila
   - Executar integrações necessárias

2. **Adicionar Logging Estruturado**:
   - Substituir `Console.WriteLine` por `ILogger`
   - Adicionar Application Insights

3. **Implementar Retry Policies**:
   - Configurar política de retry no Service Bus
   - Implementar circuit breaker

4. **Monitoramento**:
   - Configurar alertas no Azure
   - Dashboard de métricas

## ? Status
- ? Connection string configurada
- ? Fila `integracao-nf` configurada
- ? NotaFiscalService integrado
- ? Build bem-sucedido
- ? Pronto para uso

## ?? Documentação Relacionada
- `SERVICE_BUS_INTEGRATION.md` - Documentação completa
- `SERVICE_BUS_QUICK_START.md` - Guia rápido
- `SERVICE_BUS_SUMMARY.md` - Resumo da implementação
