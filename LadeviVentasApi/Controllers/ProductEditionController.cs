namespace LadeviVentasApi.Controllers;

using AutoMapper;
using KendoNET.DynamicLinq;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;
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
}
