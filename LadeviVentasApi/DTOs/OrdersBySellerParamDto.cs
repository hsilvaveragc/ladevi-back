namespace LadeviVentasApi.DTOs;

public class OrdersBySellerParamDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public long SelledId { get; set; }
    public long ProductType { get; set; }
    public long Product { get; set; }
    public long ProductEdition { get; set; }
    public bool NoComisionarImpagas { get; set; }
}