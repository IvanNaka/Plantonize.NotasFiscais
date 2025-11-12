# Service Bus - Quick Start Guide

## ? Quick Setup

### 1. Add Connection String
Edit `appsettings.json`:
```json
"ServiceBusSettings": {
  "ConnectionString": "Endpoint=sb://YOUR-NAMESPACE.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YOUR-KEY"
}
```

### 2. Test the Integration

#### Send a test message:
```bash
curl -X POST "https://localhost:7164/api/ServiceBus/send/notafiscal-queue" \
  -H "Content-Type: application/json" \
  -d '{"test": "Hello Service Bus"}'
```

#### Receive a message:
```bash
curl "https://localhost:7164/api/ServiceBus/receive/notafiscal-queue"
```

## ?? Common Tasks

### Inject Service Bus in Your Service
```csharp
public class YourService
{
    private readonly IServiceBusService _serviceBus;
    
    public YourService(IServiceBusService serviceBus)
    {
        _serviceBus = serviceBus;
    }
    
    public async Task DoSomething()
    {
        await _serviceBus.SendMessageToQueueAsync(yourObject, "queue-name");
    }
}
```

### Send NotaFiscal to Queue
```csharp
await _serviceBusService.SendMessageToQueueAsync(notaFiscal, "notafiscal-queue");
```

### Send Fatura to Topic
```csharp
await _serviceBusService.SendMessageToTopicAsync(fatura, "fatura-topic");
```

## ?? Default Queue/Topic Names
- NotaFiscal Queue: `notafiscal-queue`
- Fatura Queue: `fatura-queue`
- NotaFiscal Topic: `notafiscal-topic`
- Fatura Topic: `fatura-topic`

## ?? Troubleshooting

### Connection String Not Set
Error: "Service Bus connection string is not configured"
- Check appsettings.json
- Ensure ServiceBusSettings:ConnectionString is not empty

### Queue/Topic Not Found
Error: "The messaging entity 'X' could not be found"
- Create the queue/topic in Azure Portal
- Or use Azure CLI commands from SERVICE_BUS_INTEGRATION.md

### Message Size Too Large
Error: "Message size exceeds limit"
- Max size: 256KB (Standard tier) or 1MB (Premium tier)
- Consider storing large data elsewhere and sending reference

## ?? Full Documentation
See `SERVICE_BUS_INTEGRATION.md` for complete documentation.
