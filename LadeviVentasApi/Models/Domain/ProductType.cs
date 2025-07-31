using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class ProductType : BaseEntity
    {
        [Required, MinLength(2)] public string Name { get; set; }

        public const string Magazine = "Revista";
        public const string Newsletter = "Newsletter";

        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                .CheckUnique(this, context, x => x.Name.ToLower() == Name.ToLower(), memberNames: new[] { nameof(Name) });
        }
    }
}