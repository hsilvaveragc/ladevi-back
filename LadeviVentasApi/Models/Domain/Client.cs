using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;
using LadeviVentasApi.Helpers;
using LadeviVentasApi.Services.Xubio;

namespace LadeviVentasApi.Models.Domain
{
    public class Client : BaseEntity
    {
        [Required, MinLength(3), MaxLength(100)]
        public string BrandName { get; set; }

        [Required, MinLength(3), MaxLength(100)]
        public string LegalName { get; set; }

        public bool IsEnabled { get; set; }

        [Required, MinLength(3), MaxLength(200)]
        public string Address { get; set; }

        [Required, MaxLength(10)]
        public string PostalCode { get; set; }

        public City? City { get; set; }
        [FkCheck(TypeToCheck = typeof(City))] public long? CityId { get; set; }
        public District? District { get; set; }
        [FkCheck(TypeToCheck = typeof(District))] public long? DistrictId { get; set; }
        public State? State { get; set; }
        [FkCheck(TypeToCheck = typeof(State))] public long StateId { get; set; }
        public Country? Country { get; set; }
        [FkCheck(TypeToCheck = typeof(Country))] public long CountryId { get; set; }


        [Required, MaxLength(5)]
        public string TelephoneCountryCode { get; set; }
        [MaxLength(5)]
        public string TelephoneAreaCode { get; set; }
        [Required, MinLength(5), MaxLength(12)]
        public string TelephoneNumber { get; set; }

        [Required]
        public bool IsAgency { get; set; }
        [Required]
        public bool IsComtur { get; set; }

        [Required, EmailAddress]
        public string MainEmail { get; set; }

        [OptionalEmail]
        public string? AlternativeEmail { get; set; }

        public ApplicationUser? ApplicationUserDebtCollector { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(ApplicationUser))] public long ApplicationUserDebtCollectorId { get; set; }
        public ApplicationUser? ApplicationUserSeller { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(ApplicationUser))] public long ApplicationUserSellerId { get; set; }

        [Required]
        public string BillingPointOfSale { get; set; }
        [Required]
        public bool ElectronicBillByMail { get; set; }
        [Required]
        public bool ElectronicBillByPaper { get; set; }
        public TaxType? IdentificationType { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(TaxType))] public long TaxTypeId { get; set; }
        [Required]
        public string IdentificationValue { get; set; }
        [Required]
        public double TaxPercentage { get; set; }
        public string Contact { get; set; }

        public long? XubioId { get; set; }

        public bool? IsBigCompany { get; set; }

        public TaxCategory? TaxCategory { get; set; }
        [FkCheck(TypeToCheck = typeof(TaxCategory))] public long? TaxCategoryId { get; set; }

        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                .Check(() => ClientAuthorizer(context, applicationUser.Value))
                .CheckUnique(this, context, x => x.BrandName.ToLower() == BrandName.ToLower(), memberNames: new[] { nameof(BrandName) })
                .CheckUnique(this, context, x => x.LegalName.ToLower() == LegalName.ToLower(), memberNames: new[] { nameof(LegalName) })
                .CheckUnique(this, context, x => x.XubioId.HasValue && x.XubioId == XubioId,
                    msg: $"Ya existe otro cliente en el sistema vinculado al mismo registro de Xubio (ID: {XubioId}). " +
                        $"Verifique que no esté intentando crear un cliente duplicado.",
                    memberNames: new[] { nameof(XubioId), nameof(IdentificationValue) })
                .CheckAsync(() => ValidateXubioPointOfSale(validationContext))
                ;
        }

        private async Task<ValidationResult> ValidateXubioPointOfSale(ValidationContext validationContext)
        {
            try
            {
                var xubioService = validationContext.GetService(typeof(XubioService)) as XubioService;
                if (xubioService != null && !string.IsNullOrEmpty(BillingPointOfSale))
                {
                    var pointsOfSale = await xubioService.GetPointOfSaleAsync(IsComtur);
                    var exists = pointsOfSale.Any(ps => ps.PuntoVenta == BillingPointOfSale.PadLeft(5, '0'));

                    if (!exists)
                    {
                        return new ValidationResult("El punto de venta no existe en Xubio",
                            new[] { nameof(BillingPointOfSale) });
                    }
                }
                return ValidationResult.Success;
            }
            catch (Exception)
            {
                // Si falla la validación externa, permitir continuar pero log el error
                return ValidationResult.Success;
            }
        }

        private ValidationResult ClientAuthorizer(ApplicationDbContext context, ApplicationUser applicationUser)
        {
            if (applicationUser.ApplicationRole.IsSeller() && Id != 0)
            {
                var currentClient = context.Clients.Single(c => c.Id == Id);
                if (currentClient.ApplicationUserSellerId != applicationUser.Id)
                {
                    throw new ValidationExtensions.ValidationException(
                        "Siendo vendedor no puede editar un cliente que no este asignado a usted mismo",
                        new[] { nameof(ApplicationUserSellerId) });
                }
            }

            if (applicationUser.ApplicationRole.IsSeller())
            {
                if (applicationUser.Id != ApplicationUserSellerId)
                {
                    throw new ValidationExtensions.ValidationException(
                        "Siendo vendedor el vendedor asignado tiene que ser usted mismo",
                        new[] { nameof(ApplicationUserSellerId) });
                }

                if (applicationUser.Id != ApplicationUserDebtCollectorId)
                {
                    throw new ValidationExtensions.ValidationException(
                        "Siendo vendedor el cobrador asignado tiene que ser usted mismo",
                        new[] { nameof(ApplicationUserDebtCollectorId) });
                }
            }

            if (applicationUser.ApplicationRole.IsNationalSeller() && applicationUser.CountryId != CountryId)
            {
                throw new ValidationExtensions.ValidationException(
                    "No coincide el pais autorizado para el usuario con la ciudad elegida",
                    new[] { nameof(CountryId) });
            }

            return ValidationResult.Success;
        }
    }
}
