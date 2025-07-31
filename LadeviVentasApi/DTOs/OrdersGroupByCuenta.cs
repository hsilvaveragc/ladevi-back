namespace LadeviVentasApi.DTOs;

public class OrdersGroupByCuenta
{
    public double IVA { get; set; }
    public string Currency { get; set; }
    public TotalsDto Totals { get; set; }
}