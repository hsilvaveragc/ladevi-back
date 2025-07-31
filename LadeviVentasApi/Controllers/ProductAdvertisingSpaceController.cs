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
public class ProductAdvertisingSpaceController : RestController<ProductAdvertisingSpace, ProductAdvertisingSpaceWritingDto>
{
    public ProductAdvertisingSpaceController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public override async Task<IActionResult> Put(long id, ProductAdvertisingSpaceWritingDto x)
    {
        EnsureUserRole(UsersRole.Superuser, UsersRole.Supervisor);
        ProductAdvertisingSpace oldProductAdvertisingSpace = Context.ProductAdvertisingSpaces.AsNoTracking().Single(y => y.Id == id);

        List<ProductAdvertisingSpaceVolumeDiscount> oldVolumeDiscounts = Context.ProductAdvertisingSpaceVolumeDiscount.AsNoTracking().Where(y => y.ProductAdvertisingSpaceId == id).ToList();
        List<ProductAdvertisingSpaceLocationDiscount> oldLocationDiscounts = Context.ProductAdvertisingSpaceLocationDiscount.AsNoTracking().Where(y => y.ProductAdvertisingSpaceId == id).ToList();

        ProductAdvertisingSpace productAdvertisingSpaceToUpdate = Mapper.Map<ProductAdvertisingSpace>(x);

        var local = Context.Set<ProductAdvertisingSpace>()
                        .Local
                        .FirstOrDefault(entry => entry.Id.Equals(id));

        if (local != null)
        {
            Context.Entry(local).State = EntityState.Detached;
        }

        foreach (var locDiscount in productAdvertisingSpaceToUpdate.ProductAdvertisingSpaceLocationDiscounts.ToList())
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

        foreach (var volDiscount in productAdvertisingSpaceToUpdate.ProductAdvertisingSpaceVolumeDiscounts.ToList())
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
            Context.Entry(productAdvertisingSpaceToUpdate).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }
        catch (Exception ex)
        {

        }

        Auditory auditProduct = new Auditory();
        auditProduct.Date = DateTime.Now;
        auditProduct.Entity = "Tipo de espacio";
        auditProduct.UserId = CurrentAppUser.Value.Id;
        auditProduct.User = CurrentAppUser.Value.FullName;
        auditProduct.AuditMessage = "Se modificó el tipo de espacio " + x.Name + ". " + string.Join("|", Utils.GenerateAuditLogMessages(oldProductAdvertisingSpace, productAdvertisingSpaceToUpdate));
        Context.Add(auditProduct);
        await Context.SaveChangesAsync();

        //Acá hago la auditoria de las entidades relacionadas
        try
        {

            #region Auditoria Descuentos por Ubicacion

            foreach (var oldL in oldLocationDiscounts)
            {
                ProductAdvertisingSpaceLocationDiscount actualLocDiscount = x.ProductAdvertisingSpaceLocationDiscounts.FirstOrDefault(y => y.Id == oldL.Id);
                //Se elimino un volumen
                if (actualLocDiscount == null)
                {
                    Auditory audit = new Auditory();
                    audit.Date = DateTime.Now;
                    audit.Entity = "Tipo de espacio";
                    audit.UserId = CurrentAppUser.Value.Id;
                    audit.User = CurrentAppUser.Value.FullName;
                    audit.AuditMessage = "Se eliminó un descuento por ubicación del Tipo de espacio " + x.Name;
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
                        audit.Entity = "Tipo de espacio";
                        audit.UserId = CurrentAppUser.Value.Id;
                        audit.User = CurrentAppUser.Value.FullName;
                        audit.AuditMessage = "Modificación de descuento por ubicación del Tipo de espacio " + x.Name + ". " + string.Join('|', mods);
                        Context.Add(audit);
                        await Context.SaveChangesAsync();
                    }
                }
            }

            foreach (var actualL in x.ProductAdvertisingSpaceLocationDiscounts)
            {
                ProductAdvertisingSpaceLocationDiscount oldLocDiscount = oldLocationDiscounts.FirstOrDefault(y => y.Id == actualL.Id);
                // Se agrego una entidad
                if (oldLocDiscount == null)
                {
                    Auditory audit = new Auditory();
                    audit.Date = DateTime.Now;
                    audit.Entity = "Tipo de espacio";
                    audit.UserId = CurrentAppUser.Value.Id;
                    audit.User = CurrentAppUser.Value.FullName;
                    audit.AuditMessage = "Se Agregó un descuento por ubicación al Tipo de espacio " + x.Name;
                    Context.Add(audit);
                    await Context.SaveChangesAsync();
                }
            }
            #endregion

            #region Auditoria Descuentos por volumen

