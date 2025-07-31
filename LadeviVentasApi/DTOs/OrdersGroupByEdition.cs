namespace LadeviVentasApi.DTOs;

public class OrdersGroupByEdition
{
    public long ProductEditionId { get; set; }
    public string ProductEdition { get; set; }
    public TotalsDto Totals { get; set; }
}