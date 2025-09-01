#!/bin/bash

echo "🔍 Ladevi Ventas Coverage Reporter"
echo "=================================="
echo ""

# Find the latest coverage file
COVERAGE_FILE=$(find TestResults -name "coverage.cobertura.xml" -type f -printf '%T@ %p\n' 2>/dev/null | sort -nr | head -1 | cut -d' ' -f2-)

if [ -z "$COVERAGE_FILE" ]; then
    echo "❌ No coverage file found!"
    echo "💡 Run tests with coverage first:"
    echo "   ./run-coverage.ps1"
    exit 1
fi

echo "📊 Found coverage file: $COVERAGE_FILE"
echo "⏰ Generated: $(date -r "$COVERAGE_FILE" '+%Y-%m-%d %H:%M:%S')"
echo ""

# Create temp directory
TEMP_DIR="temp-coverage-reporter"
rm -rf "$TEMP_DIR"
mkdir "$TEMP_DIR"

# Create project file
cat > "$TEMP_DIR/CoverageReporter.csproj" << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
EOF

# Copy C# file
cp "CoverageReporter.cs" "$TEMP_DIR/Program.cs"

# Compile and run
echo "🔨 Compiling and running coverage reporter..."
cd "$TEMP_DIR"
dotnet run --verbosity quiet
EXIT_CODE=$?
cd ..

# Cleanup
rm -rf "$TEMP_DIR"

if [ $EXIT_CODE -eq 0 ]; then
    echo ""
    echo "🎉 Coverage reported successfully!"
else
    echo ""
    echo "❌ Failed to report coverage!"
fi

exit $EXIT_CODE