using LadeviVentasApi.Data;
using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class PaymentMethod : BaseEntity
    {
        [Required] public string Name { get; set; }
        public bool Deleted { get; set; }

        public const string Documented = "Documented";
        public const string OnePayment = "OnePayment";
        public const string Another = "Another";

        public bool IsDocumented() => Name.Equals(Documented);

        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> user)
        {
            return base.PerformValidate(validationContext, context, user)
                .CheckUnique(this, context, x => x.Name.ToLower() == Name.ToLower(), memberNames: new[] { nameof(Name) });
        }
    }
}
