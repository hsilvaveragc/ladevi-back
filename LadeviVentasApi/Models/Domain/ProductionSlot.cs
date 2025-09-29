using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class ProductionSlot : BaseEntity
    {
        [Required, FkCheck(TypeToCheck = typeof(ProductionTemplate))]
        public long ProductionTemplateId { get; set; }

        public virtual ProductionTemplate? ProductionTemplate { get; set; }

        [Required]
        public int SlotNumber { get; set; }

        [FkCheck(TypeToCheck = typeof(InventoryAdvertisingSpace))]
        public long? InventoryAdvertisingSpaceId { get; set; }

        public virtual InventoryAdvertisingSpace? InventoryAdvertisingSpace { get; set; }

        [FkCheck(TypeToCheck = typeof(PublishingOrder))]
        public long? PublishingOrderId { get; set; }

        public virtual PublishingOrder? PublishingOrder { get; set; }

        public string? Observations { get; set; }

        public bool IsEditorial { get; set; }

        public bool IsCA { get; set; }
    }
}