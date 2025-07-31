namespace LadeviVentasApi.DTOs;

public class PendientContractDto
{
    public long ClienteId { get; set; }
    public string Client { get; set; }
    public long SpaceTypeId { get; set; }
    public string SpaceType { get; set; }
    public string Numero { get; set; }
    public string Contrato { get; set; }
    public double SelledQuantity { get; set; }
    public double Balance { get; set; }
    public double Amount { get; set; }
    public double PendientAmount { get; set; }
    public string Invoice { get; set; }
    public long SoldSpaceId { get; set; }
    public double Quantity { get; set; }
    public double SubTotal { get; set; }
    public double Descuentos { get; set; }
    public string Moneda { get; set; }
    public string BillingCondition { get; set; }
}