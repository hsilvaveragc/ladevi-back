#!/bin/bash

echo "🐳 Starting Docker PostgreSQL for testing..."

# Crear la red si no existe
docker network create custom_network 2>/dev/null || echo "Network already exists"

# Detener y eliminar contenedores previos
docker-compose -f docker-compose.test.yml down -v

# Levantar la base de datos de testing con --build para forzar reconstrucción
docker-compose -f docker-compose.test.yml up --build -d ladevi_ventas_api_pg_db_testing

echo "⏳ Esperando que PostgreSQL termine de inicializarse completamente..."
sleep 5

echo "🔍 Estado del contenedor:"
docker ps | grep ladevi_ventas_api_pg_db_testing

echo "⏳ Waiting for PostgreSQL to be ready..."
timeout=90
elapsed=0


while ! docker-compose -f docker-compose.test.yml exec -T ladevi_ventas_api_pg_db_testing pg_isready -U sa -d ladevi_ventas_test >/dev/null 2>&1; do
    sleep 3
    elapsed=$((elapsed + 3))
    if [ $elapsed -ge $timeout ]; then
        echo "❌ Timeout waiting for PostgreSQL"
        echo "📋 Container logs:"
        docker logs ladevi_ventas_api_pg_db_testing
        exit 1
    fi
    echo "   Still waiting... ($elapsed/$timeout seconds)"
done

echo "✅ PostgreSQL is ready!"

# Ejecutar tests con cobertura
echo "🧪 Running tests with coverage..."

# Establecer las variables de entorno para modo testing
export CI_CD_MODE="true"

# Debug: verificar que las variables se establecieron
echo "🔍 DefaultConnection=$CI_CD_MODE"

if [ "$1" != "" ]; then
    echo "🎯 Filtering tests: $1"
    dotnet-coverage collect "dotnet test Tests --filter $1" -f cobertura -o TestResults/coverage.cobertura.xml -s coverage.config.xml
else
    dotnet-coverage collect "dotnet test Tests" -f cobertura -o TestResults/coverage.cobertura.xml -s coverage.config.xml
fi

TEST_EXIT_CODE=$?

# Generar reporte HTML si los tests pasaron
if [ $TEST_EXIT_CODE -eq 0 ]; then
    echo "📈 Generating HTML coverage report..."
    reportgenerator -reports:"TestResults/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:Html
    
    if [ -f "TestResults/CoverageReport/index.html" ]; then
        echo "🎉 Coverage report generated successfully!"
        echo "📖 Report location: TestResults/CoverageReport/index.html"
        
        # Abrir en navegador si es posible
        if command -v xdg-open > /dev/null; then
            echo "🌐 Opening report in browser..."
            xdg-open TestResults/CoverageReport/index.html
        fi
        
        # Ejecutar reporte de cobertura si existe
        if [ -f "./report-coverage.sh" ]; then
            echo "📤 Sending coverage report..."
            ./report-coverage.sh
        fi
    fi
else
    echo "❌ Tests failed. Skipping coverage report generation."
fi

# Limpiar contenedores y volúmenes
echo "🧹 Cleaning up Docker containers and volumes..."
docker-compose -f docker-compose.test.yml down -v

exit $TEST_EXIT_CODE