namespace LadeviVentasApi.Helpers.Attributes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Atributo que restringe el acceso a endpoints solo en ambiente de Testing
/// con validaciones adicionales de token y configuración.
/// </summary>
public class TestingOnlyAttribute : ActionFilterAttribute
{
    private readonly string _tokenParameterName;

    /// <summary>
    /// Constructor del atributo de Testing
    /// </summary>
    /// <param name="tokenParameterName">Nombre del parámetro que contiene el token (por defecto "token")</param>
    public TestingOnlyAttribute(string tokenParameterName = "token")
    {
        _tokenParameterName = tokenParameterName;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var env = context.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

        // 1. Verificar que estamos en ambiente Testing
        if (!env.IsEnvironment("Testing"))
        {
            context.Result = new NotFoundResult();
            return;
        }

        // 2. Verificar que los endpoints de testing estén habilitados
        var enableTestingEndpoints = configuration["EnableTestingEndpoints"];
        if (string.IsNullOrEmpty(enableTestingEndpoints) ||
            !bool.TryParse(enableTestingEndpoints, out bool isEnabled) ||
            !isEnabled)
        {
            context.Result = new NotFoundResult();
            return;
        }

        // 3. Verificar el token de integración
        var expectedToken = configuration["IntegrationTestSeedToken"];
        if (string.IsNullOrEmpty(expectedToken))
        {
            context.Result = new BadRequestObjectResult(new { error = "Integration test token not configured" });
            return;
        }

        // Obtener el token del parámetro de la acción
        if (!context.ActionArguments.TryGetValue(_tokenParameterName, out var tokenValue) ||
            tokenValue?.ToString() != expectedToken)
        {
            context.Result = new BadRequestObjectResult(new { error = "Invalid token for endpoint" });
            return;
        }

        base.OnActionExecuting(context);
    }
}
