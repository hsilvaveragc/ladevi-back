using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;

namespace LadeviVentasApi.Models.Domain
{
    public class ProductEdition : BaseEntity
    {
        [Required, MinLength(2), MaxLength(20)] public string Code { get; set; }
        [Required, MinLength(2), MaxLength(200)] public string Name { get; set; }
        public Product? Product { get; set; }
        public IList<PublishingOrder>? PublishingOrders { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(Product))] public long ProductId { get; set; }
        [Required] public DateTime Start { get; set; }
        public bool Closed { get; set; }
        [Required] public DateTime End { get; set; }
        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                    .CheckUnique(this, context, x => x.Name.ToLower() == Name.ToLower() && x.ProductId == ProductId, memberNames: new[] { nameof(Name) })
                    .CheckUnique(this, context, x => x.Code.ToLower().Trim() == Code.ToLower().Trim(), memberNames: new[] { nameof(Code) }, msg: "Ya existe una edición con ese código.")
                ;
        }
    }
}