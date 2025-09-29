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

namespace LadeviVentasApi.Controllers
{
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
                    // Si la nueva edición tiene PageCount e InventoryAdvertisingSpaces, generar ProductionTemplates
                    if (newEdition.PageCount.HasValue && newEdition.PageCount > 0 &&
                        newEdition.InventoryAdvertisingSpaces != null &&
                        newEdition.InventoryAdvertisingSpaces.Any())
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
                // Verificar si ya tiene ProductionTemplates antes de la actualización
                var hadProductionTemplates = await Context.ProductionTemplates
                    .AnyAsync(pt => pt.ProductEditionId == id && (!pt.Deleted.HasValue || !pt.Deleted.Value));

                var baseResult = await base.Put(id, x);

                if (baseResult is NoContentResult && !hadProductionTemplates)
                {
                    // Recargar la edición actualizada para verificar si ahora tiene los datos necesarios
                    var updatedEdition = await Context.ProductEditions
                        .Include(pe => pe.InventoryAdvertisingSpaces)
                        .FirstOrDefaultAsync(pe => pe.Id == id);

                    // Si ahora tiene PageCount e InventoryAdvertisingSpaces, generar ProductionTemplates
                    if (updatedEdition != null &&
                        updatedEdition.PageCount.HasValue && updatedEdition.PageCount > 0 &&
                        updatedEdition.InventoryAdvertisingSpaces != null &&
                        updatedEdition.InventoryAdvertisingSpaces.Any())
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
                .Include(pe => pe.InventoryAdvertisingSpaces)
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
                    InventoryAdvertisingSpaces = x.InventoryAdvertisingSpaces
                                                        .Where(ias => !ias.Deleted.HasValue || !ias.Deleted.Value)
                                                        .Select(ias => new
                                                        {
                                                            ias.Id,
                                                            ias.ProductEditionId,
                                                            ias.ProductAdvertisingSpaceId,
                                                            ias.Quantity
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
            foreach (var inventoryAdvertisingSpace in xAux.InventoryAdvertisingSpaces)
            {
                if (inventoryAdvertisingSpace.Id == 0)
                {
                    inventoryAdvertisingSpace.ProductEditionId = id;
                }
            }
            return base.PerformUpdate(id, xAux);
        }

        /// <summary>
        /// Genera ProductionTemplates y ProductionSlots para una ProductEdition usando AdvertisingSpaceClassifier
        /// </summary>
        private async Task GenerateProductionItemsForEdition(long productEditionId)
        {
            // Obtener la edición con todos los datos necesarios
            var productEdition = await Context.ProductEditions
                                            .Include(pe => pe.InventoryAdvertisingSpaces)
                                                .ThenInclude(ias => ias.ProductAdvertisingSpace)
                                            .AsNoTracking()
                                            .Select(pe => new ProductionInventoryProjection
                                            {
                                                Id = pe.Id,
                                                PageCount = pe.PageCount,
                                                InventoryAdvertisingSpaces = pe.InventoryAdvertisingSpaces
                                                    .Where(ias => ias.Quantity > 0) // ✅ Solo incluir espacios con cantidad > 0
                                                    .Select(ias => new InventorySpaceProjection
                                                    {
                                                        Id = ias.Id,
                                                        ProductAdvertisingSpaceId = ias.ProductAdvertisingSpaceId,
                                                        Quantity = ias.Quantity,
                                                        ProductAdvertisingSpaceName = ias.ProductAdvertisingSpace.Name
                                                    }).ToList(),
                                                Deleted = pe.Deleted
                                            })
                                            .SingleOrDefaultAsync(pe => (!pe.Deleted.HasValue || !pe.Deleted.Value) && pe.Id == productEditionId);


            if (productEdition == null || !productEdition.PageCount.HasValue || productEdition.PageCount <= 0)
            {
                return;
            }

            // Verificar si hay espacios disponibles después del filtro
            if (!productEdition.InventoryAdvertisingSpaces.Any())
            {
                return; // No generar template si no hay espacios con cantidad > 0
            }

            // Validar y determinar tipos de espacios
            var spaceTypeResult = ValidateAndDetermineSpaceTypes(productEdition.InventoryAdvertisingSpaces);
            if (!spaceTypeResult.IsValid)
            {
                throw new InvalidOperationException(spaceTypeResult.ErrorMessage);
            }

            // Clasificar espacios
            var spaces = ClassifyAllSpaces(productEdition.InventoryAdvertisingSpaces, spaceTypeResult.SpaceTypes);

            // Generar directamente las entidades
            var templateEntities = GenerateProductionTemplates(productEditionId, productEdition.PageCount.Value, spaces);

            // Verificar si ya existen templates para evitar duplicados
            var existingTemplatesCount = await Context.ProductionTemplates
                .CountAsync(pt => pt.ProductEditionId == productEditionId && (!pt.Deleted.HasValue || !pt.Deleted.Value));

            if (existingTemplatesCount > 0)
            {
                return; // Ya existen templates, no crear duplicados
            }

            // Agregar todo el grafo de objetos de una vez
            Context.ProductionTemplates.AddRange(templateEntities);
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

        private List<ProductionTemplate> GenerateProductionTemplates(long productEditionId, int pageCount, ClassifiedSpaces spaces)
        {
            var templates = new List<ProductionTemplate>();

            // Página 1 (Tapa)
            templates.Add(GenerateCoverPageTemplate(productEditionId, spaces));

            // Página 2 (InsideCover si existe)
            var hasInsideCover = spaces.InsideCover.Any();
            if (hasInsideCover)
            {
                templates.Add(GenerateInsideCoverPageTemplate(productEditionId, spaces));
            }

            // Páginas internas
            int startPage = hasInsideCover ? 3 : 2;
            for (int pageNumber = startPage; pageNumber < pageCount; pageNumber++)
            {
                var template = GenerateInnerPageTemplate(productEditionId, pageNumber, spaces);
                if (template.ProductionSlots.Any()) // Solo agregar si tiene slots
                {
                    templates.Add(template);
                }
            }

            // Última página (Contratapa)
            templates.Add(GenerateBackCoverPageTemplate(productEditionId, pageCount, spaces));

            return templates;
        }

        private ProductionTemplate GenerateCoverPageTemplate(long productEditionId, ClassifiedSpaces spaces)
        {
            var template = new ProductionTemplate
            {
                ProductEditionId = productEditionId,
                PageNumber = 1,
                ProductionSlots = new List<ProductionSlot>()
            };

            var slotNumber = 1;

            // CoverEye
            foreach (var coverEye in spaces.CoverEye)
            {
                template.ProductionSlots.Add(CreateSlotEntity(slotNumber++, coverEye));
            }

            // CoverFooter  
            foreach (var coverFooter in spaces.CoverFooter)
            {
                template.ProductionSlots.Add(CreateSlotEntity(slotNumber++, coverFooter));
            }

            return template;
        }

        private ProductionTemplate GenerateInsideCoverPageTemplate(long productEditionId, ClassifiedSpaces spaces)
        {
            var template = new ProductionTemplate
            {
                ProductEditionId = productEditionId,
                PageNumber = 2,
                ProductionSlots = new List<ProductionSlot>()
            };

            var slotNumber = 1;

            // InsideCover va en página 2
            foreach (var insideCover in spaces.InsideCover)
            {
                template.ProductionSlots.Add(CreateSlotEntity(slotNumber++, insideCover));
            }

            return template;
        }

        private ProductionTemplate GenerateInnerPageTemplate(long productEditionId, int pageNumber, ClassifiedSpaces spaces)
        {
            var template = new ProductionTemplate
            {
                ProductEditionId = productEditionId,
                PageNumber = pageNumber,
                ProductionSlots = new List<ProductionSlot>()
            };

            var slotNumber = 1;

            // Páginas PARES: Solo espacios tipo Page
            if (pageNumber % 2 == 0)
            {
                var availablePage = GetNextAvailableSpace(spaces.Page);
                if (availablePage != null)
                {
                    template.ProductionSlots.Add(CreateSlotEntity(slotNumber++, availablePage));
                    availablePage.Quantity = availablePage.Quantity - 1;
                }
            }
            else
            {
                // Páginas IMPARES: Un espacio por página, rotando entre tipos
                var availableSpace = GetNextAvailableSpaceForOddPage(spaces);
                if (availableSpace != null)
                {
                    template.ProductionSlots.Add(CreateSlotEntity(slotNumber++, availableSpace));
                    availableSpace.Quantity = availableSpace.Quantity - 1;
                }
            }

            return template;
        }

        private ProductionTemplate GenerateBackCoverPageTemplate(long productEditionId, int pageCount, ClassifiedSpaces spaces)
        {
            var template = new ProductionTemplate
            {
                ProductEditionId = productEditionId,
                PageNumber = pageCount,
                ProductionSlots = new List<ProductionSlot>()
            };

            var slotNumber = 1;

            // BackCover
            foreach (var backCover in spaces.BackCover)
            {
                template.ProductionSlots.Add(CreateSlotEntity(slotNumber++, backCover));
            }

            // InsideBackCover
            foreach (var insideBackCover in spaces.InsideBackCover)
            {
                template.ProductionSlots.Add(CreateSlotEntity(slotNumber++, insideBackCover));
            }

            return template;
        }

        // Método helper para crear un slot
        private ProductionSlot CreateSlotEntity(int slotNumber, InventorySpaceProjection space)
        {
            return new ProductionSlot
            {
                SlotNumber = slotNumber,
                InventoryAdvertisingSpaceId = space.Id,
                PublishingOrderId = null,
                IsEditorial = false,
                IsCA = false,
                Observations = ""
            };
        }

        // Método helper para obtener el siguiente espacio disponible
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

        private async Task AssignExistingPublishingOrders(long productEditionId, int totalPages)
        {
            // Obtener órdenes sin ProductionSlot asignado
            var unassignedOrders = await Context.PublishingOrders
                .Include(po => po.ProductAdvertisingSpace)
                .Include(po => po.AdvertisingSpaceLocationType)
                .Where(po => po.ProductEditionId == productEditionId &&
                            (!po.Deleted.HasValue || !po.Deleted.Value) &&
                            !Context.ProductionSlots.Any(ps => ps.PublishingOrderId == po.Id))
                .OrderBy(po => po.Id)
                .ToListAsync();

            if (!unassignedOrders.Any()) return;

            // Obtener ProductionSlots disponibles
            var availableSlots = await Context.ProductionSlots
                .Include(ps => ps.ProductionTemplate)
                .Where(ps => ps.ProductionTemplate.ProductEditionId == productEditionId && ps.PublishingOrderId == null)
                .ToListAsync();

            // Asignar según reglas (lógica simplificada)
            foreach (var order in unassignedOrders)
            {
                var availableSlot = availableSlots
                    .FirstOrDefault(slot => slot.PublishingOrderId == null &&
                                   GetProductAdvertisingSpaceId(slot.InventoryAdvertisingSpaceId.Value) == order.ProductAdvertisingSpaceId);

                if (availableSlot != null)
                {
                    availableSlot.PublishingOrderId = order.Id;
                    availableSlot.Observations = order.Observations ?? "";
                }
            }

            await Context.SaveChangesAsync();
        }

        private long GetProductAdvertisingSpaceId(long inventoryAdvertisingSpaceId)
        {
            var inventorySpace = Context.InventoryAdvertisingSpaces
                .AsNoTracking()
                .FirstOrDefault(ias => ias.Id == inventoryAdvertisingSpaceId);

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
}