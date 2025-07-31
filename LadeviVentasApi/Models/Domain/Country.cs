using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class Country : BaseEntity
    {
        [Required] public string Name { get; set; }
        public string CodigoTelefonico { get; set; }
        public string? XubioCode { get; set; }
        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser);
        }
    }
}
