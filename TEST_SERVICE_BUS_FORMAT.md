# ?? Teste Rápido - Service Bus Formato Customizado

## ? Teste em 3 Passos

### 1?? Criar NotaFiscal
```powershell
# PowerShell
$body = @{
  valorTotal = 500.00
  municipioPrestacao = "São Paulo"
  issRetido = $false
  medico = @{
    nome = "Dr. João Silva"
    cpfCnpj = "12345678901"
    email = "joao@email.com"
    municipio = "São Paulo"
    inscricaoMunicipal = "123456"
    medicoId = "550e8400-e29b-41d4-a716-446655440000"
  }
  tomador = @{
    nome = "Maria Santos"
    cpfCnpj = "98765432100"
    email = "maria@email.com"
    tipoTomador = "PF"
    endereco = "Av. Paulista, 1000 - Bela Vista"
    municipio = "São Paulo - SP"
  }
  servicos = @(
    @{
      descricao = "Consulta Médica"
      quantidade = 1
      valorUnitario = 500.00
      aliquotaIss = 5.00
      valorTotal = 500.00
    }
  )
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri "https://localhost:7164/api/NotasFiscais" `
  -Method Post `
  -Body $body `
  -ContentType "application/json" `
  -SkipCertificateCheck
```

### 2?? Verificar Mensagem no Service Bus
```powershell
# Receber mensagem da fila
Invoke-RestMethod -Uri "https://localhost:7164/api/ServiceBus/receive/integracao-nf" `
  -Method Get `
  -SkipCertificateCheck
```

### 3?? Verificar Formato da Mensagem
A resposta deve ser:
```json
{
  "_id": "guid-gerado",
  "numeroNota": "número-gerado",
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

## ?? Validações

### ? Checklist
- [ ] `_id` contém um GUID
- [ ] `numeroNota` foi gerado
- [ ] `codigo_servico` = "001"
- [ ] `descricao` = "Consulta Médica"
- [ ] `valor` = 500.00
- [ ] `cpf_cnpj_cliente` = "98765432100"
- [ ] `cliente` = "Maria Santos"
- [ ] `email` = "maria@email.com"
- [ ] `endereco` = "Av. Paulista"
- [ ] `numero` = "1000"
- [ ] `bairro` = "Bela Vista"
- [ ] `codigo_municipio` = "3550308"
- [ ] `municipio` = "São Paulo"
- [ ] `uf` = "SP"

## ?? Teste com Diferentes Formatos

### Teste 1: Rio de Janeiro
```powershell
$tomador = @{
  nome = "João Costa"
  cpfCnpj = "11111111111"
  email = "joao@email.com"
  tipoTomador = "PF"
  endereco = "Rua das Flores, 456 - Copacabana"
  municipio = "Rio de Janeiro - RJ"
}
```

**Resultado esperado:**
```json
{
  "endereco": "Rua das Flores",
  "numero": "456",
  "bairro": "Copacabana",
  "codigo_municipio": "3304557",
  "municipio": "Rio de Janeiro",
  "uf": "RJ"
}
```

### Teste 2: Belo Horizonte
```powershell
$tomador = @{
  nome = "Ana Silva"
  cpfCnpj = "22222222222"
  email = "ana@email.com"
  tipoTomador = "PF"
  endereco = "Av. Afonso Pena, 789 - Centro"
  municipio = "Belo Horizonte - MG"
}
```

**Resultado esperado:**
```json
{
  "endereco": "Av. Afonso Pena",
  "numero": "789",
  "bairro": "Centro",
  "codigo_municipio": "3106200",
  "municipio": "Belo Horizonte",
  "uf": "MG"
}
```

### Teste 3: Município Não Mapeado
```powershell
$tomador = @{
  nome = "Pedro Oliveira"
  cpfCnpj = "33333333333"
  email = "pedro@email.com"
  tipoTomador = "PF"
  endereco = "Rua Principal, 100 - Centro"
  municipio = "Cidade Pequena - RS"
}
```

**Resultado esperado:**
```json
{
  "endereco": "Rua Principal",
  "numero": "100",
  "bairro": "Centro",
  "codigo_municipio": "",  // ?? Vazio pois não está mapeado
  "municipio": "Cidade Pequena",
  "uf": "RS"
}
```

## ?? Troubleshooting

### Problema: Mensagem não aparece no Service Bus
**Verificar:**
1. Console do aplicativo para erros
2. Connection string está correta
3. Fila `integracao-nf` existe no Azure

**Solução:**
```powershell
# Verificar logs do aplicativo
# Procure por: "Failed to send message to Service Bus"
```

### Problema: Código IBGE vazio
**Causa:** Município não está no dicionário

**Solução:** Adicionar município em `GetCodigoIBGE()` (ver `SERVICE_BUS_CUSTOMIZATION_EXAMPLES.md`)

### Problema: Parser de endereço não funciona
**Causa:** Formato do endereço diferente do esperado

**Solução:** Ajustar `ParseEndereco()` para seu formato específico

## ?? Monitoramento no Azure Portal

### Acessar Service Bus
1. Portal Azure ? Service Bus
2. Namespace: `plantonize-servicebus`
3. Fila: `integracao-nf`
4. Service Bus Explorer

### Verificar Mensagens
- **Active Messages**: Mensagens aguardando processamento
- **Dead Letter**: Mensagens com erro
- **Scheduled**: Mensagens agendadas

## ?? Script Completo de Teste

```powershell
# test-service-bus-format.ps1

