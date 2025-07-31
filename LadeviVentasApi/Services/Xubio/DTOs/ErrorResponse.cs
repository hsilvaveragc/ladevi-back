using System.Text.Json.Serialization;

namespace LadeviVentasApi.Services.Xubio.DTOs;

public class ErrorResponse
{
    [JsonPropertyName("error_numero")]
    public long ErroNumero { get; set; }

    [JsonPropertyName("error")]
    public string Error { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("infoUri")]
    public string? InfoUri { get; set; }

    [JsonPropertyName("codeResponse")]
    public string CodeResponse { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}