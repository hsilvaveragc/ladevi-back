using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class TaxCategory : BaseEntity
    {
        [Required] public string Code { get; set; }
        [Required] public string Name { get; set; }
        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return new List<ValidationResult>();
        }
    }
}
