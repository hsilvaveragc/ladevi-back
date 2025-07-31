using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class AdvertisingSpaceLocationType : BaseEntity
    {
        [Required, MinLength(2)] public string Name { get; set; }

        public const string BeforeCentral = "Antes de central/Oro";
        public const string AfterCentral = "Después de central/Bronce";
        public const string RotaryLocation = "Ubicación rotativa";
        public const string SilverLocation = "Plata";


        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                .CheckUnique(this, context, x => x.Name.ToLower() == Name.ToLower(), memberNames: new[] { nameof(Name) });
        }
    }
}