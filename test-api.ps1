# ============================================================================
# CustomerValidationSystem - Script de Pruebas PowerShell (Windows)
# Prueba Técnica: Líder Técnico
# ============================================================================

Write-Host "============================================================================" -ForegroundColor Cyan
Write-Host "CUSTOMERVALIDATIONSYSTEM - PRUEBAS CON PowerShell" -ForegroundColor Cyan
Write-Host "============================================================================" -ForegroundColor Cyan
Write-Host ""

# Variables
$baseUrl = "http://localhost:5000"
$reqresUrl = "https://reqres.in"
$apiKey = "reqres_5e7b3102c0ef4537bb00f95a52aedc01"

# Función auxiliar para hacer requests
function Invoke-ApiTest {
    param(
        [string]$Method,
        [string]$Url,
        [string]$Body = $null,
        [hashtable]$Headers = @{}
    )
    
    try {
        if ($Body) {
            $response = Invoke-RestMethod -Uri $Url -Method $Method -Body $Body -ContentType "application/json" -Headers $Headers
        } else {
            $response = Invoke-RestMethod -Uri $Url -Method $Method -Headers $Headers
        }
        $response | ConvertTo-Json -Depth 10
    }
    catch {
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
            $errorDetails = $reader.ReadToEnd()
            Write-Host "Detalles: $errorDetails" -ForegroundColor Yellow
        }
    }
}

# ============================================================================
# 1. TESTS DE API EXTERNA REQRES (SERVICIO REAL)
# ============================================================================

Write-Host "============================================================================" -ForegroundColor Green
Write-Host "1. TESTS DE API EXTERNA REQRES" -ForegroundColor Green
Write-Host "============================================================================" -ForegroundColor Green
Write-Host ""

