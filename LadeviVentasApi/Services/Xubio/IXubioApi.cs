using LadeviVentasApi.Services.Xubio.DTOs;
using Microsoft.AspNetCore.Components;
using Refit;

namespace LadeviVentasApi.Services.Xubio;

public interface IXubioApi
{
    [Post("/TokenEndpoint")]
    Task<TokenResponse> GetTokenAsync(
        [Header("Authorization")] string authorizationHeader,
        [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> data);

    [Post("/clienteBean")]
    [Headers("accept: application/json", "Content-Type: application/json")]
    Task<ClientDtoResponse> CreateClientAsync([Header("Authorization")] string authorization, ClientDtoRequest createClientDtoRequest);

    [Put("/clienteBean/{id}")]
    [Headers("accept: application/json", "Content-Type: application/json")]
    Task<ClientDtoResponse> EditClientAsync([Header("Authorization")] string authorization, [AliasAs("Id")] long id, [Body] ClientDtoRequest createClientDtoRequest);

    [Delete("/clienteBean/{id}")]
    [Headers("accept: application/json", "Content-Type: application/json")]
    Task DeleteClientAsync([Header("Authorization")] string authorization, [AliasAs("Id")] long id);

    [Get("/clienteBean")]
    [Headers("accept: application/json")]
    Task<List<ClientDtoResponse>> GetClientsAsync([Header("Authorization")] string authorization, [Query] Dictionary<string, string> queryParams);

    [Get("/ProductoVentaBean?activo=1")]
    [Headers("accept: application/json", "Content-Type: application/json")]
    Task<List<ProductDtoResponse>> GetProductsAsync([Header("Authorization")] string authorization);

    [Get("/puntoVentaBean?activo=1")]
    [Headers("accept: application/json", "Content-Type: application/json")]
    Task<List<PointOfSaleDtoResponse>> GetPointOfSaleAsync([Header("Authorization")] string authorization);

    [Post("/comprobanteVentaBean")]
    [Headers("accept: application/json", "Content-Type: application/json")]
    Task<ReceiptDtoResponse> CreateReceiptForManualPointOfSaleAsync([Header("Authorization")] string authorization, ReceiptDtoRequest createReceiptDtoRequest);

    [Post("/facturar")]
    [Headers("accept: application/json", "Content-Type: application/json")]
    Task<ReceiptDtoResponse> CreateReceiptForAutomaticPointOfSaleAsync([Header("Authorization")] string authorization, ReceiptDtoRequest createReceiptDtoRequest);

    [Get("/talonario")]
    [Headers("accept: application/json", "Content-Type: application/json")]
    Task<List<BookDocumentDtoResponse>> GetBookDocumentsAsync([Header("Authorization")] string authorization, [Query] Dictionary<string, string> queryParams);
}
