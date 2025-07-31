namespace LadeviVentasApi.Services.Xubio.DTOs;

public class ClienteRequest
{
    public long Id { get; set; }
}

public class PuntoVentaRequest
{
    public string Codigo { get; set; }
}

public class MonedaRequest
{
    public string Codigo { get; set; }
}

public class ProductoRequest
{
    public string Codigo { get; set; }
}

public class TransaccionProductoItemRequest
{
    public ProductoRequest Producto { get; set; }
    public string Descripcion { get; set; }
    public decimal Cantidad { get; set; }
    public decimal Precio { get; set; }
}

public class ReceiptDtoRequest
{
    public long Tipo { get; set; }
    public ClienteRequest Cliente { get; set; }
    public PuntoVentaRequest PuntoVenta { get; set; }
    public string Fecha { get; set; }
    public string FechaVto { get; set; }
    public long CondicionDePago { get; set; }
    public MonedaRequest Moneda { get; set; }
    public string Descripcion { get; set; }
    public string? NumeroDocumento { get; set; }
    public List<TransaccionProductoItemRequest> TransaccionProductoItems { get; set; }
}