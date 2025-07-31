namespace LadeviVentasApi.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    protected ApplicationDbContext Context { get; set; }
    protected Lazy<ApplicationUser> CurrentAppUser;

    public ReportController(ApplicationDbContext context)
    {
        Context = context;
        CurrentAppUser = new Lazy<ApplicationUser>(GetAppUser, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private ApplicationUser GetAppUser()
    {
        var userMail = User?.Identity?.IsAuthenticated != true
            ? throw new InvalidOperationException("not logged in")
            : User.Claims.ToDictionary(c => c.Type, c => c.Value)["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
        return Context.ApplicationUsers.Include(u => u.ApplicationRole)
            .Include(u => u.Country)
            .Single(u => u.CredentialsUser.Email.ToLower() == userMail.ToLower());
    }

    [HttpPost("GetPendientContracts")]
    public async Task<ActionResult> GetPendientContracts([FromBody] PendientContractParam param)
    {
        bool isSupervisor = CurrentAppUser.Value.ApplicationRole.IsSupervisor();


        List<PendientContractDto> data = Context.Contracts
            .Join(
                Context.SoldSpaces,
                c => c.Id,
                sp => sp.ContractId,
                (c, sp) => new { Contract = c, SoldSpace = sp }
            )
            .Join(
                Context.ProductAdvertisingSpaces,
                csp => csp.SoldSpace.ProductAdvertisingSpaceId,
                pas => pas.Id,
                (csp, pas) => new { csp.Contract, csp.SoldSpace, ProductAdvertisingSpaces = pas }
            )
            .Join(
                Context.Clients,
                csppas => csppas.Contract.ClientId,
                cl => cl.Id,
                (csppas, cl) => new { csppas.Contract, csppas.SoldSpace, csppas.ProductAdvertisingSpaces, Cliente = cl }
            )
            .Join(
                Context.Currency,
                csppascl => csppascl.Contract.CurrencyId,
                cu => cu.Id,
                (csppascl, cu) => new { csppascl.Contract, csppascl.SoldSpace, csppascl.ProductAdvertisingSpaces, csppascl.Cliente, Currency = cu }
            )
            .Join(
                Context.BillingConditions,
                csppasclcu => csppasclcu.Contract.BillingConditionId,
                bc => bc.Id,
                (csppasclcu, bc) => new
                {
                    csppasclcu.Contract,
                    csppasclcu.SoldSpace,
                    csppasclcu.ProductAdvertisingSpaces,
                    csppasclcu.Cliente,
                    csppasclcu.Currency,
                    BillingCondition = bc
                }
            )
            .Where(x =>
                (param.SellerId == -1 || x.Contract.SellerId == param.SellerId) &&
                ((param.ClienteId == -1 && x.Contract.Client.CountryId == CurrentAppUser.Value.CountryId) || x.Contract.ClientId == param.ClienteId) &&
                ((param.Date.HasValue && x.Contract.Start.Date <= param.Date.Value.Date) || !param.Date.HasValue) &&
                ((param.Date.HasValue && x.Contract.End.Date >= param.Date.Value.Date) || !param.Date.HasValue) &&
                (!x.Contract.Deleted.HasValue || !x.Contract.Deleted.Value)
            )
            .Select(x => new PendientContractDto
            {
                ClienteId = x.Contract.ClientId,
                Client = x.Cliente.BrandName + " " + x.Cliente.LegalName,
                Numero = x.Contract.Number.ToString(),
                Contrato = x.Contract.Name,
                SpaceTypeId = x.ProductAdvertisingSpaces.Id,
                SpaceType = x.ProductAdvertisingSpaces.Name,
                SoldSpaceId = x.SoldSpace.Id,
                Quantity = x.SoldSpace.Quantity,
                Invoice = x.Contract.InvoiceNumber,
                BillingCondition = x.Contract == null ? "" :
                (x.Contract.BillingCondition.Name == BillingCondition.AgainstPublication ? "F" :
                (x.Contract.BillingCondition.Name == BillingCondition.Anticipated ? "A" :
                (x.Contract.BillingCondition.Name == BillingCondition.NoFee ? "S" : "C"))),
                SubTotal = x.SoldSpace.SubTotal,
                Descuentos = x.SoldSpace.TotalDiscounts,
                Moneda = x.Contract.UseEuro ? "EUR" : x.Currency.Name
            }).ToList();


        var soldSpaceIds = data.Select(x => x.SoldSpaceId).ToList();
        List<PublishingOrder> allOrdenes = Context.PublishingOrders.Where(x => x.SoldSpaceId.HasValue &&
                                                                        soldSpaceIds.Contains(x.SoldSpaceId.Value) &&
                                                                        (!x.Deleted.HasValue || !x.Deleted.Value))
                                                                    .ToList();
        foreach (PendientContractDto pc in data.ToList())
        {
            List<PublishingOrder> ordenes = allOrdenes.Where(x => x.SoldSpaceId == pc.SoldSpaceId).ToList();
            List<PublishingOrder> ordenesToDate = ordenes.Where(x => !param.Date.HasValue || !x.CreationDate.HasValue || x.CreationDate.Value.Date <= param.Date.Value.Date).ToList();
            double opToDate = ordenesToDate.Count > 0 ? ordenesToDate.Sum(y => y.Quantity) : 0;
            double opActual = ordenes.Sum(x => x.Quantity);

            if (pc.Quantity - opToDate == 0 || (param.OnlyWithBalance && pc.Quantity - opActual == 0))
            {
                data.Remove(pc);
                continue;
            }

            pc.Amount = Math.Round((pc.SubTotal - pc.Descuentos) / pc.Quantity, 2, MidpointRounding.AwayFromZero);
            pc.PendientAmount = Math.Round(pc.Amount * (pc.Quantity - opToDate), 2, MidpointRounding.AwayFromZero);

            pc.SelledQuantity = opToDate;
            pc.Balance = pc.Quantity - opToDate;

            if (pc.Amount == 0 && pc.PendientAmount == 0)
            {
                pc.Invoice = "";
                pc.BillingCondition = "";
            }
        }

        return Ok(data);

    }

    [HttpPost("SaveReportGeneration/{productEditionId}")]
    public async Task<ActionResult> SaveReportGeneration(long productEditionId)
    {
        ReportOPForProductionExports report = new ReportOPForProductionExports();
        report.Date = DateTime.Now;
        report.ProductEditionId = productEditionId;
        Context.Add(report);
        Context.SaveChanges();
        return Ok();
    }

    // Método optimizado que trabaja con la proyección en lugar de PublishingOrder
    private static TotalsDto GetTotalesOptimizado<TKey>(IGrouping<TKey, dynamic> group, bool noComisionarImpagas)
    {
        double spacesLocalCurrency = group
            .Where(x => x.CurrencyId != 2)
            .Sum(y => (double)(y.Quantity * y.UnitPriceWithDiscounts));

        double spacesDolares = group
            .Where(x => x.CurrencyId == 2)
            .Sum(y => (double)(y.Quantity * y.UnitPriceWithDiscounts));

        List<ComisionDataDto> comisiones = new List<ComisionDataDto>();
        foreach (var g in group)
        {
            comisiones.Add(CalculoComisionOptimizado(
                 g.SellerId,
                 g.SellerCommisionCoeficient,
                 g.ContractSellerId,
                 g.AliquotForSalesCommission,
                 g.Quantity,
                 g.SpaceQuantity,
                 g.SubTotal,
                 g.TotalDiscounts,
                 g.TypeSpecialDiscount,
                 g.SpecialDiscount,
                 g.CurrencyId,
                 g.BillingConditionName,
                 g.ContractPaidOut,
                 g.OrderPaidOut,
                 noComisionarImpagas
            ));
        }

        var comisionesLocalCurrency = comisiones.Sum(x => (double)x.TotalComisionMonedaLocal);
        var comisionesDolares = comisiones.Sum(x => (double)x.TotalComisionDolares);

        return new TotalsDto
        {
            SpacesLocalCurrency = spacesLocalCurrency,
            SpacesDolares = spacesDolares,
            ComisionsLocalCurrency = comisionesLocalCurrency,
            ComisionDolares = comisionesDolares
        };
    }

    // Método para calcular comisión usando propiedades específicas
    private static ComisionDataDto CalculoComisionOptimizado(
        long sellerId,
        double sellerCommisionCoeficient,
        long contractSellerId,
        double aliquotForSalesCommission,
        double quantity,
        double spaceQuantity,
        double subTotal,
        double totalDiscounts,
        short? typeSpecialDiscount,
        double specialDiscount,
        long? currencyId,
        string billingConditionName,
        bool? contractPaidOut,
        bool? orderPaidOut,
        bool noComisionarImpagas)
    {
        if (currencyId == 0 || spaceQuantity == 0)
        {
            return new ComisionDataDto();
        }

        var clientChangeSeller = contractSellerId != sellerId;

        bool noComisionar = noComisionarImpagas &&
            ((billingConditionName == BillingCondition.Anticipated && (!contractPaidOut.HasValue || !contractPaidOut.Value)) ||
             (billingConditionName == BillingCondition.AgainstPublication && (!orderPaidOut.HasValue || !orderPaidOut.Value)));

        double comisionAntes = noComisionar ? 0 : sellerCommisionCoeficient * aliquotForSalesCommission;

        double comisionDespues = !typeSpecialDiscount.HasValue
            ? comisionAntes / 100
            : (typeSpecialDiscount.Value == 1
                ? (comisionAntes / 100 - (specialDiscount / 100 * comisionAntes / 100))
                : (comisionAntes / 100 + (specialDiscount / 100 * 25 / 100)));

        double netoProporcional = quantity * subTotal / spaceQuantity;
        double descuentosProporcional = quantity * totalDiscounts / spaceQuantity;
        double totalComision = (netoProporcional - descuentosProporcional) * comisionDespues / 100;

        return new ComisionDataDto()
        {
            ComisionAntes = comisionAntes / 100,
            ComisionDespues = clientChangeSeller ? 0 : comisionDespues,
            TotalComisionDolares = !clientChangeSeller && currencyId == 2 ? totalComision : 0,
            TotalComisionMonedaLocal = !clientChangeSeller && currencyId != 2 ? totalComision : 0
        };
    }

    // Método para calcular comisión desde la primera consulta
    private static ComisionDataDto CalculoComision(
        long sellerId,
        double sellerCommisionCoeficient,
        long contractSellerId,
        double aliquotForSalesCommission,
        long? soldSpaceId,
        ICollection<SoldSpace> soldSpaces,
        double quantity,
        long? currencyId,
        string billingConditionName,
        bool? contractPaidOut,
        bool? orderPaidOut,
        bool noComisionarImpagas)
    {
        if (currencyId == 0 || soldSpaces == null || !soldSpaces.Any())
        {
            return new ComisionDataDto();
        }

        var clientChangeSeller = contractSellerId != sellerId;
        SoldSpace sp = soldSpaces.FirstOrDefault(x => x.Id == soldSpaceId);

        if (sp == null || sp.Quantity == 0)
        {
            return new ComisionDataDto();
        }

        bool noComisionar = noComisionarImpagas &&
            ((billingConditionName == BillingCondition.Anticipated && (!contractPaidOut.HasValue || !contractPaidOut.Value)) ||
             (billingConditionName == BillingCondition.AgainstPublication && (!orderPaidOut.HasValue || !orderPaidOut.Value)));

        double comisionAntes = noComisionar ? 0 : sellerCommisionCoeficient * aliquotForSalesCommission;

        double comisionDespues = !sp.TypeSpecialDiscount.HasValue
            ? comisionAntes / 100
            : (sp.TypeSpecialDiscount.Value == 1
                ? (comisionAntes / 100 - (sp.SpecialDiscount / 100 * comisionAntes / 100))
                : (comisionAntes / 100 + (sp.SpecialDiscount / 100 * 25 / 100)));

        double netoProporcional = quantity * sp.SubTotal / sp.Quantity;
        double descuentosProporcional = quantity * sp.TotalDiscounts / sp.Quantity;
        double totalComision = (netoProporcional - descuentosProporcional) * comisionDespues / 100;

        return new ComisionDataDto()
        {
            ComisionAntes = comisionAntes / 100,
            ComisionDespues = clientChangeSeller ? 0 : comisionDespues,
            TotalComisionDolares = !clientChangeSeller && currencyId == 2 ? totalComision : 0,
            TotalComisionMonedaLocal = !clientChangeSeller && currencyId != 2 ? totalComision : 0
        };
    }

    [HttpPost("GetOPByClient")]
    public async Task<ActionResult> GetOPByClient([FromBody] OrdersByClientParamDto param)
    {
        var data = Context.PublishingOrders
                        .Include(po => po.SoldSpace)
                        .Include(po => po.Contract)
                        .ThenInclude(c => c.BillingCondition)
                        .Include(po => po.Contract)
                        .ThenInclude(c => c.Currency)
            .Join(
                Context.ProductAdvertisingSpaces,
                op => op.ProductAdvertisingSpaceId,
                pas => pas.Id,
                (op, pas) => new
                {
                    PublishingOrder = op,
                    SoldSpace = op.SoldSpace,
                    Contract = op.Contract,
                    ProductAdvertisingSpaces = pas
                })
            .Join(
                Context.ProductEditions,
                opspcpas => opspcpas.PublishingOrder.ProductEditionId,
                pe => pe.Id,
                (opspcpas, pe) => new
                {
                    opspcpas.PublishingOrder,
                    opspcpas.Contract,
                    opspcpas.SoldSpace,
                    opspcpas.ProductAdvertisingSpaces,
                    ProductEdition = pe
                })
            .Join(
                Context.Products,
                opspcpaspe => opspcpaspe.ProductEdition.ProductId,
                p => p.Id,
                (opspcpaspe, p) => new
                {
                    opspcpaspe.PublishingOrder,
                    opspcpaspe.SoldSpace,
                    opspcpaspe.Contract,
                    opspcpaspe.ProductAdvertisingSpaces,
                    opspcpaspe.ProductEdition,
                    Product = p,
                    BillingCondition = opspcpaspe.Contract != null ? opspcpaspe.Contract.BillingCondition : null,
                    Currency = opspcpaspe.Contract != null ? opspcpaspe.Contract.Currency : null
                })
            .Where(x =>
                (!x.PublishingOrder.Deleted.HasValue || !x.PublishingOrder.Deleted.Value) &&
                (param.FromDate.HasValue && x.ProductEdition.End.Date >= param.FromDate.Value.Date || !param.FromDate.HasValue) &&
                (param.ToDate.HasValue && x.ProductEdition.End.Date <= param.ToDate.Value.Date || !param.ToDate.HasValue) &&
                (param.ProductType == -1 || x.Product.ProductTypeId == param.ProductType) &&
                (param.Product == -1 || x.ProductEdition.ProductId == param.Product) &&
                (param.Edition == -1 || x.PublishingOrder.ProductEditionId == param.Edition) &&
                (param.SellerId == -1 || x.PublishingOrder.SellerId == param.SellerId) &&
                x.PublishingOrder.ClientId == param.Client
            )
            .Select(x => new
            {
                Edicion = x.ProductEdition.Code,
                Salida = x.ProductEdition.End,
                TipoEspacio = x.ProductAdvertisingSpaces.Name,
                Cantidad = x.PublishingOrder.Quantity,
                Importe = x.SoldSpace == null ? 0 : x.SoldSpace.UnitPriceWithDiscounts * x.PublishingOrder.Quantity,// (x.SoldSpace.SubTotal - x.SoldSpace.TotalDiscounts) / x.SoldSpace.Quantity,
                Factura = ((x.SoldSpace == null ? 0 : x.SoldSpace.UnitPriceWithDiscounts) == 0) || x.Contract == null ?
                            "" :
                            x.BillingCondition.Name == BillingCondition.AgainstPublication ?
                                                            x.PublishingOrder.InvoiceNumber :
                                                            x.Contract.InvoiceNumber,
                BillingCondition = ((x.SoldSpace == null ? 0 : x.SoldSpace.UnitPriceWithDiscounts) == 0) || x.Contract == null ? "" :
                (x.Contract.BillingCondition.Name == BillingCondition.AgainstPublication ? "F" :
                (x.Contract.BillingCondition.Name == BillingCondition.Anticipated ? "A" :
                (x.Contract.BillingCondition.Name == BillingCondition.NoFee ? "S" : "C"))),
                Numero = x.Contract == null ? 0 : x.Contract.Number,
                Contrato = x.Contract == null ? "LATENTE" : x.Contract.Name,
                Moneda = x.Contract == null ? "" : x.Contract.UseEuro ? "EUR" : x.Currency.Name
            })
            .ToList();

        return Ok(data);
    }

    [HttpPost("GetOPBySeller")]
    public async Task<ActionResult> GetOPBySeller([FromBody] OrdersBySellerParamDto param)
    {
        DateTime? toDate = param.ToDate.HasValue ? new DateTime(param.ToDate.Value.Year, param.ToDate.Value.Month, param.ToDate.Value.Day, 23, 59, 59) : null;

        // Primera consulta optimizada con proyección
        var grid1 = await Context.PublishingOrders
            .Include(po => po.ProductEdition)
                .ThenInclude(pe => pe.Product)
            .Where(op =>
                (!op.Deleted.HasValue || !op.Deleted.Value) &&
                (param.FromDate.HasValue && op.ProductEdition.End.Date >= param.FromDate.Value.Date || !param.FromDate.HasValue) &&
                (param.ToDate.HasValue && op.ProductEdition.End.Date <= param.ToDate.Value.Date || !param.ToDate.HasValue) &&
                (param.ProductType == -1 || op.ProductEdition.Product.ProductTypeId == param.ProductType) &&
                (param.Product == -1 || op.ProductEdition.ProductId == param.Product) &&
                (param.ProductEdition == -1 || op.ProductEditionId == param.ProductEdition) &&
                (param.SelledId == -1 || op.SellerId == param.SelledId)
            )
            .Select(d => new OrdersBySellerMainDto
            {
                Edicion = d.ProductEdition.Code,
                ProductCountry = d.ProductEdition.Product.Country.Name,
                FechaSalida = d.ProductEdition.End,
                Pagina = d.PageNumber,
                Quantity = d.Quantity,
                Marca = d.Client.BrandName,
                RazonSocial = d.Client.LegalName,
                Vendedor = d.Seller.Initials,
                Numero = d.Contract == null ? "0" : d.Contract.Number.ToString(),
                Contrato = d.Contract == null ? "LATENTE" : d.Contract.Name,
                TipoEspacio = d.Contract == null ? d.ProductAdvertisingSpace.Name : d.Contract.SoldSpaces.FirstOrDefault(sp => sp.Id == d.SoldSpaceId).ProductAdvertisingSpace.Name,
                CantidadEspacios = d.Contract == null ? 0 : d.Contract.SoldSpaces.FirstOrDefault(sp => sp.Id == d.SoldSpaceId).Quantity,
                ComisionData = d.Contract == null ? new ComisionDataDto() : CalculoComision(
                    d.SellerId,
                    d.Seller.CommisionCoeficient,
                    d.Contract.SellerId,
                    d.ProductEdition.Product.AliquotForSalesCommission,
                    d.SoldSpaceId,
                    d.Contract.SoldSpaces,
                    d.Quantity,
                    d.Contract.CurrencyId,
                    d.Contract.BillingCondition.Name,
                    d.Contract.PaidOut,
                    d.PaidOut,
                    param.NoComisionarImpagas
                ),
                Importe = d.Contract == null ? 0 : d.Contract.SoldSpaces.FirstOrDefault(sp => sp.Id == d.SoldSpaceId).UnitPriceWithDiscounts * d.Quantity,
                BillingCondition = ((d.Contract == null ? 0 : d.Contract.SoldSpaces.FirstOrDefault(sp => sp.Id == d.SoldSpaceId).UnitPriceWithDiscounts) == 0) || d.Contract == null ? "" :
                    (d.Contract.BillingCondition.Name == BillingCondition.AgainstPublication ? "F" :
                    (d.Contract.BillingCondition.Name == BillingCondition.Anticipated ? "A" :
                    (d.Contract.BillingCondition.Name == BillingCondition.NoFee ? "S" : "C"))),
                Invoice = ((d.Contract == null ? 0 : d.Contract.SoldSpaces.FirstOrDefault(sp => sp.Id == d.SoldSpaceId).UnitPriceWithDiscounts) == 0) || d.Contract == null ?
                        "" :
                        (d.Contract.BillingCondition.Name == BillingCondition.AgainstPublication ? d.InvoiceNumber : d.Contract.InvoiceNumber),
                Moneda = d.Contract == null ? "" : d.Contract.UseEuro ? "EUR" : d.Contract.Currency.Name
            })
            .AsNoTracking()
            .ToListAsync();

        // Segunda consulta optimizada con proyección
        var query2Data = await Context.PublishingOrders
            .Where(op =>
                (!op.Deleted.HasValue || !op.Deleted.Value) &&
                !op.Latent &&
                (param.ProductType == -1 || op.ProductEdition.Product.ProductTypeId == param.ProductType) &&
                (param.Product == -1 || op.ProductEdition.ProductId == param.Product) &&
                (param.ProductEdition == -1 || op.ProductEditionId == param.ProductEdition) &&
                (toDate.HasValue && op.ProductEdition.End <= toDate.Value) &&
                (param.FromDate.HasValue ? op.ProductEdition.End >= param.FromDate.Value.Date : true) &&
                (param.SelledId == -1 || op.SellerId == param.SelledId)
            )
            .Select(op => new
            {
                // Datos para agrupación por vendedor
                op.SellerId,
                SellerInitials = op.Seller.Initials,

                // Datos para agrupación por edición
                op.ProductEditionId,
                ProductEditionCode = op.ProductEdition.Code,

                // Datos para agrupación por cuenta
                ProductIVA = op.ProductEdition.Product.IVA,
                CurrencyName = op.Contract != null ? op.Contract.Currency.Name : "",

                // Datos para cálculos de comisiones y totales
                Quantity = (double)op.Quantity,
                CurrencyId = op.Contract != null ? op.Contract.CurrencyId : 0,
                ContractSellerId = op.Contract != null ? op.Contract.SellerId : 0,
                op.SoldSpaceId,
                UnitPriceWithDiscounts = op.Contract != null && op.Contract.SoldSpaces.Any(sp => sp.Id == op.SoldSpaceId)
                    ? op.Contract.SoldSpaces.First(sp => sp.Id == op.SoldSpaceId).UnitPriceWithDiscounts : (double)0,
                SubTotal = op.Contract != null && op.Contract.SoldSpaces.Any(sp => sp.Id == op.SoldSpaceId)
                    ? op.Contract.SoldSpaces.First(sp => sp.Id == op.SoldSpaceId).SubTotal : 0,
                TotalDiscounts = op.Contract != null && op.Contract.SoldSpaces.Any(sp => sp.Id == op.SoldSpaceId)
                    ? op.Contract.SoldSpaces.First(sp => sp.Id == op.SoldSpaceId).TotalDiscounts : 0,
                SpaceQuantity = op.Contract != null && op.Contract.SoldSpaces.Any(sp => sp.Id == op.SoldSpaceId)
                    ? op.Contract.SoldSpaces.First(sp => sp.Id == op.SoldSpaceId).Quantity : 0,
                TypeSpecialDiscount = op.Contract != null && op.Contract.SoldSpaces.Any(sp => sp.Id == op.SoldSpaceId)
                    ? op.Contract.SoldSpaces.First(sp => sp.Id == op.SoldSpaceId).TypeSpecialDiscount : null,
                SpecialDiscount = op.Contract != null && op.Contract.SoldSpaces.Any(sp => sp.Id == op.SoldSpaceId)
                    ? op.Contract.SoldSpaces.First(sp => sp.Id == op.SoldSpaceId).SpecialDiscount : 0,
                SellerCommisionCoeficient = op.Seller.CommisionCoeficient,
                op.ProductEdition.Product.AliquotForSalesCommission,
                BillingConditionName = op.Contract != null ? op.Contract.BillingCondition.Name : "",
                ContractPaidOut = op.Contract != null ? op.Contract.PaidOut : null,
                OrderPaidOut = op.PaidOut
            })
            .AsNoTracking()
            .ToListAsync();

        // Agrupamos por vendedor con datos proyectados
        var grid2 = query2Data
            .GroupBy(g => new { Id = g.SellerId, Name = g.SellerInitials })
            .Select(d => new OrdersGroupBySeller
            {
                SellerId = d.Key.Id,
                Seller = d.Key.Name,
                Totals = GetTotalesOptimizado(d, param.NoComisionarImpagas)
            }).ToList();

        // Agrupamos por edición con datos proyectados
        var grid3 = query2Data
            .GroupBy(g => new { Id = g.ProductEditionId, Name = g.ProductEditionCode })
            .Select(d => new OrdersGroupByEdition
            {
                ProductEditionId = d.Key.Id,
                ProductEdition = d.Key.Name,
                Totals = GetTotalesOptimizado(d, param.NoComisionarImpagas)
            }).ToList();

        // Agrupamos por cuenta con datos proyectados
        var grid4 = query2Data
            .Where(g => !string.IsNullOrEmpty(g.CurrencyName)) // Evitar nulls
            .GroupBy(g => new { IVA = g.ProductIVA, Moneda = g.CurrencyName })
            .Select(d => new OrdersGroupByCuenta
            {
                Currency = d.Key.Moneda,
                IVA = d.Key.IVA,
                Totals = GetTotalesOptimizado(d, param.NoComisionarImpagas)
            }).ToList();

        return Ok(new { Main = grid1, BySeller = grid2, ByEdition = grid3, ByCuenta = grid4 });
    }

    [HttpGet("GetOrdersForProduction/{productId}/{productEditionId}/{onlyNews}")]
    public async Task<ActionResult> GetOrdersForProduction(long productId, long productEditionId, bool onlyNews)
    {
        DateTime fechaReporte = DateTime.MinValue;

        if (onlyNews)
        {
            var query = Context.ReportOPForProductionExports.Where(x => x.ProductEditionId == productEditionId);
            if (query.Count() > 0)
            {
                fechaReporte = query.Max(y => y.Date);
            }
        }

        List<OrdersForProductionDto> data = Context.PublishingOrders
                                                .Include(po => po.Contract)
                                                .Include(po => po.AdvertisingSpaceLocationType)
                                                .Include(po => po.ProductAdvertisingSpace)
                                                .Include(po => po.SoldSpace)
                                                .ThenInclude(ss => ss.AdvertisingSpaceLocationType)
                                                .Include(po => po.SoldSpace)
                                                .ThenInclude(ss => ss.ProductAdvertisingSpace)
            .Join(
                Context.Clients,
                po => po.ClientId,
                cl => cl.Id,
                (po, cl) => new
                {
                    PublishingOrder = po,
                    Contract = po.Contract,
                    SoldSpace = po.SoldSpace,
                    AdvertisingSpaceLocationTypeFromPO = po.AdvertisingSpaceLocationType,
                    ProductAdvertisingSpaceFromPO = po.ProductAdvertisingSpace,
                    AdvertisingSpaceLocationTypeFromSS = po.SoldSpace.AdvertisingSpaceLocationType,
                    ProductAdvertisingSpaceFromSS = po.SoldSpace.ProductAdvertisingSpace,
                    Client = cl
                })
            .Join(
                Context.ApplicationUsers,
                opcsppasaslcl => opcsppasaslcl.Client.ApplicationUserSellerId,
                u => u.Id,
                (opcsppasaslcl, u) => new
                {
                    opcsppasaslcl.Contract,
                    opcsppasaslcl.PublishingOrder,
                    opcsppasaslcl.SoldSpace,
                    opcsppasaslcl.AdvertisingSpaceLocationTypeFromPO,
                    opcsppasaslcl.ProductAdvertisingSpaceFromPO,
                    opcsppasaslcl.AdvertisingSpaceLocationTypeFromSS,
                    opcsppasaslcl.ProductAdvertisingSpaceFromSS,
                    opcsppasaslcl.Client,
                    Seller = u
                })
             .Join(
                Context.ProductEditions,
                opcsppasaslclu => opcsppasaslclu.PublishingOrder.ProductEditionId,
                pe => pe.Id,
                (opcsppasaslclu, pe) => new
                {
                    opcsppasaslclu.Contract,
                    opcsppasaslclu.PublishingOrder,
                    opcsppasaslclu.SoldSpace,
                    opcsppasaslclu.AdvertisingSpaceLocationTypeFromPO,
                    opcsppasaslclu.ProductAdvertisingSpaceFromPO,
                    opcsppasaslclu.AdvertisingSpaceLocationTypeFromSS,
                    opcsppasaslclu.ProductAdvertisingSpaceFromSS,
                    opcsppasaslclu.Client,
                    opcsppasaslclu.Seller,
                    ProductsEdition = pe
                })
            .Join(
                Context.Products,
                opcsppasaslclupe => opcsppasaslclupe.ProductsEdition.ProductId,
                p => p.Id,
                (opcsppasaslclupe, p) => new
                {
                    opcsppasaslclupe.Contract,
                    opcsppasaslclupe.PublishingOrder,
                    opcsppasaslclupe.SoldSpace,
                    opcsppasaslclupe.AdvertisingSpaceLocationTypeFromPO,
                    opcsppasaslclupe.ProductAdvertisingSpaceFromPO,
                    opcsppasaslclupe.AdvertisingSpaceLocationTypeFromSS,
                    opcsppasaslclupe.ProductAdvertisingSpaceFromSS,
                    opcsppasaslclupe.Client,
                    opcsppasaslclupe.Seller,
                    opcsppasaslclupe.ProductsEdition,
                    Product = p
                }
            )
            .Where(op => op.ProductsEdition.ProductId == productId
                && (op.PublishingOrder.ProductEditionId == productEditionId || productEditionId == -1)
                && op.PublishingOrder.LastUpdate >= fechaReporte
                && (!op.PublishingOrder.Deleted.HasValue || !op.PublishingOrder.Deleted.Value)
            )
            .Select(x => new OrdersForProductionDto
            {
                BrandName = x.Client.BrandName,
                BxA = x.ProductAdvertisingSpaceFromSS != null ? x.ProductAdvertisingSpaceFromSS.Width.ToString() + " x " + x.ProductAdvertisingSpaceFromSS.Height.ToString()
                    : x.ProductAdvertisingSpaceFromPO.Width.ToString() + " x " + x.ProductAdvertisingSpaceFromPO.Height.ToString(),
                ContractId = x.Contract != null ? x.Contract.Number : 0,
                ContractName = x.Contract != null ? x.Contract.Name : "LATENTE",
                LegalName = x.Client.LegalName,
                Observations = x.PublishingOrder.Observations,
                Seller = x.Seller.Initials,
                SpaceLocation = x.AdvertisingSpaceLocationTypeFromSS != null ? x.AdvertisingSpaceLocationTypeFromSS.Name : x.AdvertisingSpaceLocationTypeFromPO.Name,
                SpaceType = x.ProductAdvertisingSpaceFromSS != null ? x.ProductAdvertisingSpaceFromSS.Name : x.ProductAdvertisingSpaceFromSS.Name,
                Product = x.Product.Name,
                ProductEdition = x.ProductsEdition.Name,
                PublishingOrderId = x.PublishingOrder.Id,
                CreationDate = x.PublishingOrder.CreationDate,
                PageNumber = x.PublishingOrder.PageNumber,
                Quantity = x.PublishingOrder.Quantity
            })
            .OrderBy(o => o.ContractId)
            .ToList();
        return Ok(data);
    }
}
