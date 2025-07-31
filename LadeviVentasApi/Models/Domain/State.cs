using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class State : BaseEntity
    {
        [Required] public string Name { get; set; }
        public Country? Country { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(Country))] public long CountryId { get; set; }
        public string? XubioCode { get; set; }
        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                .CheckUnique(this, context, x => x.Name.ToLower() == Name.ToLower() && x.CountryId == CountryId, memberNames: new[] { nameof(Name) });
        }
    }
}
