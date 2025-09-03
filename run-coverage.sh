#!/bin/bash

echo "🔍 Running Integration Tests with Code Coverage..."

# Limpiar resultados previos
if [ -d "TestResults" ]; then
    rm -rf TestResults
    echo "✅ Cleaned previous test results"
fi

# Ejecutar tests con coverage
echo "📊 Running tests with coverage collection..."

if [ "$1" != "" ]; then
    echo "🎯 Filtering tests: $1"
    dotnet-coverage collect "dotnet test Tests --filter $1" -f cobertura -o TestResults/coverage.cobertura.xml -s coverage.config.xml
else
    dotnet-coverage collect "dotnet test Tests" -f cobertura -o TestResults/coverage.cobertura.xml -s coverage.config.xml
fi

# Verificar si hay archivos de coverage
if [ ! -f "TestResults/coverage.cobertura.xml" ]; then
    echo "❌ No coverage files found!"
    exit 1
fi

echo "✅ Coverage file generated successfully!"

# Generar reporte HTML
echo "📈 Generating HTML coverage report..."
reportgenerator -reports:"TestResults/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:Html

# Verificar si el reporte se generó
if [ -f "TestResults/CoverageReport/index.html" ]; then
    echo "🎉 Coverage report generated successfully!"
    echo "📖 Report location: TestResults/CoverageReport/index.html"
    
    # Intentar abrir en navegador (funciona en Ubuntu con GUI)
    if command -v xdg-open > /dev/null; then
        echo "🌐 Opening report in browser..."
        xdg-open TestResults/CoverageReport/index.html
    fi
else
    echo "❌ Failed to generate coverage report!"
    exit 1
fi

echo ""
echo "💡 Coverage includes:"
echo "   - Controllers, Services, Middleware"
echo "   - Domain models, Validators"  
echo "   - Program/Startup classes"
echo ""
echo "⚠️  Excluded from coverage:"
echo "   - DTOs (Data Transfer Objects)"
echo "   - Migrations (Entity Framework)"
echo "   - Tests projects"
echo ""
echo "📤 Sending coverage report to API..."
./report-coverage.sh