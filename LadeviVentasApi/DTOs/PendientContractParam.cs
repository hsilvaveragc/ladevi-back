namespace LadeviVentasApi.DTOs;

public class PendientContractParam
{
    public DateTime? Date { get; set; }
    public long SellerId { get; set; }
    public long ClienteId { get; set; }
    public bool OnlyWithBalance { get; set; }
}