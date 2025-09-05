
namespace Tests;

/// <summary>
/// Clase para deserializar respuestas de error de la API.
/// Coincide con el formato devuelto por los controladores: Dictionary&lt;string, string[]&gt;
/// </summary>
public class ErrorsApiResponse
{
    /// <summary>
    /// Diccionario de errores donde la clave es el nombre del campo (en camelCase)
    /// y el valor es un array de mensajes de error para ese campo.
    /// </summary>
    /// <example>
    /// {
    ///   "email": ["El email es requerido", "Formato de email inválido"],
    ///   "password": ["La contraseña debe tener al menos 6 caracteres"]
    /// }
    /// </example>
    public Dictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}
