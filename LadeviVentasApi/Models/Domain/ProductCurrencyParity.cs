using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class ProductCurrencyParity : BaseEntity
    {
        [Required] public DateTime Start { get; set; }
        [Required] public DateTime End { get; set; }
        [Required] public double LocalCurrencyToDollarExchangeRate { get; set; }
        [Required] public long ProductId { get; set; }
        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                    .Check(() => Start.Date < End.Date
                        ? ValidationResult.Success
                        : new ValidationResult("inicio debe ser menor que fin", new[] { nameof(Start), nameof(End) }))
                    .Check(() => LocalCurrencyToDollarExchangeRate > 0
                        ? ValidationResult.Success
                        : new ValidationResult("El tipo de cambio debe ser positivo", new[] { nameof(LocalCurrencyToDollarExchangeRate) }))
                ;
        }
    }
}