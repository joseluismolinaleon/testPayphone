#!/bin/bash
# ============================================================================
# CustomerValidationSystem - Script de Pruebas cURL
# Prueba Técnica: Líder Técnico
# ============================================================================

echo "============================================================================"
echo "CUSTOMERVALIDATIONSYSTEM - PRUEBAS CON cURL"
echo "============================================================================"
echo ""

# Variables
BASE_URL="http://localhost:5000"
REQRES_URL="https://reqres.in"
API_KEY="reqres_5e7b3102c0ef4537bb00f95a52aedc01"

# ============================================================================
# 1. TESTS DE API EXTERNA REQRES (SERVICIO REAL)
# ============================================================================

echo "============================================================================"
echo "1. TESTS DE API EXTERNA REQRES"
echo "============================================================================"
echo ""

echo "--- Test 1.1: Reqres API - Score Alto (cédula termina en 7) ---"
curl --location --request POST "${REQRES_URL}/api/users" \
  --header "Content-Type: application/json" \
  --header "x-api-key: ${API_KEY}" \
  --data '{
    "name": "Juan Pérez",
    "job": "0987654327"
  }' | jq .
echo ""
echo "Nota: El campo 'id' en la respuesta es el score (~750 esperado)"
echo ""
sleep 2

echo "--- Test 1.2: Reqres API - Score Medio (cédula termina en 4) ---"
curl --location --request POST "${REQRES_URL}/api/users" \
  --header "Content-Type: application/json" \
  --header "x-api-key: ${API_KEY}" \
  --data '{
    "name": "María López",
    "job": "0123456784"
  }' | jq .
echo ""
echo "Nota: El campo 'id' en la respuesta es el score (~620 esperado)"
echo ""
sleep 2

echo "--- Test 1.3: Reqres API - Score Bajo (cédula termina en 2) ---"
curl --location --request POST "${REQRES_URL}/api/users" \
  --header "Content-Type: application/json" \
  --header "x-api-key: ${API_KEY}" \
  --data '{
    "name": "Carlos Ruiz",
    "job": "0945678122"
  }' | jq .
echo ""
echo "Nota: El campo 'id' en la respuesta es el score (~450 esperado)"
echo ""
sleep 2

# ============================================================================
# 2. VALIDACIONES DE CLIENTES - REGLAS DE NEGOCIO
# ============================================================================

echo "============================================================================"
echo "2. CUSTOMERVALIDATIONSYSTEM - REGLAS DE NEGOCIO"
echo "============================================================================"
echo ""

echo "--- Test 2.1: REGLA 1 - Score >= 700 → APROBADO ---"
echo "Cliente: Juan Pérez | Cédula: 0987654327 | Monto: \$1,500"
echo "Resultado esperado: APROBADO"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Juan Pérez",
    "cedula": "0987654327",
    "montoTransaccion": 1500.00
  }' | jq .
echo ""
sleep 2

echo "--- Test 2.2: REGLA 2 - Score 500-699 + Monto < \$1000 → APROBADO ---"
echo "Cliente: María López | Cédula: 0123456784 | Monto: \$800"
echo "Resultado esperado: APROBADO"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "María López",
    "cedula": "0123456784",
    "montoTransaccion": 800.00
  }' | jq .
echo ""
sleep 2

echo "--- Test 2.3: REGLA 3 - Score 500-699 + Monto >= \$1000 → RECHAZADO ---"
echo "Cliente: Ana Torres | Cédula: 0934567814 | Monto: \$1,200"
echo "Resultado esperado: RECHAZADO"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Ana Torres",
    "cedula": "0934567814",
    "montoTransaccion": 1200.00
  }' | jq .
echo ""
sleep 2

echo "--- Test 2.4: REGLA 4 - Score < 500 → RECHAZADO ---"
echo "Cliente: Luis Gómez | Cédula: 0912345672 | Monto: \$300"
echo "Resultado esperado: RECHAZADO"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Luis Gómez",
    "cedula": "0912345672",
    "montoTransaccion": 300.00
  }' | jq .
echo ""
sleep 2

# ============================================================================
# 3. QUERIES - OBTENER VALIDACIONES
# ============================================================================

echo "============================================================================"
echo "3. QUERIES - CONSULTAS"
echo "============================================================================"
echo ""

echo "--- Test 3.1: GET ALL - Todas las validaciones ---"
curl -X GET "${BASE_URL}/api/v1/customer-validations" | jq .
echo ""
sleep 2

