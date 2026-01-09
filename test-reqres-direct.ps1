# ============================================================================
# Test directo del servicio Reqres con tu cédula (PowerShell)
# ============================================================================

Write-Host "Probando servicio Reqres con Jose Molina, cédula: 0104826441" -ForegroundColor Cyan
Write-Host ""

# Tu comando corregido para PowerShell:
$headers = @{
    "Content-Type" = "application/json"
    "x-api-key" = "reqres_5e7b3102c0ef4537bb00f95a52aedc01"
}

$body = @{
    name = "Jose Molina"
    job = "0104826441"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "https://reqres.in/api/users" -Method POST -Headers $headers -Body $body
    $response | ConvertTo-Json -Depth 10
    
    Write-Host ""
    Write-Host "============================================================================" -ForegroundColor Green
    Write-Host "EXPLICACIÓN DEL RESULTADO:" -ForegroundColor Green
    Write-Host "============================================================================" -ForegroundColor Green
    Write-Host "El campo 'id' en la respuesta es el SCORE crediticio: $($response.id)" -ForegroundColor Yellow
    Write-Host "Cédula 0104826441 termina en 1 → Score esperado: bajo (~450)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "REGLAS DE NEGOCIO:" -ForegroundColor Cyan
    Write-Host "  - Score >= 700: APROBADO" -ForegroundColor Green
    Write-Host "  - Score 500-699 + Monto < `$1000: APROBADO" -ForegroundColor Yellow
    Write-Host "  - Score < 500: RECHAZADO" -ForegroundColor Red
    Write-Host "============================================================================" -ForegroundColor Green
}
catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
        $errorDetails = $reader.ReadToEnd()
        Write-Host "Detalles: $errorDetails" -ForegroundColor Yellow
    }
}
