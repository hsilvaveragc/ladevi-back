namespace LadeviVentasApi.DTOs
{
    public class ProductionSlotDto
    {
        public long id { get; set; }
        public long productionTemplateId { get; set; }
        public int slotNumber { get; set; }
        public long? inventoryAdvertisingSpaceId { get; set; }
        public string productAdvertisingSpaceName { get; set; } = "";
        public long? publishingOrderId { get; set; }
        public long? contractId { get; set; }
        public string contractName { get; set; } = "";
        public string clientName { get; set; } = "";
        public string sellerName { get; set; } = "";
        public string observations { get; set; } = "";
        public bool isEditorial { get; set; }
        public bool isCA { get; set; }
    }
}