namespace LadeviVentasApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using LadeviVentasApi.Data;
using LadeviVentasApi.Models.Domain;
using Microsoft.EntityFrameworkCore;
using LadeviVentasApi.Services.Xubio;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Services.Xubio.DTOs;
using LadeviVentasApi.Services.Xubio.FixedValues;
using Microsoft.Extensions.Configuration;
using LadeviVentasApi.Data.Migrations;
using System.Text.RegularExpressions;
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

        // 4. Generar template de producción
        var productionItems = GenerateProductionTemplate(productEditionId, productEdition.PageCount.Value, spaces);

        return Ok(productionItems);
    }

    private async Task<ProductionInventoryProjection> GetProductEditionAsync(long productEditionId)
    {
        return await context.ProductEditions
                            .Include(pe => pe.InventoryProductAdvertisingSpaces)
                                .ThenInclude(ipas => ipas.ProductAdvertisingSpace)
                            .AsNoTracking()
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
                            .SingleOrDefaultAsync(pe => (!pe.Deleted.HasValue || !pe.Deleted.Value) && pe.Id == productEditionId);
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