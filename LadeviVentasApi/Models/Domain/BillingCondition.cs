using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class BillingCondition : BaseEntity
    {
        [Required] public string Name { get; set; }
        public bool Deleted { get; set; }

        public const string Anticipated = "Anticipado";
        public const string AgainstPublication = "Contra publicación";
        public const string Exchange = "Canje";
        public const string NoFee = "Sin cargo";

        public bool IsAnticipated() => Name.Equals(Anticipated);
        public bool IsAgainstPublication() => Name.Equals(AgainstPublication);

        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                .CheckUnique(this, context, x => x.Name.ToLower() == Name.ToLower(), memberNames: new[] { nameof(Name) });
        }
    }
}
