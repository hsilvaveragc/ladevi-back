param(
    [string]$TestFilter = "",
    [switch]$OpenReport = $true
)

Write-Host "Docker PostgreSQL for testing..." -ForegroundColor Green

# Crear la red si no existe
try {
    docker network create custom_network 2>$null
} catch {
    Write-Host "Network already exists or created successfully" -ForegroundColor Yellow
}

# Detener y eliminar contenedores previos
Write-Host "Cleaning previous containers..." -ForegroundColor Yellow
docker-compose -f docker-compose.test.yml down -v

# Levantar la base de datos de testing
Write-Host "Starting PostgreSQL container..." -ForegroundColor Blue
docker-compose -f docker-compose.test.yml up -d ladevi_ventas_api_pg_db_testing

Write-Host "Waiting for PostgreSQL to be ready..." -ForegroundColor Cyan
$timeout = 60
$elapsed = 0

do {
    Start-Sleep -Seconds 2
    $elapsed += 2
    
    $result = docker exec ladevi_ventas_api_pg_db_testing pg_isready -U sa -d ladevi_ventas_test 2>$null
    $isReady = $LASTEXITCODE -eq 0
    
    if ($elapsed -ge $timeout -and -not $isReady) {
        Write-Host "Timeout waiting for PostgreSQL" -ForegroundColor Red
        docker-compose -f docker-compose.test.yml down -v
        exit 1
    }
} while (-not $isReady)

Write-Host "PostgreSQL is ready!" -ForegroundColor Green

# Ejecutar tests con cobertura
Write-Host "Running tests with coverage..." -ForegroundColor Blue

if ($TestFilter) {
    Write-Host "Filtering tests: $TestFilter" -ForegroundColor Cyan
    dotnet-coverage collect "dotnet test Tests --filter `"$TestFilter`" --environment Testing" -f cobertura -o TestResults/coverage.cobertura.xml -s coverage.config.xml
} else {
    dotnet-coverage collect "dotnet test Tests --environment Testing" -f cobertura -o TestResults/coverage.cobertura.xml -s coverage.config.xml
}

$testExitCode = $LASTEXITCODE

# Generar reporte HTML si los tests pasaron
if ($testExitCode -eq 0) {
    Write-Host "Generating HTML coverage report..." -ForegroundColor Blue
    reportgenerator -reports:"TestResults/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:Html
    
    if (Test-Path "TestResults/CoverageReport/index.html") {
        Write-Host "Coverage report generated successfully!" -ForegroundColor Green
        Write-Host "Report location: TestResults/CoverageReport/index.html" -ForegroundColor Cyan
        
        # Abrir en navegador
        if ($OpenReport) {
            Write-Host "Opening report in browser..." -ForegroundColor Blue
            Start-Process "TestResults/CoverageReport/index.html"
        }
        
        # Ejecutar reporte de cobertura
        Write-Host "Sending coverage report..." -ForegroundColor Blue
        ./report-coverage.ps1
    }
} else {
    Write-Host "Tests failed. Skipping coverage report generation." -ForegroundColor Red
}

# Limpiar contenedores y volúmenes
Write-Host "Cleaning up Docker containers and volumes..." -ForegroundColor Yellow
docker-compose -f docker-compose.test.yml down -v
try {
    docker volume rm test_dbdata 2>$null
} catch {
    Write-Host "Volume already removed or doesn't exist" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Coverage includes:" -ForegroundColor Yellow
Write-Host "   - Controllers, Services, Middleware" -ForegroundColor Gray
Write-Host "   - Domain models, Validators" -ForegroundColor Gray  
Write-Host "   - Program/Startup classes" -ForegroundColor Gray
Write-Host ""
Write-Host "Excluded from coverage:" -ForegroundColor Yellow
Write-Host "   - DTOs (Data Transfer Objects)" -ForegroundColor Gray
Write-Host "   - Migrations (Entity Framework)" -ForegroundColor Gray
Write-Host "   - Tests projects" -ForegroundColor Gray

exit $testExitCode