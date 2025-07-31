namespace LadeviVentasApi.Services.Xubio.DTOs;


public class ReceiptDtoResponse
{
    public string NumeroDocumento { get; set; }
    public long Transaccionid { get; set; }
    public ErrorResponse Error { get; set; }
}