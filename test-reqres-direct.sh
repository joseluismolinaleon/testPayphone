#!/bin/bash
# ============================================================================
# Test directo del servicio Reqres con tu cédula
# ============================================================================

echo "Probando servicio Reqres con Jose Molina, cédula: 0104826441"
echo ""

# Tu comando corregido:
curl --location --request POST 'https://reqres.in/api/users' \
  --header 'Content-Type: application/json' \
  --header 'x-api-key: reqres_5e7b3102c0ef4537bb00f95a52aedc01' \
  --data '{
    "name": "Jose Molina",
    "job": "0104826441"
  }'

echo ""
echo ""
echo "============================================================================"
echo "EXPLICACIÓN DEL RESULTADO:"
echo "============================================================================"
echo "El campo 'id' en la respuesta es el SCORE crediticio"
echo "Cédula 0104826441 termina en 1 → Score esperado: bajo (~450)"
echo ""
echo "REGLAS DE NEGOCIO:"
echo "  - Score >= 700: APROBADO"
echo "  - Score 500-699 + Monto < \$1000: APROBADO"
echo "  - Score < 500: RECHAZADO"
echo "============================================================================"
