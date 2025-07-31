using LadeviVentasApi.Data;
using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class Currency : BaseEntity
    {
        public string Name { get; set; }
        [Required] public long CountryId { get; set; }
        [Required] public bool UseEuro { get; set; }
        public ICollection<CurrencyParity>? CurrencyParities { get; set; }

        public const string ARS = "AR$";
        public const string USS = "U$S";
        public const string CHL = "CHL $";
        public const string COL = "COL $";
        public const string PE = "PE $";
        public const string MEX = "MEX $";

        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> user)
        {
            CurrencyParities?.ToList().ForEach(x => x.CurrencyId = Id);

            if (this.UseEuro)
            {
                return base.PerformValidate(validationContext, context, user)
                    .Check(() => ValidateCountry(context))
                    .Check(ValidateCurrencyParities);
            }
            else
            {
                return base.PerformValidate(validationContext, context, user)
                    .CheckUnique(this, context, x => x.Name.ToLower() == Name.ToLower(), memberNames: new[] { nameof(Name) })
                    .Check(() => ValidateCountry(context))
                    .Check(ValidateCurrencyParities);
            }

        }

        public ValidationResult ValidateCountry(ApplicationDbContext context)
        {
            if (context.Currency.Any(x => x.CountryId == CountryId && x.Id != Id && (!x.Deleted.HasValue || !x.Deleted.Value)))
            {
                return new ValidationResult("Ya existe una moneda para este pais",
                    new[] { nameof(CountryId) });
            }
            return ValidationResult.Success;
        }

        public ValidationResult ValidateCurrencyParities()
        {
            var ranges = CurrencyParities.Select(x => new Range<DateTime>(x.Start, x.End)).ToList();
            if (Range<DateTime>.HasOverlappings(ranges))
            {
                return new ValidationResult("No puede periodos que se solapen!",
                    new[] { nameof(CurrencyParities) });
            }
            return ValidationResult.Success;
        }
    }
    public class CurrencyParity : BaseEntity
    {
        [Required] public DateTime Start { get; set; }
        [Required] public DateTime End { get; set; }
        [Required] public double LocalCurrencyToDollarExchangeRate { get; set; }
        [Required] public long CurrencyId { get; set; }
        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                    .Check(() => Start.Date < End.Date
                        ? ValidationResult.Success
                        : new ValidationResult("inicio debe ser menor que fin", new[] { nameof(Start), nameof(End) }))
                    .Check(() => LocalCurrencyToDollarExchangeRate > 0
                        ? ValidationResult.Success
                        : new ValidationResult("El tipo de cambio debe ser positivo", new[] { nameof(LocalCurrencyToDollarExchangeRate) }))
                ;
        }
    }
}
