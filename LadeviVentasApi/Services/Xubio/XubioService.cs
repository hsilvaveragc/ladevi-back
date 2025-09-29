namespace LadeviVentasApi.Services.Xubio;

using System.Drawing;
using LadeviVentasApi.Models.Domain;
using LadeviVentasApi.Services.Xubio.DTOs;

public class XubioApiConfiguration
{
    public required string ClientId { get; set; }
    public required string SecretId { get; set; }
}

public class XubioService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XubioApiConfiguration argentinaConfig;
    private readonly XubioApiConfiguration comturConfig;
    private readonly Dictionary<bool, XubioApiClient> _clients = new();

    public XubioService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        this.httpClientFactory = httpClientFactory;
        argentinaConfig = new XubioApiConfiguration
        {
            ClientId = configuration["XubioClientIdArgentina"],
            SecretId = configuration["XubioSecretIdArgentina"]
        };

        comturConfig = new XubioApiConfiguration
        {
            ClientId = configuration["XubioClientIdComtur"],
            SecretId = configuration["XubioSecretIdComtur"]
        };
    }

    private XubioApiClient GetApiClient(bool isComtur)
    {
        if (!_clients.ContainsKey(isComtur))
        {
            var config = isComtur ? comturConfig : argentinaConfig;
            _clients[isComtur] = new XubioApiClient(httpClientFactory, config.ClientId, config.SecretId);
        }
        return _clients[isComtur];
    }

    public async Task<ClientDtoResponse> CreateClientAsync(Client client)
    {
        var apiClient = GetApiClient(client.IsComtur);
        return await apiClient.CreateClientAsync(MapToXubioClient(client));
    }

    public async Task<ClientDtoResponse> EditClientAsync(Client client)
    {
        var apiClient = GetApiClient(client.IsComtur);
        return await apiClient.EditClientAsync(MapToXubioClient(client));
    }

    public async Task<ClientDtoResponse> DeleteClientAsync(long xubioId, bool isComturClient)
    {
        var apiClient = GetApiClient(isComturClient);
        return await apiClient.DeleteClientAsync(xubioId);
    }

    public async Task<List<ClientDtoResponse>> GetClientsAsync(bool isComtur, string identificationValue = "")
    {
        var apiClient = GetApiClient(isComtur);
        var queryParams = new Dictionary<string, string>
        {
            { "activo", "1" }
        };

        if (!string.IsNullOrWhiteSpace(identificationValue))
        {
            queryParams.Add("numeroIdentificacion", identificationValue);
        }

        return await apiClient.GetClientsAsync(queryParams);
    }

    public async Task<List<ProductDtoResponse>> GetProductsAsync(bool isComtur)
    {
        var apiClient = GetApiClient(isComtur);
        return await apiClient.GetProductsAsync();
    }

    public async Task<List<PointOfSaleDtoResponse>> GetPointOfSaleAsync(bool isComtur)
    {
        var apiClient = GetApiClient(isComtur);
        return await apiClient.GetPointOfSaleAsync();
    }

    public async Task<ReceiptDtoResponse> CreateReceiptAsync(ReceiptDtoRequest receiptDtoRequest, bool isComturClient, bool isManualPointOfSale)
    {
        var apiClient = GetApiClient(isComturClient);
        return await apiClient.CreateReceiptAsync(receiptDtoRequest, isManualPointOfSale);
    }

    private ClientDtoRequest MapToXubioClient(Client client)
    {
        var createClientDtoRequest = new ClientDtoRequest
        {
            ClientId = client.XubioId ?? 0,
            UsrCode = client.XubioId == null ? "" : client.XubioId.ToString(),
            Nombre = client.LegalName,
            Cuit = client.IdentificationValue,
            CategoriaFiscal = new CategoriaFiscalRequest { Codigo = client.TaxCategory.Code },
            Email = client.MainEmail,
            Telefono = client.TelephoneCountryCode + client.TelephoneAreaCode + client.TelephoneNumber,
            Direccion = client.Address,
            Pais = new PaisRequest { Codigo = client.Country.XubioCode },
        };

        if (client.CountryId == 4) //Argentina countryId
        {
            createClientDtoRequest.IdentificacionTributaria = new IdentificacionTributariaRequest { Codigo = "CUIT" };
            if (client.State != null)
            {
                createClientDtoRequest.Provincia = new ProvinciaRequest { Codigo = client.State.XubioCode };
            }
            if (client.City != null)
            {
                createClientDtoRequest.Localidad = new LocalidadRequest { Codigo = client.City.XubioCode };
            }
        }
        else
        {
            createClientDtoRequest.IdentificacionTributaria = new IdentificacionTributariaRequest { Codigo = "SIN_IDENTIFICARVENTA_GLOBAL_DIARIA" };
        }

        return createClientDtoRequest;
    }

    public async Task<List<BookDocumentDtoResponse>> GetBookDocumentsAsync(bool isComtur, string pointOfSale, string documentLetter = "", string documentType = "")
    {
        var apiClient = GetApiClient(isComtur);
        var queryParams = new Dictionary<string, string>
        {
            { "puntoDeVenta", pointOfSale }
        };

        if (!string.IsNullOrWhiteSpace(documentLetter))
        {
            queryParams.Add("letraComprobante", documentLetter);
        }

        if (!string.IsNullOrWhiteSpace(documentType))
        {
            queryParams.Add("tipoComprobante", documentType);
        }

        return await apiClient.GetBookDocumentsAsync(queryParams);
    }
}