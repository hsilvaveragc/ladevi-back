using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class TaxType : BaseEntity
    {
        [Required] public string Name { get; set; }
        public bool IsIdentificationField { get; set; }
        public long Order { get; set; }
        public string OptionsInternal { get; set; }
        [NotMapped]
        public string[] Options
        {
            get => OptionsInternal?.Split('|', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            set => OptionsInternal = string.Join("|", (value ?? new string[0]).Select(x => x.Replace("|", "")));
        }
        public Country Country { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(Country))] public long CountryId { get; set; }
        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                .CheckUnique(this, context, x => x.Name.ToLower() == Name.ToLower() && x.CountryId == CountryId, memberNames: new[] { nameof(Name) });
        }

        public bool IsValidForClient(Client client, string value)
        {
            return !Options.Any() || Options.Any(o => o.ToLowerInvariant().Equals((value ?? "").ToLowerInvariant()))
                && client.City.District.State.CountryId == CountryId;
        }
    }
}
