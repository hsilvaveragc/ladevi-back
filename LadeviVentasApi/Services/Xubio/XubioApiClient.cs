
using System.Text.Json;
using LadeviVentasApi.Services.Xubio;
using LadeviVentasApi.Services.Xubio.DTOs;
using Refit;

public class XubioApiClient
{
    private readonly HttpClient httpClient;
    private readonly IXubioApi api;
    private readonly string clientId;
    private readonly string secretId;
    private TokenResponse? currentToken;
    private DateTime tokenExpiration;

    public XubioApiClient(IHttpClientFactory httpClientFactory, string clientId, string secretId)
    {
        this.clientId = clientId;
        this.secretId = secretId;

        this.httpClient = httpClientFactory.CreateClient("WsHttpClient");
        httpClient.BaseAddress = new UriBuilder("https://xubio.com:443/API/1.1").Uri;
        var builder = RequestBuilder.ForType<IXubioApi>();
        api = RestService.For(this.httpClient, builder);
    }

    private async Task<string> GetValidTokenAsync()
    {
        if (currentToken == null || DateTime.UtcNow >= tokenExpiration)
        {
            var data = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };

            var credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{secretId}"));
            var authHeader = $"Basic {credentials}";
            currentToken = await api.GetTokenAsync(authHeader, data);
            tokenExpiration = DateTime.UtcNow.AddSeconds(int.Parse(currentToken.ExpiresIn) - 60); // Buffer of 60 seconds
        }

        return currentToken.AccessToken;
    }

    public async Task<ClientDtoResponse> CreateClientAsync(ClientDtoRequest createClientDtoRequest)
    {
        try
        {
            return await api.CreateClientAsync("Bearer " + (await GetValidTokenAsync()), createClientDtoRequest);
        }
        catch (ApiException ex)
        {
            var error = JsonSerializer.Deserialize<ErrorResponse>(ex.Content);
            return new ClientDtoResponse
            {
                Error = error
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ClientDtoResponse> EditClientAsync(ClientDtoRequest createClientDtoRequest)
    {
        try
        {
            return await api.EditClientAsync("Bearer " + (await GetValidTokenAsync()), createClientDtoRequest.ClientId, createClientDtoRequest);
        }
        catch (ApiException ex)
        {
            var error = JsonSerializer.Deserialize<ErrorResponse>(ex.Content);
            return new ClientDtoResponse
            {
                Error = error
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ClientDtoResponse> DeleteClientAsync(long xubioId)
    {
        try
        {
            await api.DeleteClientAsync("Bearer " + (await GetValidTokenAsync()), xubioId);
            return new ClientDtoResponse { ClientId = xubioId };
        }
        catch (ApiException ex)
        {
            var error = JsonSerializer.Deserialize<ErrorResponse>(ex.Content);
            return new ClientDtoResponse
            {
                Error = error
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<ClientDtoResponse>> GetClientsAsync(Dictionary<string, string> queryParams)
    {
        try
        {
            return await api.GetClientsAsync("Bearer " + (await GetValidTokenAsync()), queryParams);
        }
        catch (ApiException ex)
        {
            // En caso de error, devolvemos una lista vacía
            return new List<ClientDtoResponse>();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<ProductDtoResponse>> GetProductsAsync()
    {
        return await api.GetProductsAsync("Bearer " + (await GetValidTokenAsync()));
    }

    public async Task<List<PointOfSaleDtoResponse>> GetPointOfSaleAsync()
    {
        return await api.GetPointOfSaleAsync("Bearer " + (await GetValidTokenAsync()));
    }

    public async Task<ReceiptDtoResponse> CreateReceiptAsync(ReceiptDtoRequest createReceiptDtoRequest, bool isManualPointOfSale)
    {
        try
        {
            if (isManualPointOfSale)
            {
                return await api.CreateReceiptForManualPointOfSaleAsync("Bearer " + (await GetValidTokenAsync()), createReceiptDtoRequest);
            }
            else
            {
                return await api.CreateReceiptForAutomaticPointOfSaleAsync("Bearer " + (await GetValidTokenAsync()), createReceiptDtoRequest);
            }

        }
        catch (ApiException ex)
        {
            var error = JsonSerializer.Deserialize<ErrorResponse>(ex.Content);
            return new ReceiptDtoResponse
            {
                Error = error
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<BookDocumentDtoResponse>> GetBookDocumentsAsync(Dictionary<string, string> queryParams)
    {
        try
        {
            return await api.GetBookDocumentsAsync("Bearer " + (await GetValidTokenAsync()), queryParams);
        }
        catch (ApiException ex)
        {
            // En caso de error, devolvemos una lista vacía
            return new List<BookDocumentDtoResponse>();
        }
        catch (Exception)
        {
            throw;
        }
    }

}
