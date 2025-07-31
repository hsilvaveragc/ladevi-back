using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class District : BaseEntity
    {
        [Required] public string Name { get; set; }
        public State? State { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(State))] public long StateId { get; set; }
        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                .CheckUnique(this, context, x => x.Name.ToLower() == Name.ToLower() && x.StateId == StateId, memberNames: new[] { nameof(Name) });
        }
    }
}
