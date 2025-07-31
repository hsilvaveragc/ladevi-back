using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class City : BaseEntity
    {
        [Required] public string Name { get; set; }
        public string CodigoTelefonico { get; set; }
        public District? District { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(District))] public long DistrictId { get; set; }
        public string? XubioCode { get; set; }
        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                .CheckUnique(this, context, x => x.Name.ToLower() == Name.ToLower() && x.DistrictId == DistrictId, memberNames: new[] { nameof(Name) });
        }
    }
}
