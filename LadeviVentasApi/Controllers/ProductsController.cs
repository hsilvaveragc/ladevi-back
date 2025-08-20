namespace LadeviVentasApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using LadeviVentasApi.Data;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;
using Microsoft.EntityFrameworkCore;
using KendoNET.DynamicLinq;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Services.Xubio;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : RestController<Product, ProductWritingDto>
{
    private readonly XubioService xubioService;
    private readonly IConfiguration configuration;

    public ProductsController(ApplicationDbContext context, IMapper mapper, XubioService xubioService, IConfiguration configuration) : base(context, mapper)
    {
        this.xubioService = xubioService;
        this.configuration = configuration;
    }

    public override async Task<IActionResult> Put(long id, ProductWritingDto x)
    {
        EnsureUserRole(UsersRole.Superuser, UsersRole.Supervisor);
        Product oldProduct = Context.Products.AsNoTracking().Single(y => y.Id == id);

        List<ProductVolumeDiscount> oldVolumeDiscounts = Context.ProductVolumeDiscount.AsNoTracking().Where(y => y.ProductId == id).ToList();
        List<ProductLocationDiscount> oldLocationDiscounts = Context.ProductLocationDiscount.AsNoTracking().Where(y => y.ProductId == id).ToList();

        //var result = base.Put(id, x);
        Product productToUpdate = Mapper.Map<Product>(x);

        var local = Context.Set<Product>()
                        .Local
                        .FirstOrDefault(entry => entry.Id.Equals(id));

        if (local != null)
        {
            Context.Entry(local).State = EntityState.Detached;
        }

        foreach (var locDiscount in productToUpdate.ProductLocationDiscounts.ToList())
        {
            if (locDiscount.Id > 0)
            {
                Context.Entry(locDiscount).State = locDiscount.ShouldDelete.HasValue && locDiscount.ShouldDelete.Value ? EntityState.Deleted : EntityState.Modified;
            }
            else
            {
                Context.Add(locDiscount);
            }
        }

        foreach (var volDiscount in productToUpdate.ProductVolumeDiscounts.ToList())
        {
            if (volDiscount.Id > 0)
            {
                Context.Entry(volDiscount).State = volDiscount.ShouldDelete.HasValue && volDiscount.ShouldDelete.Value ? EntityState.Deleted : EntityState.Modified;
            }
            else
            {
                Context.Add(volDiscount);
            }
        }

        try
        {
            Context.Entry(productToUpdate).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }
        catch (Exception ex)
        {

        }

        Auditory auditProduct = new Auditory();
        auditProduct.Date = DateTime.Now;
        auditProduct.Entity = "Producto";
        auditProduct.UserId = CurrentAppUser.Value.Id;
        auditProduct.User = CurrentAppUser.Value.FullName;
        auditProduct.AuditMessage = "Se modificó el producto " + x.Name + ". " + string.Join("|", Utils.GenerateAuditLogMessages(oldProduct, productToUpdate));
        Context.Add(auditProduct);
        await Context.SaveChangesAsync();

        //Acá hago la auditoria de las entidades relacionadas
        try
        {
            #region Auditoria Descuentos por Ubicacion

            foreach (var oldL in oldLocationDiscounts)
            {
                ProductLocationDiscount actualLocDiscount = x.ProductLocationDiscounts.FirstOrDefault(y => y.Id == oldL.Id);
                //Se elimino un volumen
                if (actualLocDiscount == null)
                {
                    Auditory audit = new Auditory();
                    audit.Date = DateTime.Now;
                    audit.Entity = "Producto";
                    audit.UserId = CurrentAppUser.Value.Id;
                    audit.User = CurrentAppUser.Value.FullName;
                    audit.AuditMessage = "Se eliminó un descuento por ubicación del producto " + x.Name;
                    Context.Add(audit);
                    await Context.SaveChangesAsync();
                }
                else
                {
                    List<string> mods = Utils.GenerateAuditLogMessages(oldL, actualLocDiscount);
                    if (mods.Count > 0)
                    {
                        Auditory audit = new Auditory();
                        audit.Date = DateTime.Now;
                        audit.Entity = "Producto";
                        audit.UserId = CurrentAppUser.Value.Id;
                        audit.User = CurrentAppUser.Value.FullName;
                        audit.AuditMessage = "Modificación de descuento por ubicación del producto " + x.Name + ". " + string.Join('|', mods);
                        Context.Add(audit);
                        await Context.SaveChangesAsync();
                    }
                }
            }

            foreach (var actualL in x.ProductLocationDiscounts)
            {
                ProductLocationDiscount oldLocDiscount = oldLocationDiscounts.FirstOrDefault(y => y.Id == actualL.Id);
                // Se agrego una entidad
                if (oldLocDiscount == null)
                {
                    Auditory audit = new Auditory();
                    audit.Date = DateTime.Now;
                    audit.Entity = "Producto";
                    audit.UserId = CurrentAppUser.Value.Id;
                    audit.User = CurrentAppUser.Value.FullName;
                    audit.AuditMessage = "Se Agregó un descuento por ubicación al producto " + x.Name;
                    Context.Add(audit);
                    await Context.SaveChangesAsync();
                }
            }
            #endregion

            #region Auditoria Descuentos por volumen

            foreach (var oldV in oldVolumeDiscounts)
            {
                ProductVolumeDiscount actualVolDiscount = x.ProductVolumeDiscounts.FirstOrDefault(y => y.Id == oldV.Id);
                //Se elimino un volumen
                if (actualVolDiscount == null)
                {
                    Auditory audit = new Auditory();
                    audit.Date = DateTime.Now;
                    audit.Entity = "Producto";
                    audit.UserId = CurrentAppUser.Value.Id;
                    audit.User = CurrentAppUser.Value.FullName;
                    audit.AuditMessage = "Se eliminó un descuento por volumen del producto " + x.Name;
                    Context.Add(audit);
                    await Context.SaveChangesAsync();
                }
                else
                {
                    List<string> mods = Utils.GenerateAuditLogMessages(oldV, actualVolDiscount);
                    if (mods.Count > 0)
                    {
                        Auditory audit = new Auditory();
                        audit.Date = DateTime.Now;
                        audit.Entity = "Producto";
                        audit.UserId = CurrentAppUser.Value.Id;
                        audit.User = CurrentAppUser.Value.FullName;
                        audit.AuditMessage = "Modificación de descuento por volumen del producto " + x.Name + ". " + string.Join('|', mods);
                        Context.Add(audit);
                        await Context.SaveChangesAsync();
                    }
                }
            }

            foreach (var actualV in x.ProductVolumeDiscounts)
            {
                ProductVolumeDiscount oldVolDiscount = oldVolumeDiscounts.FirstOrDefault(y => y.Id == actualV.Id);
                // Se agrego una entidad
                if (oldVolDiscount == null)
                {
                    Auditory audit = new Auditory();
                    audit.Date = DateTime.Now;
                    audit.Entity = "Producto";
                    audit.UserId = CurrentAppUser.Value.Id;
                    audit.User = CurrentAppUser.Value.FullName;
                    audit.AuditMessage = "Se Agregó un descuento por volumen al producto " + x.Name;
                    Context.Add(audit);
                    await Context.SaveChangesAsync();
                }
            }
            #endregion
        }
        catch (Exception ex)
        {

        }

        return NoContent();
        // return result;
    }

    public override Task<ActionResult<Product>> Delete(long id)
    {
        EnsureUserRole(UsersRole.Superuser, UsersRole.Supervisor);
        return base.Delete(id);
    }

    [HttpGet("Options")]
    public async Task<IActionResult> Options(long? productTypeId)
    {
        var user = this.CurrentAppUser.Value;

        var productsQuery = Context.Products
                            .AsNoTracking()
                            .Where(p => !p.Deleted.HasValue || !p.Deleted.Value);

        if (user != null && user.ApplicationRole != null && user.ApplicationRole.IsNationalSeller())
        {
            productsQuery = productsQuery.Where(p => p.CountryId == user.CountryId);
        }

        if (productTypeId.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.ProductTypeId == productTypeId.Value);
        }

        var products = await productsQuery.Select(p => new
        {
            p.Id,
            p.Name
        })
        .OrderBy(p => p.Name)
        .ToListAsync();

        return Ok(products);
    }

    [HttpGet("OptionsFull")]
    public async Task<IActionResult> OptionsFull()
    {
        var user = this.CurrentAppUser.Value;

        var productsQuery = Context.Products
                            .Include(p => p.Country)
                            .Include(p => p.ProductLocationDiscounts)
                            .AsNoTracking()
                            .Where(p => !p.Deleted.HasValue || !p.Deleted.Value);

        if (user != null && user.ApplicationRole != null && user.ApplicationRole.IsNationalSeller())
        {
            productsQuery = productsQuery.Where(p => p.CountryId == user.CountryId);
        }

        var products = await productsQuery.Select(p => new
        {
            p.Id,
            p.Name,
            p.CountryId,
            CountryName = p.Country != null ? p.Country.Name : string.Empty,
            p.MaxAplicableDiscount,
            p.IVA,
            productLocationDiscounts = p.ProductLocationDiscounts.Select(ld => new
            {
                ld.AdvertisingSpaceLocationTypeId,
                ld.Discount
            })
        })
        .OrderBy(p => p.Name)
        .ToListAsync();

        return Ok(products);
    }

    [HttpGet("GetXubioProducts")]
    public async Task<IActionResult> GetXubioProducts(bool? isComtur)
    {
        var xubioProducts = (await this.xubioService.GetProductsAsync(!isComtur.HasValue ? false : isComtur.Value)).Select(p => new { Code = p.Codigo, Name = p.Nombre })
                            .Where(p => p.Code != this.configuration["XubioGenericProductCode"])
                            .ToList();
        return Ok(xubioProducts);
    }

    /// <summary>
    /// Obtener el producto "Pauta Multiplataforma" para facturación consolidada
    /// </summary>
    [HttpGet("GetXubioGenericProductCode")]
    public async Task<IActionResult> GetXubioGenericProductCode(bool? isComtur)
    {
        try
        {
            var xubioProducts = (await this.xubioService.GetProductsAsync(!isComtur.HasValue ? false : isComtur.Value)).Select(p => new { Code = p.Codigo, Name = p.Nombre }).ToList();
            var xubioGenericProductCode = xubioProducts.FirstOrDefault(xp => xp.Code == this.configuration["XubioGenericProductCode"]);
            if (xubioGenericProductCode == null)
            {
                return NotFound(new
                {
                    message = "Producto 'Pauta Multiplataforma' no encontrado",
                    suggestion = "Por favor, cree el producto 'Pauta Multiplataforma' en el ABM de productos"
                });
            }
            return Ok(xubioGenericProductCode);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    protected override IQueryable<Product> GetQueryableWithIncludes()
    {
        EnsureUserRole(UsersRole.Superuser, UsersRole.Supervisor);

        bool isSupervisor = CurrentAppUser.Value.ApplicationRole.IsSupervisor();

        var allProducts = base.GetQueryableWithIncludes()
                .Include(c => c.ProductLocationDiscounts)
                .Include(c => c.ProductVolumeDiscounts)
                .Include(c => c.Country)
                .Include(c => c.ProductType);

        return allProducts;
    }

    protected override DataSourceResult GetSearchDataSourceResult(KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
    {
        var result = GetSearchQueryable()
            .Select(x => new
            {
                x.AliquotForSalesCommission,
                x.Country,
                x.CountryId,
                x.DiscountByManager,
                x.DiscountForAgency,
                x.DiscountForCheck,
                x.DiscountForLoyalty,
                x.DiscountForOtherCountry,
                x.DiscountForSameCountry,
                x.DiscountSpecialBySeller,
                x.Id,
                x.MaxAplicableDiscount,
                x.Name,
                x.IVA,
                x.XubioProductCode,
                x.ComturXubioProductCode,
                ProductLocationDiscounts = x.ProductLocationDiscounts.Where(pld => !pld.Deleted.HasValue || pld.Deleted.Value),
                x.ProductType,
                x.ProductTypeId,
                ProductVolumeDiscounts = x.ProductVolumeDiscounts.Where(pvd => !pvd.Deleted.HasValue || pvd.Deleted.Value),
                CanDelete = !Context.ProductEditions.Any(pe => pe.ProductId == x.Id)
            }).ToDataSourceResult(request);

        return result;
    }
}