echo "--- Test 3.2: GET BY ID - Validación ID 1 (seed data) ---"
curl -X GET "${BASE_URL}/api/v1/customer-validations/1" | jq .
echo ""
sleep 2

echo "--- Test 3.3: GET BY ID - Validación ID 6 (primera nueva) ---"
curl -X GET "${BASE_URL}/api/v1/customer-validations/6" | jq .
echo ""
sleep 2

# ============================================================================
# 4. CASOS DE PRUEBA COMPLETOS - EDGE CASES
# ============================================================================

echo "============================================================================"
echo "4. CASOS DE PRUEBA EDGE CASES"
echo "============================================================================"
echo ""

echo "--- Test 4.1: Cliente Premium - Alto Score + Monto Alto ---"
echo "Cliente: Roberto Sánchez | Cédula: 0987654329 | Monto: \$5,000"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Roberto Sánchez Premium",
    "cedula": "0987654329",
    "montoTransaccion": 5000.00
  }' | jq .
echo ""
sleep 2

echo "--- Test 4.2: Cliente Regular - Score Medio + Monto Pequeño ---"
echo "Cliente: Patricia Morales | Cédula: 0945678125 | Monto: \$500"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Patricia Morales",
    "cedula": "0945678125",
    "montoTransaccion": 500.00
  }' | jq .
echo ""
sleep 2

echo "--- Test 4.3: Edge Case - Justo \$1,000 (RECHAZADO) ---"
echo "Cliente: Sandra Ramírez | Cédula: 0934567816 | Monto: \$1,000"
echo "Resultado esperado: RECHAZADO (monto NO es < \$1,000)"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Sandra Ramírez",
    "cedula": "0934567816",
    "montoTransaccion": 1000.00
  }' | jq .
echo ""
sleep 2

echo "--- Test 4.4: Edge Case - \$999.99 (APROBADO) ---"
echo "Cliente: Diego Fernández | Cédula: 0987654324 | Monto: \$999.99"
echo "Resultado esperado: APROBADO (monto < \$1,000)"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Diego Fernández",
    "cedula": "0987654324",
    "montoTransaccion": 999.99
  }' | jq .
echo ""
sleep 2

echo "--- Test 4.5: Cliente Riesgoso - Score Bajo + Monto Alto ---"
echo "Cliente: Andrés Vargas | Cédula: 0912345671 | Monto: \$2,000"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Andrés Vargas",
    "cedula": "0912345671",
    "montoTransaccion": 2000.00
  }' | jq .
echo ""
sleep 2

# ============================================================================
# 5. VALIDACIONES DE ENTRADA (ERRORES 400)
# ============================================================================

echo "============================================================================"
echo "5. VALIDACIONES DE ENTRADA - ERRORES 400"
echo "============================================================================"
echo ""

echo "--- Test 5.1: Nombre vacío (400 Bad Request) ---"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "",
    "cedula": "0987654321",
    "montoTransaccion": 500.00
  }' | jq .
echo ""
sleep 2

echo "--- Test 5.2: Cédula inválida - menos de 10 dígitos (400 Bad Request) ---"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Test Usuario",
    "cedula": "123",
    "montoTransaccion": 500.00
  }' | jq .
echo ""
sleep 2

echo "--- Test 5.3: Monto negativo (400 Bad Request) ---"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Test Usuario",
    "cedula": "0987654321",
    "montoTransaccion": -100.00
  }' | jq .
echo ""
sleep 2

echo "--- Test 5.4: Monto cero (400 Bad Request) ---"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Test Usuario",
    "cedula": "0987654321",
    "montoTransaccion": 0
  }' | jq .
echo ""
sleep 2

echo "--- Test 5.5: Múltiples errores (400 Bad Request) ---"
curl -X POST "${BASE_URL}/api/v1/customer-validations" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "",
    "cedula": "abc",
    "montoTransaccion": -500
  }' | jq .
echo ""
sleep 2

# ============================================================================
# RESUMEN FINAL
# ============================================================================

echo "============================================================================"
echo "RESUMEN DE PRUEBAS COMPLETADAS"
echo "============================================================================"
echo ""
echo "✅ Tests de API Externa Reqres (3)"
echo "✅ Validaciones con 4 Reglas de Negocio (4)"
echo "✅ Queries (GET ALL, GET BY ID) (3)"
echo "✅ Edge Cases y Casos Completos (5)"
echo "✅ Validaciones de Entrada - 400 Bad Request (5)"
echo ""
echo "Total: 20 pruebas ejecutadas"
echo ""
echo "============================================================================"
