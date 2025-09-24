namespace LadeviVentasApi.Controllers;

using AutoMapper;
using KendoNET.DynamicLinq;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

using LadeviVentasApi.Helpers.Utilities;
using LadeviVentasApi.Projections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ProductEditionController : RestController<ProductEdition, ProductEditionWritingDto>
{
    public ProductEditionController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    [HttpGet("Options")]
    public async Task<IActionResult> Options(long productId, bool includeClosed = true)
    {
        var productEditionsQuery = Context.ProductEditions.AsNoTracking()
                                                    .Where(pe => (!pe.Deleted.HasValue || !pe.Deleted.Value)
                                                            && pe.ProductId == productId);
        if (!includeClosed)
        {
            productEditionsQuery = productEditionsQuery.Where(pe => !pe.Closed);
        }

        return Ok(await productEditionsQuery.OrderBy(pe => pe.Start)
                                            .Select(pe => new
                                            {
                                                pe.Id,
                                                pe.Name,
                                                pe.Code,
                                            })
                                            .ToListAsync());
    }

    public override async Task<IActionResult> Post(ProductEditionWritingDto x)
    {
        using var transaction = await Context.Database.BeginTransactionAsync();
        try
        {
            var baseResult = await base.Post(x);

            if (baseResult is CreatedAtActionResult createdResult && createdResult.Value is ProductEdition newEdition)
            {
                // Si la nueva edición tiene PageCount e InventoryProductAdvertisingSpaces, generar ProductionItems
                if (newEdition.PageCount.HasValue && newEdition.PageCount > 0 &&
                    newEdition.InventoryProductAdvertisingSpaces != null &&
                    newEdition.InventoryProductAdvertisingSpaces.Any())
                {
                    await GenerateProductionItemsForEdition(newEdition.Id);
                }
            }

            await transaction.CommitAsync();
            return baseResult;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest($"Error al crear la edición: {ex.Message}");
        }
    }

    public override async Task<IActionResult> Put(long id, ProductEditionWritingDto x)
    {
        using var transaction = await Context.Database.BeginTransactionAsync();
        try
        {
            // Verificar si ya tiene ProductionItems antes de la actualización
            var hadProductionItems = await Context.ProductionItems
                .AnyAsync(pi => pi.ProductEditionId == id && (!pi.Deleted.HasValue || !pi.Deleted.Value));

            var baseResult = await base.Put(id, x);

            if (baseResult is NoContentResult && !hadProductionItems)
            {
                // Recargar la edición actualizada para verificar si ahora tiene los datos necesarios
                var updatedEdition = await Context.ProductEditions
                    .Include(pe => pe.InventoryProductAdvertisingSpaces)
                    .FirstOrDefaultAsync(pe => pe.Id == id);

                // Si ahora tiene PageCount e InventoryProductAdvertisingSpaces, generar ProductionItems
                if (updatedEdition != null &&
                    updatedEdition.PageCount.HasValue && updatedEdition.PageCount > 0 &&
                    updatedEdition.InventoryProductAdvertisingSpaces != null &&
                    updatedEdition.InventoryProductAdvertisingSpaces.Any())
                {
                    await GenerateProductionItemsForEdition(updatedEdition.Id);
                }
            }

            await transaction.CommitAsync();
            return baseResult;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest($"Error al actualizar la edición: {ex.Message}");
        }
    }

    [HttpPost("Import")]
    public async Task<IActionResult> Import(List<ProductImportWritingDto> productsImport)
    {
        var index = 2;
        var productImportErrors = new List<ProductImportError>();
        var products = Context.Products.Where(x => (!x.Deleted.HasValue || !x.Deleted.Value)).ToList();
        var editions = Context.ProductEditions.Where(x => (!x.Deleted.HasValue || !x.Deleted.Value)).ToList();

        foreach (var productImport in productsImport)
        {
            //Product validation
            if (string.IsNullOrWhiteSpace(productImport.Product))
            {
                productImportErrors.Add(new ProductImportError { Line = index, MessageError = "Debe cargar el producto de la edición." });
            }
            else
            {
                var productMatched = products.SingleOrDefault(p => p.Name.ToLower().Trim() == productImport.Product.ToLower().Trim());
                if (productMatched != null)
                {
                    productImport.Product = productMatched.Name;
                }
                else
                {
                    productImportErrors.Add(new ProductImportError { Line = index, MessageError = "El producto no exíste." });
                }
            }

            //Title validation
            if (string.IsNullOrWhiteSpace(productImport.Title))
            {
                productImportErrors.Add(new ProductImportError { Line = index, MessageError = "Debe cargar un título de la edición." });
            }

            //Code validation
            if (string.IsNullOrWhiteSpace(productImport.Code))
            {
                productImportErrors.Add(new ProductImportError { Line = index, MessageError = "Debe cargar el código de la edición." });
            }
            else
            {
                var editionMatched = editions.SingleOrDefault(p => p.Code.ToLower() == productImport.Code.ToLower());
                if (editionMatched != null)
                {
                    productImportErrors.Add(new ProductImportError { Line = index, MessageError = "El código ya exíste en otra edición." });
                }
                else
                {
                    var productsFileSameCode = productsImport.Where(p => p.Code != null && p.Code.ToLower() == productImport.Code.ToLower());
                    if (productsFileSameCode.Count() > 1)
                    {
                        productImportErrors.Add(new ProductImportError { Line = index, MessageError = "El código ya exíste en el archivo." });
                    }
                }
            }

            //Title validation
            if (productImport.Closed == null)
            {
                productImportErrors.Add(new ProductImportError { Line = index, MessageError = "El cierre de la edición es invalido" });
            }
            else if (string.IsNullOrWhiteSpace(productImport.Closed))
            {
                productImportErrors.Add(new ProductImportError { Line = index, MessageError = "Debe cargar el cierre de la edición." });
            }

            //DepartureDate validation
            if (productImport.DepartureDate == null)
            {
                productImportErrors.Add(new ProductImportError { Line = index, MessageError = "La fecha de salida de la edición es invalida" });
            }
            else if (string.IsNullOrWhiteSpace(productImport.DepartureDate))
            {
                productImportErrors.Add(new ProductImportError { Line = index, MessageError = "Debe cargar la fecha de salida de la edición." });
            }

            index++;
        }

        if (productImportErrors.Count == 0)
        {
            Context.ProductEditions.AddRange(productsImport.Select(pi => new ProductEdition
            {
                ProductId = products.Single(p => p.Name == pi.Product).Id,
                Name = pi.Title,
                Code = pi.Code,
                Closed = bool.Parse(pi.Closed),
                End = DateTime.Parse(pi.DepartureDate),
                Start = DateTime.Parse(pi.DepartureDate).AddDays(-1)
            }).ToList());

            Context.Add(new Auditory
            {
                Date = DateTime.Now,
                Entity = "Ediciones",
                UserId = CurrentAppUser.Value.Id,
                User = CurrentAppUser.Value.FullName,
                AuditMessage = "Importacion de Ediciones. Id=" + CurrentAppUser.Value.Id.ToString() + ". Nombre: " + CurrentAppUser.Value.FullName
            });

            Context.SaveChanges();

            return Ok(new
            {
                ImportSuccess = true,
                Errors = productImportErrors
            });
        }

        return Ok(new
        {
            ImportSuccess = false,
            Errors = productImportErrors
        });
    }

    protected override IQueryable<ProductEdition> GetQueryableWithIncludes()
    {
        var allEditions = base.GetQueryableWithIncludes()
            .Include(pe => pe.Product)
                .ThenInclude(p => p.ProductAdvertisingSpaces)
            .Include(pe => pe.PublishingOrders)
            .Include(pe => pe.InventoryProductAdvertisingSpaces)
            .AsNoTracking();

        return allEditions;
    }

    protected override DataSourceResult GetSearchDataSourceResult(KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
    {
        var result = GetQueryableWithIncludes()
            .Select(x => new
            {
                x.Id,
                x.ProductId,
                ProductName = x.Product != null ? x.Product.Name.Trim() : "",
                ProductAdvertisingSpaces = x.Product.ProductAdvertisingSpaces
                                                    .Where(pas => (!pas.Deleted.HasValue || !pas.Deleted.Value) && pas.Show)
                                                    .OrderBy(pas => pas.Name)
                                                    .Select(pas => new
                                                    {
                                                        pas.Id,
                                                        pas.Name,
                                                    }),
                InventoryProductAdvertisingSpaces = x.InventoryProductAdvertisingSpaces
                                                    .Where(ipas => !ipas.Deleted.HasValue || !ipas.Deleted.Value)
                                                    .Select(ipas => new
                                                    {
                                                        ipas.Id,
                                                        ipas.ProductEditionId,
                                                        ipas.ProductAdvertisingSpaceId,
                                                        ipas.Quantity
                                                    }),
                Code = x.Code.Trim(),
                x.End,
                x.Closed,
                Name = x.Name.Trim(),
                x.PageCount,
                x.Deleted,
                CanDelete = x.PublishingOrders.Where(po => !po.Deleted.HasValue || po.Deleted.Value == false).Count() == 0
            })
            .Where(x => !x.Deleted.HasValue || !x.Deleted.Value)
            .ToDataSourceResult(request);

        return result;
    }

    protected override Task<IActionResult> PerformUpdate(long id, ProductEdition xAux)
    {
        foreach (var inventoryProductAdvertisingSpace in xAux.InventoryProductAdvertisingSpaces)
        {
            if (inventoryProductAdvertisingSpace.Id == 0)
            {
                inventoryProductAdvertisingSpace.ProductEditionId = id;
            }
        }
        return base.PerformUpdate(id, xAux);
    }

    /// <summary>
    /// Genera ProductionItems para una ProductEdition usando AdvertisingSpaceClassifier
    /// </summary>
    private async Task GenerateProductionItemsForEdition(long productEditionId)
    {
        // Obtener la edición con todos los datos necesarios (misma query que ProductionController)
        var productEdition = await Context.ProductEditions
            .Include(pe => pe.InventoryProductAdvertisingSpaces)
                .ThenInclude(ipas => ipas.ProductAdvertisingSpace)
            .AsNoTracking()
            .Select(pe => new ProductionInventoryProjection
            {
                Id = pe.Id,
                PageCount = pe.PageCount,
                InventoryProductAdvertisingSpaces = pe.InventoryProductAdvertisingSpaces.Where(ipas => ipas.Quantity != 0).Select(ipas => new InventorySpaceProjection
                {
                    Id = ipas.Id,
                    ProductAdvertisingSpaceId = ipas.ProductAdvertisingSpaceId,
                    Quantity = ipas.Quantity,
                    ProductAdvertisingSpaceName = ipas.ProductAdvertisingSpace.Name
                }).ToList(),
                Deleted = pe.Deleted
            })
            .SingleOrDefaultAsync(pe => (!pe.Deleted.HasValue || !pe.Deleted.Value) && pe.Id == productEditionId);

        if (productEdition == null || !productEdition.PageCount.HasValue || productEdition.PageCount <= 0)
        {
            return;
        }

        // CORRECCIÓN: Usar el método correcto (plural)
        var spaceTypeResult = ValidateAndDetermineSpaceTypes(productEdition.InventoryProductAdvertisingSpaces);
        if (!spaceTypeResult.IsValid)
        {
            throw new InvalidOperationException(spaceTypeResult.ErrorMessage);
        }

        // CORRECCIÓN: Pasar List<string> en lugar de string
        var spaces = ClassifyAllSpaces(productEdition.InventoryProductAdvertisingSpaces, spaceTypeResult.SpaceTypes);
        var productionItemDtos = GenerateProductionTemplate(productEditionId, productEdition.PageCount.Value, spaces);

        // Convertir a entidades y persistir
        var productionItemEntities = productionItemDtos.Select(dto => new ProductionItem
        {
            ProductEditionId = dto.ProductEditionId,
            PageNumber = dto.PageNumber,
            Slot = dto.Slot,
            InventoryProductAdvertisingSpaceId = dto.InventoryProductAdvertisingSpaceId,
            PublishingOrderId = null,
            IsEditorial = dto.IsEditorial,
            IsCA = dto.IsCA,
            Observations = dto.Observations
        }).ToList();

        Context.ProductionItems.AddRange(productionItemEntities);
        await Context.SaveChangesAsync();

        // Asignar órdenes existentes
        await AssignExistingPublishingOrders(productEditionId, productEdition.PageCount.Value);
    }

    #region Lógica reutilizada del ProductionController

    private (bool IsValid, string ErrorMessage, List<string> SpaceTypes) ValidateAndDetermineSpaceTypes(List<InventorySpaceProjection> spaces)
    {
        var hasPrint = spaces.Any(x => x.ProductAdvertisingSpaceName.ToLower().EndsWith("print") && x.Quantity.HasValue && x.Quantity != 0);
        var hasDigital = spaces.Any(x => x.ProductAdvertisingSpaceName.ToLower().EndsWith("digital") && x.Quantity.HasValue && x.Quantity != 0);
        var hasGeneric = spaces.Any(x => !x.ProductAdvertisingSpaceName.ToLower().EndsWith("digital") &&
                                         !x.ProductAdvertisingSpaceName.ToLower().EndsWith("print") &&
                                         x.Quantity.HasValue && x.Quantity != 0);

        var activeTypes = new List<string>();

        if (hasPrint) activeTypes.Add("print");
        if (hasDigital) activeTypes.Add("digital");
        if (hasGeneric) activeTypes.Add("generic");

        if (activeTypes.Count == 0)
            return (false, "No se encontraron espacios publicitarios con cantidad válida", null);

        return (true, null, activeTypes);
    }

    private ClassifiedSpaces ClassifyAllSpaces(List<InventorySpaceProjection> inventorySpaces, List<string> spaceTypes)
    {
        return new ClassifiedSpaces
        {
            CoverEye = AdvertisingSpaceClassifier.GetCoverEye(inventorySpaces, spaceTypes),
            CoverFooter = AdvertisingSpaceClassifier.GetCoverFooter(inventorySpaces, spaceTypes),
            InsideCover = AdvertisingSpaceClassifier.GetInsideCover(inventorySpaces, spaceTypes),
            Page = AdvertisingSpaceClassifier.GetPage(inventorySpaces, spaceTypes),
            InsideBackCover = AdvertisingSpaceClassifier.GetInsideBackCover(inventorySpaces, spaceTypes),
            BackCover = AdvertisingSpaceClassifier.GetBackCover(inventorySpaces, spaceTypes),
            HalfPage = AdvertisingSpaceClassifier.GetHalfPage(inventorySpaces, spaceTypes),
            CuarterPage = AdvertisingSpaceClassifier.GetCuarterPage(inventorySpaces, spaceTypes),
            FooterPage = AdvertisingSpaceClassifier.GetFooterPage(inventorySpaces, spaceTypes),
            OtherSpaces = AdvertisingSpaceClassifier.GetOtherSpaces(inventorySpaces, spaceTypes).ToList()
        };
    }

    private List<ProductionItemDto> GenerateProductionTemplate(long productEditionId, int pageCount, ClassifiedSpaces spaces)
    {
        var productionItems = new List<ProductionItemDto>();

        // Página 1 (Tapa)
        productionItems.AddRange(GenerateCoverPage(productEditionId, spaces));

        // Página 2 (InsideCover si existe)
        var hasInsideCover = spaces.InsideCover.Any();
        if (hasInsideCover)
        {
            productionItems.AddRange(GenerateInsideCoverPage(productEditionId, spaces));
        }

        // Páginas internas - comienzan en página 2 si NO hay InsideCover, o página 3 si SÍ hay InsideCover
        int startPage = hasInsideCover ? 3 : 2;
        for (int pageNumber = startPage; pageNumber < pageCount; pageNumber++)
        {
            productionItems.AddRange(GenerateInnerPage(productEditionId, pageNumber, spaces));
        }

        // Última página (Contratapa)
        productionItems.AddRange(GenerateBackCoverPage(productEditionId, pageCount, spaces));

        return productionItems;
    }

    private List<ProductionItemDto> GenerateCoverPage(long productEditionId, ClassifiedSpaces spaces)
    {
        var items = new List<ProductionItemDto>();
        var slot = 0;

        // Solo Ojos de tapa y Pie de tapa en página 1
        foreach (var coverEye in spaces.CoverEye)
        {
            AddSpaceItems(items, coverEye, productEditionId, 1, ref slot);
        }

        foreach (var coverFooter in spaces.CoverFooter)
        {
            AddSpaceItems(items, coverFooter, productEditionId, 1, ref slot);
        }

        // InsideCover NO va aquí, va en página 2

        return items;
    }

    private List<ProductionItemDto> GenerateInsideCoverPage(long productEditionId, ClassifiedSpaces spaces)
    {
        var items = new List<ProductionItemDto>();
        var slot = 0;

        // InsideCover va en página 2
        foreach (var insideCover in spaces.InsideCover)
        {
            AddSpaceItems(items, insideCover, productEditionId, 2, ref slot);
        }

        return items;
    }

    private List<ProductionItemDto> GenerateInnerPage(long productEditionId, int pageNumber, ClassifiedSpaces spaces)
    {
        var items = new List<ProductionItemDto>();
        var slot = 0;

        // Páginas PARES: Solo espacios tipo Page
        if (pageNumber % 2 == 0)
        {
            // Buscar el siguiente Page disponible
            var availablePage = GetNextAvailableSpace(spaces.Page);
            if (availablePage != null)
            {
                AddSpaceItems(items, availablePage, productEditionId, pageNumber, ref slot);
                // Marcar como usado reduciendo quantity
                availablePage.Quantity = availablePage.Quantity - 1;
            }
        }
        else
        {
            // Páginas IMPARES: Un espacio por página, rotando entre tipos
            var availableSpace = GetNextAvailableSpaceForOddPage(spaces);
            if (availableSpace != null)
            {
                AddSpaceItems(items, availableSpace, productEditionId, pageNumber, ref slot);
                // Marcar como usado reduciendo quantity
                availableSpace.Quantity = availableSpace.Quantity - 1;
            }
        }

        return items;
    }

    // Método helper para obtener el siguiente Page disponible
    private InventorySpaceProjection GetNextAvailableSpace(List<InventorySpaceProjection> spaces)
    {
        return spaces.FirstOrDefault(space => space.Quantity > 0);
    }

    // Método helper para obtener el siguiente espacio disponible en páginas impares (rotando)
    private InventorySpaceProjection GetNextAvailableSpaceForOddPage(ClassifiedSpaces spaces)
    {
        // Orden de prioridad: HalfPage -> CuarterPage -> FooterPage -> OtherSpaces
        var availableSpace = GetNextAvailableSpace(spaces.HalfPage);
        if (availableSpace != null) return availableSpace;

        availableSpace = GetNextAvailableSpace(spaces.CuarterPage);
        if (availableSpace != null) return availableSpace;

        availableSpace = GetNextAvailableSpace(spaces.FooterPage);
        if (availableSpace != null) return availableSpace;

        availableSpace = GetNextAvailableSpace(spaces.OtherSpaces);
        if (availableSpace != null) return availableSpace;

        return null;
    }

    private List<ProductionItemDto> GenerateBackCoverPage(long productEditionId, int pageCount, ClassifiedSpaces spaces)
    {
        var items = new List<ProductionItemDto>();
        var slot = 0;

        foreach (var insideBackCover in spaces.InsideBackCover)
        {
            AddSpaceItems(items, insideBackCover, productEditionId, pageCount, ref slot);
        }

        foreach (var backCover in spaces.BackCover)
        {
            AddSpaceItems(items, backCover, productEditionId, pageCount, ref slot);
        }

        return items;
    }

    // El método AddSpaceItems ahora agrega solo 1 item por llamada
    private void AddSpaceItems(List<ProductionItemDto> items, InventorySpaceProjection space, long productEditionId, int pageNumber, ref int slot)
    {
        if (space?.Quantity == null || space.Quantity <= 0) return;

        // Solo agregar 1 item por página
        slot++;
        items.Add(new ProductionItemDto
        {
            ProductEditionId = productEditionId,
            PageNumber = pageNumber,
            Slot = slot,
            InventoryProductAdvertisingSpaceId = space.Id,
            ProductAdvertisingSpaceName = space.ProductAdvertisingSpaceName,
            IsEditorial = false,
            IsCA = false,
            Observations = ""
        });
    }

    private async Task AssignExistingPublishingOrders(long productEditionId, int totalPages)
    {
        // Obtener órdenes sin ProductionItem asignado
        var unassignedOrders = await Context.PublishingOrders
            .Include(po => po.ProductAdvertisingSpace)
            .Include(po => po.AdvertisingSpaceLocationType)
            .Where(po => po.ProductEditionId == productEditionId &&
                        (!po.Deleted.HasValue || !po.Deleted.Value) &&
                        !Context.ProductionItems.Any(pi => pi.PublishingOrderId == po.Id))
            .OrderBy(po => po.Id)
            .ToListAsync();

        if (!unassignedOrders.Any()) return;

        // Obtener ProductionItems disponibles
        var availableItems = await Context.ProductionItems
            .Where(pi => pi.ProductEditionId == productEditionId && pi.PublishingOrderId == null)
            .ToListAsync();

        // Asignar según reglas (lógica simplificada)
        foreach (var order in unassignedOrders)
        {
            var availableItem = availableItems
                .FirstOrDefault(item => item.PublishingOrderId == null &&
                               GetProductAdvertisingSpaceId(item.InventoryProductAdvertisingSpaceId) == order.ProductAdvertisingSpaceId);

            if (availableItem != null)
            {
                availableItem.PublishingOrderId = order.Id;
            }
        }

        await Context.SaveChangesAsync();
    }

    private long GetProductAdvertisingSpaceId(long inventoryProductAdvertisingSpaceId)
    {
        var inventorySpace = Context.InventoryProductAdvertisingSpaces
            .AsNoTracking()
            .FirstOrDefault(ipas => ipas.Id == inventoryProductAdvertisingSpaceId);

        return inventorySpace?.ProductAdvertisingSpaceId ?? 0;
    }

    private class ClassifiedSpaces
    {
        public List<InventorySpaceProjection> CoverEye { get; set; } = new List<InventorySpaceProjection>();
        public List<InventorySpaceProjection> CoverFooter { get; set; } = new List<InventorySpaceProjection>();
        public List<InventorySpaceProjection> InsideCover { get; set; } = new List<InventorySpaceProjection>();
        public List<InventorySpaceProjection> Page { get; set; } = new List<InventorySpaceProjection>();
        public List<InventorySpaceProjection> InsideBackCover { get; set; } = new List<InventorySpaceProjection>();
        public List<InventorySpaceProjection> BackCover { get; set; } = new List<InventorySpaceProjection>();
        public List<InventorySpaceProjection> HalfPage { get; set; } = new List<InventorySpaceProjection>();
        public List<InventorySpaceProjection> CuarterPage { get; set; } = new List<InventorySpaceProjection>();
        public List<InventorySpaceProjection> FooterPage { get; set; } = new List<InventorySpaceProjection>();
        public List<InventorySpaceProjection> OtherSpaces { get; set; } = new List<InventorySpaceProjection>();
    }

    #endregion
}