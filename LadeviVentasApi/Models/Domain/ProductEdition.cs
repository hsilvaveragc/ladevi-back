using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class ProductEdition : BaseEntity
    {
        [Required, MinLength(2), MaxLength(20)]
        public string Code { get; set; }

        [Required, MinLength(2), MaxLength(200)]
        public string Name { get; set; }

        [Required, FkCheck(TypeToCheck = typeof(Product))]
        public long ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        public bool Closed { get; set; }

        public int? PageCount { get; set; }

        public IList<PublishingOrder>? PublishingOrders { get; set; }

        public IList<InventoryAdvertisingSpace>? InventoryAdvertisingSpaces { get; set; }

        public virtual ICollection<ProductionTemplate> ProductionTemplates { get; set; } = new List<ProductionTemplate>();

        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                    .CheckUnique(this, context, x => x.Name.ToLower() == Name.ToLower() && x.ProductId == ProductId, memberNames: new[] { nameof(Name) })
                    .CheckUnique(this, context, x => x.Code.ToLower().Trim() == Code.ToLower().Trim(), memberNames: new[] { nameof(Code) }, msg: "Ya existe una edición con ese código.")
                    .Check(() => ValidateIfTheProductsAdvertisingSpacesAreAssignedToTheSameProductEdition(validationContext, context));
            ;
        }

        public ValidationResult ValidateIfTheProductsAdvertisingSpacesAreAssignedToTheSameProductEdition(ValidationContext validationContext, ApplicationDbContext context)
        {
            // var productAdvertisingSpacesIds = ProductAdvertisingSpaces.Select(x => x.Id).ToList();
            // var productAdvertisingSpaces = context.ProductAdvertisingSpaces.Where(x => productAdvertisingSpacesIds.Contains(x.Id)).ToList();
            // if (productAdvertisingSpaces.Any(x => x.ProductEditionId != Id))
            // {
            //     return new ValidationResult("Todos los espacios publicitarios deben pertenecer a esta edición del producto.",
            //         new[] { nameof(ProductAdvertisingSpaces) });
            // }
            return ValidationResult.Success;
        }
    }
}