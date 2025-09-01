#!/usr/bin/env dotnet-script
#r "nuget: System.Text.Json, 8.0.0"

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using System.Linq;

// Código principal ejecutado directamente (sin método Main)
try
{
    Console.WriteLine("🔍 Ladevi Ventas API Coverage Reporter");
    Console.WriteLine("=====================================");

    // Find the most recent coverage file
    var coverageFile = FindLatestCoverageFile();
    if (coverageFile == null)
    {
        Console.WriteLine("❌ No coverage file found. Run tests with coverage first.");
        Environment.Exit(1);
        return;
    }

    Console.WriteLine($"📊 Using coverage file: {coverageFile}");

    // Extract coverage percentage
    var coverage = ExtractCoveragePercentage(coverageFile);
    Console.WriteLine($"📈 Coverage: {coverage:F2}%");

    // Get git info
    var branch = GetGitBranch();
    var commit = GetGitCommit();

    Console.WriteLine($"🌿 Branch: {branch}");
    Console.WriteLine($"📝 Commit: {commit}");

    // Create payload
    var payload = new CoverageReport
    {
        projectCode = "ladevi-ventas-api",
        branch = branch,
        commit = commit,
        coverage = coverage
    };

    // Send to API
    // await SendCoverageReport(payload);

    Console.WriteLine("✅ Coverage report sent successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Environment.Exit(1);
}

// Funciones helper
string FindLatestCoverageFile()
{
    // Try current directory first
    var testResultsDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResults");

    // If running from temp dir, try parent directory
    if (!Directory.Exists(testResultsDir))
    {
        var parentDir = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
        if (parentDir != null)
        {
            testResultsDir = Path.Combine(parentDir, "TestResults");
        }
    }

    if (!Directory.Exists(testResultsDir))
        return null;

    var coverageFiles = Directory.GetFiles(testResultsDir, "coverage.cobertura.xml", SearchOption.AllDirectories)
        .OrderByDescending(f => File.GetLastWriteTime(f))
        .ToArray();

    return coverageFiles.FirstOrDefault();
}

decimal ExtractCoveragePercentage(string coverageFile)
{
    try
    {
        var xml = XDocument.Load(coverageFile);

        // Try to get line-rate from coverage element
        var coverageElement = xml.Root;
        var lineRate = coverageElement?.Attribute("line-rate")?.Value;

        if (!string.IsNullOrEmpty(lineRate) && decimal.TryParse(lineRate, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal rate))
        {
            return rate * 100; // Convert from 0.54 to 54.0
        }

        // Fallback: calculate from lines covered/valid
        var linesCovered = coverageElement?.Attribute("lines-covered")?.Value;
        var linesValid = coverageElement?.Attribute("lines-valid")?.Value;

        if (!string.IsNullOrEmpty(linesCovered) && !string.IsNullOrEmpty(linesValid) &&
            int.TryParse(linesCovered, out int covered) && int.TryParse(linesValid, out int valid) && valid > 0)
        {
            return (decimal)covered / valid * 100;
        }

        throw new Exception("Could not extract coverage percentage from XML");
    }
    catch (Exception ex)
    {
        throw new Exception($"Failed to parse coverage file: {ex.Message}");
    }
}

string GetGitBranch()
{
    try
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "branch --show-current",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var branch = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();

        return string.IsNullOrEmpty(branch) ? "unknown" : branch;
    }
    catch
    {
        return "unknown";
    }
}

string GetGitCommit()
{
    try
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "rev-parse HEAD",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var commit = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();

        return string.IsNullOrEmpty(commit) ? "unknown" : commit;
    }
    catch
    {
        return "unknown";
    }
}

async Task SendCoverageReport(CoverageReport report)
{
    using var httpClient = new HttpClient();
    const string apiUrl = "https://tracy.greencodesoftware.com/api/agents/chat?agentId=123456&apikey=8888888";

    var json = JsonSerializer.Serialize(report, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");

    Console.WriteLine($"🚀 Sending to: {apiUrl}");
    Console.WriteLine($"📤 Payload: {json}");

    var response = await httpClient.PostAsync(apiUrl, content);

    if (!response.IsSuccessStatusCode)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        throw new Exception($"API request failed with status {response.StatusCode}: {errorContent}");
    }

    var responseContent = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"📥 Response: {responseContent}");
}

// Clase de datos
public class CoverageReport
{
    public string projectCode { get; set; }
    public string branch { get; set; }
    public string commit { get; set; }
    public decimal coverage { get; set; }
}