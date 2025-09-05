using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Helpers.Attributes;

/// <summary>
/// Atributo de validación personalizado para campos ID que deben ser mayor a 0.
/// Soluciona el problema de que [Required] considera válido el valor 0 para tipos long/int.
/// </summary>
/// <example>
/// <code>
/// [RequiredId]
/// public long ApplicationRoleId { get; set; }
/// 
/// [RequiredId, FkCheck(TypeToCheck = typeof(Country))]
/// public long CountryId { get; set; }
/// </code>
/// </example>
public class RequiredIdAttribute : ValidationAttribute
{
    /// <summary>
    /// Valida que el valor sea un ID válido (mayor a 0).
    /// </summary>
    /// <param name="value">El valor a validar</param>
    /// <param name="validationContext">Contexto de validación que contiene información sobre el campo</param>
    /// <returns>ValidationResult.Success si es válido, o un ValidationResult con error si no lo es</returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is long longValue && longValue == 0)
        {
            return new ValidationResult(
                $"The {validationContext.DisplayName} field is required.",
                new[] { validationContext.MemberName }
            );
        }

        return ValidationResult.Success;
    }
}