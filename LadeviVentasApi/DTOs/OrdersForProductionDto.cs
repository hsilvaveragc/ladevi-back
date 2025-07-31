namespace LadeviVentasApi.DTOs;

public class OrdersForProductionDto
{
    public string BrandName { get; set; }
    public string LegalName { get; set; }
    public string SpaceType { get; set; }
    public string SpaceLocation { get; set; }
    public long ContractId { get; set; }
    public string ContractName { get; set; }
    public string PageNumber { get; set; }
    public string Observations { get; set; }
    public string Seller { get; set; }
    public string BxA { get; set; }
    public string Product { get; set; }
    public string ProductEdition { get; set; }
    public long PublishingOrderId { get; set; }
    public DateTime? CreationDate { get; set; }
    public double Quantity { get; set; }
}