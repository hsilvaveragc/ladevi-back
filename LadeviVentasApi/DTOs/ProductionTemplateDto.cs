namespace LadeviVentasApi.DTOs
{
    public class ProductionTemplateDto
    {
        public long id { get; set; }
        public long productEditionId { get; set; }
        public int pageNumber { get; set; }
        public List<ProductionSlotDto> productionSlots { get; set; } = new List<ProductionSlotDto>();
    }
}