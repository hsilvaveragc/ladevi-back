using System.Text.Json.Serialization;

namespace LadeviVentasApi.Services.Xubio.DTOs;

public class PaisReponse
{
    public string Codigo { get; set; }
}

public class CategoriaFiscalResponse
{
    public string Codigo { get; set; }
}

public class ClientDtoResponse
{
    [JsonPropertyName("cliente_id")]
    public long ClientId { get; set; }

    public string Nombre { get; set; }

    public string Cuit { get; set; }

    public PaisReponse Pais { get; set; }

    public CategoriaFiscalResponse CategoriaFiscal { get; set; }

    public ErrorResponse Error { get; set; }
}