            foreach (var oldV in oldVolumeDiscounts)
            {
                ProductAdvertisingSpaceVolumeDiscount actualVolDiscount = x.ProductAdvertisingSpaceVolumeDiscounts.FirstOrDefault(y => y.Id == oldV.Id);
                //Se elimino un volumen
                if (actualVolDiscount == null)
                {
                    Auditory audit = new Auditory();
                    audit.Date = DateTime.Now;
                    audit.Entity = "Tipo de espacio";
                    audit.UserId = CurrentAppUser.Value.Id;
                    audit.User = CurrentAppUser.Value.FullName;
                    audit.AuditMessage = "Se eliminó un descuento por volumen del Tipo de espacio " + x.Name;
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
                        audit.Entity = "Tipo de espacio";
                        audit.UserId = CurrentAppUser.Value.Id;
                        audit.User = CurrentAppUser.Value.FullName;
                        audit.AuditMessage = "Modificación de descuento por volumen del Tipo de espacio " + x.Name + ". " + string.Join('|', mods);
                        Context.Add(audit);
                        await Context.SaveChangesAsync();
                    }
                }
            }

            foreach (var actualV in x.ProductAdvertisingSpaceVolumeDiscounts)
            {
                ProductAdvertisingSpaceVolumeDiscount oldVolDiscount = oldVolumeDiscounts.FirstOrDefault(y => y.Id == actualV.Id);
                // Se agrego una entidad
                if (oldVolDiscount == null)
                {
                    Auditory audit = new Auditory();
                    audit.Date = DateTime.Now;
                    audit.Entity = "Tipo de espacio";
                    audit.UserId = CurrentAppUser.Value.Id;
                    audit.User = CurrentAppUser.Value.FullName;
                    audit.AuditMessage = "Se Agregó un descuento por volumen al Tipo de espacio " + x.Name;
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

    public override Task<ActionResult<ProductAdvertisingSpace>> Delete(long id)
    {
        EnsureUserRole(UsersRole.Superuser, UsersRole.Supervisor);
        return base.Delete(id);
    }

    protected override IQueryable<ProductAdvertisingSpace> GetQueryableWithIncludes()
    {
        return base.GetQueryableWithIncludes()
            .Include(pas => pas.Product)
            .Include(c => c.ProductAdvertisingSpaceLocationDiscounts)
            .Include(c => c.ProductAdvertisingSpaceVolumeDiscounts);
    }

    protected override DataSourceResult GetSearchDataSourceResult(KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
    {
        var result = GetSearchQueryable()
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Product,
                x.ProductId,
                x.Width,
                x.Height,
                x.DollarPrice,
                x.Show,
                x.DiscountForCheck,
                x.DiscountForLoyalty,
                x.DiscountForSameCountry,
                x.DiscountForOtherCountry,
                x.DiscountForAgency,
                ProductAdvertisingSpaceLocationDiscounts = x.ProductAdvertisingSpaceLocationDiscounts.Where(pld => !pld.Deleted.HasValue || pld.Deleted.Value),
                ProductAdvertisingSpaceVolumeDiscounts = x.ProductAdvertisingSpaceVolumeDiscounts.Where(pvd => !pvd.Deleted.HasValue || pvd.Deleted.Value),
                CanDelete = !Context.SoldSpaces.Any(sp => sp.ProductAdvertisingSpaceId == x.Id && (!x.Deleted.HasValue || !x.Deleted.Value)),
            }).ToDataSourceResult(request);

        return result;
    }

    [HttpGet("Options")]
    public async Task<IActionResult> Options()
    {
        var productAdvertisingSpaces = Context.ProductAdvertisingSpaces
                            .AsNoTracking()
                            .Where(p => !p.Deleted.HasValue || !p.Deleted.Value)
                            .Select(p => new
                            {
                                p.Id,
                                p.Name
                            }).ToList();

        return Ok(productAdvertisingSpaces);
    }

    [HttpGet("OptionsFull")]
    public async Task<IActionResult> OptionsFull()
    {
        var productAdvertisingSpaces = Context.ProductAdvertisingSpaces
                            .Include(p => p.ProductAdvertisingSpaceLocationDiscounts)
                            .Include(p => p.ProductAdvertisingSpaceVolumeDiscounts)
                            .AsNoTracking()
                            .Where(p => !p.Deleted.HasValue || !p.Deleted.Value)
                            .Select(p => new
                            {
                                p.Id,
                                p.Name,
                                p.DiscountForAgency,
                                p.DiscountForCheck,
                                p.DiscountForLoyalty,
                                p.DiscountForOtherCountry,
                                p.DiscountForSameCountry,
                                p.DollarPrice,
                                ProductAdvertisingSpaceLocationDiscounts = p.ProductAdvertisingSpaceLocationDiscounts
                                                                        .Select(ld => new
                                                                        {
                                                                            ld.Id,
                                                                            ld.AdvertisingSpaceLocationTypeId
                                                                        }),
                                ProductAdvertisingSpaceVolumeDiscounts = p.ProductAdvertisingSpaceVolumeDiscounts
                                                                        .Select(vd => new
                                                                        {
                                                                            vd.RangeStart,
                                                                            vd.RangeEnd,
                                                                            vd.Discount
                                                                        })
                            }).ToList();

        return Ok(productAdvertisingSpaces);
    }
}
