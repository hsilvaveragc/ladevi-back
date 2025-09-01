#!/bin/bash
# LadeviVentasApi/init-test-db.sh

echo "🚀 Iniciando entorno de testing..."

# Esperar a que la base de datos esté lista
until pg_isready -h test-db -p 5432 -U sa; do
  echo "Esperando PostgreSQL..."
  sleep 2
done

echo "✅ Base de datos lista"

# Ejecutar migraciones
# echo "🔄 Ejecutando migraciones..."
# dotnet ef database update --no-build

echo "🌱 Insertando datos de prueba..."
# Aquí puedes agregar seed data específico para testing

echo "🎯 Iniciando API en modo testing..."
dotnet LadeviVentasApi.dll