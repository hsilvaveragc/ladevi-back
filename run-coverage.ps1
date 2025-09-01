# Script para ejecutar coverage de integration tests
param(
    [string]$TestFilter = "",
    [switch]$OpenReport = $true
)

Write-Host "🔍 Running Integration Tests with Code Coverage..." -ForegroundColor Green

# Limpiar resultados previos
if (Test-Path "TestResults") {
    Remove-Item "TestResults" -Recurse -Force
    Write-Host "✅ Cleaned previous test results" -ForegroundColor Yellow
}

# Ejecutar tests con coverage
Write-Host "📊 Running tests with coverage collection..." -ForegroundColor Blue
$testCommand = "dotnet test TracyCommerceApi.Tests --collect:`"XPlat Code Coverage`" --settings coverlet.runsettings --results-directory ./TestResults --verbosity minimal"

if ($TestFilter) {
    $testCommand += " --filter `"$TestFilter`""
    Write-Host "🎯 Filtering tests: $TestFilter" -ForegroundColor Cyan
}

Invoke-Expression $testCommand

# Verificar si hay archivos de coverage
$coverageFiles = Get-ChildItem -Path "TestResults" -Recurse -Filter "coverage.cobertura.xml"
if ($coverageFiles.Count -eq 0) {
    Write-Host "❌ No coverage files found!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Coverage files found: $($coverageFiles.Count)" -ForegroundColor Green

# Generar reporte HTML
Write-Host "📈 Generating HTML coverage report..." -ForegroundColor Blue
reportgenerator -reports:"TestResults\*\coverage.cobertura.xml" -targetdir:"TestResults\CoverageReport" -reporttypes:Html

# Abrir reporte en navegador
$reportPath = "TestResults\CoverageReport\index.html"
if (Test-Path $reportPath) {
    Write-Host "🎉 Coverage report generated successfully!" -ForegroundColor Green
    Write-Host "📖 Report location: $reportPath" -ForegroundColor Cyan
    
    if ($OpenReport) {
        Write-Host "🌐 Opening report in browser..." -ForegroundColor Blue
        Start-Process $reportPath
    }
} else {
    Write-Host "❌ Failed to generate coverage report!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "💡 Coverage includes:" -ForegroundColor Yellow
Write-Host "   - Controllers, Services, Middleware" -ForegroundColor Gray
Write-Host "   - Domain models, Validators" -ForegroundColor Gray  
Write-Host "   - Program/Startup classes" -ForegroundColor Gray
Write-Host ""
Write-Host "⚠️  Excluded from coverage:" -ForegroundColor Yellow
Write-Host "   - DTOs (Data Transfer Objects)" -ForegroundColor Gray
Write-Host "   - Migrations (Entity Framework)" -ForegroundColor Gray
Write-Host "   - Tests projects" -ForegroundColor Gray
Write-Host ""
Write-Host "📤 Sending coverage report to Ladevi Ventas API..." -ForegroundColor Blue
./report-coverage.ps1