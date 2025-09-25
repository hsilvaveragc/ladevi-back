using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class ProductionTemplate : BaseEntity
    {
        [Required, FkCheck(TypeToCheck = typeof(ProductEdition))]
        public long ProductEditionId { get; set; }

        public virtual ProductEdition? ProductEdition { get; set; }

        [Required]
        public int PageNumber { get; set; }

        public virtual ICollection<ProductionSlot> ProductionSlots { get; set; } = new List<ProductionSlot>();
    }
}