using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class ProductVolumeDiscount : BaseEntity
    {
        [Required] public long RangeStart { get; set; }
        [Required] public long RangeEnd { get; set; }
        [Required] public double Discount { get; set; }
        [Required] public long ProductId { get; set; }
        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                .Check(() => RangeStart < RangeEnd
                    ? ValidationResult.Success
                    : new ValidationResult("inicio debe ser menor que fin", new[] { nameof(RangeStart), nameof(RangeEnd) }))
                ;
        }
    }
}