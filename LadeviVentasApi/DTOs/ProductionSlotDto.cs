namespace LadeviVentasApi.DTOs
{
    public class ProductionSlotDto
    {
        public long id { get; set; }
        public long productionTemplateId { get; set; }
        public int slotNumber { get; set; }
        public long? inventoryAdvertisingSpaceId { get; set; }
        public string productAdvertisingSpaceName { get; set; } = "";
        public ProductionPublishiingOrderDto order { get; set; }
        public string observations { get; set; } = "";
        public bool isEditorial { get; set; }
        public bool isCA { get; set; }
    }
}