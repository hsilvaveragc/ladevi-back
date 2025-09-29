namespace LadeviVentasApi.DTOs
{
    public class ProductionPublishiingOrderDto
    {
        public long id { get; set; }
        public long? contractId { get; set; }
        public string contractName { get; set; } = "";
        public string clientName { get; set; } = "";
        public string sellerName { get; set; } = "";
    }
}