namespace LadeviVentasApi.DTOs;

public class OrdersGroupBySeller
{
    public long SellerId { get; set; }
    public string Seller { get; set; }
    public TotalsDto Totals { get; set; }
}