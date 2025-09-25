namespace LadeviVentasApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using LadeviVentasApi.Data;
using LadeviVentasApi.Models.Domain;
using LadeviVentasApi.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

[Route("api/[controller]")]
[ApiController]
public class ProductionController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly Lazy<ApplicationUser> currentAppUser;
    private readonly IConfiguration configuration;

    public ProductionController(ApplicationDbContext context, IConfiguration configuration)
    {
        this.context = context;
        this.currentAppUser = new Lazy<ApplicationUser>(GetAppUser, LazyThreadSafetyMode.ExecutionAndPublication);
        this.configuration = configuration;
    }

    private ApplicationUser GetAppUser()
    {
        if (User?.Identity?.IsAuthenticated != true)
            throw new InvalidOperationException("Usuario no autenticado");

        var userMail = User.Claims
            .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value
            ?? throw new InvalidOperationException("Email de usuario no encontrado");

        return this.context.ApplicationUsers
            .Include(u => u.ApplicationRole)
            .Include(u => u.Country)
            .Single(u => u.CredentialsUser.Email.ToLower() == userMail.ToLower());
    }

    [HttpGet("ProductionTemplates")]
    public async Task<IActionResult> GetProductionTemplates(long productEditionId)
    {
        // Verificar que la edición exista y esté válida
        var productEdition = await context.ProductEditions
            .AsNoTracking()
            .Where(pe => (!pe.Deleted.HasValue || !pe.Deleted.Value) && !pe.Closed && pe.Id == productEditionId)
            .FirstOrDefaultAsync();

        if (productEdition == null)
            return NotFound("No se encontró la edición del producto o está cerrada.");

        // Obtener todos los ProductionTemplates con sus slots para esta edición
        var templates = await context.ProductionTemplates
            .Include(pt => pt.ProductionSlots)
                .ThenInclude(ps => ps.InventoryAdvertisingSpace)
                    .ThenInclude(ias => ias.ProductAdvertisingSpace)
            .Include(pt => pt.ProductionSlots)
                .ThenInclude(ps => ps.PublishingOrder)
                    .ThenInclude(po => po.Contract)
                        .ThenInclude(c => c.Client)
            .Include(pt => pt.ProductionSlots)
                .ThenInclude(ps => ps.PublishingOrder.Contract.Seller)
            .AsNoTracking()
            .Where(pt => pt.ProductEditionId == productEditionId &&
                        (!pt.Deleted.HasValue || !pt.Deleted.Value))
            .OrderBy(pt => pt.PageNumber)
            .Select(pt => new ProductionTemplateDto
            {
                id = pt.Id,
                productEditionId = pt.ProductEditionId,
                pageNumber = pt.PageNumber,
                productionSlots = pt.ProductionSlots
                    .Where(ps => !ps.Deleted.HasValue || !ps.Deleted.Value)
                    .OrderBy(ps => ps.SlotNumber)
                    .Select(ps => new ProductionSlotDto
                    {
                        id = ps.Id,
                        productionTemplateId = ps.ProductionTemplateId,
                        slotNumber = ps.SlotNumber,
                        inventoryAdvertisingSpaceId = ps.InventoryAdvertisingSpaceId,
                        productAdvertisingSpaceName = ps.InventoryAdvertisingSpace.ProductAdvertisingSpace != null ? ps.InventoryAdvertisingSpace.ProductAdvertisingSpace.Name : string.Empty,
                        order = ps.PublishingOrderId != null ? new ProductionPublishiingOrderDto
                        {
                            id = ps.PublishingOrder.Id,
                            contractId = ps.PublishingOrder.ContractId,
                            contractName = ps.PublishingOrder.Contract != null ? ps.PublishingOrder.Contract.Name : "LATENTE",
                            clientName = ps.PublishingOrder.Contract != null && ps.PublishingOrder.Contract.Client != null ?
                                    (ps.PublishingOrder.Contract.Client.LegalName ?? ps.PublishingOrder.Contract.Client.BrandName ?? "") : "",
                            sellerName = ps.PublishingOrder.Contract != null && ps.PublishingOrder.Contract.Seller != null ?
                                    ps.PublishingOrder.Contract.Seller.FullName : "",
                        } : null,
                        observations = ps.Observations != null ? ps.Observations : "",
                        isEditorial = ps.IsEditorial,
                        isCA = ps.IsCA
                    }).ToList()
            })
            .ToListAsync();

        return Ok(templates);
    }
}