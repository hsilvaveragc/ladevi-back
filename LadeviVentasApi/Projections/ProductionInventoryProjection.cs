// Projections/ProductionInventoryProjection.cs - Para proyecciones de LINQ
namespace LadeviVentasApi.Projections
{
    public class ProductionInventoryProjection
    {
        public long Id { get; set; }
        public int? PageCount { get; set; }
        public List<InventorySpaceProjection> InventoryProductAdvertisingSpaces { get; set; }
        public bool? Deleted { get; set; }
    }

    public class InventorySpaceProjection
    {
        public long Id { get; set; }
        public long ProductAdvertisingSpaceId { get; set; }
        public int? Quantity { get; set; }
        public string ProductAdvertisingSpaceName { get; set; }
    }
}