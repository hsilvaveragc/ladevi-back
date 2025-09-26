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

    [HttpGet("GetEditionsForOP/{productId}/{editionId}")]
    public async Task<IActionResult> GetEditionsForOP(long productId, long editionId)
    {
        var editions = Context.ProductEditions
                                .AsNoTracking()
                                .Where(x => x.ProductId == productId && (!x.Deleted.HasValue || !x.Deleted.Value) && !x.Closed || x.Id == editionId)
                                .OrderBy(x => x.Start)
                                .Select(x => new
                                {
                                    x.Id,
                                    x.Name,
                                    x.ProductId,
                                })
                                .ToList();
        return Ok(editions);
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

    [HttpGet("GetPublishingOrdersByEdition/{editionId}")]
    public async Task<ActionResult> GetPublishingOrdersByEdition(long editionId, [FromQuery] bool isComturClient = false)
    {
        try
        {
            bool isSeller = CurrentAppUser.Value.ApplicationRole.IsSeller();
            long userId = CurrentAppUser.Value.Id;

            var posQuery = Context.PublishingOrders.AsNoTracking()
                                .Include(x => x.Client)
                                .Include(x => x.Contract)
                                    .ThenInclude(c => c.Product)
                                .Include(x => x.Contract)
                                    .ThenInclude(c => c.Currency)
                                .Include(x => x.ProductEdition)
                                    .ThenInclude(pe => pe.Product)
                                .Include(x => x.ProductAdvertisingSpace)
                                .Include(x => x.AdvertisingSpaceLocationType)
                                .Include(x => x.SoldSpace)
                                .Include(x => x.Seller)
                                .Where(x => (!x.Deleted.HasValue || !x.Deleted.Value) &&
                                        x.Client.XubioId.HasValue &&
                                        x.ProductEditionId == editionId && // Filtro principal por edición
                                        x.SoldSpace.Total != 0 &&
                                        x.ContractId.HasValue &&
                                        x.Contract.BillingConditionId == 2 &&
                                        string.IsNullOrWhiteSpace(x.XubioDocumentNumber) &&
                                        string.IsNullOrWhiteSpace(x.InvoiceNumber) &&
                                        x.Client.IsComtur == isComturClient);
            if (isSeller)
            {
                var clients = Context.Clients.Where(c => c.ApplicationUserSellerId == userId).Select(c => c.Id);

                if (clients.Any())
                {
                    posQuery = posQuery.Where(x => clients.Contains(x.ClientId));
                }
            }

            var orders = await posQuery
                .OrderBy(x => x.Client.BrandName)
                .ThenBy(x => x.Contract.Number)
                .Select(x => new
                {
                    x.Id,
                    x.ClientId,
                    ClientBrandName = x.Client.BrandName,
                    ClientLegalName = x.Client.LegalName,
                    x.ContractId,
                    ContractNumber = x.Contract.Number,
                    ContractName = x.Contract.Name,
                    x.ProductEdition.ProductId,
                    XubioProductCode = !x.Client.IsComtur ? x.ProductEdition.Product.XubioProductCode : x.ProductEdition.Product.ComturXubioProductCode,
                    ProductName = x.ProductEdition.Product.Name,
                    x.ProductEditionId,
                    ProductEditionName = x.ProductEdition.Name,
                    x.ProductAdvertisingSpaceId,
                    ProductAdvertisingSpaceName = x.ProductAdvertisingSpace.Name,
                    x.AdvertisingSpaceLocationTypeId,
                    AdvertisingSpaceLocationTypeName = x.AdvertisingSpaceLocationType.Name,
                    x.Quantity,
                    x.Contract.CurrencyId,
                    CurrencyName = x.Contract.UseEuro ? "EUR" : x.Contract.Currency.Name,
                    Total = x.Latent ? 0 : x.Contract.SoldSpaces.FirstOrDefault(y => y.Id == x.SoldSpaceId).UnitPriceWithDiscounts * x.Quantity,
                    TotalTaxes = x.Latent ? 0 : x.Contract.SoldSpaces.FirstOrDefault(y => y.Id == x.SoldSpaceId).TotalTaxes * x.Quantity,
                    x.SellerId,
                    SellerFullName = x.Seller.FullName,
                    x.Observations,
                    // Campos adicionales necesarios para la facturación
                    x.PageNumber,
                    x.CreationDate,
                    UnitPriceWithDiscounts = x.Latent ? 0 : x.Contract.SoldSpaces.FirstOrDefault(y => y.Id == x.SoldSpaceId).UnitPriceWithDiscounts,
                })
                .ToListAsync();

            return Ok(orders);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener órdenes por edición: {ex.Message}");
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
