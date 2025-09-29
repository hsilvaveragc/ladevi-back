using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class AdversitingSpaceGroup : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required, FkCheck(TypeToCheck = typeof(Product))]
        public long ProductId { get; set; }

        public Product? Product { get; set; }
    }
}