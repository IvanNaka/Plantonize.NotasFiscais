# Azure Service Bus Integration

## Overview
This project now includes Azure Service Bus integration for asynchronous message processing.

## Configuration

### appsettings.json
Add the following configuration to your `appsettings.json` and `appsettings.Development.json`:

```json
{
  "ServiceBusSettings": {
    "ConnectionString": "Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your-key",
    "NotaFiscalQueueName": "notafiscal-queue",
    "FaturaQueueName": "fatura-queue",
    "NotaFiscalTopicName": "notafiscal-topic",
    "FaturaTopicName": "fatura-topic"
  }
}
```

### Environment Variables (Production)
For production, it's recommended to use environment variables or Azure Key Vault:
- `ServiceBusSettings__ConnectionString`

## Components

### 1. ServiceBusSettings
Located in: `Plantonize.NotasFiscais.Infrastructure/Configuration/ServiceBusSettings.cs`

Configuration class that holds Service Bus connection details.

### 2. IServiceBusService
Located in: `Plantonize.NotasFiscais.Domain/Interfaces/IServiceBusService.cs`

Interface defining Service Bus operations:
- `SendMessageAsync<T>()` - Send message to queue or topic
- `SendMessageToQueueAsync<T>()` - Send message to queue
- `SendMessageToTopicAsync<T>()` - Send message to topic
- `ReceiveMessageAsync<T>()` - Receive message from queue

### 3. ServiceBusService
Located in: `Plantonize.NotasFiscais.Infrastructure/Services/ServiceBusService.cs`

Implementation of the Service Bus service with:
- JSON serialization/deserialization
- Error handling and logging
- Message completion
- Proper resource disposal

## Usage Examples

### Sending Messages

```csharp
// Inject IServiceBusService in your service
public class NotaFiscalService : INotaFiscalService
{
    private readonly IServiceBusService _serviceBusService;

    public NotaFiscalService(IServiceBusService serviceBusService)
    {
        _serviceBusService = serviceBusService;
    }

    public async Task CreateNotaFiscal(NotaFiscal nota)
    {
        // Your business logic...
        
        // Send message to queue
        await _serviceBusService.SendMessageToQueueAsync(nota, "notafiscal-queue");
        
        // Or send to topic
        await _serviceBusService.SendMessageToTopicAsync(nota, "notafiscal-topic");
    }
}
```

### Receiving Messages

```csharp
public class MessageProcessor
{
    private readonly IServiceBusService _serviceBusService;

    public MessageProcessor(IServiceBusService serviceBusService)
    {
        _serviceBusService = serviceBusService;
    }

    public async Task ProcessMessages()
    {
        var message = await _serviceBusService.ReceiveMessageAsync<NotaFiscal>("notafiscal-queue");
        
        if (message != null)
        {
            // Process the message
            Console.WriteLine($"Received NotaFiscal: {message.NumeroNota}");
        }
    }
}
```

## Azure Service Bus Setup

### Creating Resources

1. **Create Service Bus Namespace:**
   ```bash
   az servicebus namespace create \
     --resource-group <resource-group> \
     --name <namespace-name> \
     --location <location> \
     --sku Standard
   ```

2. **Create Queue:**
   ```bash
   az servicebus queue create \
     --resource-group <resource-group> \
     --namespace-name <namespace-name> \
     --name notafiscal-queue
   ```

3. **Create Topic:**
   ```bash
   az servicebus topic create \
     --resource-group <resource-group> \
     --namespace-name <namespace-name> \
     --name notafiscal-topic
   ```

4. **Get Connection String:**
   ```bash
   az servicebus namespace authorization-rule keys list \
     --resource-group <resource-group> \
     --namespace-name <namespace-name> \
     --name RootManageSharedAccessKey \
     --query primaryConnectionString \
     --output tsv
   ```

### Azure Portal Setup

1. Navigate to Azure Portal
2. Create a new Service Bus Namespace
3. Create queues/topics as needed:
   - `notafiscal-queue`
   - `fatura-queue`
   - `notafiscal-topic`
   - `fatura-topic`
4. Copy the connection string from "Shared access policies"

## Use Cases

### 1. NotaFiscal Creation
When a new NotaFiscal is created, a message is sent to the Service Bus queue for:
- Email notification processing
- PDF generation
- Integration with external systems
- Audit logging

### 2. Fatura Processing
Faturas can be sent to queues for:
- Batch processing
- Payment processing
- Report generation

### 3. Event-Driven Architecture
Use topics and subscriptions for:
- Multiple subscribers to the same event
- Fan-out scenarios
- Event sourcing patterns

## Best Practices

1. **Error Handling:** Always wrap Service Bus calls in try-catch blocks
2. **Retry Policies:** Configure retry policies in Service Bus client
3. **Message Size:** Keep messages under 256KB for Standard tier
4. **Connection String Security:** Never commit connection strings to source control
5. **Dead Letter Queue:** Monitor dead letter queues for failed messages
6. **Logging:** Enable comprehensive logging for troubleshooting

## Packages Installed
- `Azure.Messaging.ServiceBus` (v7.20.1)

## Dependencies
The Service Bus service is registered as a singleton in the DI container and is available throughout the application.

## Notes
- The Service Bus connection string is currently empty in appsettings.json - you need to add your actual connection string
- Messages are automatically serialized to JSON
- The service implements `IAsyncDisposable` for proper resource cleanup
- All operations are async for better performance
