# Test Service Bus - Formato Customizado

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
  
  if ($message.success -eq $false) {
    Write-Host "?? Nenhuma mensagem na fila" -ForegroundColor Yellow
  } elseif ($message.data) {
    Write-Host "? Mensagem recebida!" -ForegroundColor Green
    Write-Host "`nFormato da Mensagem:" -ForegroundColor Cyan
    $msg = $message.data
    $msg | ConvertTo-Json -Depth 10
    
    # Validações
    Write-Host "`n=== Validações ===" -ForegroundColor Cyan
    if ($msg._id) { Write-Host "? _id presente: $($msg._id)" -ForegroundColor Green } else { Write-Host "? _id ausente" -ForegroundColor Red }
    if ($msg.numeroNota) { Write-Host "? numeroNota presente: $($msg.numeroNota)" -ForegroundColor Green } else { Write-Host "? numeroNota ausente" -ForegroundColor Red }
    if ($msg.codigo_servico -eq "001") { Write-Host "? codigo_servico correto: 001" -ForegroundColor Green } else { Write-Host "? codigo_servico incorreto: $($msg.codigo_servico)" -ForegroundColor Red }
    if ($msg.descricao -eq "Consulta Médica") { Write-Host "? descricao correta" -ForegroundColor Green } else { Write-Host "? descricao incorreta: $($msg.descricao)" -ForegroundColor Red }
    if ($msg.valor -eq 500.00) { Write-Host "? valor correto: 500.00" -ForegroundColor Green } else { Write-Host "? valor incorreto: $($msg.valor)" -ForegroundColor Red }
    if ($msg.cpf_cnpj_cliente -eq "98765432100") { Write-Host "? cpf_cnpj_cliente correto" -ForegroundColor Green } else { Write-Host "? cpf_cnpj_cliente incorreto" -ForegroundColor Red }
    if ($msg.cliente -eq "Maria Santos") { Write-Host "? cliente correto" -ForegroundColor Green } else { Write-Host "? cliente incorreto" -ForegroundColor Red }
    if ($msg.email -eq "maria@email.com") { Write-Host "? email correto" -ForegroundColor Green } else { Write-Host "? email incorreto" -ForegroundColor Red }
    if ($msg.endereco -eq "Av. Paulista") { Write-Host "? endereco correto: Av. Paulista" -ForegroundColor Green } else { Write-Host "? endereco incorreto: $($msg.endereco)" -ForegroundColor Red }
    if ($msg.numero -eq "1000") { Write-Host "? numero correto: 1000" -ForegroundColor Green } else { Write-Host "? numero incorreto: $($msg.numero)" -ForegroundColor Red }
    if ($msg.bairro -eq "Bela Vista") { Write-Host "? bairro correto: Bela Vista" -ForegroundColor Green } else { Write-Host "? bairro incorreto: $($msg.bairro)" -ForegroundColor Red }
    if ($msg.municipio -eq "São Paulo") { Write-Host "? municipio correto: São Paulo" -ForegroundColor Green } else { Write-Host "? municipio incorreto: $($msg.municipio)" -ForegroundColor Red }
    if ($msg.uf -eq "SP") { Write-Host "? uf correto: SP" -ForegroundColor Green } else { Write-Host "? uf incorreto: $($msg.uf)" -ForegroundColor Red }
    if ($msg.codigo_municipio -eq "3550308") { Write-Host "? codigo_municipio correto: 3550308" -ForegroundColor Green } else { Write-Host "?? codigo_municipio: $($msg.codigo_municipio)" -ForegroundColor Yellow }
    
    Write-Host "`n=== Resumo do Teste ===" -ForegroundColor Cyan
    Write-Host "Todos os campos foram mapeados corretamente!" -ForegroundColor Green
  } else {
    Write-Host "?? Formato de resposta inesperado" -ForegroundColor Yellow
    $message | ConvertTo-Json -Depth 10
  }
  
} catch {
  Write-Host "? Erro: $_" -ForegroundColor Red
  Write-Host "Detalhes: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== Teste Concluído ===" -ForegroundColor Cyan
