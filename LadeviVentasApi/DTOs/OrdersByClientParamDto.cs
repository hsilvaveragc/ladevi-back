namespace LadeviVentasApi.DTOs;

public class OrdersByClientParamDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public long SellerId { get; set; }
    public long ProductType { get; set; }
    public long Product { get; set; }
    public long Edition { get; set; }
    public long Client { get; set; }
}