namespace LadeviVentasApi.Extensions.Startup;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensiones para configurar las settings de la aplicación
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Configura y carga los archivos de configuración de la aplicación.
    /// </summary>
    /// <param name="webApplicationBuilder">El constructor de la aplicación web</param>
    /// <param name="showSettings">Indica si se deben mostrar las configuraciones cargadas</param>
    /// <param name="additionalConfigFiles">Archivos de configuración adicionales opcionales</param>
    public static void SetupConfiguration(
        this WebApplicationBuilder webApplicationBuilder,
        bool showSettings = true,
        string[]? additionalConfigFiles = null)
    {
        // Configuración base
        var configuration = webApplicationBuilder.Configuration;
        var environment = webApplicationBuilder.Environment;

        configuration
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

        // Agregar archivos de configuración adicionales si se proporcionan
        if (additionalConfigFiles != null)
        {
            foreach (var configFile in additionalConfigFiles)
            {
                configuration.AddJsonFile(configFile, optional: true, reloadOnChange: true);
            }
        }

        configuration
            .AddJsonFile("appsettings.private.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        // Añadir configuración como singleton para inyección de dependencias
        webApplicationBuilder.Services.AddSingleton(configuration);

        // Mostrar configuraciones si está habilitado
        if (showSettings)
        {
            PrintConfigurationSettings(configuration);
        }
    }

    /// <summary>
    /// Imprime las configuraciones cargadas en la consola.
    /// </summary>
    /// <param name="configuration">La configuración a imprimir</param>
    private static void PrintConfigurationSettings(IConfiguration configuration)
    {
        try
        {
            Console.WriteLine("\nCurrent Configuration Settings:");
            configuration.GetChildren()
                .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                .ToList()
                .ForEach(x => Console.WriteLine($"\t{x.Key}: {x.Value}"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error printing configuration settings: {ex.Message}");
        }
    }
}