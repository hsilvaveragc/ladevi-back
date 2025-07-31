namespace LadeviVentasApi.Models;

using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Extensiones para el sistema de validaciones de BaseEntity.
/// Proporciona métodos fluidos para construir validaciones complejas con mapeo específico de campos.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Agrega una validación manual a la lista de resultados de validación.
    /// </summary>
    /// <param name="v">Lista de validaciones existente</param>
    /// <param name="msg">Mensaje de error a mostrar</param>
    /// <param name="memberNames">Nombres de los campos asociados al error (opcional)</param>
    /// <returns>La misma lista para permitir chaining</returns>
    /// <example>
    /// validations.AddValidation("Error personalizado", new[] { "CampoEspecifico" })
    /// </example>
    public static IList<ValidationResult> AddValidation(this IList<ValidationResult> v, string msg, IEnumerable<string> memberNames = null)
    {
        v.Add(new ValidationResult(msg, memberNames));
        return v;
    }

    /// <summary>
    /// Verifica que una propiedad sea única en la base de datos para la entidad actual.
    /// Excluye automáticamente el registro actual (por ID) de la verificación.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad que hereda de BaseEntity</typeparam>
    /// <param name="v">Lista de validaciones existente</param>
    /// <param name="x">Instancia actual de la entidad</param>
    /// <param name="context">Contexto de la base de datos</param>
    /// <param name="compare">Expresión lambda para comparar duplicados</param>
    /// <param name="msg">Mensaje personalizado (opcional, se genera automáticamente si no se provee)</param>
    /// <param name="memberNames">Campos asociados al error</param>
    /// <returns>La misma lista para permitir chaining</returns>
    /// <example>
    /// // Verificar que el email sea único
    /// validations.CheckUnique(this, context, x => x.Email.ToLower() == Email.ToLower(), 
    ///     "Ya existe un cliente con este email", new[] { nameof(Email) })
    /// </example>
    public static IList<ValidationResult> CheckUnique<T>(this IList<ValidationResult> v,
        T x, DbContext context, Expression<Func<T, bool>> compare, string msg = null, IEnumerable<string> memberNames = null)
     where T : BaseEntity
    {
        if (context.Set<T>().Where(o => o.Id != x.Id).Any(compare))
        {
            v.Add(new ValidationResult(msg ?? $"Unicidad no respetada para entidad de tipo {typeof(T).Name}", memberNames));
        }
        return v;
    }

    /// <summary>
    /// Ejecuta una validación personalizada que puede lanzar ValidationException.
    /// Captura automáticamente las excepciones y las convierte en ValidationResult.
    /// </summary>
    /// <param name="v">Lista de validaciones existente</param>
    /// <param name="validator">Función que retorna ValidationResult o lanza ValidationException</param>
    /// <returns>La misma lista para permitir chaining</returns>
    /// <example>
    /// validations.Check(() => ValidateBusinessLogic(context, user))
    /// </example>
    public static IList<ValidationResult> Check(this IList<ValidationResult> v, Func<ValidationResult> validator)
    {
        try
        {
            var result = validator.Invoke();
            if (result != null && result != ValidationResult.Success) v.Add(result);
        }
        catch (ValidationException e)
        {
            v.Add(new ValidationResult(e.Message, e.Fields));
        }
        return v;
    }

    /// <summary>
    /// Valida una regla de negocio simple basada en una condición booleana.
    /// Si la condición es verdadera, agrega el error a la lista.
    /// </summary>
    /// <param name="v">Lista de validaciones existente</param>
    /// <param name="condition">Condición que indica si hay error (true = error, false = válido)</param>
    /// <param name="message">Mensaje de error a mostrar</param>
    /// <param name="fieldName">Nombre del campo asociado al error (opcional)</param>
    /// <returns>La misma lista para permitir chaining</returns>
    /// <example>
    /// // Validar que el precio no sea negativo
    /// validations.CheckBusinessRule(Price < 0, "El precio no puede ser negativo", nameof(Price))
    /// </example>
    public static IList<ValidationResult> CheckBusinessRule(this IList<ValidationResult> v,
        bool condition, string message, string fieldName = null)
    {
        if (condition)
        {
            var memberNames = !string.IsNullOrEmpty(fieldName) ? new[] { fieldName } : null;
            v.Add(new ValidationResult(message, memberNames));
        }
        return v;
    }

    /// <summary>
    /// Ejecuta una validación asíncrona y espera su resultado de forma síncrona.
    /// Útil para validaciones que requieren llamadas a servicios externos.
    /// ADVERTENCIA: Usar con cuidado para evitar deadlocks en contextos síncronos.
    /// </summary>
    /// <param name="v">Lista de validaciones existente</param>
    /// <param name="asyncValidator">Función asíncrona que retorna ValidationResult</param>
    /// <returns>La misma lista para permitir chaining</returns>
    /// <example>
    /// // Validar punto de venta en servicio externo
    /// validations.CheckAsync(() => ValidatePointOfSaleInXubio(pointOfSale))
    /// </example>
    public static IList<ValidationResult> CheckAsync(this IList<ValidationResult> v,
        Func<Task<ValidationResult>> asyncValidator)
    {
        try
        {
            var result = asyncValidator.Invoke().GetAwaiter().GetResult();
            if (result != null && result != ValidationResult.Success) v.Add(result);
        }
        catch (ValidationException e)
        {
            v.Add(new ValidationResult(e.Message, e.Fields));
        }
        return v;
    }

    /// <summary>
    /// Método helper para lanzar ValidationException de forma condicional.
    /// Útil para validaciones en servicios o controladores fuera del contexto de BaseEntity.
    /// </summary>
    /// <param name="condition">Condición que indica si hay error (true = lanza excepción)</param>
    /// <param name="message">Mensaje de error</param>
    /// <param name="fieldName">Nombre del campo asociado al error (opcional)</param>
    /// <exception cref="ValidationException">Se lanza si la condición es true</exception>
    /// <example>
    /// // En un servicio o controlador
    /// ValidationExtensions.ThrowIfInvalid(pointOfSale == null, 
    ///     "El punto de venta no existe en Xubio", "billingPointOfSale");
    /// </example>
    public static void ThrowIfInvalid(bool condition, string message, string fieldName = null)
    {
        if (condition)
        {
            var fields = !string.IsNullOrEmpty(fieldName) ? new[] { fieldName } : new string[0];
            throw new ValidationException(message, fields);
        }
    }

    /// <summary>
    /// Método helper para lanzar ValidationException para múltiples campos.
    /// Útil cuando un error afecta a varios campos simultáneamente.
    /// </summary>
    /// <param name="condition">Condición que indica si hay error (true = lanza excepción)</param>
    /// <param name="message">Mensaje de error</param>
    /// <param name="fieldNames">Array de nombres de campos asociados al error</param>
    /// <exception cref="ValidationException">Se lanza si la condición es true</exception>
    /// <example>
    /// ValidationExtensions.ThrowIfInvalid(
    ///     startDate > endDate, 
    ///     "La fecha de inicio debe ser anterior a la fecha de fin",
    ///     new[] { "startDate", "endDate" });
    /// </example>
    public static void ThrowIfInvalid(bool condition, string message, string[] fieldNames)
    {
        if (condition)
        {
            throw new ValidationException(message, fieldNames ?? new string[0]);
        }
    }

    /// <summary>
    /// Excepción personalizada para errores de validación de negocio.
    /// Permite asociar el error a campos específicos para su mapeo en el frontend.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Array de nombres de campos asociados a este error.
        /// Se usa para mapear errores a campos específicos en formularios del frontend.
        /// </summary>
        public string[] Fields { get; }

        /// <summary>
        /// Constructor básico con mensaje de error.
        /// El error se considerará general (sin campo específico).
        /// </summary>
        /// <param name="msg">Mensaje de error</param>
        public ValidationException(string msg) : base(msg)
        {
            Fields = new string[0];
        }

        /// <summary>
        /// Constructor con mensaje y campos específicos.
        /// </summary>
        /// <param name="msg">Mensaje de error</param>
        /// <param name="fields">Array de nombres de campos asociados al error</param>
        public ValidationException(string msg, string[] fields) : base(msg)
        {
            Fields = fields ?? new string[0];
        }

        /// <summary>
        /// Constructor con mensaje, campos e inner exception.
        /// </summary>
        /// <param name="msg">Mensaje de error</param>
        /// <param name="fields">Array de nombres de campos asociados al error</param>
        /// <param name="innerException">Excepción interna que causó este error</param>
        public ValidationException(string msg, string[] fields, Exception innerException) : base(msg, innerException)
        {
            Fields = fields ?? new string[0];
        }
    }
}
