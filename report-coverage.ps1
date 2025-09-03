# Script para reportar cobertura de codigo a Ladevi Ventas API
Write-Host "Coverage Reporter" -ForegroundColor Green
Write-Host "=================" -ForegroundColor Green
Write-Host ""

# Verificar que existe el archivo de coverage
$latestCoverage = Get-ChildItem -Path "TestResults" -Recurse -Filter "coverage.cobertura.xml" -ErrorAction SilentlyContinue | 
    Sort-Object LastWriteTime -Descending | 
    Select-Object -First 1

if (-not $latestCoverage) {
    Write-Host "No coverage file found!" -ForegroundColor Red
    Write-Host "Run tests with coverage first:" -ForegroundColor Yellow
    Write-Host "   ./run-tests-with-coverage.ps1" -ForegroundColor Gray
    exit 1
}

Write-Host "Found coverage file: $($latestCoverage.FullName)" -ForegroundColor Cyan
Write-Host "Generated: $($latestCoverage.LastWriteTime)" -ForegroundColor Gray
Write-Host ""

try {
    # Ejecutar usando dotnet-script directamente (sin compilación)
    Write-Host "Running coverage reporter with dotnet-script..." -ForegroundColor Blue
    dotnet script CoverageReporter.cs
    $exitCode = $LASTEXITCODE
    
    if ($exitCode -eq 0) {
        Write-Host ""
        Write-Host "Coverage reported successfully!" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "Failed to report coverage!" -ForegroundColor Red
    }
    
    exit $exitCode
}
catch {
    Write-Host "Error executing coverage reporter: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}