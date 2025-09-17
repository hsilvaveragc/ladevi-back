using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class ProductionItem : BaseEntity
    {
        [Required]
        public long ProductEditionId { get; set; }

        [Required]
        public int PageNumber { get; set; }

        [Required]
        public int Slot { get; set; }

        [Required, FkCheck(TypeToCheck = typeof(InventoryProductAdvertisingSpace))]
        public long InventoryProductAdvertisingSpaceId { get; set; }

        public InventoryProductAdvertisingSpace? InventoryProductAdvertisingSpace { get; set; }

        [FkCheck(TypeToCheck = typeof(PublishingOrder))]
        public long? PublishingOrderId { get; set; }

        public PublishingOrder? PublishingOrder { get; set; }

        public bool IsEditorial { get; set; }

        public bool IsCA { get; set; }

        public string Observations { get; set; }
    }
}