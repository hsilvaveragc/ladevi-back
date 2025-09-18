namespace LadeviVentasApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using LadeviVentasApi.Data;
using LadeviVentasApi.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LadeviVentasApi.Helpers.Utilities;
using LadeviVentasApi.Projections;

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

    [HttpGet("ProductionInventory")]
    public async Task<IActionResult> GetProductionInventory(long productEditionId)
    {
        // 1. Obtener datos
        var productEdition = await GetProductEditionAsync(productEditionId);
        if (productEdition == null)
            return NotFound("No se encontró la edición del producto.");

        // 2. Validar y determinar tipo de espacios
        var spaceTypeResult = ValidateAndDetermineSpaceType(productEdition.InventoryProductAdvertisingSpaces);
        if (!spaceTypeResult.IsValid)
            return BadRequest(spaceTypeResult.ErrorMessage);

        // 3. Clasificar todos los espacios
        var spaces = ClassifyAllSpaces(productEdition.InventoryProductAdvertisingSpaces, spaceTypeResult.SpaceType);

        // 4. Generar template de producción (todos con Id = 0)
        var productionItems = GenerateProductionTemplate(productEditionId, productEdition.PageCount.Value, spaces);

        // 5. Consultar ProductionItems existentes y reemplazar en template
        await ReplaceWithExistingProductionItems(productionItems, productEditionId);

        // 6. Para OPs sin ProductionItem, crear y persistir nuevos ProductionItems
        await CreateProductionItemsForUnassignedOrders(productionItems, productEditionId, productEdition.PageCount.Value);

        return Ok(productionItems);
    }

    private async Task<ProductionInventoryProjection> GetProductEditionAsync(long productEditionId)
    {
        return await context.ProductEditions
                            .Include(pe => pe.InventoryProductAdvertisingSpaces)
                                .ThenInclude(ipas => ipas.ProductAdvertisingSpace)
                            .AsNoTracking()
                            .Where(pe => (!pe.Deleted.HasValue || !pe.Deleted.Value) && !pe.Closed && pe.PageCount.HasValue && pe.PageCount != 0)
                            .Select(pe => new ProductionInventoryProjection
                            {
                                Id = pe.Id,
                                PageCount = pe.PageCount,
                                InventoryProductAdvertisingSpaces = pe.InventoryProductAdvertisingSpaces.Select(ipas => new InventorySpaceProjection
                                {
                                    Id = ipas.Id,
                                    ProductAdvertisingSpaceId = ipas.ProductAdvertisingSpaceId,
                                    Quantity = ipas.Quantity,
                                    ProductAdvertisingSpaceName = ipas.ProductAdvertisingSpace.Name
                                }).ToList(),
                                Deleted = pe.Deleted
                            })
                            .SingleOrDefaultAsync(pe => pe.Id == productEditionId);
    }

    private (bool IsValid, string ErrorMessage, string SpaceType) ValidateAndDetermineSpaceType(List<InventorySpaceProjection> spaces)
    {
        var hasPrint = spaces.Any(x => x.ProductAdvertisingSpaceName.ToLower().EndsWith("print") && x.Quantity.HasValue && x.Quantity != 0);
        var hasDigital = spaces.Any(x => x.ProductAdvertisingSpaceName.ToLower().EndsWith("digital") && x.Quantity.HasValue && x.Quantity != 0);
        var hasGeneric = spaces.Any(x => !x.ProductAdvertisingSpaceName.ToLower().EndsWith("digital") &&
                                         !x.ProductAdvertisingSpaceName.ToLower().EndsWith("print") &&
                                         x.Quantity.HasValue && x.Quantity != 0);

        var typesCount = (hasPrint ? 1 : 0) + (hasDigital ? 1 : 0) + (hasGeneric ? 1 : 0);

        if (typesCount > 1)
            return (false, "Solo puede haber espacios de un solo tipo: digital, print o sin tipo", null);

        if (typesCount == 0)
            return (false, "No se encontraron espacios publicitarios con cantidad válida", null);

        string spaceType = hasPrint ? "print" : hasDigital ? "digital" : "generic";
        return (true, null, spaceType);
    }

    private ClassifiedSpaces ClassifyAllSpaces(List<InventorySpaceProjection> inventorySpaces, string spaceType)
    {
        return new ClassifiedSpaces
        {
            CoverEye = AdvertisingSpaceClassifier.GetCoverEye(inventorySpaces, spaceType),
            CoverFooter = AdvertisingSpaceClassifier.GetCoverFooter(inventorySpaces, spaceType),
            InsideCover = AdvertisingSpaceClassifier.GetInsideCover(inventorySpaces, spaceType),
            Page = AdvertisingSpaceClassifier.GetPage(inventorySpaces, spaceType),
            InsideBackCover = AdvertisingSpaceClassifier.GetInsideBackCover(inventorySpaces, spaceType),
            BackCover = AdvertisingSpaceClassifier.GetBackCover(inventorySpaces, spaceType),
            HalfPage = AdvertisingSpaceClassifier.GetHalfPage(inventorySpaces, spaceType),
            CuarterPage = AdvertisingSpaceClassifier.GetCuarterPage(inventorySpaces, spaceType),
            FooterPage = AdvertisingSpaceClassifier.GetFooterPage(inventorySpaces, spaceType),
            OtherSpaces = AdvertisingSpaceClassifier.GetOtherSpaces(inventorySpaces, spaceType).ToList()
        };
    }

    private List<ProductionItemDto> GenerateProductionTemplate(long productEditionId, int pageCount, ClassifiedSpaces spaces)
    {
        var productionItems = new List<ProductionItemDto>();

        // Página 1 (Tapa)
        productionItems.AddRange(GenerateCoverPage(productEditionId, spaces));

        // Páginas internas (2 hasta penúltima)
        for (int pageNumber = 2; pageNumber < pageCount; pageNumber++)
        {
            productionItems.AddRange(GenerateInnerPage(productEditionId, pageNumber, pageCount, spaces));
        }

        // Última página (Contratapa)
        productionItems.AddRange(GenerateBackCoverPage(productEditionId, pageCount, spaces));

        return productionItems;
    }

    private List<ProductionItemDto> GenerateCoverPage(long productEditionId, ClassifiedSpaces spaces)
    {
        var items = new List<ProductionItemDto>();
        var slot = 0;

        // Ojos de tapa
        AddSpaceItems(items, spaces.CoverEye, productEditionId, 1, ref slot);

        // Pie de tapa
        AddSpaceItems(items, spaces.CoverFooter, productEditionId, 1, ref slot);

        return items;
    }

    private List<ProductionItemDto> GenerateInnerPage(long productEditionId, int pageNumber, int pageCount, ClassifiedSpaces spaces)
    {
        var items = new List<ProductionItemDto>();
        var slot = 0;

        // Página 2 - Verificar retiro de tapa
        if (pageNumber == 2 && spaces.InsideCover != null)
        {
            AddSpaceItems(items, spaces.InsideCover, productEditionId, pageNumber, ref slot);
            return items;
        }

        // Penúltima página - Verificar retiro de contratapa
        if (pageNumber == pageCount - 1 && spaces.InsideBackCover != null)
        {
            AddSpaceItems(items, spaces.InsideBackCover, productEditionId, pageNumber, ref slot);
            return items;
        }

        // Lógica normal par/impar
        if (pageNumber % 2 == 0)
        {
            // Páginas pares - Página completa
            AddSpaceItems(items, spaces.Page, productEditionId, pageNumber, ref slot);
        }
        else
        {
            // Páginas impares - En orden: medias, cuartos, pies, otros
            AddSpaceItems(items, spaces.HalfPage, productEditionId, pageNumber, ref slot);
            AddSpaceItems(items, spaces.CuarterPage, productEditionId, pageNumber, ref slot);
            AddSpaceItems(items, spaces.FooterPage, productEditionId, pageNumber, ref slot);

            // Otros espacios
            foreach (var otherSpace in spaces.OtherSpaces)
            {
                AddSpaceItems(items, otherSpace, productEditionId, pageNumber, ref slot);
            }
        }

        return items;
    }

    private List<ProductionItemDto> GenerateBackCoverPage(long productEditionId, int pageCount, ClassifiedSpaces spaces)
    {
        var items = new List<ProductionItemDto>();
        var slot = 0;

        AddSpaceItems(items, spaces.BackCover, productEditionId, pageCount, ref slot);

        return items;
    }

    private void AddSpaceItems(List<ProductionItemDto> items, InventorySpaceProjection space, long productEditionId, int pageNumber, ref int slot)
    {
        if (space?.Quantity == null || space.Quantity <= 0) return;

        for (int i = 0; i < space.Quantity; i++)
        {
            slot++;
            items.Add(new ProductionItemDto
            {
                ProductEditionId = productEditionId,
                PageNumber = pageNumber,
                Slot = slot,
                InventoryProductAdvertisingSpaceId = space.Id,
                ProductAdvertisingSpaceName = space.ProductAdvertisingSpaceName,
            });
        }
    }

    private async Task ReplaceWithExistingProductionItems(List<ProductionItemDto> templateItems, long productEditionId)
    {
        // Consultar ProductionItems ya existentes en la BD
        var existingProductionItems = await context.ProductionItems
            .Include(pi => pi.PublishingOrder)
                .ThenInclude(po => po.Contract)
                    .ThenInclude(c => c.Client)
            .Include(pi => pi.PublishingOrder)
                .ThenInclude(po => po.Contract)
                    .ThenInclude(c => c.Seller)
            .Include(pi => pi.InventoryProductAdvertisingSpace)
                .ThenInclude(ipas => ipas.ProductAdvertisingSpace)
            .Where(pi => pi.ProductEditionId == productEditionId &&
                        (!pi.Deleted.HasValue || !pi.Deleted.Value))
            .AsNoTracking()
            .ToListAsync();

        // Reemplazar en el template los ProductionItems que ya existen
        foreach (var existingItem in existingProductionItems)
        {
            var templateItem = templateItems.FirstOrDefault(ti =>
                ti.PageNumber == existingItem.PageNumber &&
                ti.Slot == existingItem.Slot &&
                ti.InventoryProductAdvertisingSpaceId == existingItem.InventoryProductAdvertisingSpaceId);

            if (templateItem != null)
            {
                // Reemplazar con datos del ProductionItem existente
                templateItem.Id = existingItem.Id;
                templateItem.PublishingOrderId = existingItem.PublishingOrderId ?? 0;
                templateItem.ContractName = existingItem.PublishingOrder?.Contract?.Name ?? "LATENTE";
                templateItem.ClientName = existingItem.PublishingOrder?.Contract?.Client?.LegalName ??
                                         existingItem.PublishingOrder?.Contract?.Client?.BrandName ?? "";
                templateItem.SellerName = existingItem.PublishingOrder?.Contract?.Seller?.FullName ?? "";
                templateItem.Observations = existingItem.Observations ?? "";
                templateItem.IsEditorial = existingItem.IsEditorial;
                templateItem.IsCA = existingItem.IsCA;
            }
        }
    }

    private async Task CreateProductionItemsForUnassignedOrders(List<ProductionItemDto> templateItems, long productEditionId, int totalPages)
    {
        // Consultar PublishingOrders que no tienen ProductionItem asignado
        var existingProductionItemOrderIds = await context.ProductionItems
            .Where(pi => pi.ProductEditionId == productEditionId &&
                        pi.PublishingOrderId.HasValue &&
                        (!pi.Deleted.HasValue || !pi.Deleted.Value))
            .Select(pi => pi.PublishingOrderId.Value)
            .ToListAsync();

        var unassignedOrders = await context.PublishingOrders
            .Include(po => po.Contract)
                .ThenInclude(c => c.Client)
            .Include(po => po.Contract)
                .ThenInclude(c => c.Seller)
            .Include(po => po.ProductAdvertisingSpace)
            .Include(po => po.AdvertisingSpaceLocationType)
            .Where(po => po.ProductEditionId == productEditionId &&
                        (!po.Deleted.HasValue || !po.Deleted.Value) &&
                        !existingProductionItemOrderIds.Contains(po.Id)
                        && !po.Latent)
            .OrderBy(po => po.Id)
            .ToListAsync();

        if (!unassignedOrders.Any()) return;

        var newProductionItems = new List<ProductionItem>();

        // Agrupar por ProductAdvertisingSpaceId y asignar
        var ordersBySpace = unassignedOrders.GroupBy(po => po.ProductAdvertisingSpaceId);

        foreach (var spaceGroup in ordersBySpace)
        {
            var productAdvertisingSpaceId = spaceGroup.Key;
            var ordersForSpace = spaceGroup.ToList();

            // Obtener slots disponibles (Id = 0) para este tipo de espacio
            var availableSlots = templateItems
                .Where(ti => ti.Id == 0 && // Solo template items no reemplazados
                            GetProductAdvertisingSpaceId(ti.InventoryProductAdvertisingSpaceId) == productAdvertisingSpaceId)
                .OrderBy(ti => ti.PageNumber)
                .ThenBy(ti => ti.Slot)
                .ToList();

            // Asignar órdenes a slots y crear ProductionItems
            AssignOrdersAndCreateProductionItems(ordersForSpace, availableSlots, totalPages, templateItems, newProductionItems);
        }

        // Insertar nuevos ProductionItems en la BD
        if (newProductionItems.Any())
        {
            context.ProductionItems.AddRange(newProductionItems);
            await context.SaveChangesAsync();

            // Actualizar template con los IDs generados
            for (int i = 0; i < newProductionItems.Count; i++)
            {
                var newItem = newProductionItems[i];
                var templateItem = templateItems.FirstOrDefault(ti =>
                    ti.PageNumber == newItem.PageNumber &&
                    ti.Slot == newItem.Slot &&
                    ti.InventoryProductAdvertisingSpaceId == newItem.InventoryProductAdvertisingSpaceId);

                if (templateItem != null)
                {
                    templateItem.Id = newItem.Id;
                    templateItem.PublishingOrderId = newItem.PublishingOrderId ?? 0;
                    templateItem.ContractName = newItem.PublishingOrder?.Contract?.Name ?? "LATENTE";
                    templateItem.ClientName = newItem.PublishingOrder?.Contract?.Client?.LegalName ??
                                             newItem.PublishingOrder?.Contract?.Client?.BrandName ?? "";
                    templateItem.SellerName = newItem.PublishingOrder?.Contract?.Seller?.FullName ?? "";
                    templateItem.Observations = newItem.Observations ?? "";
                    templateItem.IsEditorial = newItem.IsEditorial;
                    templateItem.IsCA = newItem.IsCA;
                }
            }
        }
    }

    private void AssignOrdersAndCreateProductionItems(List<PublishingOrder> orders, List<ProductionItemDto> availableSlots,
        int totalPages, List<ProductionItemDto> templateItems, List<ProductionItem> newProductionItems)
    {
        foreach (var order in orders)
        {
            var productAdvertisingSpaceName = order.ProductAdvertisingSpace?.Name;
            var locationTypeName = order.AdvertisingSpaceLocationType?.Name;
            List<ProductionItemDto> eligibleSlots;

            // Determinar slots elegibles
            if (IsFixedLocationSpace(productAdvertisingSpaceName))
            {
                eligibleSlots = availableSlots.Where(slot => slot.Id == 0).ToList(); // Solo slots no asignados
            }
            else
            {
                eligibleSlots = GetEligibleSlotsByLocation(availableSlots, locationTypeName, totalPages)
                    .Where(slot => slot.Id == 0).ToList(); // Solo slots no asignados
            }

            var targetSlot = eligibleSlots.FirstOrDefault();
            if (targetSlot != null)
            {
                // Crear nuevo ProductionItem para persistir
                var newProductionItem = new ProductionItem
                {
                    ProductEditionId = targetSlot.ProductEditionId,
                    PageNumber = targetSlot.PageNumber,
                    Slot = targetSlot.Slot,
                    InventoryProductAdvertisingSpaceId = targetSlot.InventoryProductAdvertisingSpaceId,
                    PublishingOrderId = order.Id,
                    Observations = order.Observations,
                    IsEditorial = false,
                    IsCA = false
                };

                newProductionItems.Add(newProductionItem);

                // Remover de slots disponibles
                availableSlots.Remove(targetSlot);
            }
        }
    }

    private List<ProductionItemDto> GetEligibleSlotsByLocation(List<ProductionItemDto> availableSlots, string locationTypeName, int totalPages)
    {
        var centerPage = totalPages / 2;
        var firstInnerPage = 2; // Puede ser 3 si hay InsideCover
        var lastInnerPage = totalPages - 1; // Puede ser totalPages-2 si hay InsideBackCover

        // Ajustar límites si hay páginas especiales
        var hasInsideCover = availableSlots.Any(slot => slot.PageNumber == 2 && IsInsideCoverSpace(slot));
        var hasInsideBackCover = availableSlots.Any(slot => slot.PageNumber == totalPages - 1 && IsInsideBackCoverSpace(slot));

        if (hasInsideCover) firstInnerPage = 3;
        if (hasInsideBackCover) lastInnerPage = totalPages - 2;

        return locationTypeName switch
        {
            "Antes de central/Oro" => availableSlots.Where(slot =>
                slot.PageNumber >= firstInnerPage && slot.PageNumber <= centerPage).ToList(),

            "Después de central/Bronce" => availableSlots.Where(slot =>
                slot.PageNumber > centerPage && slot.PageNumber <= lastInnerPage).ToList(),

            "Ubicación rotativa" => availableSlots.Where(slot =>
                slot.PageNumber >= firstInnerPage && slot.PageNumber <= lastInnerPage).ToList(),

            _ => availableSlots.ToList()
        };
    }

    private bool IsFixedLocationSpace(string productAdvertisingSpaceName)
    {
        if (string.IsNullOrEmpty(productAdvertisingSpaceName)) return false;

        var name = productAdvertisingSpaceName.ToLower();

        // Espacios de posición fija: ojos de tapa, pie de tapa, retiros, contratapa
        return (name.Contains("ojo") && name.Contains("tapa")) ||
               (name.Contains("pie") && name.Contains("tapa")) ||
               name.Contains("ret") ||
               name.Contains("contratapa");
    }

    private bool IsSlotOccupied(ProductionItemDto slot)
    {
        return slot.Id > 0; // Slot está ocupado si tiene un ProductionItem real asignado
    }

    private bool IsInsideCoverSpace(ProductionItemDto slot)
    {
        // Verificar si es un espacio de tipo InsideCover basado en el nombre
        return slot.ProductAdvertisingSpaceName?.ToLower().Contains("ret") == true &&
               slot.ProductAdvertisingSpaceName?.ToLower().Contains("tapa") == true;
    }

    private bool IsInsideBackCoverSpace(ProductionItemDto slot)
    {
        // Verificar si es un espacio de tipo InsideBackCover basado en el nombre
        return slot.ProductAdvertisingSpaceName?.ToLower().Contains("ret") == true &&
               slot.ProductAdvertisingSpaceName?.ToLower().Contains("contratapa") == true;
    }

    private void AssignOrderToSlot(PublishingOrder order, ProductionItemDto slot)
    {
        slot.PublishingOrderId = order.Id;
        slot.ContractName = order.Contract?.Name ?? "LATENTE";
        slot.ClientName = order.Contract?.Client?.LegalName ?? order.Contract?.Client?.BrandName ?? "";
        slot.SellerName = order.Contract?.Seller?.FullName ?? "";

        // Copiar observaciones si existen
        if (!string.IsNullOrEmpty(order.Observations))
        {
            slot.Observations = order.Observations;
        }
    }

    private long GetProductAdvertisingSpaceId(long inventoryProductAdvertisingSpaceId)
    {
        // Consultar el ProductAdvertisingSpaceId desde InventoryProductAdvertisingSpace
        var inventorySpace = context.InventoryProductAdvertisingSpaces
            .AsNoTracking()
            .FirstOrDefault(ipas => ipas.Id == inventoryProductAdvertisingSpaceId);

        return inventorySpace?.ProductAdvertisingSpaceId ?? 0;
    }

    private class ClassifiedSpaces
    {
        public InventorySpaceProjection CoverEye { get; set; }
        public InventorySpaceProjection CoverFooter { get; set; }
        public InventorySpaceProjection InsideCover { get; set; }
        public InventorySpaceProjection Page { get; set; }
        public InventorySpaceProjection InsideBackCover { get; set; }
        public InventorySpaceProjection BackCover { get; set; }
        public InventorySpaceProjection HalfPage { get; set; }
        public InventorySpaceProjection CuarterPage { get; set; }
        public InventorySpaceProjection FooterPage { get; set; }
        public List<InventorySpaceProjection> OtherSpaces { get; set; }
    }
}