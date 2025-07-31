namespace LadeviVentasApi.Helpers;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class OptionalEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return ValidationResult.Success;

        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return emailRegex.IsMatch(value.ToString() ?? string.Empty)
            ? ValidationResult.Success
            : new ValidationResult("Email inválido");
    }
}