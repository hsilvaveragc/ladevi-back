using System.Text.Json.Serialization;

namespace LadeviVentasApi.Services.Xubio.DTOs;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("scope")]
    public string Scope { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public string ExpiresIn { get; set; }
}