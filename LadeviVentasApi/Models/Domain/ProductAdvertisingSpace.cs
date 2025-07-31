using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class ProductAdvertisingSpace : BaseEntity
    {
        [Required, MinLength(2)] public string Name { get; set; }
        public Product? Product { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(Product))] public long ProductId { get; set; }
        [Required] public double DollarPrice { get; set; }
        [Required] public double Height { get; set; }
        [Required] public double Width { get; set; }
        public bool Show { get; set; }
        public double DiscountForCheck { get; set; }
        public double DiscountForLoyalty { get; set; }
        public double DiscountForSameCountry { get; set; }
        public double DiscountForOtherCountry { get; set; }
        public double DiscountForAgency { get; set; }
        public ICollection<ProductAdvertisingSpaceVolumeDiscount> ProductAdvertisingSpaceVolumeDiscounts { get; set; }
        public ICollection<ProductAdvertisingSpaceLocationDiscount> ProductAdvertisingSpaceLocationDiscounts { get; set; }
        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            ProductAdvertisingSpaceVolumeDiscounts?.ToList().ForEach(x => x.ProductAdvertisingSpaceId = Id);
            ProductAdvertisingSpaceLocationDiscounts?.ToList().ForEach(x => x.ProductAdvertisingSpaceId = Id);
            return base.PerformValidate(validationContext, context, applicationUser)
                    .CheckUnique(this, context, x => x.Name.ToLower() == Name.ToLower() && x.ProductId == ProductId, memberNames: new[] { nameof(Name) })
                    .Check(() => Height > 0 ? ValidationResult.Success : new ValidationResult("Debe ser mayor a 0", new[] { nameof(Height) }))
                    .Check(() => Width > 0 ? ValidationResult.Success : new ValidationResult("Debe ser mayor a 0", new[] { nameof(Width) }))
                    .Check(() => DollarPrice > 0 ? ValidationResult.Success : new ValidationResult("Debe ser mayor a 0", new[] { nameof(DollarPrice) }))
                    .Check(ValidateVolumeDiscounts)
                    .Check(ValidateProductLocationDiscounts);
        }

        public ValidationResult ValidateProductLocationDiscounts()
        {
            if (ProductAdvertisingSpaceLocationDiscounts.GroupBy(x => x.AdvertisingSpaceLocationTypeId).Any(g => g.Count() > 1))
            {
                return new ValidationResult("No puede haber descuentos por ubicacion repetidos!",
                    new[] { nameof(ProductAdvertisingSpaceLocationDiscounts) });
            }
            return ValidationResult.Success;
        }

        public ValidationResult ValidateVolumeDiscounts()
        {
            if (ProductAdvertisingSpaceVolumeDiscounts.GroupBy(x => x.RangeStart).Any(g => g.Count() > 1))
            {
                return new ValidationResult("No puede haber descuentos de volumen repetidos!",
                    new[] { nameof(ProductAdvertisingSpaceLocationDiscounts) });
            }
            return ValidationResult.Success;
        }
    }
}