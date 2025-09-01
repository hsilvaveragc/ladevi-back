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
    Write-Host "   ./run-coverage.ps1" -ForegroundColor Gray
    exit 1
}

Write-Host "Found coverage file: $($latestCoverage.FullName)" -ForegroundColor Cyan
Write-Host "Generated: $($latestCoverage.LastWriteTime)" -ForegroundColor Gray
Write-Host ""

# Crear un proyecto temporal para compilar
$tempDir = "temp-coverage-reporter"
if (Test-Path $tempDir) {
    Remove-Item $tempDir -Recurse -Force
}
New-Item -ItemType Directory -Path $tempDir | Out-Null

# Crear project file
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
"@ | Out-File -FilePath "$tempDir/CoverageReporter.csproj" -Encoding UTF8

# Copiar el archivo C#
Copy-Item "CoverageReporter.cs" "$tempDir/Program.cs"

try {
    # Compilar y ejecutar
    Write-Host "Compiling and running coverage reporter..." -ForegroundColor Blue
    Push-Location $tempDir
    dotnet run --verbosity quiet
    $exitCode = $LASTEXITCODE
    Pop-Location
    
    if ($exitCode -eq 0) {
        Write-Host ""
        Write-Host "Coverage reported successfully!" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "Failed to report coverage!" -ForegroundColor Red
    }
}
finally {
    # Limpiar
    if (Test-Path $tempDir) {
        Remove-Item $tempDir -Recurse -Force
    }
    
    exit $exitCode
}