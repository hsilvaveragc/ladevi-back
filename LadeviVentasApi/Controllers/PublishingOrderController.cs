namespace LadeviVentasApi.Controllers;

using AutoMapper;
using KendoNET.DynamicLinq;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;
using LadeviVentasApi.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class PublishingOrderController : RestController<PublishingOrder, PublishingOrderWritingDto>
{
    public PublishingOrderController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    [HttpGet("GetPublishingOrdersByClient/{clientId}")]
    public async Task<ActionResult> GetPublishingOrders(long clientId)
    {
        bool isSeller = CurrentAppUser.Value.ApplicationRole.IsSeller();
        bool isSupervisor = CurrentAppUser.Value.ApplicationRole.IsSupervisor();
        long userId = CurrentAppUser.Value.Id;

        var posQuery = Context.PublishingOrders.AsNoTracking() //base.GetQueryableWithIncludes()
                            .Include(x => x.Client)
                            .Include(x => x.Contract)
                                .ThenInclude(c => c.Product)
                               .Include(x => x.Contract)
                                .ThenInclude(c => c.Currency)
                            .Include(x => x.ProductEdition)
                            .Include(x => x.ProductAdvertisingSpace)
                            .Include(x => x.AdvertisingSpaceLocationType)
                            .Include(x => x.SoldSpace)
                            .Where(x => (!x.Deleted.HasValue || !x.Deleted.Value) &&
                                    x.Client.XubioId.HasValue &&
                                    x.ClientId == clientId &&
                                    x.SoldSpace.Total != 0 &&
                                    x.ContractId.HasValue &&
                                    x.Contract.BillingConditionId == 2 &&
                                    string.IsNullOrWhiteSpace(x.XubioDocumentNumber) &&
                                    string.IsNullOrWhiteSpace(x.XubioDocumentNumber));
        if (isSeller)
        {
            var client = Context.Clients.SingleOrDefault(c => (!isSeller || c.ApplicationUserSellerId == userId) && c.Id == clientId);
            if (client == null)
            {
                return NotFound();
            }
        }
        posQuery = posQuery.Where(x => x.ClientId == clientId);

        return Ok(posQuery.Select(x => new
        {
            x.Id,
            ClientBrandName = x.Client.BrandName,
            x.ContractId,
            ContractNumber = x.Contract.Number,
            ContracName = x.Contract.Name,
            x.Contract.ProductId,
            x.Contract.Product.XubioProductCode,
            x.Contract.Product.ComturXubioProductCode,
            ProductName = x.Contract.Product.Name,
            ProductEditionId = x.ProductEdition.Id,
            ProductEditionName = x.ProductEdition.Name,
            ProductAdvertisingSpaceName = x.ProductAdvertisingSpace.Name,
            AdvertisingSpaceLocationTypeName = x.AdvertisingSpaceLocationType.Name,
            x.Quantity,
            CurrencyName = x.Contract == null ? "" : x.Contract.UseEuro ? "EUR" : x.Contract.Currency.Name,
            Total = x.Latent ? 0 : x.Contract.SoldSpaces.FirstOrDefault(y => y.Id == x.SoldSpaceId).UnitPriceWithDiscounts * x.Quantity,
            TotalTaxes = x.Latent ? 0 : x.Contract.SoldSpaces.FirstOrDefault(y => y.Id == x.SoldSpaceId).TotalTaxes * x.Quantity,
            x.SellerId,
            SellerFullName = x.Seller.FullName,
        })
          .ToList());
    }

    [HttpGet("GetEditionsForOP/{productId}/{editionId}")]
    public async Task<IActionResult> GetEditionsForOP(long productId, long editionId)
    {
        var queryEdition = Context.ProductEditions
                                .AsNoTracking();
        if (CurrentAppUser.Value.ApplicationRole.IsSuperuser())
        {
            queryEdition = queryEdition.Where(x => x.ProductId == productId && (!x.Deleted.HasValue || !x.Deleted.Value) || x.Id == editionId);
        }
        else
        {
            queryEdition = queryEdition.Where(x => x.ProductId == productId && (!x.Deleted.HasValue || !x.Deleted.Value) && !x.Closed || x.Id == editionId);
        }

        var editions = queryEdition.OrderBy(x => x.Start)
                                .Select(x => new
                                {
                                    x.Id,
                                    x.Name,
                                    x.ProductId
                                })
                                .ToList();
        return Ok(editions);
    }

    [HttpPost("SearchByEdition")]
    public async Task<IActionResult> SearchByEdition([FromBody] SearchByEditionRequest request)
    {
        try
        {
            var query = Context.PublishingOrders
                            .Include(po => po.Contract)
                                .ThenInclude(c => c.Client)
                            .Include(po => po.Contract)
                                .ThenInclude(c => c.Currency)
                            .Include(po => po.ProductEdition)
                                .ThenInclude(pe => pe.Product)
                            .Include(po => po.Seller)
                            .Where(po => po.ProductEditionId == request.EditionId)
                            .Where(po => po.XubioDocumentNumber == null) // Solo no facturadas
                            .Where(po => !po.Deleted.HasValue || !po.Deleted.Value);

            // Filtros adicionales opcionales
            if (request.ClientId.HasValue)
                query = query.Where(po => po.Contract.ClientId == request.ClientId.Value);

            if (request.SellerId.HasValue)
                query = query.Where(po => po.SellerId == request.SellerId.Value);

            if (request.IsComturClient.HasValue)
                query = query.Where(po => po.Contract.Client.IsComtur == request.IsComturClient.Value);

            var orders = await query
                .OrderBy(po => po.Contract.Client.BrandName)
                .ThenBy(po => po.Contract.Number)
                .Take(request.Take ?? 1000)
                .Select(po => new SearchByEditionResponse
                {
                    Id = po.Id,
                    OrderNumber = "0",//po.Number,
                    ClientId = po.Contract.ClientId,
                    ClientName = po.Contract.Client.BrandName,
                    ContractNumber = po.Contract.Number.ToString(),
                    ProductName = po.ProductEdition.Product.Name,
                    EditionName = po.ProductEdition.Name,
                    Description = "",//po.Description,
                    Quantity = 0,//po.Quantity,
                    Price = 0,// po.Price,
                    Total = 0,// po.Amount,
                    CurrencyId = po.Contract.CurrencyId,
                    CurrencyName = po.Contract.Currency.Name,
                    SellerId = po.SellerId,
                    SellerName = "",//po.Seller != null ? $"{po.Seller.FirstName} {po.Seller.LastName}" : null,
                    XubioProductId = "",//po.XubioProductId,
                    CreatedDate = po.CreationDate.HasValue ? po.CreationDate.Value : DateTime.Now,
                    // Campos adicionales para facturación
                    TotalTaxes = 0,//po.TotalTaxes ?? 0,
                    Observations = po.Observations
                })
                .ToListAsync();

            return Ok(new
            {
                Data = orders,
                Total = orders.Count,
                EditionId = request.EditionId
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al buscar órdenes por edición: {ex.Message}");
        }
    }

    public override async Task<IActionResult> Post(PublishingOrderWritingDto x)
    {
        x.SellerId = Context.Clients.SingleOrDefault(c => c.Id == x.ClientId).ApplicationUserSellerId;
        x.CreationDate = DateTime.Now;
        x.LastUpdate = DateTime.Now;

        using (var tx = await Context.Database.BeginTransactionAsync())
        {
            var response = await base.Post(x);
            var type = response.GetType();

            if (type.Equals(typeof(CreatedAtActionResult)) && x.SoldSpaceId.HasValue)
            {
                PublishingOrdersUC.UpdateContractBalance(x.SoldSpaceId.Value, Context);
            }

            if (x.ContractId.HasValue)
            {
                ProductAdvertisingSpace pas = Context.ProductAdvertisingSpaces.SingleOrDefault(y => y.Id == x.ProductAdvertisingSpaceId);
                ContractHistorical ch = new ContractHistorical();
                ch.Date = DateTime.Now;
                ch.User = CurrentAppUser.Value.FullName;
                ch.Changes = "Se creó una Órden de Publicación de " + pas.Name;
                ch.ContractId = x.ContractId.Value;
                Context.Add(ch);
                Context.SaveChanges();
            }

            tx.Commit();
            return response;
        }

    }

    public override async Task<IActionResult> Put(long id, PublishingOrderWritingDto x)
    {
        x.LastUpdate = DateTime.Now;

        using (var tx = await Context.Database.BeginTransactionAsync())
        {
            var response = await base.Put(id, x);

            var type = response.GetType();

            if (type.Equals(typeof(NoContentResult)) && x.SoldSpaceId.HasValue)
            {
                PublishingOrdersUC.UpdateContractBalance(x.SoldSpaceId.Value, Context);
            }

            tx.Commit();
            return response;
        }
    }

    public override async Task<ActionResult<PublishingOrder>> Delete(long id)
    {
        var op = Context.PublishingOrders
                        .AsNoTracking()
                        .Single(x => x.Id == id);

        using (var tx = await Context.Database.BeginTransactionAsync())
        {

            var response = await base.Delete(id);
            var type = response.GetType();

            if (op.SoldSpaceId.HasValue)
            {
                PublishingOrdersUC.UpdateContractBalance(op.SoldSpaceId.Value, Context);
            }
            tx.Commit();
            return response;
        }
    }

    protected override IQueryable<PublishingOrder> GetQueryableWithIncludes()
    {
        bool isSeller = CurrentAppUser.Value.ApplicationRole.IsSeller();
        bool isSupervisor = CurrentAppUser.Value.ApplicationRole.IsSupervisor();
        long userId = CurrentAppUser.Value.Id;
        List<long> clientsIds = Context.Clients.Where(c => !isSeller || c.ApplicationUserSellerId == userId).Select(c => c.Id).ToList();

        var allOP = Context.PublishingOrders.AsNoTracking() //base.GetQueryableWithIncludes()
                    .Include(x => x.Client)
                    .Include(x => x.Contract)
                        .ThenInclude(c => c.BillingCondition)
                    .Include(x => x.Contract)
                        .ThenInclude(c => c.Currency)
                    .Include(x => x.Contract)
                        .ThenInclude(c => c.SoldSpaces)
                    .Include(x => x.ProductAdvertisingSpace)
                    .Include(x => x.Seller)
                    .Include(x => x.ProductEdition);

        if (isSeller)
        {
            return allOP.Where(x => x.SellerId == userId || clientsIds.Contains(x.ClientId));
        }

        return allOP;
    }

    protected override DataSourceResult GetSearchDataSourceResult(KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
    {
        bool isSuperuser = CurrentAppUser.Value.ApplicationRole.IsSuperuser();

        var result = GetSearchQueryable()
            .Select(x => new
            {
                x.Id,
                x.Client,
                x.ClientId,
                x.ProductEdition,
                x.ProductEditionId,
                x.ProductEdition.ProductId,
                x.SellerId,
                x.Seller,
                x.ProductAdvertisingSpace,
                x.Quantity,
                x.PageNumber,
                x.Latent,
                x.ProductAdvertisingSpaceId,
                x.ContractId,
                x.SoldSpaceId,
                InvoiceNumber = x.Contract != null && x.Contract.SoldSpaces.Where(sp => sp.Id == x.SoldSpaceId).Count() > 0 && x.Contract.SoldSpaces.SingleOrDefault(sp => sp.Id == x.SoldSpaceId).UnitPriceWithDiscounts == 0 ? "" : x.Contract != null && x.Contract.BillingCondition.Name == BillingCondition.Anticipated
                    ? x.Contract.InvoiceNumber : x.InvoiceNumber,
                x.PaidOut,
                x.Observations,
                //A definir
                Total = x.Latent ? 0 : x.Contract.SoldSpaces.FirstOrDefault(y => y.Id == x.SoldSpaceId).UnitPriceWithDiscounts * x.Quantity,
                // (x.Contract.SoldSpaces.FirstOrDefault(y => y.Id == x.SoldSpaceId).SubTotal - x.Contract.SoldSpaces.FirstOrDefault(y => y.Id == x.SoldSpaceId).TotalDiscounts) / x.Contract.SoldSpaces.FirstOrDefault(y => y.Id == x.SoldSpaceId).Quantity,
                x.AdvertisingSpaceLocationType,
                x.AdvertisingSpaceLocationTypeId,
                Contract = x.ContractId.HasValue ? new Contract
                {
                    Id = x.ContractId.Value,
                    Number = x.Contract.Number,
                    Name = x.Contract.Name,
                    BillingConditionId = x.Contract.BillingConditionId,
                    PublishingOrders = null
                } : null,
                x.Deleted,
                x.CreationDate,
                Moneda = x.Contract == null ? "" : x.Contract.UseEuro ? "EUR" : x.Contract.Currency.Name,
                CanDelete = x.ProductEdition != null && !x.ProductEdition.Closed && string.IsNullOrWhiteSpace(x.PageNumber)
            })
            .Where(x => !x.Deleted.HasValue || !x.Deleted.Value)
            .ToDataSourceResult(request);

        return result;
    }
}
