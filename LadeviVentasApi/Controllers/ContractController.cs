namespace LadeviVentasApi.Controllers;

using System.Reflection;
using AutoMapper;
using KendoNET.DynamicLinq;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

[Route("api/[controller]")]
[ApiController]
public class ContractController : RestController<Contract, ContractWritingDto>
{
    private readonly IOptions<Configuration> _options;

    public ContractController(ApplicationDbContext context, IMapper mapper, IOptions<Configuration> options) : base(context, mapper)
    {
        this._options = options;
    }

    [HttpPost("Search")]
    public override DataSourceResult Search([FromBody] KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
    {
        bool isSeller = CurrentAppUser.Value.ApplicationRole.IsSeller();
        if (isSeller && request.filter != null)
        {
            request.filter.Filters = request.filter.Filters.Where(x => x.Field == null || x.Field.ToLower() != "sellerid");

            if (request.filter.Filters.Count() == 0)
            {
                request.filter = null;
            }

        }
        return base.Search(request);
    }

    [HttpGet("GetContractByClient/{clientId}")]
    public async Task<ActionResult> GetPendingContracts(long clientId)
    {
        var contracts = Context.Contracts
            .Include(c => c.Client)
            .Include(c => c.Product)
            .Include(c => c.Seller)
            .Include(c => c.Currency)
            .Include(c => c.SoldSpaces)
                .ThenInclude(sp => sp.ProductAdvertisingSpace)
            .Include(c => c.SoldSpaces)
                .ThenInclude(sp => sp.AdvertisingSpaceLocationType)
            .AsNoTracking()
            .Where(c => (!c.Deleted.HasValue || !c.Deleted.Value) &&
                c.Total != 0 &&
                c.ClientId == clientId &&
                c.Client.XubioId.HasValue &&
                c.BillingConditionId == 1 &&
                c.End >= DateTime.Now &&
                string.IsNullOrWhiteSpace(c.InvoiceNumber) &&
                c.SoldSpaces.Any(sp => (!sp.Deleted.HasValue || !sp.Deleted.Value) && string.IsNullOrWhiteSpace(sp.XubioDocumentNumber)))
            .Select(c => new
            {
                c.Id,
                c.Number,
                ClientBrandName = c.Client.BrandName,
                ProductName = c.Product.Name,
                c.ProductId,
                XubioProductCode = !c.Client.IsComtur ? c.Product.XubioProductCode : c.Product.ComturXubioProductCode,
                c.SellerId,
                SellerFullName = c.Seller.FullName,
                c.Name,
                Balance = c.SoldSpaces.Select(sp => sp.Balance),
                c.End,
                CurrencyName = c.UseEuro ? "EUR" : c.Currency.Name,
                SoldSpaces = c.SoldSpaces.Where(sp => (!sp.Deleted.HasValue || !sp.Deleted.Value) && string.IsNullOrWhiteSpace(sp.XubioDocumentNumber)).Select(sp => new
                {
                    sp.Id,
                    ProductAdvertisingSpaceName = sp.ProductAdvertisingSpace.Name,
                    AdvertisingSpaceLocationTypeName = sp.AdvertisingSpaceLocationType.Name,
                    sp.Quantity,
                    Total = sp.SubTotal - sp.TotalDiscounts,
                    sp.TotalTaxes,
                    sp.UnitPriceWithDiscounts
                })
            })
            .ToList();

        return Ok(contracts);
    }


    [HttpGet("GetClientsWithBalance/{clientId}/{productId}")]
    public async Task<ActionResult> GetClientsWithBalance(long clientId, long productId)
    {

        var user = CurrentAppUser.Value;
        var isSeller = user.ApplicationRole.Name == ApplicationRole.COMTURSellerRole || user.ApplicationRole.Name == ApplicationRole.NationalSellerRole;

        var clients = Context.Clients
        .Where(c => (!c.Deleted.HasValue || !c.Deleted.Value) && !isSeller || c.ApplicationUserSellerId == user.Id)
        .Select(c => new
        {
            c.Id,
            c.BrandName,
            c.LegalName,
            c.IsEnabled,
            c.ApplicationUserSellerId
        })
        .ToList();

        var clientIds = clients.Select(c => c.Id).ToList();
        var contracts = Context.Contracts
                .Include(c => c.BillingCondition)
                .Include(c => c.SoldSpaces)
                .Where(c => clientIds.Contains(c.ClientId)
                    && (!c.Deleted.HasValue || !c.Deleted.Value)
                    && c.End >= DateTime.Now
                    && (c.BillingCondition.Name != BillingCondition.Anticipated
                        || (c.BillingCondition.Name == BillingCondition.Anticipated && !String.IsNullOrEmpty(c.InvoiceNumber)))
                    && c.ProductId == productId
                    && c.SoldSpaces.Count(sp => sp.Balance > 0) > 0)
                .ToList();

        foreach (var cl in clients.ToList())
        {
            var contractsByClient = contracts.Where(c => c.ClientId == cl.Id);

            if (contractsByClient.Count() == 0 && cl.Id != clientId)
            {
                clients.Remove(cl);
            }
        }

        return Ok(clients);
    }

    [HttpGet("GetContractsForOP/{clientId}/{productId}/{contractId}")]
    public async Task<IActionResult> GetContractsForOP(long clientId, long productId, long contractId)
    {
        var contracts = GetQueryableWithIncludes()
                        .Where(x => (!x.Deleted.HasValue || !x.Deleted.Value)
                            && x.ClientId == clientId
                            && x.ProductId == productId
                            && x.End >= DateTime.Now
                            && (x.BillingCondition.Name != BillingCondition.Anticipated || (x.BillingCondition.Name == BillingCondition.Anticipated && !String.IsNullOrEmpty(x.InvoiceNumber)))
                                ).ToList();

        var contractWithSaldo = contracts.Where(x => x.SoldSpaces.Sum(sp => sp.Quantity) > x.PublishingOrders.Where(y => !y.Deleted.HasValue || !y.Deleted.Value).Sum(op => op.Quantity));

        var result = contractWithSaldo.Select(x => new
        {
            x.Id,
            x.Name,
            x.SellerId,
            x.BillingConditionId
        })
            .ToList();

        //Si es una edicion de op y tiene contrato tengo que traer el mismo por mas que no tenga saldo
        if (contractId > 0 && !result.Exists(x => x.Id == contractId))
        {
            var actualContract = Context.Contracts.Single(x => x.Id == contractId);
            result.Add(new
            {
                actualContract.Id,
                actualContract.Name,
                actualContract.SellerId,
                actualContract.BillingConditionId
            });
        }

        return Ok(result);
    }

    [HttpGet("GetAdversitingSpaceLocation/{latent}/{soldSpaceId}/{productId}/{opId}")]
    public async Task<IActionResult> GetAdversitingSpaceLocation(bool latent, long soldSpaceId, long productId, long opId)
    {
        var ubicaciones = new List<AdvertisingSpaceLocationType>();
        if (latent)
        {
            List<long> ubicacionesId = Context.Products
                .Include(p => p.ProductLocationDiscounts)
                .Where(p => p.Id == productId)
                .SelectMany(p => p.ProductLocationDiscounts
                    .Select(pld => pld.AdvertisingSpaceLocationTypeId)).ToList();

            ubicaciones = Context.AdvertisingSpaceLocationTypes
                .Where(x => ubicacionesId.Contains(x.Id) && x.Name != AdvertisingSpaceLocationType.RotaryLocation).ToList();
        }
        else
        {
            var soldSpace = Context.SoldSpaces.Include(sp => sp.AdvertisingSpaceLocationType).Single(x => x.Id == soldSpaceId);

            if (soldSpace.AdvertisingSpaceLocationTypeId == _options.Value.RotativaID)
            {
                // var beforeCentral = Context.AdvertisingSpaceLocationTypes.Single(x => x.Name == AdvertisingSpaceLocationType.BeforeCentral);
                // var afterCentral = Context.AdvertisingSpaceLocationTypes.Single(x => x.Name == AdvertisingSpaceLocationType.AfterCentral);

                var beforeCentral = Context.AdvertisingSpaceLocationTypes.Single(x => x.Id == _options.Value.AntesCentralID);
                var afterCentral = Context.AdvertisingSpaceLocationTypes.Single(x => x.Id == _options.Value.DespuesCentralID);

                //Chequeamos el saldo para ver si cumplen con el balance 50-50
                var ops = Context.PublishingOrders.Include(u => u.AdvertisingSpaceLocationType).Where(x => x.SoldSpaceId == soldSpaceId && (!x.Deleted.HasValue || !x.Deleted.Value)).ToList();
                // var opsBC = ops.Where(x => x.AdvertisingSpaceLocationType.Name == AdvertisingSpaceLocationType.BeforeCentral).ToList();
                var opsBC = ops.Where(x => x.AdvertisingSpaceLocationTypeId == _options.Value.AntesCentralID).ToList();
                double quantityOpsBC = opsBC == null ? 0 : opsBC.Sum(x => x.Quantity);
                //var opsAC = ops.Where(x => x.AdvertisingSpaceLocationType.Name == AdvertisingSpaceLocationType.AfterCentral).ToList();
                var opsAC = ops.Where(x => x.AdvertisingSpaceLocationTypeId == _options.Value.DespuesCentralID).ToList();
                double quantityOpsAC = opsAC == null ? 0 : opsAC.Sum(x => x.Quantity);

                bool isPairQuantity = (soldSpace.Quantity % 2 == 0);

                if (isPairQuantity)
                {
                    if (quantityOpsBC / soldSpace.Quantity < 0.5)
                    {
                        ubicaciones.Add(beforeCentral);
                    }

                    if (quantityOpsAC / soldSpace.Quantity < 0.5)
                    {
                        ubicaciones.Add(afterCentral);
                    }
                }
                else
                {
                    if (quantityOpsBC / (soldSpace.Quantity - 1) == 0.5 && quantityOpsAC / (soldSpace.Quantity - 1) == 0.5)
                    {
                        ubicaciones.Add(afterCentral);
                    }
                    else
                    {
                        if (quantityOpsBC == 0 || quantityOpsBC / (soldSpace.Quantity - 1) < 0.5)
                        {
                            ubicaciones.Add(beforeCentral);
                        }

                        if (quantityOpsAC == 0 || quantityOpsAC / (soldSpace.Quantity - 1) <= 0.5)
                        {
                            ubicaciones.Add(afterCentral);
                        }
                    }
                }

                //Agregamos la ubicacion propia de la op si es una edicion
                if (opId != -1)
                {
                    PublishingOrder op = ops.FirstOrDefault(x => x.Id == opId);
                    if (!ubicaciones.Exists(x => x.Id == op.AdvertisingSpaceLocationTypeId))
                        ubicaciones.Add(op.AdvertisingSpaceLocationTypeId == _options.Value.AntesCentralID ? beforeCentral : afterCentral);
                }
            }
            else
            {
                ubicaciones.Add(soldSpace.AdvertisingSpaceLocationType);
            }
        }

        return Ok(ubicaciones);
    }

    [HttpGet("GetSpaceTypesWithBalance/{latent}/{contractId}/{productId}/{soldSpaceId}")]
    public async Task<IActionResult> GetSpaceTypesWithBalance(bool latent, long contractId, long productId, long? soldSpaceId)
    {
        var spaceTypes = new List<ProductAdvertisingSpace>();
        if (latent)
        {
            spaceTypes = Context.ProductAdvertisingSpaces.Where(x => x.ProductId == productId).ToList();
            return Ok(spaceTypes);
        }

        // var contract = GetById(contractId).Result.Value;
        var contract = GetQueryableWithIncludes()
            .Single(x => x.Id == contractId);

        var soldSpaces = contract.SoldSpaces;
        List<SoldSpace> soldSpacesWithBalance = new List<SoldSpace>();

        foreach (var sp in soldSpaces)
        {
            var ops = contract.PublishingOrders.Where(op => op.ProductAdvertisingSpaceId == sp.ProductAdvertisingSpaceId
                    && op.AdvertisingSpaceLocationTypeId == sp.AdvertisingSpaceLocationTypeId
                    && op.SoldSpaceId == sp.Id
                    && (!op.Deleted.HasValue || !op.Deleted.Value));

            double opQuantity = ops.Sum(x => x.Quantity);
            //Si es para la edicion de una op debemos traer el sp de la op aunque no tenga saldo
            if (opQuantity < sp.Quantity || (soldSpaceId.HasValue && soldSpaceId.Value == sp.Id))
            {
                // ProductAdvertisingSpace pas = Context.ProductAdvertisingSpaces.Single(x => x.Id == sp.ProductAdvertisingSpaceId);
                // spaceTypes.Add(pas);
                soldSpacesWithBalance.Add(sp);
            }
        }

        var result = soldSpacesWithBalance.Select(sp => new
        {
            Id = sp.Id,
            Name = sp.ProductAdvertisingSpace.Name,
            ProductAdvertisingSpaceId = sp.ProductAdvertisingSpaceId
        }).ToList();

        return Ok(result);
    }

    public override async Task<IActionResult> Post(ContractWritingDto x)
    {
        ControllerContext.HttpContext.Items["current-user"] = CurrentAppUser;
        Contract contract = Mapper.Map<Contract>(x);

        contract.ContractHistoricals = new List<ContractHistorical> {
            new ContractHistorical
            {
                Date = DateTime.Now,
                User = CurrentAppUser.Value.FullName,
                Changes = "Creación del contrato"
            }};

        contract.Number = Context.Contracts.Count() > 0 ? Context.Contracts.Max(c => c.Number) + 1 : 1;
        contract.ContractDate = DateTime.Now;

        //Calculamos los totales de los espacios vendidos
        foreach (SoldSpace sp in contract.SoldSpaces)
        {
            double checkDiscount = sp.ApplyDiscountForCheck ? sp.DiscountForCheck.Value : 0;
            double loyaltyDiscount = sp.ApplyDiscountForLoyalty ? sp.DiscountForLoyalty.Value : 0;
            double sameCountryDiscount = sp.ApplyDiscountForSameCountry ? sp.DiscountForSameCountry.Value : 0;
            double otherCountryDiscount = sp.ApplyDiscountForOtherCountry ? sp.DiscountForOtherCountry.Value : 0;
            double agencyDiscount = sp.AppyDiscountForAgency ? sp.DiscountForAgency.Value : 0;
            double volumeDiscount = sp.ApplyDiscountForVolume ? sp.DiscountForVolume.Value : 0;

            sp.SubTotal = Math.Round(sp.Quantity * sp.SpacePrice * (!x.CurrencyParity.HasValue || contract.CurrencyId == 2 ? 1 : x.CurrencyParity.Value), 2, MidpointRounding.AwayFromZero);

            double newSubtotal = sp.SubTotal - (sp.SubTotal * checkDiscount / 100);
            newSubtotal = newSubtotal - (newSubtotal * loyaltyDiscount / 100);
            newSubtotal = newSubtotal - (newSubtotal * sameCountryDiscount / 100);
            newSubtotal = newSubtotal - (newSubtotal * otherCountryDiscount / 100);
            newSubtotal = newSubtotal - (newSubtotal * agencyDiscount / 100);
            newSubtotal = newSubtotal - (newSubtotal * volumeDiscount / 100);

            newSubtotal = newSubtotal - (newSubtotal * (sp.SpecialDiscount * (sp.TypeSpecialDiscount == 2 ? -1 : 1)) / 100);
            newSubtotal = newSubtotal - (newSubtotal * (sp.GerentialDiscount * (sp.TypeGerentialDiscount == 2 ? -1 : 1)) / 100);
            newSubtotal = newSubtotal - (newSubtotal * sp.LocationDiscount / 100);

            sp.TotalDiscounts = Math.Round(sp.SubTotal - newSubtotal, 2, MidpointRounding.AwayFromZero);
            sp.TotalTaxes = Math.Round((sp.SubTotal - sp.TotalDiscounts) * x.IVA, 2, MidpointRounding.AwayFromZero);
            //El total no se calcula ya que se puede ingresar manualmente y para evitar diferencias por redondeo se toma directamente
            //sp.Total = sp.SubTotal - sp.TotalDiscounts + sp.TotalTaxes;
            sp.Balance = sp.Quantity;
        }

        //Calculamos el total del contrato
        double totalNeto = contract.SoldSpaces.Sum(sp => sp.SubTotal);

        double totalDescuentos = contract.SoldSpaces.Sum(sp => sp.TotalDiscounts);

        contract.TotalDiscounts = Math.Round(totalDescuentos, 2, MidpointRounding.AwayFromZero);
        contract.TotalTaxes = Math.Round((totalNeto - totalDescuentos) * x.IVA, 2, MidpointRounding.AwayFromZero);
        contract.Total = Math.Round(totalNeto - totalDescuentos + contract.TotalTaxes, 2, MidpointRounding.AwayFromZero);

        //Generamos los pagos en cheque si corresponden
        if (contract.CheckQuantity > 0 && contract.DaysToFirstPayment > 0 && contract.DaysBetweenChecks > 0)
        {
            contract.CheckPayments = new List<CheckPayment>();
            for (int i = 0; i < contract.CheckQuantity; i++)
            {
                CheckPayment cp = new CheckPayment();
                cp.Order = i + 1;
                cp.Date = DateTime.Now.AddDays(i * contract.DaysBetweenChecks.Value + contract.DaysToFirstPayment.Value);
                cp.Total = contract.Total / contract.CheckQuantity.Value;
                contract.CheckPayments.Add(cp);
            }
        }

        TryValidateModel(contract);
        if (ModelState.IsValid)
        {
            Context.Add(contract);
            Context.SaveChanges();

            #region Auditory
            try
            {
                Auditory audit = new Auditory();
                audit.Date = DateTime.Now;
                audit.Entity = "Contrato";
                audit.UserId = CurrentAppUser.Value.Id;
                audit.User = CurrentAppUser.Value.FullName;
                string name = contract.Name;
                audit.AuditMessage = "Creación de " + audit.Entity + ". Id=" + contract.Id.ToString() + (string.IsNullOrEmpty(name) ? "" : ". Nombre= " + name);
                Context.Add(audit);
                await Context.SaveChangesAsync();
            }
            //El catch vacio es simplemente para que un error aqui no interrumpa el proceso normal. Deberia ir algun tipo de log.
            catch (Exception ex)
            {
            }
            #endregion
            x.Number = contract.Number;
            return Created(nameof(Post), x);
        }


        return ActionResultForModelStateValidation();
    }

    public override Task<ActionResult<Contract>> Delete(long id)
    {
        EnsureUserRole(UsersRole.Superuser, UsersRole.Supervisor);

        return base.Delete(id);
    }

    private static List<string> GenerateAuditLogMessages(Contract originalObject, Contract changedObject)
    {
        List<string> list = new List<string>();
        string className = string.Concat("[", originalObject.GetType().Name, "] ");

        foreach (PropertyInfo property in originalObject.GetType().GetProperties())
        {
            //Type comparable =  property.PropertyType.GetInterface("System.IEquatable");

            //if (comparable != null)
            //{
            object originalPropertyValue = property.GetValue(originalObject, null);
            object newPropertyValue = property.GetValue(changedObject, null);

            if (originalPropertyValue != null && !originalPropertyValue.Equals(newPropertyValue))
            {
                list.Add(string.Concat(property.Name,
                    "': '", (originalPropertyValue != null) ? originalPropertyValue.ToString() : "[NULL]",
                    "' >> '", (newPropertyValue != null) ? newPropertyValue.ToString() : "[NULL]", "'"));
            }
            //}
        }

        return list;
    }

    public override async Task<IActionResult> Put(long id, ContractWritingDto x)
    {
        Contract oldContract = Context.Contracts
                .Include(c => c.SoldSpaces)
                .AsNoTracking().Single(c => c.Id == id);

        ControllerContext.HttpContext.Items["current-user"] = CurrentAppUser;
        if (id != x.Id) return BadRequest();

        Contract contractToUpdate = Mapper.Map<Contract>(x);
        //El number el calculado y como no cambia y el dto no lo trae lo obtenemos de la entidad original
        contractToUpdate.Number = oldContract.Number;
        contractToUpdate.ContractDate = oldContract.ContractDate;

        var product = Context.Products
            .Include(v => v.ProductVolumeDiscounts)
            .Include(p => p.ProductLocationDiscounts)
            .Single(p => p.Id == contractToUpdate.ProductId);

        List<ContractHistorical> changeAudit = new List<ContractHistorical>();
        //Calculamos los totales de los espacios vendidos
        foreach (SoldSpace sp in contractToUpdate.SoldSpaces)
        {
            double checkDiscount = sp.ApplyDiscountForCheck ? sp.DiscountForCheck.Value : 0;
            double loyaltyDiscount = sp.ApplyDiscountForLoyalty ? sp.DiscountForLoyalty.Value : 0;
            double sameCountryDiscount = sp.ApplyDiscountForSameCountry ? sp.DiscountForSameCountry.Value : 0;
            double otherCountryDiscount = sp.ApplyDiscountForOtherCountry ? sp.DiscountForOtherCountry.Value : 0;
            double agencyDiscount = sp.AppyDiscountForAgency ? sp.DiscountForAgency.Value : 0;
            double volumeDiscount = sp.ApplyDiscountForVolume ? sp.DiscountForVolume.Value : 0;

            sp.SubTotal = Math.Round(sp.Quantity * sp.SpacePrice * (!x.CurrencyParity.HasValue || contractToUpdate.CurrencyId == 2 ? 1 : x.CurrencyParity.Value), 2, MidpointRounding.AwayFromZero);

            double newSubtotal = sp.SubTotal - (sp.SubTotal * checkDiscount / 100);
            newSubtotal = newSubtotal - (newSubtotal * loyaltyDiscount / 100);
            newSubtotal = newSubtotal - (newSubtotal * sameCountryDiscount / 100);
            newSubtotal = newSubtotal - (newSubtotal * otherCountryDiscount / 100);
            newSubtotal = newSubtotal - (newSubtotal * agencyDiscount / 100);
            newSubtotal = newSubtotal - (newSubtotal * volumeDiscount / 100);
            newSubtotal = newSubtotal - (newSubtotal * (sp.SpecialDiscount * (sp.TypeSpecialDiscount == 2 ? -1 : 1)) / 100);
            newSubtotal = newSubtotal - (newSubtotal * (sp.GerentialDiscount * (sp.TypeGerentialDiscount == 2 ? -1 : 1)) / 100);
            newSubtotal = newSubtotal - (newSubtotal * sp.LocationDiscount / 100);

            sp.TotalDiscounts = Math.Round(sp.SubTotal - newSubtotal, 2, MidpointRounding.AwayFromZero);
            sp.TotalTaxes = Math.Round((sp.SubTotal - sp.TotalDiscounts) * x.IVA, 2, MidpointRounding.AwayFromZero);

            //Obtenemos las op vinculadas al espacio
            var ops = Context.PublishingOrders.Where(op => op.SoldSpaceId == sp.Id && (!op.Deleted.HasValue || !op.Deleted.Value)).ToList();

            sp.Balance = sp.Quantity - ops.Sum(y => y.Quantity);

            sp.ContractId = contractToUpdate.Id;

            List<string> changesSP = new List<string>();
            SoldSpace oldSoldSpace = Context.SoldSpaces.AsNoTracking().FirstOrDefault(y => y.Id == sp.Id);
            if (oldSoldSpace != null)
            {
                changesSP = Utils.GenerateAuditLogMessages(oldSoldSpace, sp);
            }

            if (changesSP.Count > 0)
            {
                changeAudit.Add(new ContractHistorical
                {
                    Date = DateTime.Now,
                    User = CurrentAppUser.Value.FullName,
                    Changes = "Modificación en espacios vendidos: " + string.Join("|", changesSP),
                    ContractId = id
                });
            }
        }

        //Recalculamos los totales
        //Calculamos el total del contrato
        double totalNeto = contractToUpdate.SoldSpaces.Sum(sp => sp.SubTotal);

        double totalDescuentos = contractToUpdate.SoldSpaces.Sum(sp => sp.TotalDiscounts);

        contractToUpdate.TotalDiscounts = Math.Round(totalDescuentos, 2, MidpointRounding.AwayFromZero);
        contractToUpdate.TotalTaxes = Math.Round((totalNeto - totalDescuentos) * x.IVA, 2, MidpointRounding.AwayFromZero);
        contractToUpdate.Total = Math.Round(totalNeto - totalDescuentos + contractToUpdate.TotalTaxes, 2, MidpointRounding.AwayFromZero);

        TryValidateModel(contractToUpdate);
        if (!ModelState.IsValid) return ActionResultForModelStateValidation();

        List<string> changes = Utils.GenerateAuditLogMessages(oldContract, contractToUpdate);

        foreach (string c in changes)
        {
            changeAudit.Add(new ContractHistorical
            {
                Date = DateTime.Now,
                User = CurrentAppUser.Value.FullName,
                Changes = c,
                ContractId = id
            });
        }

        #region Auditory
        try
        {
            Auditory audit = new Auditory();
            audit.Date = DateTime.Now;
            audit.Entity = "Contrato";
            audit.UserId = CurrentAppUser.Value.Id;
            audit.User = CurrentAppUser.Value.FullName;
            string name = x.Name;
            audit.AuditMessage = "Id=" + id.ToString() + (string.IsNullOrEmpty(name) ? "" : ". Nombre= " + name) + ". Modificación de " + audit.Entity + ". " + string.Join('|', changes);
            Context.Add(audit);
            await Context.SaveChangesAsync();
        }
        //El catch vacio es simplemente para que un error aqui no interrumpa el proceso normal. Deberia ir algun tipo de log.
        catch (Exception ex)
        {
        }
        #endregion

        try
        {
            var local = Context.Set<Contract>()
                        .Local
                        .FirstOrDefault(entry => entry.Id.Equals(id));

            if (local != null)
            {
                Context.Entry(local).State = EntityState.Detached;
            }

            foreach (var sp in contractToUpdate.SoldSpaces.ToList())
            {
                if (sp.Id > 0)
                {
                    Context.Entry(sp).State = EntityState.Modified;
                }
                else
                {
                    Context.Add(sp);
                }
            }

            foreach (var soldSpacesDeleted in oldContract.SoldSpaces.Where(soldSpace => !contractToUpdate.SoldSpaces.Select(y => y.Id).Contains(soldSpace.Id)))
            {


                Context.Entry(soldSpacesDeleted).State = EntityState.Deleted;
                var changesSP = Utils.GenerateAuditLogMessages(soldSpacesDeleted, null);
                changeAudit.Add(new ContractHistorical
                {
                    Date = DateTime.Now,
                    User = CurrentAppUser.Value.FullName,
                    Changes = "Eliminacion en espacios vendidos: " + string.Join("|", changesSP),
                    ContractId = id
                });
            }

            foreach (var ca in changeAudit)
            {
                Context.Add(ca);
            }

            Context.Entry(contractToUpdate).State = EntityState.Modified;

            await Context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }


        return NoContent();
    }

    protected override IQueryable<Contract> GetQueryableWithIncludes()
    {
        bool isSeller = CurrentAppUser.Value.ApplicationRole.IsSeller();
        long userId = CurrentAppUser.Value.Id;
        List<long> clientsIds = Context.Clients.Where(c => !isSeller || c.ApplicationUserSellerId == userId).Select(c => c.Id).ToList();

        var allContracts = Context.Contracts
            .Include(c => c.Product)
            .Include(c => c.Client)
                .ThenInclude(c => c.ApplicationUserSeller)
            .Include(c => c.Seller)
            .Include(c => c.ContractHistoricals)
            .Include(c => c.SoldSpaces)
                .ThenInclude(sp => sp.ProductAdvertisingSpace)
            .Include(c => c.PublishingOrders)
                .ThenInclude(op => op.AdvertisingSpaceLocationType)
            .Include(c => c.PublishingOrders)
                .ThenInclude(op => op.ProductAdvertisingSpace)
            .Include(c => c.PublishingOrders)
                .ThenInclude(op => op.ProductEdition)
            .AsNoTracking();

        if (isSeller)
        {
            return allContracts.Where(c => c.SellerId == userId || clientsIds.Contains(c.ClientId));
        }

        return allContracts;
    }

    protected override DataSourceResult GetSearchDataSourceResult(KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
    {
        bool isSuperuser = CurrentAppUser.Value.ApplicationRole.IsSuperuser();

        var result = GetSearchQueryable()
            .Select(x => new
            {
                x.Id,
                x.BillingConditionId,
                x.BillingCountryId,
                x.CheckQuantity,
                x.Client,
                x.ClientId,
                x.Client.BrandName,
                x.Client.LegalName,
                x.ContractDate,
                x.CurrencyId,
                x.UseEuro,
                x.DaysBetweenChecks,
                x.DaysToFirstPayment,
                x.End,
                x.InvoiceNumber,
                x.Name,
                x.Number,
                x.PaidOut,
                x.PaymentMethodId,
                x.Product,
                x.ProductId,
                x.Seller,
                x.SellerId,
                x.IVA,
                SoldSpaces = x.SoldSpaces.Select(sp => new
                {
                    sp.AdvertisingSpaceLocationTypeId,
                    sp.ProductAdvertisingSpaceId,
                    sp.TypeSpecialDiscount,
                    sp.TypeGerentialDiscount,
                    sp.Quantity,
                    sp.SpecialDiscount,
                    sp.GerentialDiscount,
                    sp.DescriptionSpecialDiscount,
                    sp.DescriptionGerentialDiscount,
                    Total = sp.Total == 0 ? 0 : Convert.ToDouble(sp.Total.ToString("#.##")),
                    sp.TotalDiscounts,
                    sp.TotalTaxes,
                    sp.Balance,
                    sp.Id,
                    sp.ContractId,
                    sp.LocationDiscount,
                    sp.ApplyDiscountForCheck,
                    sp.ApplyDiscountForLoyalty,
                    sp.ApplyDiscountForOtherCountry,
                    sp.ApplyDiscountForSameCountry,
                    sp.ApplyDiscountForVolume,
                    sp.AppyDiscountForAgency,
                    sp.DiscountForAgency,
                    sp.DiscountForCheck,
                    sp.DiscountForLoyalty,
                    sp.DiscountForOtherCountry,
                    sp.DiscountForSameCountry,
                    sp.DiscountForVolume,
                    SpacePrice = sp.SpacePrice == 0 ? sp.ProductAdvertisingSpace.DollarPrice : sp.SpacePrice,
                    UnitPriceWithDiscounts = sp.UnitPriceWithDiscounts == 0 ? 0 : Convert.ToDouble(sp.UnitPriceWithDiscounts.ToString("#.##"))
                }),
                x.Start,
                x.Total,
                x.TotalDiscounts,
                x.TotalTaxes,
                x.Observations,
                x.CurrencyParity,
                PublishingOrders = x.PublishingOrders.Select(po => new
                {
                    po.Id,
                    po.Quantity,
                    po.Observations,
                    InvoiceNumber = x.SoldSpaces.Where(ss => ss.Id == po.SoldSpaceId).Count() > 0 && x.SoldSpaces.SingleOrDefault(ss => ss.Id == po.SoldSpaceId).UnitPriceWithDiscounts == 0 ? "" : x.BillingCondition.Name == BillingCondition.Anticipated ? x.InvoiceNumber : po.InvoiceNumber,
                    po.AdvertisingSpaceLocationType,
                    po.AdvertisingSpaceLocationTypeId,
                    po.ProductAdvertisingSpace,
                    po.ProductAdvertisingSpaceId,
                    po.ProductEdition,
                    po.ProductEditionId,
                    po.SoldSpaceId,
                    po.Deleted,
                    po.PageNumber,
                    po.SellerId,
                    CanDelete = po.ProductEdition != null && !po.ProductEdition.Closed && string.IsNullOrWhiteSpace(po.PageNumber)
                }).Where(po => !po.Deleted.HasValue || !po.Deleted.Value),
                ContractHistoricals = x.ContractHistoricals.Select(ch => new
                {
                    ch.Changes,
                    Date = ch.Date.ToString("dd/MM/yyyy HH:mm:ss"),
                    ch.User,
                    ch.ContractId
                }),
                x.Deleted,
                x.DeletedDate,
                x.DeletedUser,
                sellerChanged = x.Client.ApplicationUserSellerId != CurrentAppUser.Value.Id && !CurrentAppUser.Value.ApplicationRole.IsSuperuser()
                                                                                            && !CurrentAppUser.Value.ApplicationRole.IsSupervisor(),
                sellerChangeName = x.Client.ApplicationUserSellerId != CurrentAppUser.Value.Id && !CurrentAppUser.Value.ApplicationRole.IsSuperuser()
                                                                                            && !CurrentAppUser.Value.ApplicationRole.IsSupervisor() ? x.Client.ApplicationUserSeller.FullName : ""
            })
            .Where(x => !x.Deleted.HasValue || !x.Deleted.Value)
            .ToDataSourceResult(request);

        //var ids = JObject.FromObject(result.Data).AsJEnumerable().Select(x => x["Id"].ToObject<long>()).ToArray();
        //result.Data = GetQueryableWithIncludes().Where(c => ids.Contains(c.Id)).ToList();

        return result;
    }

    [HttpGet("GetContractHistorial/{contractId}")]
    public async Task<IActionResult> GetContractHistorial(long contractId)
    {
        var data = Context.ContractHistoricals.AsNoTracking().Where(x => x.ContractId == contractId).OrderByDescending(ch => ch.Date).ToList();
        return Ok(data);
    }
}