Write-Host "--- Test 1.1: Reqres API - Score Alto (cédula termina en 7) ---" -ForegroundColor Yellow
$body = @{
    name = "Juan Pérez"
    job = "0987654327"
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$reqresUrl/api/users" -Body $body -Headers @{ "x-api-key" = $apiKey }
Write-Host ""
Write-Host "Nota: El campo 'id' en la respuesta es el score (~750 esperado)" -ForegroundColor Cyan
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 1.2: Reqres API - Score Medio (cédula termina en 4) ---" -ForegroundColor Yellow
$body = @{
    name = "María López"
    job = "0123456784"
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$reqresUrl/api/users" -Body $body -Headers @{ "x-api-key" = $apiKey }
Write-Host ""
Write-Host "Nota: El campo 'id' en la respuesta es el score (~620 esperado)" -ForegroundColor Cyan
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 1.3: Reqres API - Score Bajo (cédula termina en 2) ---" -ForegroundColor Yellow
$body = @{
    name = "Carlos Ruiz"
    job = "0945678122"
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$reqresUrl/api/users" -Body $body -Headers @{ "x-api-key" = $apiKey }
Write-Host ""
Write-Host "Nota: El campo 'id' en la respuesta es el score (~450 esperado)" -ForegroundColor Cyan
Write-Host ""
Start-Sleep -Seconds 2

# ============================================================================
# 2. VALIDACIONES DE CLIENTES - REGLAS DE NEGOCIO
# ============================================================================

Write-Host "============================================================================" -ForegroundColor Green
Write-Host "2. CUSTOMERVALIDATIONSYSTEM - REGLAS DE NEGOCIO" -ForegroundColor Green
Write-Host "============================================================================" -ForegroundColor Green
Write-Host ""

Write-Host "--- Test 2.1: REGLA 1 - Score >= 700 → APROBADO ---" -ForegroundColor Yellow
Write-Host "Cliente: Juan Pérez | Cédula: 0987654327 | Monto: `$1,500"
Write-Host "Resultado esperado: APROBADO" -ForegroundColor Green
$body = @{
    nombre = "Juan Pérez"
    cedula = "0987654327"
    montoTransaccion = 1500.00
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 2.2: REGLA 2 - Score 500-699 + Monto < `$1000 → APROBADO ---" -ForegroundColor Yellow
Write-Host "Cliente: María López | Cédula: 0123456784 | Monto: `$800"
Write-Host "Resultado esperado: APROBADO" -ForegroundColor Green
$body = @{
    nombre = "María López"
    cedula = "0123456784"
    montoTransaccion = 800.00
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 2.3: REGLA 3 - Score 500-699 + Monto >= `$1000 → RECHAZADO ---" -ForegroundColor Yellow
Write-Host "Cliente: Ana Torres | Cédula: 0934567814 | Monto: `$1,200"
Write-Host "Resultado esperado: RECHAZADO" -ForegroundColor Red
$body = @{
    nombre = "Ana Torres"
    cedula = "0934567814"
    montoTransaccion = 1200.00
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 2.4: REGLA 4 - Score < 500 → RECHAZADO ---" -ForegroundColor Yellow
Write-Host "Cliente: Luis Gómez | Cédula: 0912345672 | Monto: `$300"
Write-Host "Resultado esperado: RECHAZADO" -ForegroundColor Red
$body = @{
    nombre = "Luis Gómez"
    cedula = "0912345672"
    montoTransaccion = 300.00
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

# ============================================================================
# 3. QUERIES - OBTENER VALIDACIONES
# ============================================================================

Write-Host "============================================================================" -ForegroundColor Green
Write-Host "3. QUERIES - CONSULTAS" -ForegroundColor Green
Write-Host "============================================================================" -ForegroundColor Green
Write-Host ""

Write-Host "--- Test 3.1: GET ALL - Todas las validaciones ---" -ForegroundColor Yellow
Invoke-ApiTest -Method GET -Url "$baseUrl/api/v1/customer-validations"
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 3.2: GET BY ID - Validación ID 1 (seed data) ---" -ForegroundColor Yellow
Invoke-ApiTest -Method GET -Url "$baseUrl/api/v1/customer-validations/1"
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 3.3: GET BY ID - Validación ID 6 (primera nueva) ---" -ForegroundColor Yellow
Invoke-ApiTest -Method GET -Url "$baseUrl/api/v1/customer-validations/6"
Write-Host ""
Start-Sleep -Seconds 2

# ============================================================================
# 4. CASOS DE PRUEBA COMPLETOS - EDGE CASES
# ============================================================================

Write-Host "============================================================================" -ForegroundColor Green
Write-Host "4. CASOS DE PRUEBA EDGE CASES" -ForegroundColor Green
Write-Host "============================================================================" -ForegroundColor Green
Write-Host ""

Write-Host "--- Test 4.1: Cliente Premium - Alto Score + Monto Alto ---" -ForegroundColor Yellow
Write-Host "Cliente: Roberto Sánchez | Cédula: 0987654329 | Monto: `$5,000"
$body = @{
    nombre = "Roberto Sánchez Premium"
    cedula = "0987654329"
    montoTransaccion = 5000.00
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 4.2: Cliente Regular - Score Medio + Monto Pequeño ---" -ForegroundColor Yellow
Write-Host "Cliente: Patricia Morales | Cédula: 0945678125 | Monto: `$500"
$body = @{
    nombre = "Patricia Morales"
    cedula = "0945678125"
    montoTransaccion = 500.00
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 4.3: Edge Case - Justo `$1,000 (RECHAZADO) ---" -ForegroundColor Yellow
Write-Host "Cliente: Sandra Ramírez | Cédula: 0934567816 | Monto: `$1,000"
Write-Host "Resultado esperado: RECHAZADO (monto NO es < `$1,000)" -ForegroundColor Red
$body = @{
    nombre = "Sandra Ramírez"
    cedula = "0934567816"
    montoTransaccion = 1000.00
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 4.4: Edge Case - `$999.99 (APROBADO) ---" -ForegroundColor Yellow
Write-Host "Cliente: Diego Fernández | Cédula: 0987654324 | Monto: `$999.99"
Write-Host "Resultado esperado: APROBADO (monto < `$1,000)" -ForegroundColor Green
$body = @{
    nombre = "Diego Fernández"
    cedula = "0987654324"
    montoTransaccion = 999.99
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 4.5: Cliente Riesgoso - Score Bajo + Monto Alto ---" -ForegroundColor Yellow
Write-Host "Cliente: Andrés Vargas | Cédula: 0912345671 | Monto: `$2,000"
$body = @{
    nombre = "Andrés Vargas"
    cedula = "0912345671"
    montoTransaccion = 2000.00
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

# ============================================================================
# 5. VALIDACIONES DE ENTRADA (ERRORES 400)
# ============================================================================

Write-Host "============================================================================" -ForegroundColor Green
Write-Host "5. VALIDACIONES DE ENTRADA - ERRORES 400" -ForegroundColor Green
Write-Host "============================================================================" -ForegroundColor Green
Write-Host ""

Write-Host "--- Test 5.1: Nombre vacío (400 Bad Request) ---" -ForegroundColor Yellow
$body = @{
    nombre = ""
    cedula = "0987654321"
    montoTransaccion = 500.00
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 5.2: Cédula inválida - menos de 10 dígitos (400 Bad Request) ---" -ForegroundColor Yellow
$body = @{
    nombre = "Test Usuario"
    cedula = "123"
    montoTransaccion = 500.00
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 5.3: Monto negativo (400 Bad Request) ---" -ForegroundColor Yellow
$body = @{
    nombre = "Test Usuario"
    cedula = "0987654321"
    montoTransaccion = -100.00
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 5.4: Monto cero (400 Bad Request) ---" -ForegroundColor Yellow
$body = @{
    nombre = "Test Usuario"
    cedula = "0987654321"
    montoTransaccion = 0
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

Write-Host "--- Test 5.5: Múltiples errores (400 Bad Request) ---" -ForegroundColor Yellow
$body = @{
    nombre = ""
    cedula = "abc"
    montoTransaccion = -500
} | ConvertTo-Json

Invoke-ApiTest -Method POST -Url "$baseUrl/api/v1/customer-validations" -Body $body
Write-Host ""
Start-Sleep -Seconds 2

# ============================================================================
# RESUMEN FINAL
# ============================================================================

Write-Host "============================================================================" -ForegroundColor Cyan
Write-Host "RESUMEN DE PRUEBAS COMPLETADAS" -ForegroundColor Cyan
Write-Host "============================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "✅ Tests de API Externa Reqres (3)" -ForegroundColor Green
Write-Host "✅ Validaciones con 4 Reglas de Negocio (4)" -ForegroundColor Green
Write-Host "✅ Queries (GET ALL, GET BY ID) (3)" -ForegroundColor Green
Write-Host "✅ Edge Cases y Casos Completos (5)" -ForegroundColor Green
Write-Host "✅ Validaciones de Entrada - 400 Bad Request (5)" -ForegroundColor Green
Write-Host ""
Write-Host "Total: 20 pruebas ejecutadas" -ForegroundColor Cyan
Write-Host ""
Write-Host "============================================================================" -ForegroundColor Cyan
