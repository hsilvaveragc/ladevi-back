using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class InventoryProductAdvertisingSpace : BaseEntity
    {
        [Required]
        public long ProductEditionId { get; set; }

        public ProductEdition? ProductEdition { get; set; }

        [Required, FkCheck(TypeToCheck = typeof(ProductAdvertisingSpace))]
        public long ProductAdvertisingSpaceId { get; set; }

        public ProductAdvertisingSpace? ProductAdvertisingSpace { get; set; }

        [Required]
        public int? Quantity { get; set; }

    }
}