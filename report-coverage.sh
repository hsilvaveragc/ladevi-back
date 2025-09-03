#!/bin/bash

echo "🔍 Ladevi Ventas Coverage Reporter"
echo "=================================="
echo ""

# Find the latest coverage file
COVERAGE_FILE=$(find TestResults -name "coverage.cobertura.xml" -type f -printf '%T@ %p\n' 2>/dev/null | sort -nr | head -1 | cut -d' ' -f2-)

if [ -z "$COVERAGE_FILE" ]; then
    echo "❌ No coverage file found!"
    echo "💡 Run tests with coverage first:"
    echo "   ./run-tests-with-coverage.sh"
    exit 1
fi

echo "📊 Found coverage file: $COVERAGE_FILE"
echo "⏰ Generated: $(date -r "$COVERAGE_FILE" '+%Y-%m-%d %H:%M:%S')"
echo ""

# Execute using dotnet-script directly (no compilation needed)
echo "🔨 Running coverage reporter with dotnet-script..."
dotnet script CoverageReporter.cs
EXIT_CODE=$?

if [ $EXIT_CODE -eq 0 ]; then
    echo ""
    echo "🎉 Coverage reported successfully!"
else
    echo ""
    echo "❌ Failed to report coverage!"
fi

exit $EXIT_CODE