Write-Host "=== Teste Service Bus - Formato Customizado ===" -ForegroundColor Cyan

# 1. Criar NotaFiscal
Write-Host "`n1. Criando NotaFiscal..." -ForegroundColor Yellow
$body = @{
  valorTotal = 500.00
  municipioPrestacao = "São Paulo"
  issRetido = $false
  medico = @{
    nome = "Dr. João Silva"
    cpfCnpj = "12345678901"
    email = "joao@email.com"
    municipio = "São Paulo"
    inscricaoMunicipal = "123456"
    medicoId = "550e8400-e29b-41d4-a716-446655440000"
  }
  tomador = @{
    nome = "Maria Santos"
    cpfCnpj = "98765432100"
    email = "maria@email.com"
    tipoTomador = "PF"
    endereco = "Av. Paulista, 1000 - Bela Vista"
    municipio = "São Paulo - SP"
  }
  servicos = @(
    @{
      descricao = "Consulta Médica"
      quantidade = 1
      valorUnitario = 500.00
      aliquotaIss = 5.00
      valorTotal = 500.00
    }
  )
} | ConvertTo-Json -Depth 10

try {
  $response = Invoke-RestMethod -Uri "https://localhost:7164/api/NotasFiscais" `
    -Method Post `
    -Body $body `
    -ContentType "application/json" `
    -SkipCertificateCheck
  
  Write-Host "? NotaFiscal criada: $($response.id)" -ForegroundColor Green
  
  # 2. Aguardar processamento
  Write-Host "`n2. Aguardando processamento (2s)..." -ForegroundColor Yellow
  Start-Sleep -Seconds 2
  
  # 3. Verificar mensagem
  Write-Host "`n3. Verificando mensagem no Service Bus..." -ForegroundColor Yellow
  $message = Invoke-RestMethod -Uri "https://localhost:7164/api/ServiceBus/receive/integracao-nf" `
    -Method Get `
    -SkipCertificateCheck
  
  if ($message) {
    Write-Host "? Mensagem recebida!" -ForegroundColor Green
    Write-Host "`nFormato da Mensagem:" -ForegroundColor Cyan
    $message | ConvertTo-Json -Depth 10
    
    # Validações
    Write-Host "`n=== Validações ===" -ForegroundColor Cyan
    if ($message._id) { Write-Host "? _id presente" -ForegroundColor Green } else { Write-Host "? _id ausente" -ForegroundColor Red }
    if ($message.codigo_servico -eq "001") { Write-Host "? codigo_servico correto" -ForegroundColor Green } else { Write-Host "? codigo_servico incorreto" -ForegroundColor Red }
    if ($message.municipio -eq "São Paulo") { Write-Host "? municipio correto" -ForegroundColor Green } else { Write-Host "? municipio incorreto" -ForegroundColor Red }
    if ($message.uf -eq "SP") { Write-Host "? uf correto" -ForegroundColor Green } else { Write-Host "? uf incorreto" -ForegroundColor Red }
    if ($message.codigo_municipio -eq "3550308") { Write-Host "? codigo_municipio correto" -ForegroundColor Green } else { Write-Host "? codigo_municipio incorreto" -ForegroundColor Red }
    if ($message.endereco -eq "Av. Paulista") { Write-Host "? endereco correto" -ForegroundColor Green } else { Write-Host "? endereco incorreto" -ForegroundColor Red }
    if ($message.numero -eq "1000") { Write-Host "? numero correto" -ForegroundColor Green } else { Write-Host "? numero incorreto" -ForegroundColor Red }
    if ($message.bairro -eq "Bela Vista") { Write-Host "? bairro correto" -ForegroundColor Green } else { Write-Host "? bairro incorreto" -ForegroundColor Red }
  } else {
    Write-Host "?? Nenhuma mensagem na fila" -ForegroundColor Yellow
  }
  
} catch {
  Write-Host "? Erro: $_" -ForegroundColor Red
}

Write-Host "`n=== Teste Concluído ===" -ForegroundColor Cyan
```

Salve como `test-service-bus-format.ps1` e execute:
```powershell
.\test-service-bus-format.ps1
```

## ? Resultado Esperado

```
=== Teste Service Bus - Formato Customizado ===

1. Criando NotaFiscal...
? NotaFiscal criada: 550e8400-e29b-41d4-a716-446655440001

2. Aguardando processamento (2s)...

3. Verificando mensagem no Service Bus...
? Mensagem recebida!

Formato da Mensagem:
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

=== Validações ===
? _id presente
? codigo_servico correto
? municipio correto
? uf correto
? codigo_municipio correto
? endereco correto
? numero correto
? bairro correto

=== Teste Concluído ===
```

## ?? Próximos Passos

1. ? Executar teste
2. ? Validar formato
3. ? Testar com diferentes municípios
4. ? Adicionar municípios conforme necessário
5. ? Ajustar parsers se necessário

## ?? Documentação Completa

- `SERVICE_BUS_CUSTOM_FORMAT_SUMMARY.md` - Resumo da implementação
- `SERVICE_BUS_MESSAGE_FORMAT.md` - Formato detalhado
- `SERVICE_BUS_CUSTOMIZATION_EXAMPLES.md` - Exemplos de customização
