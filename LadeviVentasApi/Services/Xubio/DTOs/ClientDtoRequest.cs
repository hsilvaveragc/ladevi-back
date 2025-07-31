using System.Text.Json.Serialization;

namespace LadeviVentasApi.Services.Xubio.DTOs;

public class IdentificacionTributariaRequest
{
    public string Codigo { get; set; }
}
public class CategoriaFiscalRequest
{
    public string Codigo { get; set; }
}

public class PaisRequest
{
    public string Codigo { get; set; }
}

public class ProvinciaRequest
{
    public string Codigo { get; set; }
}

public class LocalidadRequest
{
    public string Codigo { get; set; }
}

public class CuentaVentaRequest
{
    public string Codigo { get; set; }
}

public class CuentaCompraRequest
{
    public string Codigo { get; set; }
}


public class ClientDtoRequest
{
    [JsonPropertyName("cliente_id")]
    public long ClientId { get; set; }
    public string Nombre { get; set; }
    public string PrumerApellido => string.Empty;//La api lo rquiere pero xubio no lo usa
    public string SegundoApellido => string.Empty; //La api lo rquiere pero xubio no lo usa
    public string PrimerNombre => string.Empty; //La api lo rquiere pero xubio no lo usa
    public string OtrosNombres => string.Empty; //La api lo rquiere pero xubio no lo usa  
    public string RazonSocial => Nombre;//La api lo rquiere pero xubio no lo usa   
    public string NombreComercial => string.Empty; //La api lo rquiere pero xubio no lo usa  
    public IdentificacionTributariaRequest IdentificacionTributaria { get; set; }
    public string DigitoVerificacion => string.Empty; //La api lo rquiere pero xubio no lo usa  
    public CategoriaFiscalRequest CategoriaFiscal { get; set; }
    public ProvinciaRequest Provincia { get; set; }
    public string Direccion { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }
    public string CodigoPostal => string.Empty; //La api lo rquiere pero xubio no lo usa  
    [JsonPropertyName("cuentaVenta_id")]
    public CuentaVentaRequest CuentaVenta => new CuentaVentaRequest { Codigo = "DEUDORES_POR_VENTA" };
    [JsonPropertyName("cuentaCompra_id")]
    public CuentaCompraRequest CuentaCompra => new CuentaCompraRequest { Codigo = "PROVEEDORES" };
    public PaisRequest Pais { get; set; }
    public string UsrCode { get; set; }
    public string Descripcion => string.Empty; //La api lo rquiere pero xubio no lo usa  
    public int Esclienteextranjero => 0;
    public int EsProovedor => 0;
    public string Cuit { get; set; }
    public LocalidadRequest Localidad { get; set; }
}