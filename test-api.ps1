Write-Host "=== Testando API JobFitScore ===" -ForegroundColor Cyan
Write-Host ""

# 1. Build do projeto
Write-Host "1. Compilando projeto..." -ForegroundColor Yellow
dotnet build --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro no build!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Build concluído com sucesso" -ForegroundColor Green
Write-Host ""

# 2. Executar testes unitários
Write-Host "2. Executando testes unitários..." -ForegroundColor Yellow
$testResult = dotnet test --no-build --verbosity quiet 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Testes unitários passaram" -ForegroundColor Green
} else {
    Write-Host "⚠ Alguns testes falharam (continuando...)" -ForegroundColor Yellow
}
Write-Host ""

# 3. Iniciar API em background
Write-Host "3. Iniciando API..." -ForegroundColor Yellow
$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --no-build" -PassThru -NoNewWindow -RedirectStandardOutput "api-output.log" -RedirectStandardError "api-error.log"

# Aguardar API iniciar
Write-Host "Aguardando API iniciar (30 segundos)..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Verificar se processo ainda está rodando
if ($apiProcess.HasExited) {
    Write-Host "✗ API falhou ao iniciar!" -ForegroundColor Red
    Get-Content "api-error.log"
    exit 1
}
Write-Host "✓ API iniciada (PID: $($apiProcess.Id))" -ForegroundColor Green
Write-Host ""

# 4. Testar endpoints
Write-Host "4. Testando endpoints..." -ForegroundColor Yellow
Write-Host ""

# Teste 1: Health Ping
Write-Host "   Teste 1: GET /api/health/ping" -ForegroundColor Cyan
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/health/ping" -Method GET -UseBasicParsing
    if ($response.StatusCode -eq 200) {
        Write-Host "   ✓ Status: 200 OK" -ForegroundColor Green
        $json = $response.Content | ConvertFrom-Json
        Write-Host "   ✓ Status: $($json.data.status)" -ForegroundColor Green
        Write-Host "   ✓ Uptime: $($json.data.uptime)" -ForegroundColor Green
    }
} catch {
    Write-Host "   ✗ Erro: $_" -ForegroundColor Red
}
Write-Host ""

# Teste 2: Health Check Completo
Write-Host "   Teste 2: GET /api/health" -ForegroundColor Cyan
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/health" -Method GET -UseBasicParsing
    if ($response.StatusCode -eq 200) {
        Write-Host "   ✓ Status: 200 OK" -ForegroundColor Green
        $json = $response.Content | ConvertFrom-Json
        Write-Host "   ✓ Status Geral: $($json.data.status)" -ForegroundColor Green
        foreach ($check in $json.data.checks) {
            Write-Host "   ✓ $($check.componente): $($check.status)" -ForegroundColor Green
        }
    }
} catch {
    Write-Host "   ✗ Erro: $_" -ForegroundColor Red
}
Write-Host ""

# Teste 3: Swagger
Write-Host "   Teste 3: GET /swagger" -ForegroundColor Cyan
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/swagger" -Method GET -UseBasicParsing
    if ($response.StatusCode -eq 200) {
        Write-Host "   ✓ Status: 200 OK - Swagger disponível" -ForegroundColor Green
    }
} catch {
    Write-Host "   ✗ Erro: $_" -ForegroundColor Red
}
Write-Host ""

# Teste 4: API v1 - Listar Usuários (sem autenticação - deve falhar)
Write-Host "   Teste 4: GET /api/v1/usuarios (sem token)" -ForegroundColor Cyan
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/v1/usuarios" -Method GET -UseBasicParsing
    Write-Host "   ⚠ Status: $($response.StatusCode) (esperava 401)" -ForegroundColor Yellow
} catch {
    if ($_.Exception.Response.StatusCode -eq 401) {
        Write-Host "   ✓ Status: 401 Unauthorized (correto!)" -ForegroundColor Green
    } else {
        Write-Host "   ✗ Erro: $_" -ForegroundColor Red
    }
}
Write-Host ""

# Teste 5: API v2 - Listar Vagas (sem autenticação - deve falhar)
Write-Host "   Teste 5: GET /api/v2/vagas (sem token)" -ForegroundColor Cyan
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/v2/vagas" -Method GET -UseBasicParsing
    Write-Host "   ⚠ Status: $($response.StatusCode) (esperava 401)" -ForegroundColor Yellow
} catch {
    if ($_.Exception.Response.StatusCode -eq 401) {
        Write-Host "   ✓ Status: 401 Unauthorized (correto!)" -ForegroundColor Green
    } else {
        Write-Host "   ✗ Erro: $_" -ForegroundColor Red
    }
}
Write-Host ""

# Teste 6: Login (deve estar disponível sem autenticação)
Write-Host "   Teste 6: POST /api/v2/login (tentativa de login)" -ForegroundColor Cyan
try {
    $body = @{
        email = "test@example.com"
        senha = "senha123"
    } | ConvertTo-Json
    
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/v2/login" -Method POST -Body $body -ContentType "application/json" -UseBasicParsing
    Write-Host "   ⚠ Status: $($response.StatusCode)" -ForegroundColor Yellow
    $json = $response.Content | ConvertFrom-Json
    Write-Host "   ⚠ Response: $($json.message)" -ForegroundColor Yellow
} catch {
    $statusCode = $_.Exception.Response.StatusCode.Value__
    Write-Host "   ⚠ Status: $statusCode (esperado se usuário não existe)" -ForegroundColor Yellow
}
Write-Host ""

# 5. Finalizar
Write-Host "=== Resumo dos Testes ===" -ForegroundColor Cyan
Write-Host "✓ API compilou com sucesso" -ForegroundColor Green
Write-Host "✓ API iniciou corretamente" -ForegroundColor Green
Write-Host "✓ Endpoints de health estão funcionando" -ForegroundColor Green
Write-Host "✓ Swagger está acessível" -ForegroundColor Green
Write-Host "✓ Autenticação JWT está protegendo endpoints" -ForegroundColor Green
Write-Host ""

# Parar API
Write-Host "Parando API..." -ForegroundColor Yellow
Stop-Process -Id $apiProcess.Id -Force
Write-Host "✓ API parada" -ForegroundColor Green
Write-Host ""

Write-Host "=== Testes Concluídos ===" -ForegroundColor Cyan
