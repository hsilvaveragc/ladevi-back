using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class Product : BaseEntity
    {
        public ProductType ProductType { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(ProductType))] public long ProductTypeId { get; set; }

        public Country Country { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(Country))] public long CountryId { get; set; }

        [Required, MinLength(3), MaxLength(100)]
        public string Name { get; set; }

        public double DiscountForCheck { get; set; }
        public double DiscountForLoyalty { get; set; }
        public double DiscountForAgency { get; set; }
        public double DiscountForSameCountry { get; set; }
        public double DiscountForOtherCountry { get; set; }
        public ICollection<ProductVolumeDiscount> ProductVolumeDiscounts { get; set; }
        public ICollection<ProductLocationDiscount> ProductLocationDiscounts { get; set; }
        public ICollection<ProductAdvertisingSpace> ProductAdvertisingSpaces { get; set; }
        public double DiscountSpecialBySeller { get; set; }
        public double DiscountByManager { get; set; }
        public double MaxAplicableDiscount { get; set; }
        public double AliquotForSalesCommission { get; set; }
        public double IVA { get; set; }
        public string? XubioProductCode { get; set; }
        public string? ComturXubioProductCode { get; set; }


        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            ProductVolumeDiscounts?.ToList().ForEach(x => x.ProductId = Id);
            ProductLocationDiscounts?.ToList().ForEach(x => x.ProductId = Id);
            return base.PerformValidate(validationContext, context, applicationUser)
                .CheckUnique(this, context, x => x.Name.ToLower() == Name.ToLower() && x.CountryId == CountryId, memberNames: new[] { nameof(Name) })
                .Check(ValidateVolumeDiscounts)
                .Check(ValidateProductLocationDiscounts)
                ;
        }

        public ValidationResult ValidateProductLocationDiscounts()
        {
            if (ProductLocationDiscounts.GroupBy(x => x.AdvertisingSpaceLocationTypeId).Any(g => g.Count() > 1))
            {
                return new ValidationResult("No puede haber descuentos por ubicacion repetidos!",
                    new[] { nameof(ProductLocationDiscounts) });
            }
            return ValidationResult.Success;
        }

        public ValidationResult ValidateVolumeDiscounts()
        {
            var ranges = ProductVolumeDiscounts.Select(x => new Range<long>(x.RangeStart, x.RangeEnd)).ToList();
            if (Range<long>.HasOverlappings(ranges))
            {
                return new ValidationResult("No puede rangos de volumen que se solapen!",
                    new[] { nameof(ProductLocationDiscounts) });
            }
            return ValidationResult.Success;
        }
    }
    public class Range<T> where T : IComparable
    {
        private T Min { get; }
        private T Max { get; }

        public Range(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public bool IsOverlapped(Range<T> other)
        {
            return Min.CompareTo(other.Max) < 0 && other.Min.CompareTo(Max) < 0;
        }

        public static bool HasOverlappings(List<Range<T>> ranges)
        {
            return ranges.Any(r1 => ranges.Any(r2 => r1.GetHashCode() != r2.GetHashCode()
                                                     && r1.IsOverlapped(r2)));
        }
    }
}